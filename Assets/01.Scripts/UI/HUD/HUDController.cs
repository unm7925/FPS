using System;
using TMPro;
using UnityEngine;
public class HUDController:MonoBehaviour
{
        private HP hp;
        private WeaponSwitcher weaponSwitcher;
        
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI HPText;

        private WeaponBase currentWeapon;

        public void Init(HP _hp, WeaponSwitcher _weaponSwitcher)
        {
                hp = _hp;
                
                weaponSwitcher = _weaponSwitcher;
                EventConnection();
                SwitchWeaponEvent(weaponSwitcher.currentWeapon);
                UpdateHP(hp.maxHP);
                if (currentWeapon == null) return;
                UpdateAmmo(currentWeapon.currentAmmo,currentWeapon.reserveAmmo);
        }
        private void EventConnection()
        {
                hp.OnHPChanged += UpdateHP;
                weaponSwitcher.OnWeaponChanged += SwitchWeaponEvent;
        }
        
        private void OnDisable()
        {
                hp.OnHPChanged -= UpdateHP;
                weaponSwitcher.OnWeaponChanged -= SwitchWeaponEvent;
                if (currentWeapon == null) return;
                currentWeapon.OnAmmoChanged -= UpdateAmmo;
        }
        private void SwitchWeaponEvent(WeaponBase weaponBase)
        {
                if (weaponBase == null) return;
                if (currentWeapon != null) 
                {
                        currentWeapon.OnAmmoChanged -= UpdateAmmo;
                }
                currentWeapon = weaponBase;
                currentWeapon.OnAmmoChanged += UpdateAmmo;
                UpdateAmmo(currentWeapon.currentAmmo,currentWeapon.reserveAmmo);
        }
        private void Start()
        {
                
        }
        private void UpdateAmmo(int currentAmmo, int reserveAmmo)
        {
                ammoText.text = $"{currentAmmo} / {reserveAmmo}";
        }
        private void UpdateHP(int currentHP)
        {
                HPText.text = $"{currentHP}";
        }
}

