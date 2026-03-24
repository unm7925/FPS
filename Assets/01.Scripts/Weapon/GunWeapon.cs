using UnityEngine;
public class GunWeapon:WeaponBase
{
    
    public override void Fire()
    {
        if (currentAmmo == 0 || isReloading) return;
        if (Time.time - lastFireTime < fireRate) return;
        if (cam == null) {
            cam = Camera.main;
        }
        
        anim.Play("Fire");

        if (cam != null) {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            Vector2 offSet = Random.insideUnitCircle * finalSpread;
            Vector3 camDir = cam.transform.right * offSet.x + cam.transform.up * offSet.y;
            Vector3 direction = (ray.direction + camDir).normalized;

            if (Physics.Raycast(ray.origin , direction, out RaycastHit hit, range,hitboxLayer )) 
            {
                Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
                HP targetHp = hit.collider.GetComponentInParent<HP>();
                if(hitbox != null) 
                {
                    hitbox.ApplyDamage(damage,targetHp,transform.root.gameObject);
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
                effectHandler.ShowTracer(ray.origin + direction * range);
            }
        }


        currentAmmo--;
        recoilSpread += recoilIncrement;
        
        InvokeAmmoChanged();
        
        
        lastFireTime = Time.time;
    }
}
