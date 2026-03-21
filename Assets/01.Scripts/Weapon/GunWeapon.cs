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
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Vector2 offSet = Random.insideUnitCircle * finalSpread;
        Vector3 camDir = cam.transform.right * offSet.x + cam.transform.up * offSet.y;
        Vector3 direction = (ray.direction + camDir).normalized;

        if (Physics.Raycast(ray.origin , direction, out RaycastHit hit, range)) 
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null) 
            {
                Debug.Log("맞음" + damage);
                damageable.TakeDamage(damage);
            }
            effectHandler.ShowTracer(hit.point);
            effectHandler.ShowImpact(hit);
        }
        else 
        {
            effectHandler.ShowTracer(ray.origin + direction * range);
        }
        
        
        
        currentAmmo--;
        recoilSpread += recoilIncrement;
        
        InvokeAmmoChanged();
        
        
        lastFireTime = Time.time;
    }
}
