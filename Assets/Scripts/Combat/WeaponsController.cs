using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponsController : MonoBehaviour
{
    // Cache
    private Camera cam;
    public Camera Camera { get { return cam; } }
    private AutoTargetSystem atSystem;

    // Properties
    //TODO: Probably create a ScriptableObject for each type of weapons.
    [Header("Primary Weapon")]
    [SerializeField] GameObject gun;
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletCooldown = .05f;
    private ObjectPooler bulletPool;

    [Header("Secondary Weapon")]
    [SerializeField] GameObject[] launcher;
    [SerializeField] GameObject missile;
    [SerializeField] float missileCooldown = 5f;
    private ObjectPooler missilePool;

    // State
    private bool canShootPrimary = true;
    private float primaryTimer = 0f;
    private bool canShootSecondary = true;
    private float secondaryTimer = 0f;
    private int launcherIndex = -1;

    [Header("DEBUG")]
    [SerializeField] private int shotsFired = 0;
    [SerializeField] private int missilesFired = 0;

    private void Awake()
    {
        cam = Camera.main;
        bulletPool = new ObjectPooler(bullet, 5, "Player Bullet");
        missilePool = new ObjectPooler(missile, 2, "Player Missile");
        primaryTimer = bulletCooldown;
        secondaryTimer = missileCooldown;
    }

    private void Start()
    {
        atSystem = GetComponent<AutoTargetSystem>();
        StartCoroutine(ShootPrimary());
        StartCoroutine(ShootSecondary());
    }

    private void Update()
    {

    }

    IEnumerator ShootPrimary()
    {
        while (true)
        {
            primaryTimer += Time.deltaTime;

            if (Input.GetButton("Fire1") && primaryTimer >= bulletCooldown && canShootPrimary)
            {
                GameObject bullet = bulletPool.GetPooledObject();
                bullet.transform.position = gun.transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
                primaryTimer = 0f;
                shotsFired++;
            }
            yield return null;
        }
    }

    IEnumerator ShootSecondary()
    {
        while (true)
        {
            secondaryTimer += Time.deltaTime;

            if (Input.GetButtonDown("Fire2") && secondaryTimer >= missileCooldown && canShootSecondary)
            {
                IncrementLauncherIndex();
                GameObject missile = missilePool.GetPooledObject();
                missile.transform.position = launcher[launcherIndex].transform.position;
                missile.transform.rotation = transform.rotation;
                missile.transform.parent = launcher[launcherIndex].transform;
                missile.GetComponent<Missile>().SetTarget(atSystem.primaryTarget);
                missile.SetActive(true);
                secondaryTimer = 0f;
                missilesFired++;
            }
            yield return null;
        }
    }

    // Track and select which launcher spot will fire the missile
    private void IncrementLauncherIndex()
    {
        launcherIndex++;
        if (launcherIndex >= launcher.Length)
        {
            launcherIndex = 0;
        }
    }

}