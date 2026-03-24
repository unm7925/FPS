using TMPro;
using UnityEngine;
public class MatchRecordItem:MonoBehaviour
{
        [SerializeField] private TextMeshProUGUI gameModeTxt;
        [SerializeField] private TextMeshProUGUI isWinTxt;
        [SerializeField] private TextMeshProUGUI roundTxt;
        [SerializeField] private TextMeshProUGUI killsTxt;
        [SerializeField] private TextMeshProUGUI deathsTxt;

        public void Init(string gameMode,bool isWin,int roundWin, int roundLose, int kills, int deaths)
        {
                gameModeTxt.text = gameMode;
                isWinTxt.text = isWin ? "승리" : "패배";
                roundTxt.text = "라운드 " + roundWin + "/" + roundLose;
                killsTxt.text = "킬 " + kills;
                deathsTxt.text = "데스 " + deaths;
        }
}

