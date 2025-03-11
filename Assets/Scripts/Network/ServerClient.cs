using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.IO;
using System.Net;

[System.Serializable]
public class ServerClient
{
    private Server _server;
    public ClientData clientData;
    private TcpSocket _tcpSocket;
    private UdpSocket _udpSocket;

    public bool IsTcpConnected => _tcpSocket == null;
    public bool IsUdpConnected => _udpSocket == null;
    
    public ServerClient(int id, TcpClient t, Server s)
    {
        clientData = new ClientData(id);
        _server = s;
        _tcpSocket = new TcpSocket(t);
        _tcpSocket.InitSocket();

        Init();
        Debug.Log("[ServerClient]: 클라이언트 및 소켓 생성함");
    }

    private void Init()
    {
        //클라에게서 온 데이터를 서버로 보냄
        _tcpSocket.ReceivedAction += _server.SendToAllClient;
        _tcpSocket.StartRead();
    }

    public void ConnectUdp(string ip, int port)
    {
        _udpSocket.Connect(ip, port);
        _udpSocket.StartRead();
    }

    public void SendTcpData(byte[] data)
    {
        _tcpSocket.SendData(data);
    }
    
    public void SendUdpData(byte[] data)
    {
        _udpSocket.SendData(data);
    }

    public void Disconnect()
    {
        if(_tcpSocket != null)
            _tcpSocket.Disconnect();
        if(_udpSocket != null)
            _udpSocket.Disconnect();
    }
}
