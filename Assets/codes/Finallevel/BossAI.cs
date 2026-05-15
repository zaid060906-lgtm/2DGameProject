using UnityEngine;

public enum BossState { Idle, Chase, Attack, Hurt, Dead }

public class BossAI : MonoBehaviour
{
    [Header("Settings")]
    public float chaseRange = 8f;
    public float attackRange = 3f;
    public float moveSpeed = 3f;
    public float attackCooldown = 2f;
    public int maxHealth = 100;

    [Header("References")]
    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint; // طرف العصا

    private BossState currentState = BossState.Idle;
    private Animator anim;
    private Rigidbody2D rb;
    private int currentHealth;
    private float attackTimer;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (currentState == BossState.Dead) return;

        float dist = Vector2.Distance(transform.position, player.position);
        attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case BossState.Idle:
                anim.SetFloat("Speed", 0f);
                if (dist < chaseRange) ChangeState(BossState.Chase);
                break;

            case BossState.Chase:
                ChasePlayer();
                if (dist < attackRange && attackTimer <= 0f)
                    ChangeState(BossState.Attack);
                else if (dist > chaseRange)
                    ChangeState(BossState.Idle);
                break;

            case BossState.Attack:
                rb.velocity = Vector2.zero;
                anim.SetFloat("Speed", 0f);
                break;

            case BossState.Hurt:
                rb.velocity = Vector2.zero;
                break;
        }
    }

    void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(dir.x));

        // تقليب الشخصية
        if (dir.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        else transform.localScale = new Vector3(1, 1, 1);
    }

    void ChangeState(BossState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case BossState.Attack:
                anim.SetTrigger("Attack");
                attackTimer = attackCooldown;
                break;
            case BossState.Hurt:
                anim.SetTrigger("Hurt");
                break;
            case BossState.Dead:
                anim.SetTrigger("Death");
                rb.velocity = Vector2.zero;
                GetComponent<Collider2D>().enabled = false;
                break;
        }
    }

    // تُستدعى من Animation Event في منتصف الـ Attack animation
    public void FireProjectile()
{
    // اعمل الكرة عند طرف العصا
    Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
}

    public void TakeDamage(int dmg)
    {
        if (currentState == BossState.Dead) return;
        currentHealth -= dmg;

        if (currentHealth <= 0) ChangeState(BossState.Dead);
        else ChangeState(BossState.Hurt);
    }

    // تُستدعى من Animation Event في نهاية Hurt animation
    public void OnHurtEnd()
    {
        ChangeState(BossState.Chase);
    }

    // تُستدعى من Animation Event في نهاية Attack animation
    public void OnAttackEnd()
    {
        ChangeState(BossState.Chase);
    }
}