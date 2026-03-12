using System;
using UnityEngine;

public class HP : MonoBehaviour,IDamageable
{
    private int currentHP;
    public int maxHP = 100;

    public event Action<int> OnHPChanged;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= Mathf.Abs(damage);
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
        OnHPChanged?.Invoke(currentHP);
    }

    private void Die()
    {
        // 애니메이션
        gameObject.SetActive(false);
    }
}