using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Net;

public class Server : MonoBehaviour
{
    private const int Port = 7777;
    private TcpListener _server;
    private bool _isServerStarted = false;

    private List<ServerClient> _clients;
    private List<ServerClient> _disConnectedClients;
    
    public void CreateServer()
    {
        _clients = new List<ServerClient>();
        _disConnectedClients = new List<ServerClient>();
        
        _server = new TcpListener(IPAddress.Any, Port);
        _server.Start();
        print("서버 생성됨");
        
        _isServerStarted = true;
        StartListening();
    }
    
    private void StartListening()
    {
        _server.BeginAcceptTcpClient(AcceptTcpClient, _server);
    }

    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        _clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar), this));
        StartListening();
    }

    public void SendToAllClient(byte[] data)
    {
        foreach (ServerClient client in _clients)
        {
            client.SendData(data);
        }
    }
    
    protected void OnApplicationQuit()
    {
        if (_server != null)
            _server.Stop();
    }
}
