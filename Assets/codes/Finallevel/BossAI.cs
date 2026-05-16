using System.Collections;
using UnityEngine;

public enum BossState { Idle, Patrol, Chase, Attack, Hurt, Dead }

public class BossAI : MonoBehaviour
{
    [SerializeField] AudioClip AttackSound;
    private AudioSource audioSource;
    [Header("Settings")]
    public float detectionRange = 8f;  // مدى الالتقاط
    public float attackRange = 5f;     // مدى الهجوم
    public float moveSpeed = 7f;
    public float attackCooldown = 2f;
    public int maxHealth = 30;

    [Header("References")]
    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    private Transform currentTarget;

    [Header("Attack Timing")]
    public float attackDuration = 2f;
    public float attackRestTime = 2f;
    private float attackPhaseTimer = 0.5f;
    private bool isInAttackPhase = false;

    private BossState currentState = BossState.Patrol;
    private Animator anim;
    private Rigidbody2D rb;
    private int currentHealth;
    private float attackTimer;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        currentTarget = pointB;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
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
                // التقاط اللاعب بـ detectionRange
                if (dist <= detectionRange)
                    ChangeState(BossState.Chase);
                break;

            case BossState.Patrol:
                PatrolMove();
                // التقاط اللاعب بـ detectionRange
                if (dist <= detectionRange)
                    ChangeState(BossState.Chase);
                break;

           case BossState.Chase:
    // إذا اللاعب خرج من الـ detection — ارجع Patrol
    if (dist > detectionRange)
    {
        isInAttackPhase = false;
        attackPhaseTimer = attackRestTime;
        ChangeState(BossState.Patrol);
        break;
    }

    // وقّفي دايماً — ما تمشي وراه
    rb.velocity = Vector2.zero;
    anim.SetFloat("Speed", 0f);
    FlipTowardsPlayer();

    // هاجمي بس لما يكون في الـ attack range
    if (dist <= attackRange)
    {
        attackPhaseTimer -= Time.deltaTime;

        if (isInAttackPhase)
        {
            if (attackTimer <= 0f)
                ChangeState(BossState.Attack);

            if (attackPhaseTimer <= 0f)
            {
                isInAttackPhase = false;
                attackPhaseTimer = attackRestTime;
            }
        }
        else
        {
            if (attackPhaseTimer <= 0f)
            {
                isInAttackPhase = true;
                attackPhaseTimer = attackDuration;
            }
        }
    }
    break;

            case BossState.Attack:
                rb.velocity = Vector2.zero;
                anim.SetFloat("Speed", 0f);
                FlipTowardsPlayer();
                break;

            case BossState.Hurt:
                rb.velocity = Vector2.zero;
                anim.SetFloat("Speed", 0f);
                FlipTowardsPlayer();
                break;

           case BossState.Dead:
                anim.SetTrigger("Death");
                rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
                GameObject hpBar = GameObject.Find("BossHealthBar");
                if (hpBar != null) hpBar.SetActive(false);
                StartCoroutine(DestroyAfterDeath());
                break;
        }
        
    }

    void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(dir.x));
        FlipTowardsPlayer();
    }

    void FlipTowardsPlayer()
    {
        float dirX = player.position.x - transform.position.x;
        if (dirX < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), 1, 1);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), 1, 1);
    }

    void PatrolMove()
    {
        Vector2 dir = (currentTarget.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * moveSpeed * 0.5f, rb.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(dir.x));

        if (dir.x < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), 1, 1);
        else transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), 1, 1);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.5f)
            currentTarget = currentTarget == pointA ? pointB : pointA;
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
                rb.bodyType = RigidbodyType2D.Static;
                StartCoroutine(DestroyAfterDeath());
                break;
        }
    }

    public void FireProjectile()
    {
        Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        PlaySound(AttackSound);
    }

    public void TakeDamage(int dmg)
    {
        if (currentState == BossState.Dead) return;
        currentHealth -= dmg;

        if (currentHealth <= 0) ChangeState(BossState.Dead);
        else ChangeState(BossState.Hurt);
    }

    public void OnHurtEnd()
    {
        ChangeState(BossState.Chase);
    }

    public void OnAttackEnd()
    {
        ChangeState(BossState.Chase);
    }

    void OnDrawGizmosSelected()
    {
        // detection range بالأصفر
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        // attack range بالأحمر
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    System.Collections.IEnumerator DestroyAfterDeath()
{
    yield return new WaitForSeconds(2f); // انتظر تخلص الـ Death animation
    GetComponent<Collider2D>().enabled = false; // ✅ هون بس
    yield return new WaitForSeconds(0.1f);
    Destroy(gameObject);
}
public int GetCurrentHealth()
{
    return currentHealth;
}
 void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

}