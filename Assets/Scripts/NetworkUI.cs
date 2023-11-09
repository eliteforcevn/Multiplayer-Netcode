using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

using TMPro;

public class NetworkUI : NetworkBehaviour
{
    public Button hostBtn;
    public Button serverBtn;
    public Button clientBtn;

    public TextMeshProUGUI playerCount;

    private NetworkVariable<int> playerNum = new();

    // Start is called before the first frame update
    void Start()
    {
        hostBtn.onClick.AddListener(() =>
        {
            HostButton();
        });

        serverBtn.onClick.AddListener(() =>
        {
            ServerButton();
        });

        clientBtn.onClick.AddListener(() =>
        {
            ClientButton();
        });
    }

    // Update is called once per frame
    void Update()
    {
        ServerOnlySetPlayerCount();
    }

    public void ServerOnlySetPlayerCount()
    {
        if (IsServer)
        {
            playerNum.Value = NetworkManager.Singleton.ConnectedClients.Count;

            playerCount.text = $"Player: {playerNum.Value}";
        }

        SetPlayerCountClientRpc();
    }

    [ClientRpc]
    public void SetPlayerCountClientRpc()
    {
        playerCount.text = $"Player: {playerNum.Value}";
    }

    public void HostButton()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void ServerButton()
    {
        NetworkManager.Singleton.StartServer();
    }

    public void ClientButton()
    {
        NetworkManager.Singleton.StartClient();
    }
}
