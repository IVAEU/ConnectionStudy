using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System;
using System.Text;

public class Client : MonoBehaviour
{
    private TcpClient _client;
    private NetworkStream _stream;
    private byte[] _receiveBuffer;

    private Dictionary<int, Action<byte[]>> _connectedActions = new Dictionary<int, Action<byte[]>>
    {
        { (int)PacketType.Welcome, null },
        { (int)PacketType.Default, null },
        { (int)PacketType.ColorChange, null },
    };

    public void AddAction(int id, Action<byte[]> action)
    {
        _connectedActions[id] += action;
    }

    public void Connect(string ipAddress, int port)
    {
        _client = new TcpClient
        {
            ReceiveBufferSize = 4096,
            SendBufferSize = 4096,
        };
        _client.BeginConnect(ipAddress, port, ConnectionCallback, null);
    }

    private void ConnectionCallback(IAsyncResult ar)
    {
        _client.EndConnect(ar);
        _stream = _client.GetStream();
        print($"서버에 접속함");
        StartRead();
    }

    public void SendMessageToServer(byte[] data)
    {
        if (_stream == null)
        {
            print($"Stream이 존재하지 않습니다");
            return;
        };

        _stream.BeginWrite(data, 0, data.Length, null, null);
        print($"데이터 전송됨. 총 크기: {data.Length}");
    }

    //클라에게서 오는 데이터를 수신 시작
    private void StartRead()
    {
        _receiveBuffer = new byte[4096];
        _stream.BeginRead(_receiveBuffer, 0, 4096, ReceiveCallback, null);
    }
    
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            int packetLength = _stream.EndRead(ar);
            if (packetLength <= 0) 
            {
                _client.Close();
                Debug.Log($"서버와의 연결이 종료되었습니다");
                return;
            }
        
            byte[] data = new byte[packetLength];
            Array.Copy(_receiveBuffer, data, packetLength);
            
            //패킷 후처리
            Packet p = new Packet(_receiveBuffer);
            int length = p.ReadInt();
            int commandID = p.ReadInt();
            if (_connectedActions[commandID] != null)
            {
                _connectedActions[commandID].Invoke(p.UnreadBytes());
            }
            _stream.BeginRead(_receiveBuffer, 0, 4096, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}");
        }
    }

    protected void OnApplicationQuit()
    {
        if (_stream != null)
            _stream.Close();

        if (_client != null)
            _client.Close();
    }
}