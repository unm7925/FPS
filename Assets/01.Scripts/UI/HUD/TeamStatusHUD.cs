using System;
using UnityEngine;
using UnityEngine.UI;
public class TeamStatusHUD : MonoBehaviour
{
    [SerializeField] private SpawnManager spawnManager;

    [SerializeField] Image[] teamASlots;
    [SerializeField] Image[] teamBSlots;

    private void OnEnable()
    {
        spawnManager.OnSpawnComplete += SetTeamSlot;
    }
    private void OnDisable()
    {
        spawnManager.OnSpawnComplete -= SetTeamSlot;
    }

    private void SetTeamSlot()
    {
        foreach (var v in teamASlots) {
            v.gameObject.SetActive(false);

        }
        foreach (var v in teamBSlots) {
            v.gameObject.SetActive(false);
        }

        for (int i = 0; i < GameManager.PlayersPerTeam; i++) {
            int index = i;
            teamASlots[index].gameObject.SetActive(true);
            GameObject go = GameManager.Instance.GetTeam(GameManager.Team.TeamA)[index].gameObject;
            HP hp = go.GetComponent<HP>();
            hp.OnDie += go => OnPlayerDie(index, GameManager.Team.TeamA);

        }
        for (int i = 0; i < GameManager.PlayersPerTeam; i++) {
            int index = i;
            teamBSlots[index].gameObject.SetActive(true);
            GameObject go = GameManager.Instance.GetTeam(GameManager.Team.TeamB)[index].gameObject;
            HP hp = go.GetComponent<HP>();
            hp.OnDie += go => OnPlayerDie(index, GameManager.Team.TeamB);
        }

    }

    private void OnPlayerDie(int index, GameManager.Team team)
    {
        if (team == GameManager.Team.TeamA) {
            teamASlots[index].gameObject.SetActive(false);
        }
        else {
            teamBSlots[index].gameObject.SetActive(false);
        }
    }

}

