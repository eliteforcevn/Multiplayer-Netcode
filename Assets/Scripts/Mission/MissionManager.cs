using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class MissionManager : NetworkBehaviour
{
    public Transform dropBoxContainer;

    float positionRange = 5f;

    Vector3 spawnPosOffset = new Vector3(0f, 10f, 0f);

    public List<DropBox> dropBoxePrefabs = new();

    public List<DropBox> spawnnedList = new();

    public List<string> MissionCollectList = new();

    public bool spawnBoxes = false;

    // Start is called before the first frame update
    void Start()
    {
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        Debug.Log("UnityTransport: " + unityTransport);

        unityTransport.OnTransportEvent += OnTransportEvent;
    }

    private void OnTransportEvent(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime)
    {
        Debug.Log("OnTransportEvent: " + eventType);
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnBoxes)
        {
            spawnBoxes = false;

            SpawnDropBoxesServerRpc();
        }
    }

    [ServerRpc]
    void SpawnDropBoxesServerRpc()
    {
        foreach(DropBox box in dropBoxePrefabs)
        {
            Debug.Log($"Spawn go = {box.boxName}");

            DropBox spawnGo = Instantiate(box, spawnPosOffset, Quaternion.identity);
            SetRandomSpawnPosition(spawnGo.transform);
            spawnGo.missionManager = this;
            spawnGo.GetComponent<NetworkObject>().Spawn();

            AddSpawnedBoxServerRpc(spawnGo, default);
        }
    }

    [ServerRpc]
    void AddSpawnedBoxServerRpc(NetworkBehaviourReference referrence, ServerRpcParams serverRpcParams)
    {
        if (referrence.TryGet<DropBox>(out DropBox dropBox))
        {
            AddSpawnedBoxClientRpc(dropBox, default);
        }
        else
        {
            Debug.LogError("Didn't get DropBox");
        }
    }

    [ClientRpc]
    public void AddSpawnedBoxClientRpc(NetworkBehaviourReference referrence, ClientRpcParams clientRpcParams)
    {
        if (referrence.TryGet<DropBox>(out DropBox dropBox))
        {
            spawnnedList.Add(dropBox);
        }
        else
        {
            Debug.LogError("Didn't get DropBox");
        }
    }

    public void SetRandomSpawnPosition(Transform go)
    {
        float ranX = UnityEngine.Random.Range(positionRange, -positionRange);
        float ranZ = UnityEngine.Random.Range(positionRange, -positionRange);

        go.position = new Vector3(ranX, 10f, ranZ);
        go.rotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 180f), UnityEngine.Random.Range(0f, 180f), UnityEngine.Random.Range(0f, 180f));
    }
}
