using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UdpSocket
{
    private UdpClient _udpClient;
    public IPEndPoint _endPoint;
    public Action<byte[]> ReceivedAction;

    public UdpSocket(UdpClient client)
    {
        _udpClient = client;

        StartRead();
        Debug.Log("[UDP]: 소캣 생성됨");
    }

    public void Connect(string ip, int port)
    {
        _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        _udpClient.Connect(_endPoint);
    }

    //클라에게 데이터 전송
    public void SendData(byte[] data)
    {
        _udpClient.BeginSend(data, data.Length, null, null);
    }

    //클라에게서 오는 데이터를 수신 시작
    public void StartRead()
    {
        _udpClient.BeginReceive(ReceiveCallback, null);
    }

    //데이터 올때 실행
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            byte[] data = _udpClient.EndReceive(ar, ref _endPoint);
            
            if (data.Length < 4)
            {
                Disconnect();
                return;
            }

            ReceivedAction?.Invoke(data);
            _udpClient.BeginReceive(ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}");
        }
    }

    //연결 종료
    public void Disconnect()
    {
        if (_udpClient != null)
            _udpClient.Close();
    }
}