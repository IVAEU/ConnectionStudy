using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private Client client;
    [SerializeField] private Image coloredImage;

    private void Start()
    {
        client.AddAction((int)PacketType.ColorChange, ChangeColor);
    }

    private void ChangeColor(byte[] data)
    {
        Packet p = new Packet(data);
        float rColor = p.ReadFloat();
        float gColor = p.ReadFloat();
        float bColor = p.ReadFloat();
        Color color =  new Color(rColor, gColor, bColor);
        print(color);
        //coloredImage.color = color;
    }
}
