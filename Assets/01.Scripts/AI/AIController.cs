using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public enum BotDifficulty
{
        Easy,
        Normal,
        Hard
}
public class AIController : MonoBehaviour, IPoolable
{
        private EnemyStateMachine enemyStateMachine;
        public Animator animator {get; private set;}
        public NavMeshAgent agent {get; private set;}
        [SerializeField] private BotDifficulty botDifficulty;

        private BotGunWeapon currentWeapon;
        
        private EnemySight enemySight;

        private HP hp;

        private float sightInterval = 0.2f;
        
        public GameObject currentTarget {get; private set;}

        private void Awake()
        {
                enemyStateMachine = GetComponent<EnemyStateMachine>();
                agent = GetComponent<NavMeshAgent>();
                animator = GetComponent<Animator>();
                enemySight = GetComponent<EnemySight>();
                currentWeapon = GetComponentInChildren<BotGunWeapon>();
                hp = GetComponent<HP>();
                
        }
        private void OnEnable()
        {
                hp.OnDie += UnRegisterPlayer;
                StartDetectTarget();
        }
        private void OnDisable()
        {
                hp.OnDie -= UnRegisterPlayer;
        }
        private void UnRegisterPlayer(GameObject go)
        {
                GameManager.Instance.UnRegisterEnemies(GameManager.Team.TeamB, go);
        }
        private void Start()
        {
               
        }
        private void Update()
        {
                UpdateAnimator();
                if (currentTarget != null) 
                {
                        transform.LookAt(currentTarget.transform.position);
                        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                        
                }
        }

        private void UpdateAnimator()
        {
                Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
                localVelocity /= agent.speed;
                localVelocity *= 7f;
                animator.SetFloat("Speed", localVelocity.z,0.1f,Time.deltaTime);
                animator.SetFloat("Direction", localVelocity.x,0.1f,Time.deltaTime);
        }
        public void StartStrafeMove(float strafeDistance, float strafeTime)
        {
                if (!gameObject.activeInHierarchy) return;
                if (currentTarget == null) return;
                StartCoroutine(StrafeMove(strafeDistance,strafeTime));
        }
        private IEnumerator StrafeMove(float strafeDistance, float strafeTime)
        {
                if(!agent.isActiveAndEnabled) yield break;
                agent.isStopped = false;
                agent.updateRotation = false;
                int moveX = Random.Range(0, 2) == 0 ? 1 : -1;
                Vector3 direction = currentTarget.transform.position - transform.position;
                Vector3 move = Vector3.Cross(direction, Vector3.up).normalized;
                agent.SetDestination(transform.position + move * strafeDistance * moveX);
                
                yield return new WaitForSeconds(strafeTime);
                if(!agent.isActiveAndEnabled) yield break;
                LoseTarget();
        }

        public void StartDetectTarget()
        {
                StartCoroutine(DetectTarget());
        }
        private IEnumerator DetectTarget()
        {
                while (true) 
                {
                        foreach (var teamA in GameManager.Instance.GetEnemeies(GameManager.Team.TeamB).ToList()) 
                        {
                                if (enemySight.CanSeeTarget(teamA)) 
                                {
                                        currentTarget = teamA;
                                        currentWeapon.SetTarget(currentTarget.transform,botDifficulty);
                                        enemyStateMachine.ChangeState(enemyStateMachine.enemyAttackState);
                                        yield break;
                                }
                        }
                        yield return new WaitForSeconds(sightInterval);
                }
        }
        public void FireAndStrafe()
        {
                agent.isStopped = true;
                currentWeapon.Fire();
                enemyStateMachine.ChangeState(enemyStateMachine.EnemyStrafState);
        }



        private void LoseTarget()
        {
                
                if (currentTarget == null) 
                {
                        enemyStateMachine.ChangeState(enemyStateMachine.enemyPatrolState);
                        agent.updateRotation = true;
                        return;
                }

                if (!enemySight.CanSeeTarget(currentTarget)) 
                {
                        currentTarget = null;
                        enemyStateMachine.ChangeState(enemyStateMachine.enemySearchState);
                        agent.updateRotation = true;
                        return;
                }
                enemyStateMachine.ChangeState(enemyStateMachine.enemyAttackState);
        }
        
        public void StartSearchRotate(float speed, float time)
        {
                StartCoroutine(SearchRotate(speed, time));
        }
        
        private IEnumerator SearchRotate(float speed, float time)
        {
                while (true) 
                {
                        time -= Time.deltaTime;
                        if (time <= 0) 
                        {
                                enemyStateMachine.ChangeState(enemyStateMachine.enemyPatrolState);
                                yield break;
                        }
                        
                        transform.Rotate(Vector3.up, speed * Time.deltaTime);
                        yield return null;
                }
        }
        public void OnSpawn()
        {
                currentTarget = null;
                enemyStateMachine.ChangeState(enemyStateMachine.enemyPatrolState);
                agent.isStopped = false;
                agent.updateRotation = true;
                hp.Init();
                
        }
        public void OnReturn()
        {
                StopAllCoroutines();
                if (!agent.isActiveAndEnabled || !agent.isOnNavMesh) return;
                agent.isStopped = true;
                agent.ResetPath();
        }
}

