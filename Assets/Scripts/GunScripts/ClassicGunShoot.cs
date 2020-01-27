using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicGunShoot : MonoBehaviour, IWeaponAction
{
    [SerializeField]
    GameObject ammo;
    [SerializeField]
    Transform ammoInitialPos;
    [SerializeField]
    Transform ammoInitialPosFlipY;
    PlayerController playerController;

    [SerializeField]
    Gradient player1AmmoTrail;
    [SerializeField]
    Gradient player2AmmoTrail;
    [SerializeField]
    Gradient player3AmmoTrail;
    [SerializeField]
    Gradient player4AmmoTrail;

    private void Start()
    {
        playerController = this.transform.parent.parent.GetComponent<PlayerController>();
    }

    void IWeaponAction.Reload()
    {
        throw new System.NotImplementedException();
    }

    void IWeaponAction.Shoot()
    {
        print("Shoot");
        GameObject spawnedAmmo = Instantiate(ammo);

        if (GetComponent<SpriteRenderer>().flipY == false)
            spawnedAmmo.transform.position = ammoInitialPos.position;
        else
            spawnedAmmo.transform.position = ammoInitialPosFlipY.position;

        spawnedAmmo.transform.rotation = Quaternion.Euler(0, 0, playerController.aimAngle * Mathf.Rad2Deg);

        spawnedAmmo.GetComponent<AmmoVelocity>().direction = Vector2.right;
        spawnedAmmo.GetComponent<DamageZone>().ignorePlayer = playerController.gameObject;
        SetTrailColor(spawnedAmmo.GetComponent<TrailRenderer>(), GameManager.instance.GetPlayerId(playerController.gameObject));

        Destroy(spawnedAmmo, 10f);
    }

    void SetTrailColor(TrailRenderer trail, int playerId)
    {
        Gradient colorToUse = player1AmmoTrail;
        if (playerId == 1)
            colorToUse = player2AmmoTrail;
        else if (playerId == 2)
            colorToUse = player3AmmoTrail;
        else if (playerId == 3)
            colorToUse = player4AmmoTrail;

        trail.colorGradient = colorToUse;
    }

    /*void IWeaponAction.Shoot()
    {
        print("Shoot");
        GameObject spawnedAmmo = Instantiate(ammo);

        if (GetComponent<SpriteRenderer>().flipY == false)
            spawnedAmmo.transform.position = ammoInitialPos.position;
        else
            spawnedAmmo.transform.position = ammoInitialPosFlipY.position;

        /*Vector2 aimDirection = playerController.GetAimDirection();
        if (aimDirection.x > 0)
            spawnedAmmo.GetComponent<AmmoVelocity>().direction = aimDirection;
        else
            spawnedAmmo.GetComponent<AmmoVelocity>().direction = new Vector2(Mathf.Abs(aimDirection.x), 1 - aimDirection.y);
        spawnedAmmo.transform.rotation = Quaternion.AngleAxis(playerController.aimAngle * Mathf.Rad2Deg, Vector3.forward);*/

    /* spawnedAmmo.GetComponent<AmmoVelocity>().direction = playerController.GetAimDirection();

     Destroy(spawnedAmmo, 10f);
 }*/
}
