using System;
using UnityEngine;
public class EnemyStateMachine:MonoBehaviour
{
    private AIController aiController;
    
    public EnemyAttackState enemyAttackState {get;private set;}
    public EnemyIdleState enemyIdleState {get;private set;}
    public EnemyMoveState enemyMoveState {get;private set;}
    public EnemyPatrolState enemyPatrolState {get;private set;}
    public EnemySearchState enemySearchState {get;private set;}
    
    public IState currentState {get;private set;}

    private void Awake()
    {
        aiController = GetComponent<AIController>();
        Initialize();
    }
    private void Start()
    {
        ChangeState(enemyPatrolState);
    }

    private void Update()
    {
        currentState?.Update();
    }

    private void Initialize()
    {
        enemyPatrolState = new EnemyPatrolState(aiController);
        enemyAttackState = new EnemyAttackState(aiController);
        enemyIdleState = new EnemyIdleState(aiController);
        enemyMoveState = new EnemyMoveState(aiController);
        enemySearchState = new EnemySearchState(aiController);
    }

    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
}

