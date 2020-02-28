using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingOfTheBarrelBehaviour : MonoBehaviour
{
    IWeaponAction currentWeaponScript;
    ClassicGunShoot gunShootScript;
    PlayerController playerController;

    private void Start()
    {
        currentWeaponScript = GetComponent<IWeaponAction>();
        playerController = this.transform.parent.parent.GetComponent<PlayerController>();
        gunShootScript = GetComponent<ClassicGunShoot>();

        currentWeaponScript.OnShoot += ShootShoddyBarrels;
    }

    void ShootShoddyBarrels()
    {
        float aimAngle = playerController.aimAngle;

        gunShootScript.ShootAmmo(aimAngle + GetRandomDeltaAngle());
        gunShootScript.ShootAmmo(aimAngle - GetRandomDeltaAngle());
    }

    float GetRandomDeltaAngle()
    {
        float output = Random.Range(0.2f, 0.5f);

        return output;
    }
}
