using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Image soundOnIcon;
    [SerializeField] Image soundOffIcon;
    private bool Muted = false;

    private static SoundManager instance;

    void Awake()
    {
        // إذا في نسخة ثانية، احذفها وابقي الأولى
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // لا تحذف عند تغيير المشهد

        Load();
        AudioListener.pause = Muted;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    
   private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    AudioListener.pause = Muted;
    
    if (Muted)
    {
       
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudioSources)
        {
            audio.Pause();
        }
    }
    
    UpdateButtonIcon();
}

    void Start()
    {
        UpdateButtonIcon();
    }

    private void UpdateButtonIcon()
    {
        if (soundOnIcon == null || soundOffIcon == null) return;

        if (Muted)
        {
            soundOnIcon.enabled = true;
            soundOffIcon.enabled = false;
        }
        else
        {
            soundOnIcon.enabled = false;
            soundOffIcon.enabled = true;
        }
    }

   public void ToggleSound()
{
    Muted = !Muted;
    AudioListener.pause = Muted;

    AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
    foreach (AudioSource audio in allAudioSources)
    {
        if (Muted)
            audio.Pause();
        else
            audio.UnPause();
    }

    Save();
    UpdateButtonIcon();
}

    private void Load()
    {
        Muted = PlayerPrefs.GetInt("Muted", 0) == 1;
    }

    private void Save()
    {
        PlayerPrefs.SetInt("Muted", Muted ? 1 : 0);
    }
}