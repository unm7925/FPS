using UnityEngine;
using UnityEngine.InputSystem.DualShock;
public class BotGunWeapon : WeaponBase
{
    
    [SerializeField]private Transform head;
    protected override void Awake()
    {
        anim = GetComponentInParent<Animator>();
        
        Initialize();
    }

    public override void Fire()
    {
        if (currentAmmo == 0 || isReloading) return;
        if (Time.time - lastFireTime < fireRate) return;

        anim.Play("Fire",1);

        Vector2 offSet = Random.insideUnitCircle * finalSpread;
        Vector3 HeadDir = head.right * offSet.x + head.up * offSet.y;
        Vector3 direction = (head.forward + HeadDir).normalized;

        RaycastHit hit;

        if (Physics.Raycast(head.position, direction, out hit, range,~LayerMask.GetMask("Enemy")))
        {
            Debug.Log(hit.transform.name);
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null) 
            {
                Debug.Log(damage);
                damageable.TakeDamage(damage);
            }
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
