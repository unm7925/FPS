using UnityEngine;
public class EnemyStrafState:IState
{
    
    private AIController aiController;
    private float strafeTime = 2f;
    private float strafeDistance = 5f;
    public EnemyStrafState(AIController controller)
    {
        
        aiController = controller;
        
    }
    public void Enter()
    {
        aiController.StartStrafeMove(strafeDistance, strafeTime);
    }
    public void Update()
    {
        
    }
    public void Exit()
    {
        aiController.StopAllCoroutines();
    }
}
