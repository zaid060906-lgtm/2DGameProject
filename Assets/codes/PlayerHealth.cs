using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;       
    public int currentHealth;
    public Transform respawnPoint1;
    public AudioSource audioSource;
    public AudioClip respawnSound;
    public AudioClip hurtSound;

    [Header("UI Settings")]
    public Image[] hearts;     
    public Sprite fullHeart;     
    public Sprite emptyHeart;     

    [Header("Damage Settings")]
    public float damageCooldown = 0.3f;
    private bool canTakeDamage = true;

    [Header("Knockback Settings")]
    public float knockbackForce    = 8f;
    public float knockbackUpward   = 3f;
    public float knockbackDuration = 0.15f;

    // ==============================
    // الدرع
    // ==============================
    private bool isBlocking = false;

    private Rigidbody2D rb;
    private bool isKnockedBack = false;
    private Animator m_animator;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        UpdateHeartsUI();
    }

    // تُستدعى من HeroKnight لما يضغط زر الدرع
    public void SetBlocking(bool blocking)
    {
        isBlocking = blocking;
    }

    // ==============================
    // TakeDamage بدون مصدر (من Trigger القديم)
    // ==============================
    public void TakeDamage(int damage)
    {
        TakeDamage(damage, null);
    }

    // ==============================
    // TakeDamage مع مصدر (من السكيلاتون)
    // ==============================
    public void TakeDamage(int damage, Transform damageSource)
{
    if (isBlocking)
    {
        Debug.Log("الدرع صد الضربة!");
        return;
    }

    if (!canTakeDamage) return;

    currentHealth -= damage;
    if (currentHealth < 0) currentHealth = 0;

    UpdateHeartsUI();
    audioSource.PlayOneShot(hurtSound);

    // ← شغّل animation الـ Hurt
    if (m_animator != null)
        m_animator.SetTrigger("Hurt");

    if (damageSource != null && rb != null)
        ApplyKnockback(damageSource);

    if (currentHealth <= 0)
    {
        Die();
        return;
    }

    StartCoroutine(DamageCooldownCoroutine());
}

    // ==============================
    // Knockback
    // ==============================
    void ApplyKnockback(Transform source)
    {
        if (isKnockedBack) return;

        Vector2 dir = (transform.position - source.position).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(dir.x * knockbackForce, knockbackUpward), ForceMode2D.Impulse);

        StartCoroutine(KnockbackCoroutine());
    }

    IEnumerator KnockbackCoroutine()
    {
        isKnockedBack = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    // ==============================
    // UI
    // ==============================
    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
    }

    void Die()
{
    Debug.Log("Player Died! Respawning...");
    StopAllCoroutines();

    // شغّل Death animation
    if (m_animator != null)
    {
        m_animator.SetBool("noBlood", false);
        m_animator.SetTrigger("Death");
    }

    StartCoroutine(RespawnAfterDeath());
}

IEnumerator RespawnAfterDeath()
{
    // ننتظر الـ Death animation ينتهي
    yield return new WaitForSeconds(1.5f);

    transform.position = respawnPoint1.position;
    audioSource.PlayOneShot(respawnSound);
    currentHealth = maxHealth;
    UpdateHeartsUI();

    // نرجع لـ Idle
    if (m_animator != null)
        m_animator.Play("Idle");

    yield return new WaitForSeconds(0.5f);
    canTakeDamage = true;
}
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Damage"))
            TakeDamage(1, other.transform);
    }

    private IEnumerator DamageCooldownCoroutine()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }
}