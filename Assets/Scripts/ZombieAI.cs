using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ZombieAI : MonoBehaviour
{
    Vector2 currentTargetPosition;
    IAstarAI currentAiPathfinding;
    [SerializeField]
    GameObject bulletPrefab;
    public float damage = 10f;
    public LayerMask ignoreLayerBullets;

    private void Start()
    {
        currentAiPathfinding = GetComponent<IAstarAI>();
        InvokeRepeating("Shoot", 1f, 1f);
    }

    void Update()
    {
        currentTargetPosition = currentAiPathfinding.destination;
        Debug.DrawRay(this.transform.position, GetAmmoDirection());

        /*if (Input.GetKeyDown(KeyCode.L))
            Shoot();*/
    }

    void Shoot()
    {
        if (GameManager.instance.isInWeaponSelection == true)
            return;

        GameObject spawnedAmmo = Instantiate(bulletPrefab);
        spawnedAmmo.transform.position = transform.position;

        float angle = Vector2.Angle(GetAmmoDirection(), Vector2.left);
        if (transform.position.y < currentTargetPosition.y)
        {
            angle += 180f;
            angle = 180f - angle;
        }
        spawnedAmmo.transform.rotation = Quaternion.Euler(0, 0, angle + 180f);

        spawnedAmmo.GetComponent<AmmoVelocity>().direction = Vector2.right;
        spawnedAmmo.GetComponent<AmmoVelocity>().speed = 20f;
        DamageZone ammoDamage = spawnedAmmo.GetComponent<DamageZone>();
        ammoDamage.ignorePlayer = this.gameObject;
        ammoDamage.damage = damage;
        ammoDamage.destroyOnWallCollision = true;
        ammoDamage.destroyOnPlayerCollision = true;
        ammoDamage.ignoreLayer = ignoreLayerBullets;
        spawnedAmmo.GetComponent<TrailRenderer>().startColor = Color.white;
    }

    Vector2 GetAmmoDirection()
    {
        return (Vector2)(currentTargetPosition - (Vector2)this.transform.position).normalized;
    }
}
