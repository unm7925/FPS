using System;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private WeaponBase[] slots;
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private WeaponData defaultWeaponData;
    private RuntimeAnimatorController defultController;

    public event Action<WeaponBase> OnWeaponChanged;
    
    public WeaponBase currentWeapon{get; private set;}
    private int currentSlotIndex;
    private PlayerInputHandler playerInputHandler;
    private Animator animator;

    private void Awake()
    {
        playerInputHandler = GetComponentInParent<PlayerInputHandler>();
        animator = GetComponent<Animator>();
        defultController = animator.runtimeAnimatorController;
        
        DefaultWeaponSet();
    }
    private void OnEnable()
    {
        playerInputHandler.OnSlotChanged += SwitchWeapon;
        playerInputHandler.OnWeaponDrop += DropWeapon;
    }

    private void OnDisable()
    {
        playerInputHandler.OnSlotChanged -= SwitchWeapon;
        playerInputHandler.OnWeaponDrop -= DropWeapon;
    }

    private void Start()
    {
        OnWeaponChanged?.Invoke(currentWeapon);
    }

    private void DefaultWeaponSet()
    {
        GameObject weapon = Instantiate(defaultWeaponData.prefab, weaponHolder);
        currentWeapon = weapon.GetComponent<WeaponBase>();
        currentWeapon.ApplyAnimator();
        currentSlotIndex = (int)currentWeapon.weaponType;
        slots[currentSlotIndex] = currentWeapon;
    }

    private void SwitchWeapon(int _slotIndex)
    {
        if (slots[_slotIndex] == currentWeapon || slots[_slotIndex] == null) return;
        
        if(currentWeapon != null) 
        {
            currentWeapon.CancelAct();
            currentWeapon.gameObject.SetActive(false);
        }
        currentWeapon = slots[_slotIndex];
        currentWeapon.ApplyAnimator();
        currentWeapon.gameObject.SetActive(true);
        currentSlotIndex = _slotIndex;
        
        OnWeaponChanged?.Invoke(currentWeapon);
    }
    public void PickupWeapon(WeaponBase _weapon)
    {
        int index = (int)_weapon.weaponType;
        if (slots[index] != null) return;
        slots[index] = _weapon;
        _weapon.transform.SetParent(weaponHolder);
        _weapon.ChangeTrans();
        
        if (currentWeapon == null) 
        {
            currentWeapon = _weapon;
            OnWeaponChanged?.Invoke(currentWeapon);
            currentWeapon.ApplyAnimator();
        }
        else 
        {
            _weapon.gameObject.SetActive(false);
        }
    }

    private void DropWeapon()
    {
        if (currentWeapon == null) return; 
        DropSet();
        slots[currentSlotIndex] = null;
        
        currentWeapon.CancelAct();
        currentWeapon = null;
        
        for (int i = 0; i < slots.Length; i++) 
        {
            if(slots[i] != null) 
            {
                SwitchWeapon(i);
                return;
            }
        }
        animator.runtimeAnimatorController = defultController;
        OnWeaponChanged?.Invoke(null);
    }

    private void DropSet()
    {
        currentWeapon.gameObject.transform.SetParent(null);
        currentWeapon.transform.position = transform.position + transform.forward * 1f;
        currentWeapon.gameObject.AddComponent<Rigidbody>();
        currentWeapon.gameObject.AddComponent<BoxCollider>();
        SphereCollider collider = currentWeapon.gameObject.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = 1f;
        currentWeapon.gameObject.AddComponent<DroppedWeapon>();
    }
}
