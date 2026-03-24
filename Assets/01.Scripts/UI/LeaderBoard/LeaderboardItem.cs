using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LeaderboardItem:MonoBehaviour
{
    [SerializeField] private RawImage avatar;
    [SerializeField] private TextMeshProUGUI rankTxt;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI rankScoreTxt;

    public void Init(int _rankScore, string _name, int _rank, Texture2D _texture)
    {
        avatar.texture = _texture;
        nameTxt.text = _name;
        rankScoreTxt.text = _rankScore.ToString();
        rankTxt.text = _rank.ToString();
    }
}

