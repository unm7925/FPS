using System.Collections;
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
        Callback<LobbyMatchList_t> randomLobbyResult;
        
        [SerializeField] private GameObject RoomPrefab;
        [SerializeField] private TextMeshProUGUI roomID_Txt;
        [SerializeField] private LobbyPlayerSlot[] playerSlots;
        [SerializeField] private TMP_InputField enterRoomID_Txt;
        
        public LobbyPlayerSlot[] PlayerSlots => playerSlots;
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
                randomLobbyResult = Callback<LobbyMatchList_t>.Create(RandomMatch);
        }
        private void RandomMatch(LobbyMatchList_t param)
        {
                if (param.m_nLobbiesMatching>0) 
                {
                        CSteamID id = SteamMatchmaking.GetLobbyByIndex(0);
                        EnterRoom((ulong) id);
                }
                else 
                {
                        CreatedRoom();
                }
        }
        public void StartRandomMatch()
        {
                SteamMatchmaking.RequestLobbyList();
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
                RoomPrefab.SetActive(true);
                roomID = result.m_ulSteamIDLobby;
                roomID_Txt.text = roomID.ToString();
                NetworkManager.singleton.StartHost();
                StartCoroutine(InitAfterHost());
        }
        private IEnumerator InitAfterHost()
        {
                yield return null;
                playerSlots[0].Init(SteamUser.GetSteamID());
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
                playerSlots[0].Init(id);
                playerSlots[1].Init(SteamUser.GetSteamID());
                if (SteamUser.GetSteamID() == id) return;
                NetworkManager.singleton.networkAddress = id.ToString();
                if (!NetworkManager.singleton.isNetworkActive)
                        NetworkManager.singleton.StartClient();
        }

        public void LeaveRoom()
        {
                foreach (var v in playerSlots) 
                {
                        v.StopAllCoroutines();
                        v.SlotClear();
                }
                if (NetworkManager.singleton.isNetworkActive)
                        NetworkLobby.Instance?.CmdResetReady();
                RoomPrefab.SetActive(false);
                SteamMatchmaking.LeaveLobby(new CSteamID(roomID));
        }

        public void JoinRoom()
        {
                if(ulong.TryParse((enterRoomID_Txt.text), out ulong id))
                {
                        EnterRoom(id);
                }
        }
}
