
using System.Collections;
using System.Linq;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPlayerSlot : MonoBehaviour
{
    [SerializeField] private RawImage avatar;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI stateTxt;
    [SerializeField] private int slotIndex;
    
    
    private Callback<AvatarImageLoaded_t> avatarLoaded;
    
    public CSteamID userID{get; private set;}
    private string ready = "Ready";
    private string waiting = "Waiting";
    
    private bool isReady;

    public void Init(CSteamID _userID)
    {
        userID = _userID;
        avatarLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarLoaded);
        
        avatar.texture = SteamHelper.GetAvatar(_userID);
        Debug.Log("아바타드감");
        nameTxt.text = _userID == SteamUser.GetSteamID() ? 
            SteamFriends.GetPersonaName() : SteamFriends.GetFriendPersonaName(_userID);
        stateTxt.text = waiting;
        
    }
    private void OnAvatarLoaded(AvatarImageLoaded_t param)
    {
        if (param.m_steamID != userID) return;
        
        Texture2D tex = SteamHelper.GetAvatar(userID);
        if (tex != null) 
        {
            avatar.texture = tex;
            Debug.Log("아바타드감");
        }
    }

    
    public void ReadyClicked()
    {
        if (userID != SteamUser.GetSteamID()) return; 
        NetworkLobby.Instance.CmdSetReady(slotIndex,!isReady);
    }
    
    public void SlotClear()
    {
        avatar.texture = null;
        nameTxt.text = "";
        stateTxt.text = "";
    }

    public void SetReadyUI(bool value)
    {
        isReady = value;
        stateTxt.text = value ? ready : waiting;
    }
}
