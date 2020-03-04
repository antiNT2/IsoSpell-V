using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem.UI;

[System.Serializable]
public class SyncListConnectedPlayer : SyncList<ConnectedPlayer> { }

public class GameManagerSyncer : NetworkBehaviour
{
    [SyncVar]
    public int syncNumberOfLives = 2;
    public SyncListConnectedPlayer connectedPlayersToSync;

    public static GameManagerSyncer instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        UpdateAll();

        //NetworkManager.singleton.sce
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

        syncNumberOfLives = GameManager.instance.numberOfLivesThisGame;

        for (int i = 0; i < GameManager.instance.connectedPlayers.Count; i++)
        {
            connectedPlayersToSync.Add(GameManager.instance.connectedPlayers[i]);
        }
    }

    void UpdateLocalList()
    {
        //print("UPDATING LOCAL LIST");

        GameManager.instance.numberOfLivesThisGame = syncNumberOfLives;
        GameManager.instance.RefreshNumberOfLivesDisplay();

        int myPlayerId = -1;
        ConnectedPlayer myPlayer = null;
        if (connectedPlayersToSync.Count == GameManager.instance.connectedPlayers.Count)
        {
            myPlayerId = GameManager.instance.GetPlayerId(NetworkPlayer.localPlayer.gameObject);
            myPlayer = GameManager.instance.connectedPlayers[myPlayerId];
        }

        GameManager.instance.connectedPlayers.Clear();

        for (int i = 0; i < connectedPlayersToSync.Count; i++)
        {
            if (myPlayerId == i && myPlayerId != -1)
                GameManager.instance.connectedPlayers.Add(myPlayer);
            else
                GameManager.instance.connectedPlayers.Add(connectedPlayersToSync[i]);

            GameManager.instance.RefreshReadyStateDisplay(GameManager.instance.connectedPlayers[i].playerObject);
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
