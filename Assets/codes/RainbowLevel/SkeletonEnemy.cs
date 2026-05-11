using System.Collections;
using UnityEngine;

public class SkeletonEnemy : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 6f;
    public float attackRange    = 2f;

    [Header("Movement")]
    public float moveSpeed = 2.5f;

    [Header("Combat")]
    public int   maxHealth      = 3;
    public int   attack1Damage  = 1;
    public int   attack2Damage  = 2;
    public float attackCooldown = 1.4f;

    [SerializeField] AudioClip S_attackSound;
    [SerializeField] AudioClip s_MoveSound;

    private enum State { Idle, Chase, Attack1, Attack2, Hurt, Dead }
    private State currentState = State.Idle;

    private Animator      anim;
    private Rigidbody2D   rb;
    private Transform     player;
    private AudioSource S_audioSource;
   

    private int   currentHealth;
    private float attackTimer;
    private bool  isAttacking = false;
    private bool  facingRight = true;
    private int   comboStep   = 0;
   private PlayerHealth playerHealth;

    private static readonly int HashIdle    = Animator.StringToHash("Idle");
    private static readonly int HashWalk    = Animator.StringToHash("Walk");
    private static readonly int HashAttack1 = Animator.StringToHash("Attack1");
    private static readonly int HashAttack2 = Animator.StringToHash("Attack2");
    private static readonly int HashHurt    = Animator.StringToHash("Hurt");
    private static readonly int HashDead    = Animator.StringToHash("Dead");

    void Start()
    {
        anim = GetComponent<Animator>();
        rb   = GetComponent<Rigidbody2D>();
        S_audioSource = GetComponent<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player       = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();

            if (playerHealth == null)
                Debug.LogError("  PlayerHealth1  !");
        }
        else
        {
            Debug.LogError(" Tag = Player!");
        }

        currentHealth = maxHealth;
        SetState(State.Idle);
    }

    void Update()
    {
        if (currentState == State.Dead || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                if (dist <= detectionRange)
                    SetState(State.Chase);
                break;

            case State.Chase:
                HandleChase(dist);
                break;
        }
    }

    void HandleChase(float dist)
    {
        if (dist <= attackRange && attackTimer <= 0f && !isAttacking)
        {
            rb.velocity = Vector2.zero;
            ChooseAttack();
            return;
        }

        if (dist > detectionRange)
        {
            rb.velocity = Vector2.zero;
            SetState(State.Idle);
            return;
        }

        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
        FlipSprite(dir.x);
    }

    void ChooseAttack()
    {
        comboStep++;
        if (comboStep > 2) comboStep = 1;
        attackTimer = attackCooldown;
        SetState(comboStep == 1 ? State.Attack1 : State.Attack2);
    }

    void SetState(State newState)
    {
        currentState = newState;

        if (newState != State.Chase)
            rb.velocity = Vector2.zero;

        anim.SetBool(HashIdle, false);
        anim.SetBool(HashWalk, false);
        anim.SetBool(HashHurt, false);
        anim.SetBool(HashDead, false);

        switch (newState)
        {
            case State.Idle:
                anim.SetBool(HashIdle, true);
                break;

            case State.Chase:
                anim.SetBool(HashWalk, true);
                    PlaySound(s_MoveSound);
                break;

            case State.Attack1:
                isAttacking = true;
                anim.SetTrigger(HashAttack1);
                PlaySound(S_attackSound);
                StartCoroutine(DealDamageAfterDelay(attack1Damage, 0.4f));
                StartCoroutine(ReturnAfterAttack(0.8f));
                break;

            case State.Attack2:
                isAttacking = true;
                anim.SetTrigger(HashAttack2);
                PlaySound(S_attackSound);
                StartCoroutine(DealDamageAfterDelay(attack2Damage, 0.4f));
                StartCoroutine(ReturnAfterAttack(0.8f));
                break;

            case State.Hurt:
                anim.SetBool(HashHurt, true);
                StartCoroutine(ReturnToChaseAfterHurt());
                break;

            case State.Dead:
                anim.SetBool(HashDead, true);
                rb.bodyType = RigidbodyType2D.Static;
                Collider2D col = GetComponent<Collider2D>();
                if (col) col.enabled = false;
                StartCoroutine(DestroyAfterDeath());
                break;
        }
    }

    IEnumerator DealDamageAfterDelay(int damage, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (player == null || playerHealth == null) yield break;
        if (currentState == State.Dead) yield break;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange)
        {
            Debug.Log("Skeleton ضرب اللاعب! Damage: " + damage);
            playerHealth.TakeDamage(damage, transform);
        }
    }

    IEnumerator ReturnAfterAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
        if (currentState == State.Attack1 || currentState == State.Attack2)
            SetState(State.Chase);
    }

    IEnumerator ReturnToChaseAfterHurt()
    {
        yield return new WaitForSeconds(0.5f);
        if (currentState == State.Hurt)
            SetState(State.Chase);
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (currentState == State.Dead) return;

        currentHealth -= damage;
        Debug.Log("Skeleton أخذ ضرر! HP: " + currentHealth);

        if (currentHealth <= 0)
            SetState(State.Dead);
        else
            SetState(State.Hurt);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerSword"))
            TakeDamage(1);
    }

    void FlipSprite(float xDir)
    {
        if (xDir > 0 && !facingRight)
        {
            facingRight = true;
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }
        else if (xDir < 0 && facingRight)
        {
            facingRight = false;
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
     void PlaySound(AudioClip clip)
    {
        if (clip != null)
            S_audioSource.PlayOneShot(clip);
    }
}