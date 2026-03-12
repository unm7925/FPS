using System;
using UnityEngine;
public class DroppedWeapon:MonoBehaviour
{
    private WeaponBase weaponBase;

    private void Awake()
    {
        weaponBase = GetComponent<WeaponBase>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            WeaponSwitcher weaponSwitcher = other.GetComponentInChildren<WeaponSwitcher>();
            weaponSwitcher.PickupWeapon(weaponBase);
            DestroyComponent();
        }
    }
    private void DestroyComponent()
    {
        Destroy(GetComponent<Rigidbody>());
        foreach (var col in GetComponents<Collider>()) 
        {
            Destroy(col);    
        }
        
        Destroy(this);
    }
}
