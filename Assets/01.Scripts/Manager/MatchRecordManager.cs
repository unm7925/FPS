using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;


public class MatchRecordManager:MonoBehaviour
{
        public static MatchRecordManager Instance;
        List<MatchRecord> matchHistory;
        private string savePath;
        
        private void Awake()
        {
                if(Instance == null) 
                {
                        Instance = this;
                        DontDestroyOnLoad(gameObject);
                        savePath = Application.persistentDataPath + "/records.json";
                        LoadFromJson();
                }
                else 
                {
                        Destroy(gameObject);
                }
        }
        private void OnEnable()
        {
                GameManager.Instance.OnGameEnd += SaveRecord;
        }
        private void OnDisable()
        {
                GameManager.Instance.OnGameEnd -= SaveRecord;
        }

        void SaveRecord(GameManager.Team winnerTeam, int roundWin,int roundLose)
        {
                MatchRecord record = new MatchRecord();
                record.gameMode = GameManager.Instance.matchData.matchType.ToString();
                record.isWin = winnerTeam == GameManager.Team.TeamA;
                
                record.kills = StatsManager.Instance.GetKill(StatsManager.Instance.player);
                record.deaths = StatsManager.Instance.GetDeath(StatsManager.Instance.player);
                record.roundScore = roundWin;
                record.enemyRoundScore = roundLose;
                
                matchHistory.Add(record);
                SaveToJson();
                StatsManager.Instance.Clear();
        }
        private void SaveToJson()
        {
                
                var json = JsonConvert.SerializeObject(matchHistory, Formatting.Indented);
                File.WriteAllText(savePath, json);
                
        }
        private void LoadFromJson()
        {
                matchHistory = File.Exists(savePath) ? 
                        JsonConvert.DeserializeObject<List<MatchRecord>>(File.ReadAllText(savePath)) : new List<MatchRecord>();
        }
        public List<MatchRecord> GetHistory()
        {
                var list = new List<MatchRecord>(matchHistory);
                list.Reverse();
                return list;
        }
}

