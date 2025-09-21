using System;
using UnityEngine;

public class PointController : MonoBehaviour
{
    public int expValue = 10; // bu obje kaç exp verir
    public float moveSpeed = 10f; // Oyuncuya doğru çekilme hızı
    private Transform targetPlayer; // Oyuncu referansı
    private bool isFollowing = false; // Takip etmeye başladı mı?

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            return;
        }
        
        PlayerController player = PlayerController.instance;
        if (player != null)
        {
            player.addHealth(5);
            player.addExp(expValue);
            Destroy(gameObject); // obje toplandıktan sonra kaybolsun
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("player"))
            return;

        targetPlayer = other.transform;
        isFollowing = true;
    }

    private void Update()
    {
        if (isFollowing && targetPlayer)
        {
            // Oyuncuya doğru hareket et
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);
            moveSpeed += 1;
            

            // Oyuncuya çok yaklaştığında exp ver
            float distance = Vector3.Distance(transform.position, targetPlayer.position);
            if (distance < 0.5f)
            {
                PlayerController player = PlayerController.instance;
                if (player)
                {
                    player.addExp(expValue);
                }

                Destroy(gameObject); // obje toplandı
            }
        }
    }
    
}