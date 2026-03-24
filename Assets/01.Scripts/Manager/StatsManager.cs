using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    public GameObject player;
    
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
    public void RegisterPlayer(GameObject _player)
    {
        player = _player;
    }

    public int GetKill(GameObject killer)
    {
        return !playerStats.ContainsKey(killer) ? 0 : playerStats[killer].kills;
    }
    public int GetDeath(GameObject killer)
    {
        return !playerStats.ContainsKey(killer) ? 0 : playerStats[killer].deaths;
    }
    public void Clear()
    {
        playerStats.Clear();
    }
}
