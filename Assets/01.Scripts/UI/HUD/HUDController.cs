using System;
using TMPro;
using UnityEngine;
public class HUDController:MonoBehaviour
{
        [SerializeField] private HP hp;
        [SerializeField] private WeaponSwitcher weaponSwitcher;
        
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI HPText;

        private WeaponBase currentWeapon;
        private void OnEnable()
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
                SwitchWeaponEvent(weaponSwitcher.currentWeapon);
                UpdateHP(hp.maxHP);
                UpdateAmmo(currentWeapon.currentAmmo,currentWeapon.reserveAmmo);
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

