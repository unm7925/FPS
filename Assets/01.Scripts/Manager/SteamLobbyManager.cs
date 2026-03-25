using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
public class SteamLobbyManager:MonoBehaviour
{
        CallResult<LobbyCreated_t> createLobbyResult;
        CallResult<LobbyEnter_t> enterLobbyResult;
        Callback<LobbyChatUpdate_t> chatUpdateResult;
        Callback<LobbyMatchList_t> randomLobbyResult;
        Callback<LobbyDataUpdate_t> lobbyDataUpdateResult;
        
        [SerializeField] private GameObject RoomPrefab;
        [SerializeField] private TextMeshProUGUI roomID_Txt;
        [SerializeField] private LobbyPlayerSlot[] playerSlots;
        [SerializeField] private TMP_InputField enterRoomID_Txt;
        
        Dictionary<CSteamID,LobbyPlayerSlot> slots = new Dictionary<CSteamID, LobbyPlayerSlot>();

        private void Awake()
        {
                createLobbyResult = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);
                enterLobbyResult = CallResult<LobbyEnter_t>.Create(OnLobbyEntered);
                chatUpdateResult = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
                randomLobbyResult = Callback<LobbyMatchList_t>.Create(RandomMatch);
                lobbyDataUpdateResult = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
        }
        private void OnLobbyDataUpdate(LobbyDataUpdate_t result)
        {
                if (NetworkManager.singleton.isNetworkActive) return;
                var idLobby = new CSteamID(result.m_ulSteamIDLobby);
                int index = SteamMatchmaking.GetNumLobbyMembers(idLobby);
                int check = 0;
                for (int i = 0; i < index; i++) 
                {
                        var id = SteamMatchmaking.GetLobbyMemberByIndex(idLobby, i);
                        slots.TryGetValue(id, out var slot);
                        if(SteamMatchmaking.GetLobbyMemberData(idLobby, id,"ready") == "1") 
                        {
                                slot?.SetReadyUI(true);
                                check++;
                        }
                        else 
                        {
                                slot?.SetReadyUI(false);
                        }
                }
                if (check != index || index <2) return;
                CSteamID ownerId = SteamMatchmaking.GetLobbyOwner(idLobby);
                if (ownerId == SteamUser.GetSteamID()) 
                {
                        NetworkManager.singleton.StartHost();
                        NetworkManager.singleton.ServerChangeScene("InGameScene");
                }
                else 
                {
                        NetworkManager.singleton.networkAddress = ownerId.ToString();
                        NetworkManager.singleton.StartClient();
                }
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
                        if (slots.ContainsKey(userID)) return;
                        var slot = playerSlots.FirstOrDefault(s => s.userID == CSteamID.Nil);
                        if (slot == null) return;
                        slot.Init(userID);
                        slots[userID] = slot;
                }
                else 
                {
                        foreach (var v in playerSlots) 
                        {
                                if (v.userID == userID) 
                                {
                                        slots.Remove(v.userID);
                                        v.SlotClear();
                                }
                        }
                }
        }

        public void CreatedRoom()
        {
                SteamAPICall_t handle = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, GameManager.Instance.matchData.playersPerTeam*2);
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
                SessionData.RoomID = result.m_ulSteamIDLobby;
                roomID_Txt.text = SessionData.RoomID.ToString();
                playerSlots[0].Init(SteamUser.GetSteamID());
                slots[SteamUser.GetSteamID()] =  playerSlots[0];
        }
        private void EnterRoom(ulong _roomID)
        {
                
                SteamAPICall_t handle = SteamMatchmaking.JoinLobby(new CSteamID(_roomID));
                enterLobbyResult.Set(handle);
        }
        
        private void OnLobbyEntered(LobbyEnter_t result,bool bIOFailure)
        {
                if (bIOFailure || result.m_EChatRoomEnterResponse != (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess) 
                {
                        return;
                }
                SessionData.RoomID = result.m_ulSteamIDLobby;
                RoomPrefab.SetActive(true);
                roomID_Txt.text = result.m_ulSteamIDLobby.ToString();
                CSteamID id = SteamMatchmaking.GetLobbyOwner(new CSteamID(SessionData.RoomID));
                if (id == SteamUser.GetSteamID()) return;
                playerSlots[0].Init(id);
                playerSlots[1].Init(SteamUser.GetSteamID());
                slots[id] = playerSlots[0];
                slots[SteamUser.GetSteamID()] = playerSlots[1];
        }

        public void LeaveRoom()
        {
                foreach (var v in playerSlots) 
                {
                        slots.Remove(v.userID);
                        v.StopAllCoroutines();
                        v.SlotClear();
                }
                if (NetworkManager.singleton.isNetworkActive) 
                {
                        if (SteamUser.GetSteamID() == SteamMatchmaking.GetLobbyOwner(new CSteamID(SessionData.RoomID))) 
                        {
                                NetworkManager.singleton.StopHost();
                        }
                        else 
                        {
                                NetworkManager.singleton.StopClient();
                        }
                }

                RoomPrefab.SetActive(false);
                SteamMatchmaking.LeaveLobby(new CSteamID(SessionData.RoomID));
        }

        public void JoinRoom()
        {
                if(ulong.TryParse((enterRoomID_Txt.text), out ulong id))
                {
                        EnterRoom(id);
                }
        }

        private void OnDestroy()
        {
                createLobbyResult.Dispose();
                enterLobbyResult.Dispose();
                chatUpdateResult.Dispose();
                randomLobbyResult.Dispose();
                lobbyDataUpdateResult.Dispose();
        }
}
