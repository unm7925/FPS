using UnityEngine;
public class PlayerMoveState:IState
{
    private PlayerController controller;
    private PlayerInputHandler playerInputHandler;
    private Animator animator;
    public PlayerMoveState(PlayerController _controller ,PlayerInputHandler _playerInputHandler, Animator _animator)
    {
        controller = _controller;
        playerInputHandler = _playerInputHandler;
        animator = _animator;
    }
    public void Enter()
    {
        
    }
    public void Update()
    {
        controller.OnMove(playerInputHandler.Move);
        animator.SetFloat("Speed", controller.GetCurrentSpeed());
    }
    public void Exit()
    {
        animator.SetFloat("Speed", 0);
    }
}
