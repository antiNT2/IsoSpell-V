using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using TMPro;
using UnityEngine.UI;

public class PlayerWeapon : MonoBehaviour
{
    public int currentWeapon = 1;
    [SerializeField]
    Transform weaponHolder;
    [SerializeField]
    Image reloadBarContent;
    GameObject currentSpawnedWeaponPrefab;
    SpriteRenderer currentSpawnedWeaponRenderer;
    SpriteRenderer playerRenderer;
    private TextMeshProUGUI currentPlayerHealthAmmoDisplay;
    IWeaponAction currentWeaponAction;
    GameObject crosshair;
    PlayerController playerController;
    public int maxAmmo = 1;
    public int currentAmmo = 1;

    WeaponState currentWeaponState;
    enum WeaponState
    {
        ReadyToShoot,
        WaitingForNextShot,
        Reloading
    }

    private void Start()
    {
        crosshair = GetComponent<PlayerController>().crosshair;
        playerController = GetComponent<PlayerController>();
        playerRenderer = GetComponent<SpriteRenderer>();
        currentPlayerHealthAmmoDisplay = GameManager.instance.playerHealthUI[GameManager.instance.GetPlayerId(this.gameObject)].transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void EquipWeapon(int weaponId)
    {
        UnequipCurrentWeapon();
        GameManager.instance.SetPlayerEquipWeaponDisplay(GameManager.instance.GetPlayerId(this.gameObject), weaponId);
        currentWeapon = weaponId;

        WeaponInfo weaponToEquip = GameManager.instance.weaponDatabase.allWeapons[weaponId];
        if (weaponToEquip.weaponPrefab != null)
            currentSpawnedWeaponPrefab = Instantiate(weaponToEquip.weaponPrefab, weaponHolder);
        currentWeaponAction = currentSpawnedWeaponPrefab.GetComponent<IWeaponAction>();
        currentSpawnedWeaponRenderer = currentSpawnedWeaponPrefab.GetComponent<SpriteRenderer>();
        GetComponent<PlayerHealth>().SetMaxHealth(weaponId);
        maxAmmo = weaponToEquip.ammoCount;
        currentAmmo = maxAmmo;
        RefreshAmmoDisplay();
    }

    private void Update()
    {
        if (currentSpawnedWeaponPrefab != null)
        {
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
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (currentWeaponAction != null && playerController.CanMove())
        {
            if (currentWeaponState == WeaponState.ReadyToShoot)
            {
                if (currentAmmo > 0)
                {
                    currentWeaponAction.Shoot();
                    CameraShaker.Instance.ShakeOnce(1f, 2f, 0.1f, 0.1f);
                    CustomFunctions.PlaySound(GameManager.instance.weaponDatabase.allWeapons[currentWeapon].weaponShootSound);
                    currentWeaponState = WeaponState.WaitingForNextShot;
                    RemoveAmmo();
                    StartCoroutine(WaitForNextShot());
                }
                /*else if (currentAmmo == 0)
                {
                    Reload();
                }*/
            }
        }
    }

    void RemoveAmmo()
    {
        currentAmmo--;
        RefreshAmmoDisplay();
        if (currentAmmo == 0)
            Reload();
    }

    void RefreshAmmoDisplay()
    {
        if (currentPlayerHealthAmmoDisplay == null)
            currentPlayerHealthAmmoDisplay = GameManager.instance.playerHealthUI[GameManager.instance.GetPlayerId(this.gameObject)].transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>();

        currentPlayerHealthAmmoDisplay.text = currentAmmo + "/" + maxAmmo;
    }

    void Reload()
    {
        if (currentWeaponState != WeaponState.Reloading)
        {
            StopCoroutine(ReloadAmmo());
            StartCoroutine(ReloadAmmo());
        }
    }

    #region InputMethods
    void OnShoot()
    {
        Shoot();
    }

    void OnReload()
    {
        Reload();
    }
    #endregion

    IEnumerator WaitForNextShot()
    {
        yield return new WaitForSeconds(GameManager.instance.weaponDatabase.allWeapons[currentWeapon].waitTimeBetweenShots);
        currentWeaponState = WeaponState.ReadyToShoot;
    }

    IEnumerator ReloadAmmo()
    {
        currentWeaponState = WeaponState.Reloading;
        reloadBarContent.transform.parent.gameObject.SetActive(true);

        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = 0.01f / GameManager.instance.weaponDatabase.allWeapons[currentWeapon].reloadTime; //The amount of change to apply.
        reloadBarContent.fillAmount = 0;
        while (progress < 1f)
        {
            //reloadBarContent.fillAmount = Mathf.Lerp(reloadBarContent.fillAmount, 1f, progress);
            reloadBarContent.fillAmount = progress;
            progress += increment;
            yield return new WaitForSeconds(0.01f);
        }

        currentAmmo = maxAmmo;
        RefreshAmmoDisplay();
        reloadBarContent.transform.parent.gameObject.SetActive(false);
        currentWeaponState = WeaponState.ReadyToShoot;
    }
}
public interface IWeaponAction
{
    void Shoot();
    void Reload();
}
