using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public int currentWeapon = 0;

    public void EquipWeapon(int weaponId)
    {
        GameManager.instance.SetPlayerEquipWeaponDisplay(GameManager.instance.GetPlayerId(this.gameObject), weaponId);
        currentWeapon = weaponId;
    }
}
