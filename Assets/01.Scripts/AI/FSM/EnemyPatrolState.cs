using UnityEngine;
using UnityEngine.AI;
public class EnemyPatrolState:IState
{
    private AIController aiController;
    private NavMeshAgent navMeshAgent;
    private NavMeshHit hit;
    private Vector3 direction;
    private float maxDistance = 15f;
    private float minDistance = 5f;
    private float randomDistance;
    public EnemyPatrolState(AIController controller)
    {
        aiController = controller;
        navMeshAgent = aiController.agent;
    }
    public void Enter()
    {
        aiController.StartDetectTarget();
        direction = Random.insideUnitSphere;
        direction.y = 0;
        randomDistance = Random.Range(minDistance, maxDistance);
        direction = aiController.transform.position + direction * randomDistance;
        if (NavMesh.SamplePosition(direction, out hit, randomDistance, NavMesh.AllAreas)) 
        {
            navMeshAgent.SetDestination(hit.position);
        }
    }
    public void Update()
    {
        if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) 
        {
            direction = Random.insideUnitSphere;
            direction.y = 0;
            randomDistance = Random.Range(minDistance, maxDistance);
            direction = aiController.transform.position + direction * randomDistance;
            
            if (NavMesh.SamplePosition(direction, out hit, randomDistance, NavMesh.AllAreas)) 
            {
                navMeshAgent.SetDestination(hit.position);
            }
        }
    }
    public void Exit()
    {
        aiController.StopAllCoroutines();
    }
}
