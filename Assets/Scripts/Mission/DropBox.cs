using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class DropBox : NetworkBehaviour
{
    public MissionManager missionManager;

    public string boxName = "";

    private void Start()
    {
        missionManager.SetRandomSpawnPosition(transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerMovement playerMovement = collision.collider.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            Debug.Log($"Box {boxName} hit player movement");
        }
    }
}
