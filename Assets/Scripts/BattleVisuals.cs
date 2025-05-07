using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleVisuals : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI levelText;

    private int maxHealth;
    private int currentHealth;
    private int level;

    private Animator animator;

    private const string LEVEL_ABB = "Lve: ";
    private const string IS_ATTACK_TRIGGER = "IsAttack";
    private const string IS_HIT_TRIGGER = "IsHit";
    private const string IS_DEAD_TRIGGER = "IsDead";
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetStaringValues(int maxHealth, int currentHealth, int levelValue)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = currentHealth;
        this.level = levelValue;
        levelText.text = LEVEL_ABB + level.ToString();
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    public void ChangeHealth(int currentHealth)
    {
        this.currentHealth = currentHealth;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            PlayDeathAnimation();
            Destroy(gameObject, 1f);
        }

        UpdateHealthBar();
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger(IS_ATTACK_TRIGGER);
    }
    public void PlayHitAnimation()
    {
        animator.SetTrigger(IS_HIT_TRIGGER);
    }
    public void PlayDeathAnimation()
    {
        animator.SetTrigger(IS_DEAD_TRIGGER);
    }
}
