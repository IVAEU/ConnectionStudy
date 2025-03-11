using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Net;

public class Server : MonoBehaviour
{
    private const string IP = "127.0.0.1";
    private const int Port = 7777;
    private TcpListener _tcpListener;
    private UdpClient _udpListener;
    private bool _isServerStarted = false;

    private Dictionary<int, ServerClient> _clients;
    private Dictionary<int, ServerClient> _disConnectedClients;

    public void CreateServer()
    {
        _clients = new ();
        _disConnectedClients = new ();

        _tcpListener = new TcpListener(IPAddress.Any, Port);
        _tcpListener.Start();
        StartTcpListening();
        
        _udpListener = new UdpClient(Port);
        StartUdpListening();

        print("서버 생성됨");
        _isServerStarted = true;
    }

    #region TCP
    private void StartTcpListening()
    {
        _tcpListener.BeginAcceptTcpClient(AcceptTcpClient, _tcpListener);
    }

    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        int clientId = _clients.Count;
        _clients.Add(clientId, new ServerClient(clientId, listener.EndAcceptTcpClient(ar), this));
        
        //Welcome 패킷 (id 뿌리기용)
        Packet p = new Packet();
        p.Write((int)PacketType.Welcome);
        p.Write(clientId);
        p.Insert(p.Length());
        SendToAllClient(p.ToArray());
        
        Debug.Log($"Welcome: ID: [{clientId}] ({_clients[clientId].clientData.clientName})");
        
        StartTcpListening();
    }

    public void SendToAllClient(byte[] data)
    {
        foreach (ServerClient client in _clients.Values)
        {
            client.SendTcpData(data);
        }
    }
    #endregion
    
    #region UDP
    private void StartUdpListening()
    {
        _udpListener.BeginReceive(ReceiveUdpCallback, _udpListener);
    }

    private void ReceiveUdpCallback(IAsyncResult ar)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = _udpListener.EndReceive(ar, ref endPoint);

        Packet p = new Packet(data);

        int commandID = p.ReadInt();
        int clientID = p.ReadInt();
        if (!_clients[clientID].IsUdpConnected)
        {
            //New Client
            _clients[clientID].ConnectUdp(IP, Port);
        }
        
        _clients[clientID].SendUdpData(data);
        
        StartUdpListening();
    }

    public void SendToAllClientUdp(byte[] data)
    {
        foreach (ServerClient client in _clients.Values)
        {
            client.SendUdpData(data);
        }
    }
    #endregion

    protected void OnApplicationQuit()
    {
        if (_tcpListener != null)
            _tcpListener.Stop();
        if (_udpListener != null)
            _udpListener.Close();

        foreach (var c in _clients.Values)
        {
            c.Disconnect();
        }
    }
}