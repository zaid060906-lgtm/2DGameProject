using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveXBackAndForth : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 2f;
    public float destanse = 3f;

    private Vector2 startPos;
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float S = Mathf.Sin(Time.time * speed) * destanse;
        transform.position = new Vector2(startPos.x + S, startPos.y);
    }
}
