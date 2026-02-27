using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MirrorPlayer : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public Camera mainCamera;

    [SyncVar(hook = nameof(OnHpChanged))]
    public float hp = 100;

    private Rigidbody rb;
    private Vector3 movement;

    private float moveX;
    private float moveZ;

    public GameObject endMenu;
    public GameObject ultrascore;

    public GameObject bulletPrefab;
    public Transform fireSpot;
    private float LastShotTime = 0f;
    public float fireRate = 0.1f;
    public float heal = 20f;

    public Transform rotatePlayer;
    public Animator animator;

    public AudioClip shootSound;

    public GameObject pauseMenu;
    private bool pauseIsActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (isLocalPlayer)
        {
            if (mainCamera != null)
            {
                mainCamera = Camera.main;
            }

            if (endMenu != null) endMenu.SetActive(false);
            if (pauseMenu != null) pauseMenu.SetActive(false);

            SoundManager.Instance.soundSource.mute = false;
            SoundManager.Instance.musicSource.mute = false;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        if (moveX == moveZ || moveX == -moveZ)
        {
            movement = new Vector3(moveX, 0f, moveZ).normalized;
        }
        else
        {
            movement = new Vector3(moveX, 0f, moveZ);
        }

        if (Input.GetMouseButton(0) && Time.time - LastShotTime >= fireRate)
        {
            CmdShoot();
            LastShotTime = Time.time;
        }

        if (movement.magnitude > 0.1f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
            rb.velocity = Vector3.zero;
        }

        MoveCharacter();
        RotateTowardsMouse();

        if (Input.GetKeyDown(KeyCode.R) && endMenu != null && endMenu.activeSelf)
        {
            NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                pauseMenu.SetActive(true);
                pauseIsActive = true;
                Time.timeScale = 0;
            }
            else if (Time.timeScale == 0)
            {
                pauseMenu.SetActive(false);
                pauseIsActive = false;
                Time.timeScale = 1.0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && pauseIsActive == true)
        {
            Time.timeScale = 1.0f;
            NetworkManager.singleton.StopHost();
            SceneManager.LoadScene(0);
        }
    }

    [Command]
    private void CmdShoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab);
        newBullet.transform.position = fireSpot.position;
        newBullet.transform.rotation = fireSpot.rotation;

        MirrorBullet bullet = newBullet.GetComponent<MirrorBullet>();
        if (bullet != null)
        {
            bullet.ownerNetId = netId;
        }

        NetworkServer.Spawn(newBullet);
        RpcPlayShootSound();
    }

    [ClientRpc]
    private void RpcPlayShootSound()
    {
        SoundManager.Instance.PlaySound(shootSound);
    }

    void MoveCharacter()
    {
        if (!isLocalPlayer) return;

        rb.MovePosition(rb.position + new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed) * Time.deltaTime);
    }

    void RotateTowardsMouse()
    {
        if (!isLocalPlayer || mainCamera == null) return;

        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float RayLength;

        if (groundPlane.Raycast(cameraRay, out RayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(RayLength);
            rotatePlayer.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
    }

    [Server]
    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            RpcShowDeathUI();
            GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
        }
    }

    [ClientRpc]
    private void RpcShowDeathUI()
    {
        if (isLocalPlayer && endMenu != null)
        {
            endMenu.SetActive(true);
            if (ultrascore != null) ultrascore.SetActive(false);
        }
    }

    [Server]
    public void TakeHeal(float healAmount)
    {
        hp += healAmount;
        if (hp > 100)
        {
            hp = 100;
        }
    }

    public float GetHp()
    {
        return hp;
    }

    private void OnHpChanged(float oldHp, float newHp)
    {
        //for sync
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CameraFollow follow = mainCamera.GetComponent<CameraFollow>();
            if (follow != null)
            {
                follow.target = transform;
            }
        }
    }
}