using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;

    public float rotationSpeed = 240f;

    float positionRange = 5f;

    public Animator animator;

    public Transform spawnBulletPoint;
    public Bullet bulletPrefab;

    public List<Bullet> spawnedBulletList = new();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner == false) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);

        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);

        if (moveDirection.normalized != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        animator.SetBool("IsMoving", moveDirection.magnitude > 0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBulletServerRpc();
        }
    }

    public override void OnNetworkSpawn()
    {
        float ranX = Random.Range(positionRange, -positionRange);
        float ranZ = Random.Range(positionRange, -positionRange);

        transform.position = new Vector3(ranX, 0f, ranZ);

        transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 180f), 0f);
    }

    [ServerRpc]
    void SpawnBulletServerRpc()
    {
        Bullet go = Instantiate(bulletPrefab, spawnBulletPoint.position, spawnBulletPoint.rotation);
        go.shootAuthor = this;
        go.GetComponent<NetworkObject>().Spawn();

        spawnedBulletList.Add(go);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyBulletServerRpc()
    {
        Bullet bullet = spawnedBulletList[0];
        bullet.GetComponent<NetworkObject>().Despawn();
        spawnedBulletList.Remove(bullet);

        Destroy(bullet.gameObject);
    }
}
