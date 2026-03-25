using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputHandler:NetworkBehaviour
{
    [Header("Input")]
    public Vector2 Move{get; private set;}
    public Vector2 Look{get; private set;}
    public bool Fire{get; private set;}
    public bool Reload{get; private set;}
    public bool Crouch{get; private set;}
    public bool Jump{get; private set;}
    public bool Walk{get; private set;}

    InputSystem_Actions actions;
    
    public event Action<int> OnSlotChanged;
    public event Action OnWeaponDrop;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        actions.Player.Enable();
        
    }

    private void Awake()
    {
        actions = new InputSystem_Actions();
        
        RegisterInputActions();
    }
   
    private void RegisterInputActions()
    {
        actions.Player.Crouch.performed += ctx => Crouch = true;
        actions.Player.Jump.performed += ctx => Jump = true;
        actions.Player.Walk.performed += ctx => Walk = true;
        actions.Player.Attack.performed += ctx => Fire = true;
        actions.Player.Reload.performed += ctx => Reload = true;
        
        actions.Player.Crouch.canceled += ctx => Crouch = false;
        actions.Player.Jump.canceled += ctx => Jump = false;
        actions.Player.Walk.canceled += ctx => Walk = false;
        actions.Player.Attack.canceled += ctx => Fire = false;
        actions.Player.Reload.canceled += ctx => Reload = false;
        
        actions.Player.Slot1.performed += ctx => OnSlotChanged?.Invoke(0);
        actions.Player.Slot2.performed += ctx => OnSlotChanged?.Invoke(1);
        actions.Player.Slot3.performed += ctx => OnSlotChanged?.Invoke(2);
        
        actions.Player.Drop.performed += ctx => OnWeaponDrop?.Invoke();
    }

    private void Update()
    {
        Move = actions.Player.Move.ReadValue<Vector2>();
        Look = actions.Player.Look.ReadValue<Vector2>();
    }

    private void OnDestroy()
    {
        actions.Disable();
        actions.Dispose();
    }
}
