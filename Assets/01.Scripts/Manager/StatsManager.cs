using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    
    Dictionary<GameObject,PlayerStat> playerStats = new Dictionary<GameObject,PlayerStat>();

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

    public void RegisterKill(GameObject killer, GameObject victim)
    {
        if(!playerStats.ContainsKey(killer))
            playerStats[killer] = new PlayerStat();
        if(!playerStats.ContainsKey(victim))
            playerStats[victim] = new PlayerStat();
        
        playerStats[killer].kills++;
        playerStats[victim].deaths++;
        
        Debug.Log(playerStats[killer].kills + " " + playerStats[killer].deaths);
        Debug.Log(playerStats[victim].kills + " " + playerStats[victim].deaths);
    }
}
