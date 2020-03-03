using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkAmmo : NetworkBehaviour
{
    public int bulletId;
    DamageZone damageScript;
    AmmoVelocity velocityScript;

    [SyncVar]
    public NetworkAmmoSettings settings;

    private void Start()
    {
        if (GameManager.instance.isInOnlineMultiplayer == false)
        {
            this.enabled = false;
            return;
        }

        damageScript = GetComponent<DamageZone>();
        velocityScript = GetComponent<AmmoVelocity>();

        print(damageScript.playerThatShotThis);

        if (damageScript.playerThatShotThis == NetworkPlayer.localPlayer.gameObject) //this means that we spawned this bullet locally
        {
            GetSettings();
            NetworkPlayer.localPlayer.CmdSpawnBullet(bulletId, NetworkPlayer.localPlayer.gameObject, settings);
        }
        else //this bullet has just been spawned from the server
        {
            damageScript.dontDoDamage = true; //the damage has already been taken care of
            if (settings.playerWhoSpawnedThis == NetworkPlayer.localPlayer.gameObject)
            {
                this.gameObject.SetActive(false);
                return;
            }
            SetSettings();
        }

    }

    /*private void Update()
    {
        //print(damageScript.playerThatShotThis.name);
        if (GameManager.instance.isInOnlineMultiplayer == true)
        {
            if (damageScript.playerThatShotThis != NetworkPlayer.localPlayer.gameObject)
            {
                SetSettings();
            }
        }
    }*/

    void GetSettings()
    {
        settings.bulletRotation = this.transform.rotation;
        settings.ammoSpeed = velocityScript.speed;
        settings.ammoDirection = velocityScript.direction;
        settings.destroyOnWallCollision = damageScript.destroyOnWallCollision;
        settings.destroyOnPlayerCollision = damageScript.destroyOnPlayerCollision;
        settings.ignorePlayer = damageScript.ignorePlayer;
        settings.playerWhoSpawnedThis = damageScript.playerThatShotThis;
    }

    void SetSettings()
    {
        this.transform.rotation = settings.bulletRotation;
        velocityScript.speed = settings.ammoSpeed;
        velocityScript.direction = settings.ammoDirection;
        damageScript.destroyOnWallCollision = settings.destroyOnWallCollision;
        damageScript.destroyOnPlayerCollision = settings.destroyOnPlayerCollision;
        damageScript.ignorePlayer = settings.ignorePlayer;
        damageScript.playerThatShotThis = settings.playerWhoSpawnedThis;
        Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), settings.ignorePlayer.GetComponent<Collider2D>());

        if (GetComponent<TrailRenderer>() != null)
        {
            CustomFunctions.SetTrailColor(GetComponent<TrailRenderer>(), GameManager.instance.GetPlayerId(settings.playerWhoSpawnedThis));
        }
    }
}

[System.Serializable]
public class NetworkAmmoSettings
{
    public GameObject playerWhoSpawnedThis;

    public Quaternion bulletRotation;
    public float ammoSpeed;
    public Vector2 ammoDirection;

    public bool destroyOnWallCollision;
    public bool destroyOnPlayerCollision;
    public GameObject ignorePlayer;
}
