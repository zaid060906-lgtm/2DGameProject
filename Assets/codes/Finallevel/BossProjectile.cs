using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 6f;
    public int damage = 15;
    private Animator anim;
    private bool exploding = false;
    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // اتجاه نحو اللاعب
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 dir = (player.transform.position - transform.position).normalized;
            rb.velocity = dir * speed;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (exploding) return;

        if (col.CompareTag("Player"))
            col.GetComponent<PlayerHealth>()?.TakeDamage(damage);

        Explode();
    }

    void Explode()
    {
        exploding = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Explode");
        GetComponent<Collider2D>().enabled = false;
    }

    // Animation Event في نهاية Explode
    public void OnExplodeEnd() => Destroy(gameObject);
}