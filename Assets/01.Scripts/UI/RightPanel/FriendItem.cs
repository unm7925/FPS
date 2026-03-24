using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FriendItem:MonoBehaviour
{
        [SerializeField] private RawImage friendAvatarImage;
        [SerializeField] private TextMeshProUGUI friendNameText;
        [SerializeField] private TextMeshProUGUI friendStatsText;

        public void Init(CSteamID steamID)
        {
                friendAvatarImage.texture = SteamHelper.GetAvatar(steamID);
                friendNameText.text = SteamFriends.GetFriendPersonaName(steamID);
                EPersonaState state = SteamFriends.GetFriendPersonaState(steamID);
                SetStateTxt(state);
        }
        
        private void SetStateTxt(EPersonaState _state)
        {
                switch (_state) 
                {
                        case EPersonaState.k_EPersonaStateOnline:
                                friendStatsText.text = "Online";
                                break;
                        case EPersonaState.k_EPersonaStateOffline:
                                friendStatsText.text = "Offline";
                                break;
                        case EPersonaState.k_EPersonaStateAway:
                                friendStatsText.text = "Away";
                                break;
                }
        }
}

