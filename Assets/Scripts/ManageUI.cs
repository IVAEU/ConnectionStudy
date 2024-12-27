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
    
    public Button redBtn;
    public Button grnBtn;
    public Button bluBtn;

    private void Start()
    {
        serverBtn.onClick.AddListener(server.CreateServer);
        clientBtn.onClick.AddListener(()=>
        {
            string ip = ipAddressText.text == "" ? "127.0.0.1" : ipAddressText.text;
            int port = portText.text == "" ? 7777 : int.Parse(portText.text);
            client.Connect(ip, port);
        });
        
        redBtn.onClick.AddListener(() =>
        {
            SendColor(Color.red);
        });
        grnBtn.onClick.AddListener(() =>
        {
            SendColor(Color.green);
        });
        bluBtn.onClick.AddListener(() =>
        {
            SendColor(Color.blue);
        });
    }

    private void SendColor(Color c)
    {
        Packet p = new Packet();
        p.Write((int)PacketType.ColorChange);
        p.Write(c.r);
        p.Write(c.g);
        p.Write(c.b);
        p.Insert(p.Length());
        client.SendMessageToServer(p.ToArray());
    }
}
