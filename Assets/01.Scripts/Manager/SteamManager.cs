using System;
using Steamworks;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
    public static SteamManager Instance;
    
    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SteamAPI.Init();
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if(Instance != this) return;
        SteamAPI.RunCallbacks();
    }
    private void OnDestroy()
    {
        if(Instance == this)
            SteamAPI.Shutdown();
    }
}
