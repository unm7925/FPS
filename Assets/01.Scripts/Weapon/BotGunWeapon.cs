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
    
    protected override void Awake()
    {
        anim = GetComponentInParent<Animator>();
        effectHandler = GetComponent<BulletEffectHandler>();
        Initialize();
    }

    public void SetTarget(Transform _target)
    {
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
        Vector3 baseDir = (target.position- aimPoint).normalized;
        Vector3 direction = (baseDir + spreadDir).normalized; 
        
        if (Physics.Raycast(aimPoint, direction, out RaycastHit hit, range,~LayerMask.GetMask("Enemy")))
        {
            Debug.Log(hit.transform.name);
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null) 
            {
                Debug.Log(damage);
                damageable.TakeDamage(damage);
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
