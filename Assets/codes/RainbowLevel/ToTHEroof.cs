using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTHEroof : MonoBehaviour
{
    public GameObject targetObject;
    public float riseSpeed = 3f; // سرعة الصعود، تقدر تغيرها من Inspector
    private bool isInsideTrigger = false;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            isInsideTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            isInsideTrigger = false;
        }
    }

    private void Update()
    {
        if (isInsideTrigger && Input.GetKey(KeyCode.R)) // GetKey = hold مستمر
        {
            if (targetObject != null)
            {
                targetObject.transform.position += new Vector3(
                    0f,
                    riseSpeed * Time.deltaTime, // شوي شوي مع الوقت
                    0f
                );
            }
        }
    }
}