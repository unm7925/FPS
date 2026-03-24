using System;
using Steamworks;
using UnityEngine;

public class SteamLeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject rankProfile;
    [SerializeField] private Transform rankTransform;
    
    private CallResult<LeaderboardFindResult_t> findResult;
    private CallResult<LeaderboardScoreUploaded_t> uploadResult;
    private CallResult<LeaderboardScoresDownloaded_t> downloadResult;

    private SteamLeaderboard_t leaderBoard =  new SteamLeaderboard_t();

    private void Start()
    {
        findResult = new CallResult<LeaderboardFindResult_t>();
        uploadResult = new CallResult<LeaderboardScoreUploaded_t>();
        downloadResult = new CallResult<LeaderboardScoresDownloaded_t>();

        var handle = SteamUserStats.FindOrCreateLeaderboard
            ("Rank", ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
        Debug.Log(handle);
        Debug.Log(handle.m_SteamAPICall);
        findResult.Set(handle,OnLeaderboardFound);
    }
    private void OnLeaderboardFound(LeaderboardFindResult_t result, bool success)
    {
        leaderBoard = result.m_hSteamLeaderboard;
        var handle = SteamUserStats.UploadLeaderboardScore(leaderBoard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, MatchRecordManager.Instance.GetRankScore(), null, 0);
        Debug.Log(handle);
        uploadResult.Set(handle,OnScoreUploaded);
    }
    private void OnScoreUploaded(LeaderboardScoreUploaded_t result, bool success)
    {
        var handle = SteamUserStats.DownloadLeaderboardEntries(leaderBoard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, 10);
        Debug.Log(handle);
        downloadResult.Set(handle,OnScoresDownloaded);
    }
    private void OnScoresDownloaded(LeaderboardScoresDownloaded_t result,  bool success)
    {
        for (int i = 0; i < result.m_cEntryCount; i++)
        {
            SteamUserStats.GetDownloadedLeaderboardEntry(result.m_hSteamLeaderboardEntries,i,out LeaderboardEntry_t entry, null ,0);
            LeaderboardItem rank = Instantiate(rankProfile,rankTransform).GetComponent<LeaderboardItem>();
            rank.Init(entry.m_nScore, SteamUser.GetSteamID() != entry.m_steamIDUser ? 
                SteamFriends.GetFriendPersonaName(entry.m_steamIDUser) : 
                SteamFriends.GetPersonaName(), 
                entry.m_nGlobalRank, 
                SteamHelper.GetAvatar(entry.m_steamIDUser));
        }
        
    }
}
