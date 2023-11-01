using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    public PlayerMovement shootAuthor;

    float bulletSpped = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * bulletSpped * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsOwner == false) return;

        shootAuthor.DestroyBulletServerRpc();
    }
}
