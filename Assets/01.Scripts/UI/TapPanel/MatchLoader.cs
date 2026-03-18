
using UnityEngine;
using UnityEngine.SceneManagement;
public class MatchLoader:MonoBehaviour
{
        [SerializeField] private MatchData matchData;

        public void LoadMatch()
        {
                GameManager.Instance.matchData =  matchData;
                SceneManager.LoadScene(1);
        }
                
}

