using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
public class AIController:MonoBehaviour
{
        private EnemyStateMachine enemyStateMachine;
        private Animator animator;
        private NavMeshAgent agent;
        private EnemySight enemySight;

        private void Awake()
        {
                enemyStateMachine = GetComponent<EnemyStateMachine>();
                agent = GetComponent<NavMeshAgent>();
                animator = GetComponent<Animator>();
                enemySight = GetComponent<EnemySight>();
        }

        // 적 발견 시 패트롤로 전환 ( 레이 쏘기 , 장애물 있으면 인식x )
        // 패트롤 랜덤 갔다가 idle ( 주변 둘러보기 시야각 쿼터니언 돌리기 천천히 )
        // 
}

