using System;
using System.Collections.Generic;
using Mirror;
using Mirror.BouncyCastle.Bcpg;
using UnityEngine;
public class SpawnManager: MonoBehaviour
{
        [SerializeField] RoundManager roundManager;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform spawnBTeam;
        [SerializeField] private Transform spawnATeam;
        
        public RoundManager RoundManager => roundManager;
        public Transform SpawnATeam => spawnATeam;
        public Transform SpawnBTeam => spawnBTeam;

        private AIController aiController;

        private bool poolInit = false;

        private bool isPlayer;
        
        ObjectPool<AIController> aiPool = new ObjectPool<AIController>();
        
        List<AIController> teamBList = new List<AIController>();

        public event Action OnSpawnComplete;
        // 아직 멀티 구현안해서 적만 소환
        private int playerCount;
        
        private void Awake()
        {
                var customManager = (NetworkManager.singleton as CustomNetworkManager);
                if (customManager != null) 
                {
                        customManager.RegisterSpawnManager(this);
                }
                
                playerCount = GameManager.Instance.matchData.playersPerTeam;
                
                aiController = enemyPrefab.GetComponent<AIController>();
                
        }

        private void OnEnable()
        {
                roundManager.OnRoundEnd += Despawn;
                roundManager.OnRoundStart += Spawn;
                roundManager.OnRoundReady += UnlockPlayers;
        }
        private void UnlockPlayers()
        {
                foreach (var v in (NetworkManager.singleton as CustomNetworkManager).playerObjs)
                {
                        v.Value.GetComponent<PlayerController>().SetLocked(false);                        
                }

                foreach (var ai in teamBList) 
                {
                        ai.agent.isStopped = false;
                        ai.SetLocked(false);
                }
        }
        private void OnDisable()
        {
                roundManager.OnRoundEnd -= Despawn;
                roundManager.OnRoundStart -= Spawn;
                roundManager.OnRoundReady -= UnlockPlayers;
        }
        private void Spawn()
        {
                if(isPlayer) 
                {
                        var teamDick = (NetworkManager.singleton as CustomNetworkManager).teamDict;
                        foreach (var v in (NetworkManager.singleton as CustomNetworkManager).playerObjs) 
                        {
                                v.Value.transform.position = teamDick[v.Key] == 
                                                             GameManager.Team.TeamA ? spawnATeam.position : spawnBTeam.position;
                                CameraManager.Instance.TeleportToTarget();
                                v.Value.SetActive(true);
                                HP hp = v.Value.GetComponent<HP>();
                                hp.Init();
                                GameManager.Instance.RegisterTeam(v.Value, teamDick[v.Key]);
                                v.Value.GetComponent<WeaponSwitcher>().RefillAmmmo();
                                v.Value.GetComponent<PlayerController>().SetLocked(true);
                        }
                }
                else 
                {
                        foreach (var v in (NetworkManager.singleton as CustomNetworkManager).playerObjs) 
                        {
                                v.Value.GetComponent<PlayerController>().SetLocked(true);
                        }
                }
                
                if(!SessionData.isMultiplayer) 
                {
                        if(!poolInit) 
                        {
                                aiPool.Init(aiController, playerCount, spawnBTeam);
                                poolInit = true;
                        }
                        
                        for (int i = 0; i < playerCount; i++) 
                        {
                                AIController go = aiPool.Get();
                                go.transform.position = spawnBTeam.position;
                                NetworkServer.Spawn(go.gameObject);
                                go.OnSpawn();
                                GameManager.Instance.RegisterTeam(go.gameObject, GameManager.Team.TeamB);
                                teamBList.Add(go);
                        }
                }
                
                OnSpawnComplete?.Invoke();
                if(!isPlayer)
                        isPlayer = true;
        }

        private void Despawn()
        {
                foreach (var v in (NetworkManager.singleton as CustomNetworkManager).playerObjs) 
                {
                        if (v.Value == null) return;
                        v.Value.SetActive(false);
                }
                if(!SessionData.isMultiplayer) 
                {
                        foreach (AIController ai in teamBList) 
                        {
                                if (ai == null) return;
                                NetworkServer.UnSpawn(ai.gameObject);
                                aiPool.Return(ai);
                        }
                }
                teamBList.Clear();
                GameManager.Instance.ClearTeams();
        }
        
}
