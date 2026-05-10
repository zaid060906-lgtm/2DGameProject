using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;       // سرعة الطلقة
    public float lifeTime = 2f;     // الوقت قبل اختفاء الطلقة

    void Start()
    {
        // تدمير الطلقة بعد وقت محدد
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // تحريك الطلقة للأمام
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
