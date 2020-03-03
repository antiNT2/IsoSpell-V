using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    SpriteRenderer spriteRenderer;
    PlayerHealth playerHealth;

    [SerializeField]
    List<Behaviour> objectsToDisableWhenNotLocalPlayer = new List<Behaviour>();

    [SyncVar(hook = nameof(TakeDamageHook))]
    public float syncPlayerHealth;
    GameObject lastPlayerWhoHurtUs;

    public static NetworkPlayer localPlayer;

    [SyncVar]
    public VariablesToSync variablesToSync;

    private void Start()
    {
        if (GameManager.instance.isInOnlineMultiplayer == false)
        {
            this.enabled = false;
            return;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();

        Initialize();
    }

    void Initialize()
    {
        if (isLocalPlayer == false)
        {
            for (int i = 0; i < objectsToDisableWhenNotLocalPlayer.Count; i++)
            {
                objectsToDisableWhenNotLocalPlayer[i].enabled = false;
            }
            GetComponent<Rigidbody2D>().isKinematic = true;
        }
        else
        {
            localPlayer = this;
        }
    }

    private void Update()
    {
        if (isLocalPlayer == true)
        {
            SetVariablesToSynchronize();
        }
        else if (isLocalPlayer == false)
        {
            LoadVariablesToSync();
        }

        if (syncPlayerHealth > 0)
            playerHealth.currentHealth = syncPlayerHealth;
    }

    public void SetSyncListElementToServer(ConnectedPlayer clientPlayer, int index)
    {
        print("sent to server");
        CmdSetSyncListElement(clientPlayer, index);
    }

    [Command]
    void CmdSetSyncListElement(ConnectedPlayer clientPlayer, int index)
    {
        GameManager.instance.connectedPlayers[index] = clientPlayer;
    }

    [Command]
    public void CmdStartMatch()
    {
        GameManagerSyncer.instance.RpcStartMatch();
    }

    [Client]
    void SetVariablesToSynchronize()
    {
        variablesToSync.spriteFlipX = spriteRenderer.flipX;

        CmdSendVariablesToSynchronize(variablesToSync);
    }

    [Command]
    void CmdSendVariablesToSynchronize(VariablesToSync _vars)
    {
        variablesToSync = _vars;
    }

    void LoadVariablesToSync()
    {
        spriteRenderer.flipX = variablesToSync.spriteFlipX;
    }

    #region Damage Sync
    /*void TakeDamageAction(float damageAmount, GameObject playerThatShot)
    {
        CmdDoDamage(damageAmount, playerThatShot, this.gameObject);
    }

    [Command]
    void CmdDoDamage(float damageAmount, GameObject playerThatShot, GameObject playerThatTakesDamage)
    {
        RpcDoDamage(damageAmount, playerThatShot, playerThatTakesDamage);
    }

    [ClientRpc]
    void RpcDoDamage(float damageAmount, GameObject playerThatShot, GameObject playerThatTakesDamage)
    {
        if (playerThatTakesDamage != localPlayer)
            playerThatTakesDamage.GetComponent<IHealthEntity>().DoDamage(damageAmount, playerThatShot);
    }*/
    #endregion

    [Command]
    public void CmdApplyDamage(GameObject playerToHurt, float damageAmount, GameObject playerWhoHurtUs)
    {
        playerToHurt.GetComponent<NetworkPlayer>().lastPlayerWhoHurtUs = playerWhoHurtUs;
        playerToHurt.GetComponent<NetworkPlayer>().syncPlayerHealth -= damageAmount;
    }

    [Command]
    public void CmdSetHealth(GameObject playerToEdit, float newHealth)
    {
        playerToEdit.GetComponent<NetworkPlayer>().syncPlayerHealth = newHealth;
        playerToEdit.GetComponent<PlayerHealth>().RefreshDamageDisplay(); 
    }

    void TakeDamageHook(float oldHealth, float newHealth)
    {
        if (lastPlayerWhoHurtUs == NetworkPlayer.localPlayer.gameObject || oldHealth <= 0 || GameManager.instance.isInWeaponSelection)
            return;

        float damageTook = oldHealth - newHealth;

        playerHealth.GetComponent<IHealthEntity>().DoDamage(damageTook, lastPlayerWhoHurtUs);
        /*playerHealth.currentHealth = syncPlayerHealth;
        playerHealth.TakeHitEffect(damageTook, lastPlayerWhoHurtUs);*/
    }

}

[System.Serializable]
public class VariablesToSync
{
    public bool spriteFlipX;
}
