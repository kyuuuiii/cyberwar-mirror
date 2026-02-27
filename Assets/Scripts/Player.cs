using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Camera mainCamera;
    public float hp = 100;
    private Rigidbody rb;
    public Vector3 movement;

    public float moveX;
    public float moveZ;
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
        if (mainCamera != null)
        {
            mainCamera = Camera.main;
        }
        SoundManager.Instance.soundSource.mute = false;
        SoundManager.Instance.musicSource.mute = false;
    }

    void Update()
    {
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
            Shoot();
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

        if (Input.GetKeyDown(KeyCode.R) && endMenu.activeSelf)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
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
            SceneManager.LoadScene(0);
        }
    }

    private void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab);
        newBullet.transform.position = fireSpot.transform.position;
        newBullet.transform.rotation = fireSpot.transform.rotation;
        SoundManager.Instance.PlaySound(shootSound);
    }

    void MoveCharacter()
    {
        rb.MovePosition(rb.position + new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed) * Time.deltaTime);
    }

    void RotateTowardsMouse()
    {
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float RayLength;

        if (groundPlane.Raycast(cameraRay, out RayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(RayLength);
            rotatePlayer.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            endMenu.SetActive(true);
            ultrascore.SetActive(false);
        }
    }

    public void TakeHeal(float healAmount)
    {
        hp += healAmount;
        if (hp > 100)
        {
            hp = 100;
        }
    }
}