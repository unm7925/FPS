using System;
using System.Collections;
using UnityEngine;


public abstract class WeaponBase:MonoBehaviour
{
        public enum SpreadState
        {
                Idle,
                Crouch,
                Jump
        };
        
        [SerializeField] protected WeaponData weaponData;
        public WeaponData.WeaponType weaponType {get; protected set;}
        
        protected Animator anim;
        public int currentAmmo {get; protected set;}
        public int reserveAmmo {get; protected set;}
        
        protected float lastFireTime;
        protected int magazineSize;
        protected int maxReserve;
        protected int damage;
        protected float range;
        protected float fireRate;
        protected float reloadTime;
        
        protected float spreadIdle;
        protected float spreadCrouch;
        protected float spreadJump;
        protected float spreadMove;
        protected float maxSpread;

        protected float finalSpread;
        
        
        
        protected bool isReloading;
        
        
        protected  Camera cam;


        public event Action<int,int> OnAmmoChanged;
        
        private void Awake()
        {
                cam = Camera.main;
                ApplyAnimator();
                Initialize();
        }

        protected virtual void Initialize()
        {
                currentAmmo = weaponData.magazineSize;
                reserveAmmo = weaponData.maxReserve;
                weaponType = weaponData.weaponType;
                reloadTime = weaponData.reloadTime;
                magazineSize = weaponData.magazineSize;
                maxReserve = weaponData.maxReserve;
                fireRate = weaponData.fireRate;
                damage = weaponData.damage;
                range = weaponData.range;
                
                spreadIdle = weaponData.spreadIdle;
                spreadCrouch = weaponData.spreadCrouch;
                spreadJump = weaponData.spreadJump;
                spreadMove = weaponData.spreadMove;
                maxSpread = weaponData.maxSpread;
                
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

        public void SetSpreadState(SpreadState state,float speedRatio)
        {
                if (state == SpreadState.Idle) 
                {
                        finalSpread = Mathf.Min(spreadIdle + speedRatio * spreadMove,maxSpread);
                }
                else if (state == SpreadState.Crouch) 
                {
                        finalSpread =  Mathf.Min(spreadCrouch + speedRatio * spreadMove,maxSpread);        
                }
                else 
                {
                        finalSpread =  Mathf.Min(spreadJump + speedRatio * spreadMove,maxSpread);
                }
        }
}

