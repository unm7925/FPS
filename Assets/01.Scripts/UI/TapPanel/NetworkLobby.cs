
using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NetworkLobby:NetworkBehaviour
{
        public static NetworkLobby Instance;
        
        float countdownTime = 3f;
        
        [SyncVar(hook = nameof(OnP1ReadyChanged))] bool p1Ready;
        [SyncVar(hook = nameof(OnP2ReadyChanged))] bool p2Ready;

        private void Awake()
        {
                Instance = this;
        }

        private void OnP1ReadyChanged(bool old, bool next)
        {
                SteamLobbyManager.Instance.PlayerSlots[0].SetReadyUI(next);
                CheckAllReady();
        }
        
        private void OnP2ReadyChanged(bool old, bool next)
        {
                SteamLobbyManager.Instance.PlayerSlots[1].SetReadyUI(next);
                CheckAllReady();
        }

        [Command(requiresAuthority = false)]
        public void CmdSetReady(int slotIndex, bool value)
        {
                if(slotIndex == 0)
                        p1Ready = value;
                else 
                        p2Ready = value;
        }

        [Command(requiresAuthority = false)]
        public void CmdResetReady()
        {
                p1Ready = false;
                p2Ready = false;
        }
        private void CheckAllReady()
        {
                if (p1Ready && p2Ready)
                        RpcStartCoundown();
        }
        [ClientRpc]
        private void RpcStartCoundown()
        {
                StartCoroutine(Countdown());
        }
        private IEnumerator Countdown()
        {
                
                yield return new WaitForSeconds(countdownTime);
                SceneManager.LoadScene(1);
        }
}

