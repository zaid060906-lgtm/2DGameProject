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

        // تجاهل الـ Boss نفسه
        if (col.CompareTag("Boss")) return;

        if (col.CompareTag("Player"))
        {
            PlayerHealth ph = col.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
        }

        Explode();
    }

    void Explode()
    {
        exploding = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Explode");
        GetComponent<Collider2D>().enabled = false;
    }

    public void OnExplodeEnd() => Destroy(gameObject);
}