using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveYBackAndForth : MonoBehaviour
{
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
        transform.position = new Vector2(startPos.x , startPos.y + S);
    }
}
