using UnityEngine;
public class GunWeapon:WeaponBase
{
    
    
    
    public override void Fire()
    {
        if (currentAmmo == 0 || isReloading) return;
        if (Time.time - lastFireTime < fireRate) return;
        
        anim.Play("Fire");
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Vector2 offSet = Random.insideUnitCircle * finalSpread;
        Vector3 direction = (ray.direction + new Vector3(offSet.x, offSet.y, 0)).normalized;
        
        Debug.Log(finalSpread);
        
        RaycastHit hit;
        
        if (Physics.Raycast(ray.origin , direction, out hit, range)) 
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null) 
            { 
                damageable.TakeDamage(damage);
            }
        }
        currentAmmo--;

        InvokeAmmoChanged();
        
        lastFireTime = Time.time;
    }
}
