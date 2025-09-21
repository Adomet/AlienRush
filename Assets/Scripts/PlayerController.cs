using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public FloatingJoystick joystick;
    public Animator animator;
    public GunController gun;
    public Image healthBar;
    public Text magText, playerLevelText, hpText;
    public ParticleSystem bloodParticle;

    [Range(0, 100)] public float startMoveSpeed = 2f;
    [Range(0, 100)] public float startRotationSpeed = 2f;
    
    private float moveSpeed = 2f;
    private float rotationSpeed = 2f;

    public int startingHealth = 100;
    public int initalMaxXp = 50;

    private float fullHealth;
    public float health ;

    public int level = 0;
    public int maxExp = 50;
    public int exp = 0;

    public bool isDying = false;
    public bool isDead = false;
    public bool isReloading = false;


    private void Awake()
    {
        instance = this;
        fullHealth = startingHealth;
        health = fullHealth;
        maxExp = initalMaxXp;
        updateUI();
    }

    public void resetForNextLevel()
    {
        health = fullHealth;
        gun.reload();
        isReloading = false;
        updateUI();
    }

    public void resetPlayer()
    {
        level = 0;
        exp = 0;
        maxExp = initalMaxXp;
        fullHealth = startingHealth;
        health = fullHealth;
        isDead = false;
        isDying = false;
        animator.SetBool("isDead", false);
        isReloading = false;
        gun.resetGun();
        updateUI();
    }

    public void addExp(int _exp)
    {
        exp += _exp;

        if (exp >= maxExp)
        {
            exp = 0;
            level++;
            addHealth(1);
            updatePlayerLevel();
            updateUI();
        }
        GameManager.Instance.SaveGame();
    }

    public void addHealth(float _health)
    {
        health += _health;
        if (health > fullHealth)
            health = fullHealth;

        updateUI();
    }

    public void updatePlayerLevel()
    {
        maxExp = initalMaxXp +  (20 * level);
        fullHealth = startingHealth + (10 * level);
        moveSpeed = startMoveSpeed + (0.2f * level);
        rotationSpeed = startRotationSpeed + (0.1f * level);
        health = fullHealth;
        gun.upgradeGun(level);
        updateUI();
    }

    public void updatePlayerHP()
    {
    }


    public void TakeDamage(float damage)
    {
        if (isDying || health <= 0)
            return;

        health -= damage;
        bloodParticle.Play();
        if (health <= 0)
        {
            animator.SetBool("isDead", true);
        }

        updateUI();
    }

    void Start()
    {
    }


    public void updateUI()
    {
        magText.text = gun.currentMag.ToString();
        healthBar.DOFillAmount(health / fullHealth, 0.2f).SetEase(Ease.Linear);
        playerLevelText.text = (level + 1).ToString();
        hpText.text = health.ToString();
    }


    public void reloadAnim()
    {
        if (isReloading)
            return;

        isReloading = true;
        animator.SetLayerWeight(1, 1f);
        animator.SetTrigger("Reload");
    }

    public void finishReloadAnim()
    {
        animator.SetLayerWeight(1, 0f);
        gun.reload();
        isReloading = false;
        updateUI();
    }

    public void died()
    {
        isDead = true;
    }

    void Update()
    {
        if (isDying)
            return;

        float joyX = joystick.Horizontal; // or joystick.x
        float joyY = joystick.Vertical; // or joystick.y
        Vector2 movement = new Vector2(joyX, joyY);
        Vector3 movement3D = new Vector3(joyX, 0, joyY);
        Vector3 relativeDir = transform.InverseTransformDirection(movement3D);
        float magnitude = Mathf.Clamp01(movement.magnitude); // remove sqr2


        animator.SetFloat("Horizontal", relativeDir.x);
        animator.SetFloat("Vertical", relativeDir.z);


        // move joystick dir
        transform.Translate(movement3D * (moveSpeed * Time.deltaTime), Space.World);

        // look at target or move dir
        if (gun && gun.Target && gun.Target.gameObject.activeSelf)
        {
            var lookPos = new Vector3(gun.Target.transform.position.x, 0, gun.Target.transform.position.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookPos - transform.position), Time.deltaTime * 2f * rotationSpeed);
        }
        else
        {
            if (magnitude > 0.1f)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(movement3D), Time.deltaTime * rotationSpeed);
        }
    }
}