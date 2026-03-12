using System;
using System.Collections;
using UnityEngine;
public abstract class WeaponBase:MonoBehaviour
{
        [SerializeField] protected WeaponData weaponData;
        
        protected Animator anim;
        public int currentAmmo {get; protected set;}
        public int reserveAmmo {get; protected set;}
        
        public WeaponData.WeaponType weaponType {get; protected set;}
        
        protected bool isReloading;
        
        protected float lastFireTime;
        
        protected  Camera cam;

        public event Action<int,int> OnAmmoChanged;
        
        private void Awake()
        {
                cam = Camera.main;
                ApplyAnimator();
                currentAmmo = weaponData.magazineSize;
                reserveAmmo = weaponData.maxReserve;
                weaponType = weaponData.weaponType;

                
        }

        public void ApplyAnimator()
        { 
                anim = GetComponentInParent<Animator>();
                if (anim == null) return;
                anim.runtimeAnimatorController = weaponData.animatorController;
        }

        public abstract void Fire();
        
        public virtual void Reload()
        {
                if (reserveAmmo <= 0 || isReloading) return;
                if (currentAmmo == weaponData.magazineSize) return;

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

                InvokeAmmoChanged();
                
                isReloading = false;
        }
        
        protected void InvokeAmmoChanged()
        {
                OnAmmoChanged?.Invoke(currentAmmo, reserveAmmo);
        }
        
        public void CancelAct()
        {
                anim.Play("Select");
            if (isReloading == false) return;
            
            isReloading = false;
            StopAllCoroutines();
        }

        public void ChangeTrans()
        {
                transform.localPosition = weaponData.equipPosition;
                transform.localRotation = weaponData.equipRotation;
        }
}

