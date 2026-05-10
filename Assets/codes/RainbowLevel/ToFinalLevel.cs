using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToFinalLevel : MonoBehaviour
{
    [Header("Scene Settings")]
    public string FinalLevel;        // اسم الـ Scene الجديدة
    public float delaySeconds = 1f; // المدة قبل الانتقال

    [Header("Optional Fade")]
    public bool showCountdown = true; // تظهر رسالة بالـ Console للتأكد

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(TransitionAfterDelay());
        }
    }

    IEnumerator TransitionAfterDelay()
    {
        Debug.Log("انتقال للـ Scene بعد " + delaySeconds + " ثانية...");
        yield return new WaitForSeconds(delaySeconds);
        SceneManager.LoadScene(FinalLevel);
    }
}