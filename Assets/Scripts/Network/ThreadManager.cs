using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private static readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();

    public static ThreadManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        ExecuteAction();
    }

    public static void AddAction(Action action)
    {
        if (action == null) return;

        lock (ExecuteOnMainThread)
        {
            ExecuteOnMainThread.Enqueue(action); 
        }
    }

    private void ExecuteAction()
    {
        lock (ExecuteOnMainThread)
        {
            if(ExecuteOnMainThread.Count == 0) return;

            ExecuteOnMainThread.Dequeue().Invoke();
        }
    }
}
