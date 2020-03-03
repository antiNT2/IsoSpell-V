using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    SpriteRenderer spriteRenderer;
    PlayerHealth playerHealth;
    PlayerController playerController;

    [SerializeField]
    List<Behaviour> objectsToDisableWhenNotLocalPlayer = new List<Behaviour>();

    [SyncVar(hook = nameof(TakeDamageHook))]
    public float syncPlayerHealth;
    [SyncVar]
    public GameObject lastPlayerWhoHurtUs;

    public static NetworkPlayer localPlayer;

    [SyncVar]
    public VariablesToSync variablesToSync;

    float previousAimAngle;

    private void Start()
    {
        if (GameManager.instance.isInOnlineMultiplayer == false)
        {
            this.enabled = false;
            return;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();
        playerController = GetComponent<PlayerController>();

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
        else if (isLocalPlayer == false && GameManager.instance.isInOnlineMultiplayer)
        {
            LoadVariablesToSync();
        }

        //if (syncPlayerHealth > 0)
        playerHealth.currentHealth = syncPlayerHealth;
        playerHealth.RefreshDamageDisplay();
    }

    public void SetSyncListElementToServer(ConnectedPlayer clientPlayer, int index)
    {
        //print("sent to server");
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
        variablesToSync.aimAngle = playerController.aimAngle;

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

        if (Mathf.Abs(variablesToSync.aimAngle - previousAimAngle) < 5f)
            LoadAimAngle(Mathf.Lerp(previousAimAngle, variablesToSync.aimAngle, Time.deltaTime * 25f));
        else
            LoadAimAngle(variablesToSync.aimAngle);
    }

    void LoadAimAngle(float aimAngle)
    {
        var x = transform.position.x + 1f * Mathf.Cos(aimAngle);
        var y = transform.position.y + 1f * Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(x, y, 0);
        playerController.crosshair.transform.position = crossHairPosition;

        previousAimAngle = aimAngle;
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
        RpcSetLastPlayerWhoHurtUs(playerToHurt, playerWhoHurtUs);
        playerToHurt.GetComponent<NetworkPlayer>().syncPlayerHealth -= damageAmount;
    }

    [ClientRpc]
    void RpcSetLastPlayerWhoHurtUs(GameObject playerToHurt, GameObject playerWhoHurtUs)
    {
        playerToHurt.GetComponent<NetworkPlayer>().lastPlayerWhoHurtUs = playerWhoHurtUs;
    }

    [Command]
    public void CmdSetHealth(int playerToEditId, float newHealth)
    {
        GameObject playerToEdit = GameManager.instance.connectedPlayers[playerToEditId].playerObject;
        playerToEdit.GetComponent<NetworkPlayer>().syncPlayerHealth = newHealth;
        playerToEdit.GetComponent<PlayerHealth>().RefreshDamageDisplay();
    }

    void TakeDamageHook(float oldHealth, float newHealth)
    {
        /*if (lastPlayerWhoHurtUs != null)
            print("I am " + this.gameObject.name + " and last p who hurt us is " + lastPlayerWhoHurtUs.name + " / my local p is " + NetworkPlayer.localPlayer.gameObject.name);
        else
            print("last p who hurt us is null");*/

        if (lastPlayerWhoHurtUs == NetworkPlayer.localPlayer.gameObject || oldHealth <= 0 || GameManager.instance.isInWeaponSelection)
            return;

        float damageTook = oldHealth - newHealth;

        playerHealth.GetComponent<IHealthEntity>().DoDamage(damageTook, lastPlayerWhoHurtUs);
    }

    [Command]
    public void CmdSpawnBullet(int bulletId, GameObject playerAuthority, NetworkAmmoSettings settings)
    {
        GameObject bulletToSpawn = Instantiate(NetworkManager.singleton.spawnPrefabs[bulletId]);
        bulletToSpawn.transform.position = playerAuthority.transform.position;
        bulletToSpawn.GetComponent<NetworkAmmo>().settings = settings;
        NetworkServer.Spawn(bulletToSpawn, playerAuthority);
    }

}

[System.Serializable]
public class VariablesToSync
{
    public bool spriteFlipX;
    public float aimAngle;
}
