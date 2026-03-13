using UnityEngine;
public class EnemySearchState:IState
{
    private AIController aiController;
    private float rotateSpeed = 10f;
    private float searchDuration = 5f;
    public EnemySearchState(AIController controller)
    {
        aiController = controller;
    }
    public void Enter()
    {
        aiController.StartSearchRotate(rotateSpeed, searchDuration);
        aiController.StartDetectTarget();
    }
    public void Update()
    {
        
    }
    public void Exit()
    {
        aiController.StopAllCoroutines();
    }
}
