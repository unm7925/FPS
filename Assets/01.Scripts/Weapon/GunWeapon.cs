using UnityEngine;
public class GunWeapon:WeaponBase
{
    
    
    
    public override void Fire()
    {
        if (currentAmmo == 0 || isReloading) return;
        if (Time.time - lastFireTime < weaponData.fireRate) return;
        
        anim.Play("Fire");
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, weaponData.range)) 
        {
            if (hit.collider.gameObject.CompareTag("Player")) 
            {
                Debug.Log((weaponData.damage));    
            }
        }
        currentAmmo--;
        lastFireTime = Time.time;
    }
}
