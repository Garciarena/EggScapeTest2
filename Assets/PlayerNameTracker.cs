using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using System;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;

public class PlayerNameTracker : NetworkBehaviour
{
    // se llama cuando los jugadores cambian su nombre
    public static event Action<NetworkConnection, string> OnNameChange;


    [SyncObject] //colleccion para Cada jugador
    private readonly SyncDictionary<NetworkConnection, string> _playerNames = new SyncDictionary<NetworkConnection, string>();

    //singleton
    private static PlayerNameTracker _instance;

    private void Awake()
    {
        _instance = this;
        _playerNames.OnChange += _playerNames_OnChange;

    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        base.NetworkManager.ServerManager.OnRemoteConnectionState += ServerManager_OnRemoteConnectionState;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        base.NetworkManager.ServerManager.OnRemoteConnectionState -= ServerManager_OnRemoteConnectionState;
    }

    private void ServerManager_OnRemoteConnectionState(NetworkConnection arg1, FishNet.Transporting.RemoteConnectionStateArgs arg2)
    {
        if (arg2.ConnectionState != RemoteConnectionState.Started)
            _playerNames.Remove(arg1);

    }

    private void _playerNames_OnChange(SyncDictionaryOperation op, NetworkConnection key, string value, bool asServer)
    {
        if (op == SyncDictionaryOperation.Add || op == SyncDictionaryOperation.Set)
        {
            //OnNameChange? Invoke(key, value);
        }

    }


    public static string GetPlayerName (NetworkConnection conn)
    {
        if (_instance._playerNames.TryGetValue(conn, out string result))
            return result;
        else
            return string.Empty;
    }

    [Client]
    public static void SetName(string name)
    {
        _instance.ServerSetName(name);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerSetName(string name, NetworkConnection sender = null)
    {
        _playerNames[sender] = name;
    }
}
