using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using Random = System.Random;

public class AlienController : MonoBehaviour
{
    public IObjectPool<AlienController> Pool;

    public float fullHealth = 100;
    public float health = 100;


    public GameObject point;
    public GameObject ragdoll;
    public PlayerController Player; // Player
    public NavMeshAgent agent;
    public Animator animator;
    public ParticleSystem bloodParticle;

    [Header("Alien Settings")] public float damage = 10;
    public float attackRange = 2f; // Saldırı mesafesi
    public float attackCooldown = 1.5f; // Kaç saniyede bir saldırır

    private float lastAttackTime;


    public void init(float health, float speed, float damage, float hitRate)
    {
        this.health = health;
        this.damage = damage;
        attackCooldown = hitRate;
        agent.speed = speed;
        Player = PlayerController.instance;
        gameObject.transform.localScale = UnityEngine.Random.Range(0.9f, 1.1f) * Vector3.one;
    }


    private float checkCooldown = 0.1f; // Kontrol aralığı (saniye)
    private float nextCheckTime = 0f;

    void Update()
    {
        if (Player.isDying)
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true;
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", false);
            }
            return;
        }

        // Cooldown check
        if (Time.time < nextCheckTime)
            return;

        nextCheckTime = Time.time + checkCooldown + UnityEngine.Random.Range(0, 0.2f); // randomize so not every ai checks at same time

        float distance = Vector3.Distance(transform.position, Player.transform.position);
        if (distance > attackRange)
        {
            // Hedefe koş
            agent.isStopped = false;
            agent.SetDestination(Player.transform.position);

            if (animator)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isAttacking", false);
            }
        }
        else
        {
            agent.isStopped = true;

            if (animator)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", true); // Attack animasyonu
            }
        }

        var lookPos = new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookPos - transform.position), Time.deltaTime * 6f);
    }


    public void giveDamage()
    {
        Debug.Log("Alien attacks player! -" + damage + " HP");
        Player.TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        bloodParticle.Play();
        if (health <= 0)
        {
            health = 0;
            die();
        }
    }

    public void die()
    {
        GameManager.Instance.alienKilled(this);
        Instantiate(point, transform.position, Quaternion.identity);
        Instantiate(ragdoll, transform.position, transform.rotation);
        Pool.Release(this);
    }
}