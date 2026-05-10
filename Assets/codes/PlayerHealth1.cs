using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth1 : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;       
    public int currentHealth;

    [Header("UI Settings")]
    public Image[] hearts;     
    public Sprite fullHeart;     
    public Sprite emptyHeart;     

    [Header("Damage Settings")]
    public float damageCooldown = 1f;
    private bool canTakeDamage = true;

    [Header("Knockback Settings")]
    public float knockbackForce  = 8f;   // قوة الدفش
    public float knockbackUpward = 3f;   // ارتفاع شوي لفوق
    public float knockbackDuration = 0.2f; // مدة الـ knockback

    private Rigidbody2D rb;
    private bool isKnockedBack = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        UpdateHeartsUI();
    }

    // ==============================
    // TakeDamage - بتستقبل مصدر الضرر
    // ==============================
    public void TakeDamage(int damage, Transform damageSource = null)
    {
        if (!canTakeDamage) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHeartsUI();

        // نطبق الـ knockback لو في مصدر
        if (damageSource != null && rb != null)
            ApplyKnockback(damageSource);

        if (currentHealth <= 0)
            Die();

        StartCoroutine(DamageCooldownCoroutine());
    }

    // ==============================
    // Knockback
    // ==============================
    void ApplyKnockback(Transform source)
    {
        if (isKnockedBack) return;

        // اتجاه الدفش: من السكيلاتون ناحية اللاعب
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
            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }

    void Die()
    {
        currentHealth = maxHealth;    
        UpdateHeartsUI();
    }

    // ==============================
    // Trigger (لو حدا ضربه بدون مصدر محدد)
    // ==============================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Damage"))
        {
            TakeDamage(1, other.transform);
        }
    }

    private IEnumerator DamageCooldownCoroutine()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }
}