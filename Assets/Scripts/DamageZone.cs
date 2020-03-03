using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float damage = 10f;
    public GameObject ignorePlayer;
    public GameObject hitParticlePrefab;
    public bool destroyOnWallCollision = false;
    public bool destroyOnPlayerCollision = false;
    public LayerMask ignoreLayer;
    public GameObject playerThatShotThis;

    public Action<Collision2D> OnWallCollision;

    [HideInInspector]
    public bool dontDoDamage;

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IHealthEntity>() != null)
        {
            if (collision.gameObject != ignorePlayer && ignoreLayer != (ignoreLayer | (1 << collision.gameObject.layer)))
            {
                collision.GetComponent<IHealthEntity>().DoDamage(damage);
                if (destroyOnPlayerCollision)
                    DestroyThis();
            }
        }

        if (collision.tag == "Obstacle")
        {

            if (OnWallCollision != null)
            {
                OnWallCollision(collision);
            }

            if (collision.gameObject != ignorePlayer && destroyOnWallCollision)
            {
                DestroyThis();
            }
        }
    }*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<IHealthEntity>() != null)
        {
            if (collision.gameObject != ignorePlayer && ignoreLayer != (ignoreLayer | (1 << collision.gameObject.layer)))
            {
                /*collision.gameObject.GetComponent<IHealthEntity>().DoDamage(damage, playerThatShotThis);
                if (GameManager.instance.isInOnlineMultiplayer)
                    NetworkPlayer.localPlayer.CmdApplyDamage(collision.gameObject, damage, NetworkPlayer.localPlayer.gameObject);*/
                //if (GameManager.instance.isInOnlineMultiplayer == false)
                    ApplyDamage(collision.gameObject);
                if (destroyOnPlayerCollision)
                    DestroyThis();
            }
        }

        if (collision.gameObject.tag == "Obstacle")
        {

            if (OnWallCollision != null)
            {
                OnWallCollision(collision);
            }

            if (collision.gameObject != ignorePlayer && destroyOnWallCollision)
            {
                DestroyThis();
            }
        }
    }

    void ApplyDamage(GameObject playerThatWeTouched)
    {
        if (dontDoDamage)
            return;

        playerThatWeTouched.GetComponent<IHealthEntity>().DoDamage(damage, playerThatShotThis);
        if (GameManager.instance.isInOnlineMultiplayer)
            NetworkPlayer.localPlayer.CmdApplyDamage(playerThatWeTouched, damage, NetworkPlayer.localPlayer.gameObject);
    }

    public void DestroyThis()
    {
        if (hitParticlePrefab != null)
        {
            GameObject hitParticle = Instantiate(hitParticlePrefab);
            hitParticle.transform.position = this.transform.position;
            CustomFunctions.PlaySound(CustomFunctions.instance.ammoHit);
            Destroy(hitParticle, 1f);
        }
        Destroy(this.gameObject);
    }
}
