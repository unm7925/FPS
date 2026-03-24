using System.Linq;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
public class SteamLobbyManager:MonoBehaviour
{
        public static SteamLobbyManager Instance;
        
        CallResult<LobbyCreated_t> createLobbyResult;
        CallResult<LobbyEnter_t> enterLobbyResult;
        Callback<LobbyChatUpdate_t> chatUpdateResult;
        
        [SerializeField] private GameObject RoomPrefab;
        [SerializeField] private TextMeshProUGUI roomID_Txt;
        [SerializeField] private LobbyPlayerSlot[] playerSlots;
        [SerializeField] private TMP_InputField enterRoomID_Txt;
        
        
        private ulong roomID;

        private void Awake()
        {
                if (Instance == null) 
                {
                        Instance = this;
                        DontDestroyOnLoad(gameObject);
                }
                else 
                {
                        Destroy(gameObject);
                }

                createLobbyResult = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);
                enterLobbyResult = CallResult<LobbyEnter_t>.Create(OnLobbyEntered);
                chatUpdateResult = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        }
        private void OnLobbyChatUpdate(LobbyChatUpdate_t result)
        {
                CSteamID userID =  new CSteamID(result.m_ulSteamIDUserChanged);
                uint state = result.m_rgfChatMemberStateChange;

                if (state == (uint)EChatMemberStateChange.k_EChatMemberStateChangeEntered) 
                {
                        var slot = playerSlots.FirstOrDefault(s => s.userID == CSteamID.Nil);
                        slot?.Init(userID);
                }
                else 
                {
                        foreach (var v in playerSlots) {
                                if (v.userID == userID) 
                                {
                                        v.SlotClear();
                                }
                        }
                }
        }

        public void CreatedRoom()
        {
                SteamAPICall_t handle = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, GameManager.Instance.matchData.playersPerTeam);
                createLobbyResult.Set(handle);
        }

        private void OnLobbyCreated(LobbyCreated_t result, bool failure)
        {
                if (failure || result.m_eResult != EResult.k_EResultOK) 
                {
                        RoomPrefab.SetActive(false);
                        return;
                }
                playerSlots[0].Init(SteamUser.GetSteamID());
                RoomPrefab.SetActive(true);
                roomID = result.m_ulSteamIDLobby;
                roomID_Txt.text = roomID.ToString();
                NetworkManager.singleton.StartHost();
        }
        private void EnterRoom(ulong roomID)
        {
                SteamAPICall_t handle = SteamMatchmaking.JoinLobby(new CSteamID(roomID));
                enterLobbyResult.Set(handle);
        }
        
        private void OnLobbyEntered(LobbyEnter_t result,bool bIOFailure)
        {
                if (bIOFailure || result.m_EChatRoomEnterResponse != (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess) 
                {
                        return;
                }
                roomID = result.m_ulSteamIDLobby;
                RoomPrefab.SetActive(true);
                roomID_Txt.text = result.m_ulSteamIDLobby.ToString();
                CSteamID id = SteamMatchmaking.GetLobbyOwner(new CSteamID(roomID));
                if (SteamUser.GetSteamID() == id) return;
                NetworkManager.singleton.networkAddress = id.ToString();
                NetworkManager.singleton.StartClient();
        }

        public void LeaveRoom()
        {
                SteamMatchmaking.LeaveLobby(new CSteamID(roomID));
                RoomPrefab.SetActive(false);
                foreach (var v in playerSlots) 
                {
                        v.SlotClear();
                }
        }

        public void JoinRoom()
        {
                if(ulong.TryParse((enterRoomID_Txt.text), out ulong id))
                {
                        EnterRoom(id);
                }
        }
}
