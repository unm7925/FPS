
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    
    private SpawnManager spawnManager;

    private Transform aTeamPos;
    private Transform bTeamPos;
    
    public Dictionary<NetworkConnection, GameObject> playerObjs = new Dictionary<NetworkConnection, GameObject>();
    
    public Dictionary<NetworkConnection, GameManager.Team> teamDict = new Dictionary<NetworkConnection, GameManager.Team>();

    private int teamIndex = 0;

    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (conn.identity != null) return;
        
        GameObject go = Instantiate(playerPrefab);
        
        if (teamIndex % 2 == 0) 
        {
            go.transform.position = aTeamPos.position;
            GameManager.Instance.RegisterTeam(go.gameObject,GameManager.Team.TeamA);
            teamDict.Add(conn, GameManager.Team.TeamA);
            go.GetComponent<PlayerController>().myTeam = GameManager.Team.TeamA;
        }
        else 
        {
            go.transform.position = bTeamPos.position;
            GameManager.Instance.RegisterTeam(go.gameObject,GameManager.Team.TeamB);
            teamDict.Add(conn, GameManager.Team.TeamB);
            go.GetComponent<PlayerController>().myTeam = GameManager.Team.TeamB;
        }
        playerObjs.Add(conn, go);
        NetworkServer.AddPlayerForConnection(conn, go.gameObject);
        teamIndex++;

        if (playerObjs.Count < GameManager.PlayersPerTeam) return;
            spawnManager.RoundManager.StartGame();
    }
    
    
    public void RegisterSpawnManager(SpawnManager _spawnManager)
    {
        spawnManager = _spawnManager;
        aTeamPos = spawnManager.SpawnATeam;
        bTeamPos = spawnManager.SpawnBTeam;
    }

    public override void OnServerSceneChanged(string _sceneName)
    {
        if (_sceneName != "InGameScene") return;
        
        base.OnServerSceneChanged(_sceneName);
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        OnServerAddPlayer(conn);
    }
   
    public override void OnStopServer()
    {
        InitValue();
        base.OnStopServer();
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        SceneManager.LoadScene(0);
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
    }
    private void InitValue()
    {
        teamDict.Clear();
        playerObjs.Clear();
        teamIndex = 0;
    }
}
