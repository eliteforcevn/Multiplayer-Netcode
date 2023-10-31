using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    public Button hostBtn;
    public Button serverBtn;
    public Button clientBtn;

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
