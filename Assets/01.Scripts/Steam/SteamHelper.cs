using System;
using Steamworks;
using UnityEngine;
public static class SteamHelper
{
        public static Texture2D GetAvatar(CSteamID steamID)
        {
                int handle = SteamFriends.GetMediumFriendAvatar(steamID);
                if (handle == -1|| handle == 0) return null;
                
                SteamUtils.GetImageSize(handle, out uint w, out uint h);

                Byte[] buffer = new byte[w * h * 4];
                SteamUtils.GetImageRGBA(handle, buffer,buffer.Length);
                
                Texture2D texture = new Texture2D((int)w, (int)h, TextureFormat.RGBA32, false);
                texture.LoadRawTextureData(buffer);
                
                texture.Apply();
                return texture;
        }
        
}

