using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManageUI : MonoBehaviour
{
    public TMP_InputField ipAddressText;
    public TMP_InputField portText;
    public Button serverBtn;
    public Button clientBtn;
    public Server server;
    public Client client;
    
    private void Start()
    {
        serverBtn.onClick.AddListener(server.CreateServer);
        clientBtn.onClick.AddListener(()=>
        {
            string ip = ipAddressText.text == "" ? "127.0.0.1" : ipAddressText.text;
            int port = portText.text == "" ? 7777 : int.Parse(portText.text);
            client.Connect(ip, port);
        });
    }
}
