using System;
using Mirror;
using UnityEngine;

public class HP : NetworkBehaviour,IDamageable
{
    
    [SyncVar(hook = nameof(OnHPSync))]
    private int currentHP;
    public int maxHP = 100;
    
    public event Action<int> OnHPChanged;
    public event Action<GameObject> OnDie;

    [Server]
    public void Init()
    {
        currentHP = maxHP;
    }
    
    [Server]
    public void TakeDamage(int damage, GameObject attacker)
    {
        currentHP -= Mathf.Abs(damage);
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die(attacker);
        }
    }

    private void OnHPSync(int oldHP, int newHP)
    {
        OnHPChanged?.Invoke(newHP);
    }

    [Server]
    private void Die(GameObject attacker)
    {
        // 애니메이션
        
        RpcDie(attacker);
    }
    
    [ClientRpc]
    private void RpcDie(GameObject attacker)
    {
        StatsManager.Instance.RegisterKill(attacker,gameObject);
        OnDie?.Invoke(gameObject);
        gameObject.SetActive(false);
    }
}