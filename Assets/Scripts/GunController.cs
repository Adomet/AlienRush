using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class GunController : MonoBehaviour
{
    public AudioClip shootClip,reloadClip;
    public AudioSource audioSource;
    
    List<AlienController> aliens = new();

    public ParticleSystem muzzleFlash;
    public SphereCollider rangeShere;
    public Transform shootPos;
    public BulletController bulletPrefab;
    public GameObject Target;

    public int startMagSize = 20;
    public float startFireRate = 2f;
    public float startRange = 3f;
    
    public int currentMag = 20;

    private int maxMagSize = 20;
    private float shootTimer = 0.5f;
    private float timer = 0.1f;
    private float range = 10f;

    private IObjectPool<BulletController> bulletPool;
    private bool collectionCheck = true;
    private int defaultCapacity = 50;
    private int maxCapacity = 500;


    public void resetGun()
    {
        maxMagSize = startMagSize;
        currentMag = maxMagSize;
        shootTimer = 1f/startFireRate;
        range = startRange;
        rangeShere.radius = range;
    }

    public void upgradeGun(int level)
    {
        maxMagSize = startMagSize + (level * 5);
        currentMag = maxMagSize;

        float fireRate = startFireRate + (level * 1f);
        shootTimer = 1f / fireRate; // örn. fireRate=2 → 0.5s, fireRate=4 → 0.25s

        // Range: başlangıç + her level * 1
        range = startRange + (level * 1f);
        rangeShere.radius = range;
        
        Debug.Log("upgradeGun " + level);
    }

    private void Awake()
    {
        bulletPool = new ObjectPool<BulletController>(CreateBullet, OnGetFromPool, OnReleaseToPool, OnDestroyPooledBullet, collectionCheck, defaultCapacity, maxCapacity);
        rangeShere.radius = range;
    }

    public BulletController CreateBullet()
    {
        BulletController bullet = Instantiate(bulletPrefab, shootPos.position, shootPos.rotation);
        bullet.Pool = bulletPool;
        return bullet;
    }

    public void OnGetFromPool(BulletController bullet)
    {
        bullet.timer = 0;
        bullet.transform.position = shootPos.position;
        bullet.transform.forward = shootPos.forward;
        bullet.gameObject.SetActive(true);
    }

    public void OnReleaseToPool(BulletController bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    public void OnDestroyPooledBullet(BulletController bullet)
    {
        Destroy(bullet.gameObject);
    }

    private void Update()
    {
        if (PlayerController.instance.isDying || PlayerController.instance.isDead)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            findTarget();

            if (!Target)
                return;
            
            if(!Target.activeSelf)
                return;

            Shoot();
            timer = shootTimer; // sıfırdan başlat
        }
    }

    public void findTarget()
    {
        if (aliens == null || aliens.Count == 0)
            return;

        aliens = aliens.Where(z => z != null && z.gameObject.activeSelf ).ToList();

        AlienController closestAlien = aliens
            .OrderBy(z => Vector3.Distance(transform.position, z.transform.position))
            .FirstOrDefault();

        if (closestAlien)
        {
            Target = closestAlien.gameObject;
        }
    }

    public void checkRotation()
    {
        transform.LookAt(Target.transform);
    }

    public void Shoot()
    {
        if (currentMag < 1)
        {
            if (!PlayerController.instance.isReloading)
            {
                PlayerController.instance.reloadAnim();
                audioSource.Stop();
                audioSource.pitch = 1f;
                audioSource.PlayOneShot(reloadClip);
            }
            return;
        }

        currentMag--;
        PlayerController.instance.updateUI();
        bulletPool.Get();
        muzzleFlash.Play();
        audioSource.pitch = UnityEngine.Random.Range(0.55f, 0.6f);
        audioSource.PlayOneShot(shootClip);
        Debug.DrawLine(shootPos.position, shootPos.forward * 1000f, Color.red, 0.4f);
    }

    public void reload()
    {
        currentMag = maxMagSize;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out AlienController alien))
            aliens.Add(alien);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out AlienController alien))
            aliens.Remove(alien);
    }
}