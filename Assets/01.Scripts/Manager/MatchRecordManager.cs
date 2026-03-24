using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Steamworks;


public class MatchRecordManager:MonoBehaviour
{
        public static MatchRecordManager Instance;
        
        
        SaveData data = new SaveData();
        List<MatchRecord> matchHistory =  new List<MatchRecord>();
        private int winP = 5;
        private int loseP = -3;
        private string savePath ="records.json";
        
        private void Awake()
        {
                if(Instance == null) 
                {
                        Instance = this;
                        DontDestroyOnLoad(gameObject);
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
                
                data.matchRecords = matchHistory;
                if(record.isWin) 
                {
                        data.currentRankScore += winP;
                }
                else 
                {
                        data.currentRankScore += loseP;
                }
                data.currentRankScore = Mathf.Max(0, data.currentRankScore);
                SaveToJson();
                StatsManager.Instance.Clear();
        }
        private void SaveToJson()
        {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                byte[] info = new UTF8Encoding(true).GetBytes(json);
                SteamRemoteStorage.FileWrite(savePath, info, info.Length);
        }
        private void LoadFromJson()
        {
                if(SteamRemoteStorage.FileExists(savePath)) 
                {
                        int size = SteamRemoteStorage.GetFileSize(savePath);
                        byte[] bytes = new byte[size];
                        SteamRemoteStorage.FileRead(savePath,bytes,bytes.Length);
                        var json = Encoding.UTF8.GetString(bytes);
                        data = JsonConvert.DeserializeObject<SaveData>(json);
                        matchHistory = data.matchRecords;
                }
                else 
                {
                        data = new SaveData();
                        matchHistory = new List<MatchRecord>();
                }
        }
        public List<MatchRecord> GetHistory()
        {
                var list = new List<MatchRecord>(matchHistory);
                list.Reverse();
                return list;
        }
        public int GetRankScore()
        {
                return data.currentRankScore;
        }
}

