using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public int currentRound{get; private set;}
    [SerializeField]private int roundsToWin = 10;
    private int roundCooldown = 5;

    public event Action OnRoundStart;
    public event Action OnRoundEnd;

    public event Action<int, int> OnScoreUpdated;

    public event Action<int> OnCountdown;
    public event Action OnRoundReady;

    Dictionary<GameManager.Team, int> RoundWins = new Dictionary<GameManager.Team, int>();

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
    }
    public void StartGame()
    {
        StartCoroutine(StartRound());
    }
    private IEnumerator StartRound()
    {
        yield return null;
        OnRoundStart?.Invoke();
        currentRound++;
        for (int i = roundCooldown; i > 0; i--) 
        {
            OnCountdown?.Invoke(i);
            yield return new WaitForSeconds(1f);
        }
        OnRoundReady?.Invoke();
    }

    private void EndRound(GameManager.Team winner)
    {
        RoundWins[winner]++;
        OnScoreUpdated?.Invoke(RoundWins[GameManager.Team.TeamA], RoundWins[GameManager.Team.TeamB]);
        OnRoundEnd?.Invoke();
        if (roundsToWin <= RoundWins[winner]) 
        {
                GameManager.Instance.MatchWin(winner,RoundWins[GameManager.Team.TeamA],RoundWins[GameManager.Team.TeamB]);
        }
        else 
        {
            StartCoroutine(StartRound());
        }
    }
}
