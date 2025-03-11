using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System;
using System.Net;
using System.Text;

public class Client : MonoBehaviour
{
    public ClientData clientData;
    
    private TcpSocket _tcpSocket;
    private UdpSocket _udpSocket;
    private int _leftReceiveCount = 0;
    public PlayerMovement playerPrefab;
    private PlayerMovement crntPlayer;

    private Dictionary<int, Action<byte[]>> _connectedActions = new Dictionary<int, Action<byte[]>>
    {
        { (int)PacketType.Welcome, null },
        { (int)PacketType.Default, null },
        { (int)PacketType.ColorChange, null },
        { (int)PacketType.MovePosition, null },
    };

    public void AddAction(int id, Action<byte[]> action)
    {
        _connectedActions[id] += action;
    }

    public void Connect(string ipAddress, int port)
    {
        var tcp = new TcpClient
        {
            ReceiveBufferSize = 4096,
            SendBufferSize = 4096,
        };
        _tcpSocket = new TcpSocket(tcp);
        _tcpSocket.Connect(ipAddress, port);
        _tcpSocket.ReceivedAction += TcpReceiveData;

        var udp = new UdpClient();
        _udpSocket = new UdpSocket(udp);
        _udpSocket.Connect(ipAddress, port);
        _udpSocket.ReceivedAction += UdpReceiveData;
        
        AddAction((int)PacketType.Welcome, bytes =>
        {
            Packet p = new Packet(bytes);
            int id = p.ReadInt();
            clientData = new ClientData(id);
            ThreadManager.AddAction(() =>
            {
                crntPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                crntPlayer.client = this;
                crntPlayer.isOwner = true;
            });
            Debug.Log($"[Client]: {clientData.clientID} 입장");
        });
        Debug.Log("[Client]: 웰컴 액션 삽입");
    }

    //서버로 TCP 데이터 송신
    public void TcpSendMessageToServer(byte[] data)
    {
        _tcpSocket.SendData(data);
    }
    
    //서버에서 오는 TCP 데이터 수신 
    private void TcpReceiveData(byte[] data)
    {
        //패킷 후처리
        Packet p = new Packet(data);
        int length = p.ReadInt();
        if (_leftReceiveCount == 0)
        {
            _leftReceiveCount = length / 4096;
        }
        int commandID = p.ReadInt();
        if (_connectedActions[commandID] != null)
        {
            _connectedActions[commandID].Invoke(p.UnreadBytes());
        }
    }
    
    //서버로 UDP 데이터 송신
    public void UdpSendMessageToServer(byte[] data)
    {
        _udpSocket.SendData(data);
    }
    
    //서버에서 오는 UDP 데이터 수신 
    private void UdpReceiveData(byte[] data)
    {
        //패킷 후처리
        Packet p = new Packet(data);
        int commandID = p.ReadInt();
        if (_connectedActions[commandID] != null)
        {
            _connectedActions[commandID].Invoke(p.UnreadBytes());
        }
    }

    protected void OnApplicationQuit()
    {
        _tcpSocket.Disconnect();
        _udpSocket.Disconnect();
    }
}