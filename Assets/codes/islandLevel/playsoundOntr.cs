using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playsoundOntr : MonoBehaviour
{
     [SerializeField] AudioClip Sound;
     private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlaySound(Sound);
        }
    }
}
