using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class PlayerWeapon : MonoBehaviour
{
    public int currentWeapon = 0;
    [SerializeField]
    Transform weaponHolder;
    GameObject currentSpawnedWeaponPrefab;
    SpriteRenderer currentSpawnedWeaponRenderer;
    SpriteRenderer playerRenderer;
    IWeaponAction currentWeaponAction;
    GameObject crosshair;
    PlayerController playerController;

    WeaponState currentWeaponState;
    enum WeaponState
    {
        ReadyToShoot,
        WaitingForNextShot
    }

    private void Start()
    {
        crosshair = GetComponent<PlayerController>().crosshair;
        playerController = GetComponent<PlayerController>();
        playerRenderer = GetComponent<SpriteRenderer>();
    }

    public void EquipWeapon(int weaponId)
    {
        UnequipCurrentWeapon();
        GameManager.instance.SetPlayerEquipWeaponDisplay(GameManager.instance.GetPlayerId(this.gameObject), weaponId);
        currentWeapon = weaponId;

        WeaponInfo weaponToEquip = GameManager.instance.weaponDatabase.allWeapons[weaponId];
        currentSpawnedWeaponPrefab = Instantiate(weaponToEquip.weaponPrefab, weaponHolder);
        currentWeaponAction = currentSpawnedWeaponPrefab.GetComponent<IWeaponAction>();
        currentSpawnedWeaponRenderer = currentSpawnedWeaponPrefab.GetComponent<SpriteRenderer>();
        GetComponent<PlayerHealth>().SetMaxHealth(weaponId);
    }

    private void Update()
    {
        if(currentSpawnedWeaponPrefab != null)
        {
            //currentSpawnedWeaponPrefab.transform.LookAt(crosshair.transform);
            if (crosshair.transform.localPosition.x < 0)
                currentSpawnedWeaponRenderer.flipY = true;
            else
                currentSpawnedWeaponRenderer.flipY = false;

            currentSpawnedWeaponPrefab.transform.rotation = Quaternion.Euler(0, 0, playerController.aimAngle * Mathf.Rad2Deg);
        }
    }

    void UnequipCurrentWeapon()
    {
        if (weaponHolder.childCount > 0)
        {
            currentWeapon = 0;
            Destroy(weaponHolder.GetChild(0).gameObject);
            currentSpawnedWeaponPrefab = null;
            currentSpawnedWeaponRenderer = null;
            currentWeaponAction = null;
        }
    }

    void Shoot()
    {
        if (currentWeaponAction != null && GameManager.instance.isInWeaponSelection == false)
        {
            if (currentWeaponState == WeaponState.ReadyToShoot)
            {
                currentWeaponAction.Shoot();
                CameraShaker.Instance.ShakeOnce(1f, 2f, 0.1f, 0.1f);
                CustomFunctions.PlaySound(GameManager.instance.weaponDatabase.allWeapons[currentWeapon].weaponShootSound);
                currentWeaponState = WeaponState.WaitingForNextShot;
                StartCoroutine(WaitForNextShot());
            }
        }
    }

    #region InputMethods
    void OnShoot()
    {
        Shoot();
    }
    #endregion

    IEnumerator WaitForNextShot()
    {
        yield return new WaitForSeconds(GameManager.instance.weaponDatabase.allWeapons[currentWeapon].waitTimeBetweenShots);
        currentWeaponState = WeaponState.ReadyToShoot;
    }
}
public interface IWeaponAction
{
    void Shoot();
    void Reload();
}
