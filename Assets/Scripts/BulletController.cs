using System;
using UnityEngine;
using UnityEngine.Pool;

public class BulletController : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    
    private IObjectPool<BulletController> pool;
    public IObjectPool<BulletController> Pool { set => pool = value; }
    
    public float speed = 20f;       // Units per second
    public float lifeTime = 3f;     // Seconds before auto-destroy
    public float timer;

    
    void Update()
    {
        // Move forward in local space
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));

        // Destroy after lifetime
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            release();
        }
    }

    public void release()
    {
        pool.Release(this);
        trailRenderer.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("enemy"))
        {
            if (other.gameObject.TryGetComponent(out AlienController alienController))
            {
                alienController.TakeDamage(10);
                release();
            }
            
        }
    }
}