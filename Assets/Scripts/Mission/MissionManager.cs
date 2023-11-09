using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class MissionManager : NetworkBehaviour
{
    public Transform dropBoxContainer;

    float positionRange = 5f;

    Vector3 spawnPosOffset = new Vector3(0f, 10f, 0f);

    public List<DropBox> dropBoxePrefabs = new();

    public List<DropBox> spawnnedList = new();

    public string collectBoxName = string.Empty;

    public TextMeshProUGUI missionCollectText;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        NetworkManager.Singleton.OnServerStarted += OnserverStarted;

        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        Debug.Log("UnityTransport: " + unityTransport);
        unityTransport.OnTransportEvent += OnTransportEvent;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnNetworkSpawn()
    {

    }

    public void OnserverStarted()
    {
        ServerOnlySpawnBoxes();

        ServerOnlySetMission();
    }

    public void OnTransportEvent(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime)
    {
        Debug.Log("OnTransportEvent: " + eventType);
    }

    public void OnClientConnected(ulong clientId)
    {
        Debug.Log($"OnClientConnected id = {clientId}");

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            CorrectSpawnedBoxesServerRpc();

            CorrectMissionServerRpc();
        }
    }

    #region Server only

    void ServerOnlySpawnBoxes()
    {
        foreach (DropBox box in dropBoxePrefabs)
        {
            Debug.Log($"Spawn go = {box.boxName}");

            DropBox spawnGo = Instantiate(box, spawnPosOffset, Quaternion.identity);
            SetRandomSpawnPosition(spawnGo.transform);
            spawnGo.missionManager = this;
            spawnGo.GetComponent<NetworkObject>().Spawn();

            spawnnedList.Add(spawnGo);
        }
    }

    void ServerOnlySetMission()
    {
        collectBoxName = "No mission";

        if(spawnnedList.Count > 0)
        {
            int ranNnum = UnityEngine.Random.Range(0, spawnnedList.Count);

            collectBoxName = spawnnedList[ranNnum].boxName;

            missionCollectText.text = "Mission collect target = " + collectBoxName;
        }
    }

    #endregion Server only

    #region Client

    [ServerRpc(RequireOwnership = false)]
    void CorrectSpawnedBoxesServerRpc()
    {
        Debug.Log("CorrectSpawnedBoxesServerRpc");

        foreach (DropBox box in spawnnedList)
        {
            AddSpawnedBoxClientRpc(box, default);
        }
    }

    [ClientRpc]
    public void AddSpawnedBoxClientRpc(NetworkBehaviourReference referrence, ClientRpcParams clientRpcParams)
    {
        if (referrence.TryGet<DropBox>(out DropBox dropBox))
        {
            if (spawnnedList.Contains(dropBox) == false)
            {
                if(dropBox.missionManager == null)
                {
                    dropBox.missionManager = this;
                }

                spawnnedList.Add(dropBox);
            }
        }
        else
        {
            Debug.LogError("Didn't get DropBox");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CorrectMissionServerRpc()
    {
        CorrectMissionClientRpc(collectBoxName);
    }

    [ClientRpc]
    public void CorrectMissionClientRpc(string findBoxName)
    {
        collectBoxName = findBoxName;

        missionCollectText.text = "Mission collect target = " + collectBoxName;
    }

    #endregion Client

    public void SetRandomSpawnPosition(Transform go)
    {
        float ranX = UnityEngine.Random.Range(positionRange, -positionRange);
        float ranZ = UnityEngine.Random.Range(positionRange, -positionRange);

        go.position = new Vector3(ranX, 10f, ranZ);
        go.rotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 180f), UnityEngine.Random.Range(0f, 180f), UnityEngine.Random.Range(0f, 180f));
    }
}
