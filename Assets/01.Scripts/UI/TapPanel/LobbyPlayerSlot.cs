using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerSlot : MonoBehaviour
{
    [SerializeField] private RawImage avatar;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI stateTxt;
    
    private Callback<AvatarImageLoaded_t> avatarLoaded;
    
    public CSteamID userID{get; private set;}
    private string ready = "ready";
    private string waiting = "waiting";
    
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
        SteamMatchmaking.SetLobbyMemberData(new CSteamID(SteamLobbyManager.Instance.roomID), ready, isReady ? "0" : "1");
        isReady = !isReady;
    }
    
    public void SlotClear()
    {
        avatar.texture = null;
        nameTxt.text = "";
        stateTxt.text = "";
        userID = CSteamID.Nil;
    }

    public void SetReadyUI(bool value)
    {
        isReady = value;
        stateTxt.text = value ? ready : waiting;
    }
}
