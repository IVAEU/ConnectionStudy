using System;
using System.Net;
using System.Net.Sockets;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TcpSocket
{
    private TcpClient _tcpClient;
    private NetworkStream _stream;
    private byte[] _receiveBuffer;

    public Action<byte[]> ReceivedAction;

    public TcpSocket(TcpClient client)
    {
        _tcpClient = client;
        Debug.Log("[TCP]: 소캣 생성됨");
    }

    #region Client Only
    //서버에 연결
    public void Connect(string ip, int port)
    {
        _tcpClient.BeginConnect(IPAddress.Parse(ip), port, ConnectionCallback, null);
    }

    //서버에 접속시 실행
    private void ConnectionCallback(IAsyncResult ar)
    {
        _tcpClient.EndConnect(ar);
        InitSocket();
        Debug.Log($"서버에 접속함");
        StartRead();
    }
    #endregion

    //연결 후 실행
    public void InitSocket()
    {
        _stream = _tcpClient.GetStream();
    }

    //클라에게 데이터 전송
    public void SendData(byte[] data)
    {
        if (_stream == null)
        {
            Debug.Log($"Stream이 존재하지 않습니다");
            return;
        };
        
        _stream.BeginWrite(data, 0, data.Length, null, null);
        Debug.Log($"데이터 전송됨. 총 크기: {data.Length}");
    }

    //오는 데이터를 수신 시작
    public void StartRead()
    {
        _receiveBuffer = new byte[4096];
        _stream.BeginRead(_receiveBuffer, 0, 4096, ReceiveCallback, null);
    }

    //데이터 올때마다 실행
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            int packetLength = _stream.EndRead(ar);
            if (packetLength <= 0)
            {
                Disconnect();
                return;
            }

            byte[] data = new byte[packetLength];
            Array.Copy(_receiveBuffer, data, packetLength);

            ReceivedAction?.Invoke(data);
            _stream.BeginRead(_receiveBuffer, 0, 4096, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}");
        }
    }

    //연결 종료
    public void Disconnect()
    {
        if (_stream != null)
            _stream.Close();

        if (_tcpClient != null)
            _tcpClient.Close();
    }
}