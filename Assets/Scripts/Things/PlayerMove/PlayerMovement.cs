using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isOwner = false;
    public Client client;
    private Vector2 _inputAxis;
    [SerializeField] private float moveSpeed;

    private void Start()
    {
        if (!isOwner)
        {
            client.AddAction((int)PacketType.MovePosition, bytes =>
            {
                Packet p = new Packet(bytes);
                p.ReadInt();
                if (client.clientData.clientID == p.ReadInt())
                {
                    transform.position = p.ReadVector2();
                }
            });
        }
    }

    private void Update()
    {
        if (!isOwner) return;
        _inputAxis.x = Input.GetAxisRaw("Horizontal");
        _inputAxis.y = Input.GetAxisRaw("Vertical");
        transform.Translate(_inputAxis.normalized * (moveSpeed * Time.deltaTime));
    }

    private void FixedUpdate()
    {
        if (!isOwner) return;
        Packet p = new Packet();
        p.Write((int)PacketType.MovePosition);
        p.Write(client.clientData.clientID);
        p.Write(transform.position);
        client.UdpSendMessageToServer(p.ToArray());
    }
}