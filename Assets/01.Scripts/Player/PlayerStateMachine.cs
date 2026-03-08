using System;
using UnityEngine;
public class PlayerStateMachine:MonoBehaviour
{
    private PlayerInputHandler actions;
    
    [Header("State")]
    public PlayerIdleState IdleState {get; private set;}
    public PlayerMoveState MoveState{get; private set;}
    public PlayerAttackState AttackState{get; private set;}
    public PlayerJumpState JumpState{get; private set;}
    public PlayerWalkState WalkState{get; private set;}
    public PlayerCrouchState CrouchState{get; private set;}
    
    IState currentState;
    
    public IState CurrentState => currentState;

    private void Awake()
    {
        actions = GetComponent<PlayerInputHandler>();
        SetState(actions);
        
        ChangeState(IdleState);
    }
    
    private void SetState(PlayerInputHandler action)
    {
        IdleState = new PlayerIdleState(action);
        MoveState = new PlayerMoveState(action);
        AttackState = new PlayerAttackState(action);
        JumpState = new PlayerJumpState(action);
        WalkState = new PlayerWalkState(action);
        CrouchState = new PlayerCrouchState(action);
    }
    
    private void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(IState newStateMachine)
    {
        currentState?.Exit();
        currentState = newStateMachine;
        currentState?.Enter();
    }
}

