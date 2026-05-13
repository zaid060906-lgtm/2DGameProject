using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // تحقق من حالة الصوت عند بداية المشهد
        if (PlayerPrefs.GetInt("Muted", 0) == 1)
        {
            audioSource.Pause();
        }
    }
}