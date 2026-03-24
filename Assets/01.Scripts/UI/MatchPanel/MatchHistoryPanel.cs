using System;
using UnityEngine;
public class MatchHistoryPanel:MonoBehaviour
{
        [SerializeField] private GameObject matchHistoryPrefab;

        private void Awake()
        {
                
                CreateMatchHistory();
        }
        
        private void CreateMatchHistory()
        {
            foreach (var match in MatchRecordManager.Instance.GetHistory()) 
            {
                MatchRecordItem matchHistory = Instantiate(matchHistoryPrefab, transform).GetComponent<MatchRecordItem>();
                matchHistory.Init(match.gameMode,match.isWin,match.roundScore,match.enemyRoundScore,match.kills,match.deaths);
            }
        }
}