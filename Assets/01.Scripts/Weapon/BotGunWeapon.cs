using System;
using UnityEngine;
using UnityEngine.InputSystem.DualShock;
using Random = UnityEngine.Random;
public class BotGunWeapon : WeaponBase
{
    
    [SerializeField]private Transform head;
    private Transform target;
    private Vector3 aimPoint;
    private float aimHeight = 1.5f;
    private float targetHeight;
    
    protected override void Awake()
    {
        anim = GetComponentInParent<Animator>();
        effectHandler = GetComponent<BulletEffectHandler>();
        Initialize();
    }

    public void SetTarget(Transform _target, BotDifficulty _difficulty)
    {
        switch (_difficulty) 
        {
            case BotDifficulty.Easy:
                targetHeight = Random.Range(0f, 6f);
                break;
            case BotDifficulty.Normal:
                targetHeight = Random.Range(3f, 6.6f);
                break;
            case BotDifficulty.Hard:
                targetHeight = Random.Range(5f, 6.6f);
                break;
        }
        
        target = _target;
    }

    public override void Fire()
    {
        if (currentAmmo == 0 || isReloading) return;
        if (Time.time - lastFireTime < fireRate) return;

        anim.Play("Fire",1);
        
        aimPoint = head.position + Vector3.up * aimHeight;
        
        Vector2 offSet = Random.insideUnitCircle * finalSpread;
        Vector3 spreadDir = head.right * offSet.x + head.up * offSet.y;
        Vector3 baseDir = (target.position + Vector3.up * targetHeight - aimPoint).normalized;
        Vector3 direction = (baseDir + spreadDir).normalized; 
        
        if (Physics.Raycast(aimPoint, direction, out RaycastHit hit, range, hitboxLayer))
        {
            Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
            HP targetHP = hit.collider.GetComponentInParent<HP>();
            if (hitbox != null) 
            {
                hitbox.ApplyDamage(damage,targetHP,transform.root.gameObject);
            }
            else 
            {
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null) 
                {
                    damageable.TakeDamage(damage,transform.root.gameObject);
                }                
            }
            
            effectHandler.ShowTracer(hit.point);
            effectHandler.ShowImpact(hit);
        }
        else 
        {
            effectHandler.ShowTracer(head.position + direction * range);
        }
        
        currentAmmo--;
        recoilSpread += recoilIncrement;
        if (currentAmmo == 0) 
        {
            Reload();
        }
        
        lastFireTime = Time.time;
    }
}
