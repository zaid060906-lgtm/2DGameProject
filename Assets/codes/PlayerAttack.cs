using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attackDamage = 1;
    
    // مساحة الضرب حول اللاعب
    public float attackRangeX = 1f;
    public float attackRangeY = 0.5f;
    public LayerMask enemyLayer;

    private Animator m_animator;
    private int m_facingDirection = 1;
    private bool m_canDealDamage = false;

    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        // نتابع اتجاه اللاعب من الـ SpriteRenderer
        if (GetComponent<SpriteRenderer>().flipX)
            m_facingDirection = -1;
        else
            m_facingDirection = 1;
    }

    // ==============================
    // تُستدعى من Animation Event
    // على كل Attack1 و Attack2 و Attack3
    // ==============================
    public void EnableDamage()
    {
        m_canDealDamage = true;
        CheckHit();
    }

    public void DisableDamage()
    {
        m_canDealDamage = false;
    }

    void CheckHit()
    {
        if (!m_canDealDamage) return;

        // نحدد مركز منطقة الضرب أمام اللاعب
        Vector2 attackCenter = (Vector2)transform.position 
                             + new Vector2(m_facingDirection * attackRangeX, 0);

        // نشوف كل الأعداء في النطاق
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            attackCenter,
            new Vector2(attackRangeX * 2, attackRangeY * 2),
            0f,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            // نحاول نوصل للسكيلاتون
            SkeletonEnemy skeleton = hit.GetComponent<SkeletonEnemy>();
            if (skeleton != null)
            {
                skeleton.TakeDamage(attackDamage);
                Debug.Log("اللاعب ضرب: " + hit.name);
            }

            // لو عندك أعداء ثانيين بالمستقبل
            // OtherEnemy other = hit.GetComponent<OtherEnemy>();
            // if (other != null) other.TakeDamage(attackDamage);
        }
    }

    // ==============================
    // Gizmos تشوف منطقة الضرب بالـ Editor
    // ==============================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 attackCenter = (Vector2)transform.position 
                             + new Vector2(m_facingDirection * attackRangeX, 0);
        Gizmos.DrawWireCube(attackCenter, 
                            new Vector3(attackRangeX * 2, attackRangeY * 2, 0));
    }
}