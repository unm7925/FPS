using System;
using System.Collections.Generic;
using UnityEngine;
public class GameManager:MonoBehaviour
{
        public static int PlayersPerTeam{get;private set;}
        
        public MatchData matchData;
        
        public event Action<Team,int,int> OnGameEnd;
        public event Action<Team> OnTeamEliminated;
        
        public static GameManager Instance;

        public enum Team
        {
                TeamA,
                TeamB
        };
        
        List<GameObject> teamAList = new List<GameObject>();
        List<GameObject> teamBList = new List<GameObject>();

        private void Awake()
        {
                if (Instance == null) 
                {
                        Instance = this;
                        DontDestroyOnLoad(gameObject);
                }
                else 
                {
                        Destroy(gameObject);
                }
                Time.timeScale = 1;
                if (matchData == null) return;
                PlayersPerTeam = matchData.playersPerTeam;
        }


        public void RegisterTeam(GameObject player, Team team)
        {
                if (team == Team.TeamA) 
                {
                        teamAList.Add(player);
                }
                else 
                {
                        teamBList.Add(player);
                }
        }

        public List<GameObject> GetEnemeies(Team team)
        {
                if (team == Team.TeamA) 
                {
                        return teamBList;
                }
                else 
                {
                        return teamAList;
                }
        }
        public List<GameObject> GetTeam(Team team)
        {
                if (team == Team.TeamA) 
                {
                        return teamAList;
                }
                else 
                {
                        return teamBList;
                }
        }
        

        public void UnRegisterEnemies(Team team,GameObject player)
        {
                if (team == Team.TeamA) 
                {
                        teamAList.Remove(player);
                        
                }
                else 
                {
                        teamBList.Remove(player);
                        
                }
                CheckRoundWindContition();
        }
        private void CheckRoundWindContition()
        {
                if(teamAList.Count == 0) 
                {
                        OnTeamEliminated?.Invoke(Team.TeamB);
                }
                else if (teamBList.Count == 0) 
                {
                        OnTeamEliminated?.Invoke(Team.TeamA);
                }
        }
        public void MatchWin(Team winner,int roundWin, int roundLose)
        {
                OnGameEnd?.Invoke(winner,roundWin,roundLose);                         
        }
}

