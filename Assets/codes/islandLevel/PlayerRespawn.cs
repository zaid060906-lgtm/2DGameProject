using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint; 
    public AudioSource audioSource;
    public AudioClip respawnSound;


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            transform.position = respawnPoint.position;
            audioSource.PlayOneShot(respawnSound);
        }
    }
}
