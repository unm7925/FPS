using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float sensitivity;
    
    private Animator animator;
    
    private float xRotation=0;

    private WeaponSwitcher weaponSwitcher;
    private PlayerStateMachine stateMachine;
    private PlayerInputHandler inputHandler;
    private CharacterController controller;
    private HP hp;

    Vector3 velocity = Vector3.zero;
    Vector3 horizontalVelocity = Vector3.zero;

    private bool isCrunch;
    private bool isWalk;
    private bool prevCrouch = false;
    private bool isJump;
    
    private float crunchHeight = 1.5f;
    private Vector3 crunchCenter = new Vector3(0,-0.25f,0);
    private Vector3 crunchCamPos = new Vector3(0,1f,0);

    private Vector3 targetCamPos = Vector3.zero;
    private Vector3 currentCamPos = Vector3.zero;

    private float cameraSpeed = 5f;
    
    private Vector3 standCamPos;
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

    private void Start()
    {
        GameManager.Instance.RegisterTeam(gameObject, GameManager.Team.TeamA);
        StandardSet();
    }

    private void OnEnable()
    {
        hp.OnDie += UnregisterPlayer;
    }
    private void OnDisable()
    {
        hp.OnDie -=  UnregisterPlayer;
    }
    private void UnregisterPlayer()
    {
        GameManager.Instance.UnRegisterEnemies(GameManager.Team.TeamA, gameObject);
    }

    void Update()
    {
        UpdateCameraPos();

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
        Look();
    }
    void StandardSet()
    {
        standHeight = controller.height;
        standCamPos = cam.transform.localPosition;
    }

    void UpdateCameraPos()
    {
        if (targetCamPos == currentCamPos) return;
        currentCamPos = cam.transform.localPosition;
        cam.transform.localPosition = Vector3.Lerp(currentCamPos, targetCamPos, cameraSpeed * Time.deltaTime);
    }

    void IsCrunch()
    {
        if (isCrunch) 
        {
            controller.height = crunchHeight;
            controller.center = crunchCenter;
            targetCamPos = crunchCamPos;
        }
        else 
        {
            controller.height = standHeight;
            controller.center = Vector3.zero;
            targetCamPos =  standCamPos;
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
    void Look()
    {
        xRotation -= (inputHandler.Look.y * sensitivity);
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        transform.Rotate(Vector3.up, inputHandler.Look.x * sensitivity);
    }
}
