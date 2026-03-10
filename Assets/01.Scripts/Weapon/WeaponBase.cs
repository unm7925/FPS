using System;
using System.Collections;
using UnityEngine;
public abstract class WeaponBase:MonoBehaviour
{
        [SerializeField] protected WeaponData weaponData;
        protected Animator anim;
        protected int currentAmmo;
        protected int reserveAmmo;
        
        protected bool isReloading;
        
        protected float lastFireTime;
        
        protected  Camera cam;
        private void Awake()
        {
                cam = Camera.main;
                anim = GetComponentInParent<Animator>();
                reserveAmmo = weaponData.maxReserve;

                anim.runtimeAnimatorController = weaponData.animatorController;
        }

        public abstract void Fire();
        
        public virtual void Reload()
        {
                if (reserveAmmo <= 0 || isReloading) return;

                StartCoroutine(ReloadAnimaion());
        }
        private IEnumerator ReloadAnimaion()
        {
                isReloading = true;
                
                anim.Play("Reload");
                
                yield return new WaitForSeconds(weaponData.reloadTime);
                
                int fillAmmo = weaponData.magazineSize - currentAmmo;
                
                fillAmmo = Mathf.Min(fillAmmo, reserveAmmo);
                
                reserveAmmo -= fillAmmo;
                currentAmmo += fillAmmo;
                
                isReloading = false;
        }
}

