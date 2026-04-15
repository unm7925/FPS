using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class HUDController:MonoBehaviour
{
        private HP hp;
        private WeaponSwitcher weaponSwitcher;
        
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI HPText;
        [SerializeField] private TextMeshProUGUI roundScoreText;

        private WeaponBase currentWeapon;

        private void OnEnable()
        {
                PlayerController.OnLocalPlayerSpawned += Init;
                roundManager.OnScoreUpdated += UpdateRoundScore;
        }

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

        private void UpdateRoundScore(int teamAScore, int teamBScore)
        {
                roundScoreText.text = $"{teamAScore} : {teamBScore}";
        }
        private void EventConnection()
        {
                hp.OnHPChanged += UpdateHP;
                weaponSwitcher.OnWeaponChanged += SwitchWeaponEvent;
        }
        
        private void OnDisable()
        {
                PlayerController.OnLocalPlayerSpawned -= Init;
                if(hp !=null)
                        hp.OnHPChanged -= UpdateHP;
                if(weaponSwitcher !=null)
                        weaponSwitcher.OnWeaponChanged -= SwitchWeaponEvent;
                if (currentWeapon != null) 
                        currentWeapon.OnAmmoChanged -= UpdateAmmo;
                
                roundManager.OnScoreUpdated -= UpdateRoundScore;
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

