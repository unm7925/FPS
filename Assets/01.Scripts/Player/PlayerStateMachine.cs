using System;
using UnityEngine;
public class PlayerStateMachine:MonoBehaviour
{
    private PlayerInputHandler actions;
    private PlayerController controller;
    private Animator animator;

    private float deadZone = 0.3f;
    
    public PlayerIdleState IdleState {get; private set;}
    public PlayerMoveState MoveState{get; private set;}
    
    public IState CurrentState {get; private set;}

    private void Awake()
    {
        GetComponents();
        
        SetState(controller,actions,animator);
        
        ChangeState(IdleState);
    }

    private void GetComponents()
    {
        actions = GetComponent<PlayerInputHandler>();
        controller = GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
    }
    
    private void SetState(PlayerController _controller, PlayerInputHandler _action,  Animator _animator)
    {
        IdleState = new PlayerIdleState(_controller, _action,_animator);
        MoveState = new PlayerMoveState(_controller, _action,_animator);
    }
    
    private void Update()
    {
        CurrentState?.Update();
        ChangeMovementState();
    }

    private void ChangeState(IState newStateMachine)
    {
        CurrentState?.Exit();
        CurrentState = newStateMachine;
        CurrentState?.Enter();
    }

    private void ChangeMovementState()
    {
        if(Mathf.Abs(actions.Move.x) <= deadZone && Mathf.Abs(actions.Move.y) <= deadZone) 
        {
            if (CurrentState == IdleState) return;
            ChangeState(IdleState);
        }
        else 
        {
            if (CurrentState == MoveState) return;
            ChangeState(MoveState);
        }
    }
}

