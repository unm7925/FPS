using System;
using System.Collections.Generic;
using UnityEngine;
public class SpawnManager: MonoBehaviour
{
        [SerializeField] RoundManager roundManager;
        [SerializeField] HUDController hudController;
        [SerializeField] CrossHairController crossHairController;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform spawnBTeam;
        [SerializeField] private Transform spawnATeam;
        
        ObjectPool<PlayerController> playerPool = new ObjectPool<PlayerController>();
        ObjectPool<AIController> aiPool = new ObjectPool<AIController>();
        
        List<AIController> teamBList = new List<AIController>();
        List<PlayerController> teamAList = new List<PlayerController>();

        public event Action OnSpawnComplete;
        
        private Vector3 spawnPosPlus = Vector3.right;
        // 아직 멀티 구현안해서 적만 소환
        private int playerCount;

        private bool IsSet;

        private void Awake()
        {
                playerCount = GameManager.Instance.matchData.playersPerTeam;
                
                AIController aiController = enemyPrefab.GetComponent<AIController>();
                aiPool.Init(aiController,playerCount,spawnBTeam);
                
                PlayerController playerController = playerPrefab.GetComponent<PlayerController>();
                playerPool.Init(playerController,playerCount,spawnATeam);
                
                
        }
        private void Start()
        {
                
        }

        private void OnEnable()
        {
                roundManager.OnRoundEnd += Despawn;
                roundManager.OnRoundStart += Spawn;
        }
        private void OnDisable()
        {
                roundManager.OnRoundEnd -= Despawn;
                roundManager.OnRoundStart -= Spawn;
        }
        private void Spawn()
        {
                for (int i = 0; i < playerCount; i++) 
                {
                        AIController go = aiPool.Get();
                        go.transform.position = spawnBTeam.position;
                        HP hp = go.GetComponent<HP>();
                        hp.Init();
                        GameManager.Instance.RegisterTeam(go.gameObject, GameManager.Team.TeamB);
                        go.gameObject.transform.SetParent(null);
                        teamBList.Add(go);
                }
                for (int i = 0; i < 1; i++) 
                {
                        PlayerController player = playerPool.Get();
                        player.transform.position = spawnATeam.position;
                        HP hp = player.GetComponent<HP>();
                        hp.Init();
                        GameManager.Instance.RegisterTeam(player.gameObject, GameManager.Team.TeamA);
                        player.gameObject.transform.SetParent(null);
                        teamAList.Add(player);
                        if (IsSet == false) 
                        {
                                hudController.Init(player.GetComponent<HP>(),player.GetComponentInChildren<WeaponSwitcher>());
                                crossHairController.Init(player.GetComponentInChildren<WeaponSwitcher>());
                                StatsManager.Instance.RegisterPlayer(player.gameObject);
                                IsSet = true;
                        }
                }
                
                OnSpawnComplete?.Invoke();
        }

        private void Despawn()
        {
                foreach (PlayerController player in teamAList) 
                {
                        playerPool.Return(player);        
                }
                foreach (AIController ai in teamBList)
                {
                        aiPool.Return(ai);
                }
                teamAList.Clear();
                teamBList.Clear();
                GameManager.Instance.ClearTeams();
        }
}
