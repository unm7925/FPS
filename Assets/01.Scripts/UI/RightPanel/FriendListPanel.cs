using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine;
public class FriendListPanel:MonoBehaviour
{
        [SerializeField] private GameObject friendProfile;
        private List<CSteamID> ids = new List<CSteamID>();
        private int friendCount;
        private void Start()
        {
                friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);

                for (int i = 0; i < friendCount; i++) 
                {
                        ids.Add(SteamFriends.GetFriendByIndex(i,EFriendFlags.k_EFriendFlagImmediate));
                }

                var sorted = ids.OrderBy(friend => SteamFriends.GetFriendPersonaState(friend)).ToList();

                foreach (var v in sorted) 
                {
                        FriendItem fi = Instantiate(friendProfile, transform).GetComponent<FriendItem>();
                        fi.Init(v);
                }
        }
}

