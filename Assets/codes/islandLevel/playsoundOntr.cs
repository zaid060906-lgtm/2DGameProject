using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playsoundOntr : MonoBehaviour
{
    [SerializeField] AudioClip Sound;
    private AudioSource audioSource;
    private bool hasPlayed = false; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasPlayed) 
        {
            PlaySound(Sound);
            hasPlayed = true; 
        }
    }
}