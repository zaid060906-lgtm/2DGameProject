using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Sprite bulletSprite;
    public GameObject cannonObject;

    private GameObject[] bulletObjects = new GameObject[GameData.MAX_BULLETS];

    void Start()
    {
        // تأكد إن الـ Tag موجود في Unity قبل هاد
        GameData.cannonPosX        = cannonObject.transform.position.x;
        GameData.cannonPosY        = cannonObject.transform.position.y;
        GameData.cannonFireRate    = 1f;
        GameData.cannonNextFireTime = 0f;

        for (int i = 0; i < GameData.MAX_BULLETS; i++)
        {
            bulletObjects[i] = new GameObject("Bullet_" + i);

            SpriteRenderer sr = bulletObjects[i].AddComponent<SpriteRenderer>();
            sr.sprite       = bulletSprite;
            sr.color        = Color.yellow;
            sr.sortingOrder = 10;

            bulletObjects[i].transform.localScale = new Vector3(2f, 2f, 1f);

            CircleCollider2D col = bulletObjects[i].AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            bulletObjects[i].tag = "Damage";
            bulletObjects[i].SetActive(false);
        }
    }

    void Update()
    {
        // إطلاق تلقائي
        CannonFireSystem.Update(Time.time);
        BulletMoveSystem.Update(Time.deltaTime);

        // تحديث مواقع الطلقات على الشاشة
        for (int i = 0; i < GameData.bulletCount; i++)
        {
            if (GameData.bulletActive[i])
            {
                bulletObjects[i].SetActive(true);
                bulletObjects[i].transform.position = new Vector3(
                    GameData.bulletPosX[i],
                    GameData.bulletPosY[i],
                    0f
                );
            }
            else
            {
                bulletObjects[i].SetActive(false);
            }
        }
    }
}