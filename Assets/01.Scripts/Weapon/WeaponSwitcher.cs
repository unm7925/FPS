using System;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private WeaponBase[] slots;
    [SerializeField] private Transform weaponHolder;

    private WeaponBase currentWeapon;
    private int currentSlotIndex;
    private PlayerInputHandler playerInputHandler;
    private Animator animator;

    private void Awake()
    {
        playerInputHandler = GetComponentInParent<PlayerInputHandler>();
        currentWeapon = GetComponentInChildren<WeaponBase>();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        playerInputHandler.OnSlotChanged += SwitchWeapon;
    }

    private void OnDisable()
    {
        playerInputHandler.OnSlotChanged -= SwitchWeapon;
    }

    private void Start()
    {
        currentSlotIndex = 1;
        
    }

    private void SwitchWeapon(int _slotIndex)
    {
        if (slots[_slotIndex] == currentWeapon || slots[_slotIndex] == null || currentWeapon == null) return;
        
        currentWeapon.gameObject.SetActive(false);
        currentWeapon = slots[_slotIndex];
        currentWeapon.gameObject.SetActive(true);
        currentSlotIndex = _slotIndex;

    }
    public void PickupWeapon(WeaponBase _weapon)
    {
        int index = (int)_weapon.weaponType;
        if (slots[index] != null) return;
        slots[index] = _weapon;
        _weapon.transform.SetParent(weaponHolder);
    }

    private void DropWeapon()
    {
        currentWeapon.gameObject.AddComponent<Rigidbody>();
        currentWeapon.gameObject.transform.SetParent(null);
        slots[currentSlotIndex] = null;
        currentWeapon = null;
    }
}
