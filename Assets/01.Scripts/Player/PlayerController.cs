using System;
using System.Numerics;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float sensitivity;
    private float xRotation=0;
    
    
    private PlayerStateMachine stateMachine;
    private PlayerInputHandler inputHandler;
    private CharacterController controller;

    Vector3 velocity = Vector3.zero;
    private float jumpForce = 7f;
    private float speed = 10f;
    private float fallMult = 1f;
    private float lowJumpMult = 1.5f;
    
    
    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        inputHandler = GetComponent<PlayerInputHandler>();
        controller = GetComponent<CharacterController>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        Move();
        Gravity();
        Jump();
    }

    private void LateUpdate()
    {
        Look();
    }

    void Move()
    {
        float x = inputHandler.Move.x;
        float z = inputHandler.Move.y;
        Vector3 movement = transform.TransformDirection(new Vector3(x, 0, z));
        controller.Move(movement * (speed * Time.deltaTime));
    }

    public void Jump()
    {
        if(inputHandler.Jump && controller.isGrounded) 
        {
            velocity.y = jumpForce;
        }
    }

    void Gravity()
    {
        if(!controller.isGrounded) 
        {
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
            controller.Move(velocity * Time.deltaTime);
        }
        else 
        {
            velocity.y = 0;
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
