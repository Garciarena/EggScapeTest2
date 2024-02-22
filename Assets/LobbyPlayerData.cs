using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyPlayerData : MonoBehaviour
{
    public static LobbyPlayerData Instance { get; private set; }

    public string _playerName;
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}

  
