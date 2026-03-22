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
        [SerializeField] protected LayerMask hitboxLayer;
        [SerializeField] protected WeaponData weaponData;
        protected BulletEffectHandler effectHandler;
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

        public float finalSpread{get; protected set;}
        
        protected float recoilIncrement;
        protected float recoilSpread =0;
        protected float recoverDelay;
        protected float recoverRate;
        
        
        protected bool isReloading;
        
        
        protected  Camera cam;


        public event Action<int,int> OnAmmoChanged;
        
        protected virtual void Awake()
        {
                effectHandler = GetComponent<BulletEffectHandler>();
                Init();
                ApplyAnimator();
                Initialize();
        }
        public void Init()
        {
                cam = Camera.main;
                
        }

        private void Update()
        {
                if (Time.time - lastFireTime > recoverDelay && recoilSpread >0) 
                {
                        recoilSpread = Mathf.Max(0, recoilSpread - recoverRate*Time.deltaTime);
                }
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
                range = 200f;                   // 임시
                
                spreadIdle = weaponData.spreadIdle;
                spreadCrouch = weaponData.spreadCrouch;
                spreadJump = weaponData.spreadJump;
                spreadMove = weaponData.spreadMove;
                maxSpread = weaponData.maxSpread;

                recoilIncrement = weaponData.recoilIncrement;
                recoverRate = weaponData.recoverRate;
                recoverDelay = weaponData.recoverDelay;
                
                
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
                finalSpread = Mathf.Min(GetBaseSpread(state) + speedRatio * spreadMove + recoilSpread,maxSpread);
        }

        private float GetBaseSpread(SpreadState state)
        {
                if (state == SpreadState.Jump)
                        return spreadJump;
                if (state == SpreadState.Crouch) 
                        return spreadCrouch;
                 
                return spreadIdle;
        }
}

