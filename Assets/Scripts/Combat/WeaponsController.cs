using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponsController : MonoBehaviour
{
    // Cache
    private Camera cam;

    // Properties
    [Header("Primary Weapon")]
    [SerializeField] GameObject gun;
    [SerializeField] GameObject bullet;
    [SerializeField] float fireCooldown = .1f;
    private ObjectPooler bulletPool;    // TODO: Create separate pooler for rockets/AT Weapon
    public Camera Camera { get { return cam; } }

    // State
    private bool canShoot = true;
    private float fireTimer = 0f;

    [Header("DEBUG")]
    [SerializeField] private int shotsFired = 0;

    private void Awake()
    {
        cam = Camera.main;
        bulletPool = new ObjectPooler(bullet, 5, "Player Bullet");
    }

    private void Start()
    {
        StartCoroutine(ShootPrimary());
    }

    private void Update()
    {

    }

    IEnumerator ShootPrimary()
    {
        while (true)
        {
            fireTimer += Time.deltaTime;

            if (Input.GetButton("Fire1") && fireTimer >= fireCooldown && canShoot)
            {
                GameObject bullet = bulletPool.GetPooledObject();
                bullet.transform.position = gun.transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
                fireTimer = 0f;
                shotsFired++;
            }
            yield return null;
        }
    }

}