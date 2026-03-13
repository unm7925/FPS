using System.Collections.Generic;
using UnityEngine;
public class GameManager:MonoBehaviour
{
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
}

