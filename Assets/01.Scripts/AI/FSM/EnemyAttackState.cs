public class EnemyAttackState:IState
{
    private AIController aiController;
    public EnemyAttackState(AIController controller)
    {
        aiController = controller;
    }
    public void Enter()
    {
        aiController.FireAndStrafe();
    }
    public void Update()
    {
        
    }
    public void Exit()
    {
        aiController.StopAllCoroutines();
    }
}
