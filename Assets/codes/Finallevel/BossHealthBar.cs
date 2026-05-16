using UnityEngine;
using MagicPigGames;

public class BossHealthBar : MonoBehaviour
{
    public ProgressBar progressBar;
    public BossAI boss;

    void Start()
    {
        progressBar.SetProgress(1f); // ابدأ بـ 100%
    }

    void Update()
    {
        // حوّل الـ HP لقيمة بين 0 و 1
        float ratio = (float)boss.GetCurrentHealth() / (float)boss.maxHealth;
        progressBar.SetProgress(ratio);
    }
}