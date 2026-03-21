using System;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairController : MonoBehaviour
{
    private WeaponSwitcher weaponSwitcher;
    [SerializeField] private RectTransform[] crossHair;
    private Vector2[] directions = {Vector2.up, Vector2.down,Vector2.right,  Vector2.left};
    private float baseOffset = 12f;
    private float spreadScale = 100f;
    
    private WeaponBase weaponBase;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Init(WeaponSwitcher _weaponSwitcher)
    {
        weaponSwitcher = _weaponSwitcher;
        
        weaponSwitcher.OnWeaponChanged += SetCrossHair;
        weaponBase = weaponSwitcher.currentWeapon;
    }
    

    private void OnDisable()
    {
        weaponSwitcher.OnWeaponChanged -= SetCrossHair;
    }

    private void Update()
    {
        if (weaponBase == null) return;
        SetSpread(weaponBase.finalSpread);
    }
    private void SetSpread(float spread)
    {
        for (int i = 0; i < crossHair.Length; i++) 
        {
            crossHair[i].anchoredPosition = directions[i] * (baseOffset + spread * spreadScale);
        }
    }
    private void SetCrossHair(WeaponBase obj)
    {
        weaponBase = obj;
    }



}
