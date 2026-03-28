using UnityEngine;

[CreateAssetMenu(fileName = "MatchData", menuName = "Scriptable Objects/MatchData")]
public class MatchData : ScriptableObject
{
    public enum MatchType
    {
        Match_1v1,
        Match_2v2,
        DeathMatch,
        RoundMatch
    };
    
    public MatchType matchType;
    
    public int playersPerTeam;
}
