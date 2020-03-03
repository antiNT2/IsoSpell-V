﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem.UI;

[System.Serializable]
public class SyncListConnectedPlayer : SyncList<ConnectedPlayer> { }

public class GameManagerSyncer : NetworkBehaviour
{
    public SyncListConnectedPlayer connectedPlayersToSync;

    public static GameManagerSyncer instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        UpdateAll();
    }

    void UpdateAll()
    {
        if (isServer)
            UpdateSyncList();
        if (isClient)
            UpdateLocalList();
    }

    void UpdateSyncList()
    {
        //print("UPDATING SYNC LIST");
        connectedPlayersToSync.Clear();

        for (int i = 0; i < GameManager.instance.connectedPlayers.Count; i++)
        {
            connectedPlayersToSync.Add(GameManager.instance.connectedPlayers[i]);
        }
    }

    void UpdateLocalList()
    {
        //print("UPDATING LOCAL LIST");
        GameManager.instance.connectedPlayers.Clear();

        for (int i = 0; i < connectedPlayersToSync.Count; i++)
        {
            GameManager.instance.connectedPlayers.Add(connectedPlayersToSync[i]);

            GameManager.instance.RefreshReadyStateDisplay(connectedPlayersToSync[i].playerObject);
            PlayerWeapon thisPlayerAttackScript = GameManager.instance.connectedPlayers[i].playerObject.GetComponent<PlayerWeapon>();

            if (thisPlayerAttackScript.currentWeapon != connectedPlayersToSync[i].equipedWeaponId && thisPlayerAttackScript.gameObject != NetworkPlayer.localPlayer.gameObject)
                thisPlayerAttackScript.EquipWeapon(connectedPlayersToSync[i].equipedWeaponId);
        }
    }

    [ClientRpc]
    public void RpcStartMatch()
    {
        if (isClient)
            GameManager.instance.CloseWeaponSelectionMenu();
    }
}