using System;
using UnityEngine;
public class SpawnManager: MonoBehaviour
{
        [SerializeField] RoundManager roundManager;
        [SerializeField] HUDController hudController;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform spawnBTeam;
        [SerializeField] private Transform spawnATeam;

        public event Action OnSpawnComplete;
        
        private Vector3 spawnPosPlus = Vector3.right;
        // 아직 멀티 구현안해서 적만 소환
        private int playerCount;

        private void Awake()
        {
                playerCount = GameManager.Instance.matchData.playersPerTeam;
        }
        private void Start()
        {
                
        }

        private void OnEnable()
        {
                roundManager.OnRoundStart += Spawn;
        }
        private void OnDisable()
        {
                roundManager.OnRoundStart -= Spawn;
        }
        private void Spawn()
        {
                for (int i = 0; i < playerCount; i++) 
                {
                        GameObject Bplayer = Instantiate(enemyPrefab, spawnBTeam);
                        GameManager.Instance.RegisterTeam(Bplayer, GameManager.Team.TeamB);
                }
                GameObject Aplayer = Instantiate(playerPrefab, spawnATeam);
                GameManager.Instance.RegisterTeam(Aplayer, GameManager.Team.TeamA);
                hudController.Init(Aplayer.GetComponent<HP>(),Aplayer.GetComponentInChildren<WeaponSwitcher>());
                OnSpawnComplete?.Invoke();
        }
}
