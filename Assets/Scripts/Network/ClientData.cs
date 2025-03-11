using UnityEngine;

[System.Serializable]
public class ClientData
{
    public int clientID;
    public string clientName;

    public ClientData(int id, string name = "Guest")
    {
        clientID = id;
        clientName = name;
        
        Debug.Log("[ClientData]: 클라이언트 데이터 생성됨");
    }
}