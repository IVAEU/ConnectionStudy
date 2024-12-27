using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.IO;

public class ServerClient
{
    private Server _server;
    public TcpClient tcp;
    public string name;
    private NetworkStream _stream;
    private byte[] _receiveBuffer;
    private Packet _receivePacket;

    public ServerClient(TcpClient t, Server s)
    {
        name = "Guest";
        tcp = t;
        _server = s;
        StartRead();
    }
    
    //클라에게 데이터 전송
    public void SendData(byte[] data)
    {
        _stream.BeginWrite(data, 0, data.Length, null, null);
    }
    
    //클라에게서 오는 데이터를 수신 시작
    private void StartRead() 
    {
        _stream = tcp.GetStream();
        _receiveBuffer = new byte[4096];
        _stream.BeginRead(_receiveBuffer, 0, 4096, ReceiveCallback, null);
    }

    //데이터 올때 실행
    private void ReceiveCallback(IAsyncResult ar)
    {
        int packetLength = _stream.EndRead(ar);
        if (packetLength <= 0) 
        {
            tcp.Close();
            Debug.Log($"{name} 나감");
            return;
        }
        
        byte[] data = new byte[packetLength];
        Array.Copy(_receiveBuffer, data, packetLength);
        
        _server.SendToAllClient(data);
        _stream.BeginRead(_receiveBuffer, 0, 4096, ReceiveCallback, null);
    }
}
