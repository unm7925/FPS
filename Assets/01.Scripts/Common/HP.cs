using System;
using UnityEngine;

public class HP : MonoBehaviour,IDamageable
{
    private int currentHP;
    public int maxHP = 100;

    public event Action<int> OnHPChanged;
    public event Action<GameObject> OnDie;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        currentHP -= Mathf.Abs(damage);
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die(attacker);
        }
        OnHPChanged?.Invoke(currentHP);
    }

    private void Die(GameObject attacker)
    {
        // 애니메이션
        StatsManager.Instance.RegisterKill(attacker,gameObject);
        OnDie?.Invoke(gameObject);
        Destroy(gameObject);
    }
}