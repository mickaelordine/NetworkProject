using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;

/// <summary>
/// in this class we are goona handle all the network stuffs like:
/// 1. applying network settings
/// 2. connecting to  the room
/// 3. spawning all the prefabs we need to play
/// 4. handle leave and switching master situation
/// </summary>
public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Network Settings")]
    [Range(10, 60)]
    [Tooltip("Quante volte al secondo vengono inviati i pacchetti di rete")]
    [SerializeField] 
    private int m_CustomSendRate = 33; //33 pack per second

    [Range(5, 100)]
    [Tooltip("Quante volte al secondo vengono serializzati i dati")]
    [SerializeField] 
    private int m_CustomSerializationRate = 15; // sync ~66 ms
    
    [Range(10, 300)]
    [Tooltip("Nums of cubes")]
    [SerializeField] 
    private int m_CuboidAmount = 100;
    
    [SerializeField]
    private FollowCamera m_Camera;
    
    private List<String> m_PlayerHistory = new List<String>();
    
    

    
    private void Start()
    {
        SetUpNetworkSettings();
        PhotonNetwork.ConnectUsingSettings();
    }

    private void SpawnActorsInScene()
    {
        
        //CleanUp() in case needed
        
        Vector3 StartingLocationCharacter = new Vector3(0, 3.0f, 0);
        float spacing = 1.0f;
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(m_CuboidAmount));

        for (int i = 0; i < m_CuboidAmount; ++i)
        {
            int x = i % gridSize;      // colonna
            int z = i / gridSize;      // riga

            Vector3 pos = new Vector3(x * spacing, 0, z * spacing);
            PhotonNetwork.Instantiate("Cuboid", pos, Quaternion.identity);
        }
        
        //spawn the character
        PhotonNetwork.Instantiate("Character", StartingLocationCharacter, Quaternion.identity);
        
        // set up the camera
        m_Camera.FindPlayerToFollow();
    }

    private void SetUpNetworkSettings()
    {
        PhotonNetwork.SendRate = m_CustomSendRate;
        PhotonNetwork.SerializationRate = m_CustomSerializationRate;
    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        SetUpNetworkSettings();
        PhotonNetwork.JoinRandomRoom();
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No room available, creating new room");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}");
        
        AddPlayerToHistory(PhotonNetwork.PlayerList.GetValue(0).ToString());

        // Se sei un client, sincronizza le impostazioni con quelle del master
        if (!PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("SendRate", out object sendRateObj))
            {
                PhotonNetwork.SendRate = (int)sendRateObj;
            }
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("SerializationRate", out object serializationRateObj))
            {
                PhotonNetwork.SerializationRate = (int)serializationRateObj;
            }
            Debug.Log($"Synced network settings with room - Send: {PhotonNetwork.SendRate}, Serialization: {PhotonNetwork.SerializationRate}");
        }

        if(PhotonNetwork.IsMasterClient) //&& m_PlayerHistory.Count > 0
            SpawnActorsInScene();
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerToHistory(newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerToHistory(otherPlayer.NickName);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"New Master Client: {newMasterClient.NickName}");

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("You are hosting the match");
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from Photon: {cause}");
    }

    private void AddPlayerToHistory(String name)
    {
        string log = $"[{System.DateTime.Now}] {name} has joined.";
        m_PlayerHistory.Add(log);
        Debug.Log(log);
    }
    
    private void RemovePlayerToHistory(String name)
    {
        string log = $"[{System.DateTime.Now}] {name} has left the room.";
        Debug.Log(log);
    }
}
