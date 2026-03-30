using System;
using Mirror;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : NetworkBehaviour
{
    [SyncVar] public GameManager.Team myTeam;
    
    [SerializeField] private float sensitivity;

    [SerializeField] private Transform cameraMount;

    public static event Action<HP, WeaponSwitcher> OnLocalPlayerSpawned;
    
    private Animator animator;

    private WeaponSwitcher weaponSwitcher;
    private PlayerStateMachine stateMachine;
    private PlayerInputHandler inputHandler;
    private CharacterController controller;
    private HP hp;
    private GunWeapon currentWeapon;

    Vector3 velocity = Vector3.zero;
    Vector3 horizontalVelocity = Vector3.zero;

    private bool isCrunch;
    private bool isWalk;
    private bool prevCrouch = false;
    private bool isJump;
    
    private float crunchHeight = 1.5f;
    private Vector3 crunchCenter = new Vector3(0,-0.25f,0);
    private Vector3 crunchCamPos = new Vector3(0,2f,0);
    
    private Vector3 standCamPos = new Vector3(0,3f,0);
    private float standHeight;
    
    private float jumpForce = 7f;
    private float crouchJumpMult = 1.2f;
    
    private float speed = 10f;
    private float crouchSpeedMul = 0.3f;
    private float walkSpeedMul = 0.5f;
    
    private float fallMult = 1f;
    private float lowJumpMult = 1.5f;
    private float airControlMult = 0.3f;

    public void OnMove(Vector2 input) => Move(input);
    
    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        inputHandler = GetComponent<PlayerInputHandler>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();
        hp = GetComponent<HP>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        CameraManager.Instance.SetViewTarget(cameraMount);
        
        StandardSet();
        
        Hitbox[] hitboxes = GetComponentsInChildren<Hitbox>();
        foreach (Hitbox hitbox in hitboxes) 
        {
            hitbox.gameObject.layer = LayerMask.NameToLayer("SelfHitbox");
        }
        OnLocalPlayerSpawned?.Invoke(hp,weaponSwitcher);
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        hp.OnDie += UnregisterPlayer;
        weaponSwitcher.OnWeaponChanged += HandleWeaponChanged;
    }
    
    private void OnDisable()
    {
        hp.OnDie -=  UnregisterPlayer;
        weaponSwitcher.OnWeaponChanged -= HandleWeaponChanged;
    }
    
    private void UnregisterPlayer(GameObject go)
    {
        GameManager.Instance.UnRegisterEnemies(myTeam, go);
    }
    
    private void HandleWeaponChanged(WeaponBase obj)
    {
        if(currentWeapon != null) 
            currentWeapon.OnDamageDealt -= CmdApplyDamage;
        
        currentWeapon = obj as GunWeapon;
        
        if(currentWeapon != null)
            currentWeapon.OnDamageDealt += CmdApplyDamage;
    }
    
    void Update()
    {
        if (!isLocalPlayer) return;
        
        if (animator == null) return;
        GetCurrentBool();
        if (prevCrouch != isCrunch) 
        {
            IsCrunch();
            prevCrouch = isCrunch;
        }
       
        
        Gravity();
        Jump();

        if (weaponSwitcher.currentWeapon == null) return;
        if (inputHandler.Fire) 
        {
            weaponSwitcher.currentWeapon.Fire();
            
        }
        if (inputHandler.Reload) 
        {
            weaponSwitcher.currentWeapon.Reload();
        }
        
        weaponSwitcher.currentWeapon.SetSpreadState(GetCurrentState(),inputHandler.Move.magnitude);
        
        
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer) return;
        LookYaw();
    }
    void StandardSet()
    {
        standHeight = controller.height;
        CameraManager.Instance.UpdateTargetPos(standCamPos);
    }

   

    void IsCrunch()
    {
        if (isCrunch) 
        {
            controller.height = crunchHeight;
            controller.center = crunchCenter;
            CameraManager.Instance.UpdateTargetPos(crunchCamPos);
        }
        else 
        {
            controller.height = standHeight;
            controller.center = Vector3.zero;
            CameraManager.Instance.UpdateTargetPos(standCamPos);
        }
    }

    private WeaponBase.SpreadState GetCurrentState()
    {
        if (!controller.isGrounded) return WeaponBase.SpreadState.Jump;
        if (isCrunch) return WeaponBase.SpreadState.Crouch;
        return WeaponBase.SpreadState.Idle;
    }

    void Move(Vector2 input)
    {
        
        float x = input.x;
        float z = input.y;
        
        Vector3 movement = transform.TransformDirection(new Vector3(x, -2f, z));
        
        float currentSpeed = GetCurrentSpeed();
        if(controller.isGrounded) 
        {
            controller.Move(movement * (currentSpeed * Time.deltaTime));
            horizontalVelocity = movement * currentSpeed;
        }
        else 
        {
            horizontalVelocity += movement * airControlMult;
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, currentSpeed);
        }
        
    }
    void GetCurrentBool()
    {
        isCrunch = inputHandler.Crouch;
        animator.SetBool("IsCrouching", isCrunch);
        isWalk = inputHandler.Walk;
        
    }

    public float GetCurrentSpeed()
    {
        float curentSpeed = speed;
        if (isCrunch) 
        {
            curentSpeed  *= crouchSpeedMul;
            return curentSpeed;
        }
        else if (isWalk) {
            curentSpeed *= walkSpeedMul;
            return curentSpeed;
        }
        else 
        {
            return curentSpeed;
        }
    }

    float GetJumpForce()
    {
        float currentJumpForce = jumpForce;
        if(isCrunch)
        {
            return currentJumpForce * crouchJumpMult;
        }
        else 
        {
            return currentJumpForce;
        }
    }

    void Jump()
    {
        if(inputHandler.Jump && controller.isGrounded) 
        {
            velocity.y = GetJumpForce();
            isJump = true;
            animator.SetTrigger("IsJumped");
        }
        
    }

    void Gravity()
    {
        animator.SetBool("IsGrounded", controller.isGrounded);
        if (velocity.y < 0) 
        {
            velocity.y += Physics.gravity.y * Time.deltaTime* fallMult;         
        }
        else if(velocity.y > 0 && !inputHandler.Jump) 
        {
                velocity.y += Physics.gravity.y * Time.deltaTime* lowJumpMult;
        }
        else 
        {
                velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        
        velocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
        controller.Move(velocity * Time.deltaTime);
        
        if (controller.isGrounded) 
        {
            velocity.y = -2f;
            
            horizontalVelocity = Vector3.zero;
            if(isJump) 
            {
                animator.SetTrigger("IsLanded");
                isJump = false;
            }
        }
        
    }
    void LookYaw()
    {
        CameraManager.Instance.AddPitch(inputHandler.Look.y);
        transform.Rotate(Vector3.up, inputHandler.Look.x * sensitivity);
    }

    [Command]
    private void CmdApplyDamage(uint targetNetId, int damage)
    {
        GameObject attcker = connectionToClient.identity.gameObject;
        HP target = NetworkServer.spawned[targetNetId].gameObject.GetComponent<HP>();
        target.TakeDamage(damage,attcker);
    }
}
