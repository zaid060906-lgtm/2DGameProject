using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneMusicTrigger : MonoBehaviour
{
    [Header("Music Settings")]
    public AudioClip backgroundMusic;   
    public AudioClip zoneMusic;         
 
    
    [Range(0f, 1f)] public float volume = 1f;
    public float fadeSpeed = 1f;      
    public string playerTag = "Player"; 
 
    AudioSource audioSource;
    AudioClip targetClip;
    bool fading = false;
 
    void Awake()
    {
      
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
 
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
 
        PlayClip(backgroundMusic);
    }
 
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        SwitchTo(zoneMusic);
    }
 
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        SwitchTo(backgroundMusic);
    }
 
    void SwitchTo(AudioClip clip)
    {
        if (clip == null || clip == audioSource.clip) return;
        targetClip = clip;
        if (!fading)
            StartCoroutine(FadeSwitch());
    }
 
    System.Collections.IEnumerator FadeSwitch()
    {
        fading = true;
 
        // Fade out
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
 
     
        PlayClip(targetClip);
 
        // Fade in
        while (audioSource.volume < volume)
        {
            audioSource.volume += Time.deltaTime * fadeSpeed;
            yield return null;
        }
 
        audioSource.volume = volume;
        fading = false;
    }
 
    void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.clip = clip;
        audioSource.Play();
    }
}