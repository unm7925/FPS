using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerProfileUI:MonoBehaviour
{
        [SerializeField] TextMeshProUGUI playerNameText;
        [SerializeField] TextMeshProUGUI playerStatsText;
        [SerializeField] RawImage avatarImage;
        private CSteamID id;
        private void Start()
        {
                id = SteamUser.GetSteamID();
                playerNameText.text = SteamFriends.GetPersonaName();
                EPersonaState state = SteamFriends.GetPersonaState();
                SetStateTxt(state);
                avatarImage.texture = SteamHelper.GetAvatar(id);
        }

        private void SetStateTxt(EPersonaState _state)
        {
                switch (_state) 
                {
                        case EPersonaState.k_EPersonaStateOnline:
                                playerStatsText.text = "Online";
                                break;
                        case EPersonaState.k_EPersonaStateOffline:
                                playerStatsText.text = "Offline";
                                break;
                        case EPersonaState.k_EPersonaStateAway:
                                playerStatsText.text = "Away";
                                break;
                }
        }
}
