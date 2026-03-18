using System;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public int currentRound{get; private set;}
    [SerializeField]private int roundsToWin = 10;

    public event Action OnRoundStart;
    
    Dictionary<GameManager.Team,int> RoundWins = new Dictionary<GameManager.Team,int>();

    private void OnEnable()
    {
        GameManager.Instance.OnTeamEliminated += EndRound;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnTeamEliminated -= EndRound;
    }
    private void Start()
    {
        RoundWins.Add(GameManager.Team.TeamA,0);
        RoundWins.Add(GameManager.Team.TeamB,0);
        currentRound = 0;
        StartRound();
    }
    private void StartRound()
    {
        
        OnRoundStart?.Invoke();
        currentRound++;
    }

    private void EndRound(GameManager.Team winner)
    {
        RoundWins[winner]++;
        GameManager.Instance.ClearTeam();
        if (roundsToWin <= RoundWins[winner]) 
        {
            GameManager.Instance.MatchWin(winner);
        }
        else 
        {
            StartRound();
        }
    }
}
