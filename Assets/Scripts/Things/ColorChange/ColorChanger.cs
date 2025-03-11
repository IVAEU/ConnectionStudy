using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private Client client;
    [SerializeField] private Image coloredImage;
    
    public Button redBtn;
    public Button grnBtn;
    public Button bluBtn;

    private void Start()
    {
        client.AddAction((int)PacketType.ColorChange, ChangeColor);
                
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
        client.TcpSendMessageToServer(p.ToArray());
    }

    private void ChangeColor(byte[] data)
    {
        Packet p = new Packet(data);
        float rColor = p.ReadFloat();
        float gColor = p.ReadFloat();
        float bColor = p.ReadFloat();
        Color color =  new Color(rColor, gColor, bColor);
        ThreadManager.AddAction(() =>
        {
            coloredImage.color = color;  
        });
    }
    

}
