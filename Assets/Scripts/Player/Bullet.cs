using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    [SerializeField]
    PlayerMovement shootAuthor;

    float bulletSpped = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer == false) return;

        transform.position += transform.forward * bulletSpped * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsOwner == false) return;

        if(other.GetComponent<PlayerMovement>() == null)
        {
            shootAuthor.ServerOnlyDestroyBullet(this);
        }
    }

    public void SetAuthor(PlayerMovement playerMovement)
    {
        shootAuthor = playerMovement;

        SetShootAuthorClientRpc(playerMovement, default);
    }

    [ClientRpc]
    public void SetShootAuthorClientRpc(NetworkBehaviourReference referrence, ClientRpcParams clientRpcParams)
    {
        if (referrence.TryGet<PlayerMovement>(out PlayerMovement playerMovement))
        {
            shootAuthor = playerMovement;
        }
        else
        {
            Debug.LogError("Didn't get PlayerMovement");
        }
    }
}
