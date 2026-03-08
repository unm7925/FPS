using UnityEngine;
public class PlayerIdleState:IState
{
    private PlayerController controller;
    private PlayerInputHandler playerInputHandler;
    private Animator animator;
    public PlayerIdleState(PlayerController _controller ,PlayerInputHandler _playerInputHandler, Animator _animator)
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
        
    }
    public void Exit()
    {
        
    }
}
