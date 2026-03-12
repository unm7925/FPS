using System;
using TMPro;
using UnityEngine;
public class HUDController:MonoBehaviour
{
        [SerializeField] private HP hp;
        [SerializeField] private WeaponBase weaponBase;
        
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI HPText;

        private void OnEnable()
        {
                hp.OnHPChanged += UpdateHP;
                weaponBase.OnAmmoChanged += UpdateAmmo;
        }
        private void Start()
        {
                UpdateHP(hp.maxHP);
                UpdateAmmo(weaponBase.currentAmmo,weaponBase.reserveAmmo);
        }
        private void OnDisable()
        {
                hp.OnHPChanged -= UpdateHP;
                weaponBase.OnAmmoChanged -= UpdateAmmo;
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

