using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;
using Unity.Collections;

using TMPro;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] Transform canvas;
    public TextMeshProUGUI playerNumNameText;
    [SerializeField] private NetworkVariable<FixedString128Bytes> networkPlayerNumName = new NetworkVariable<FixedString128Bytes>(
        "Player: -", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

    private void LateUpdate()
    {
        CanvasLookCamera();
    }

    void CanvasLookCamera()
    {
        canvas.LookAt(canvas.position + Camera.main.transform.forward);
    }

    public override void OnNetworkSpawn()
    {
        networkPlayerNumName.Value = "Player: " + (OwnerClientId);
        playerNumNameText.text = networkPlayerNumName.Value.ToString();

        SetRandomSpawnPositionServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void SetRandomSpawnPositionServerRpc()
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
        go.GetComponent<NetworkObject>().Spawn();
        go.SetShootAuthorServerRpc(this, default);

        spawnedBulletList.Add(go);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyBulletServerRpc(NetworkBehaviourReference referrence, ServerRpcParams serverRpcParams)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        if (referrence.TryGet<Bullet>(out Bullet bullet))
        {
            bullet.GetComponent<NetworkObject>().Despawn();
            spawnedBulletList.Remove(bullet);

            Destroy(bullet.gameObject);

            //bullet.NetworkObject.ChangeOwnership(clientId);
            //Debug.Log("SERVER: " + clientId + " change owner ship");
        }
        else
        {
            Debug.LogError("Didn't get Bullet");
        }
    }
}
