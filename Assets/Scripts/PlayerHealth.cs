using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IHealthEntity
{
    public int currentLives = 4;
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    GameObject currentPlayerHealthUI;
    GameObject currentLifeHolderHealthUI;
    SpriteRenderer playerRenderer;
    PlayerMotor playerMotor;
    [SerializeField]
    AudioClip getHitSound;
    [SerializeField]
    AudioClip parryHitSound;
    [SerializeField]
    AudioClip parryBubbleSound;
    [SerializeField]
    GameObject healthIconPrefab;
    [SerializeField]
    public GameObject damageDisplayPrefab;
    [SerializeField]
    Image healthContent;
    Animator playerAnim;
    [SerializeField]
    bool isInvincible = false;

    [SerializeField]
    GameObject parryCircle;
    public ParryState currentParryState;
    bool parryWasSuccessful = false;
    public enum ParryState
    {
        None,
        IsParrying,
        ParryCooldown
    }

    private void Start()
    {
        currentPlayerHealthUI = GameManager.instance.playerHealthUI[GameManager.instance.GetPlayerId(this.gameObject)];
        playerRenderer = GetComponent<SpriteRenderer>();
        playerAnim = GetComponent<Animator>();
        playerMotor = GetComponent<PlayerMotor>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            GetComponent<IHealthEntity>().DoDamage(15f, this.gameObject);

        if (Input.GetKeyDown(KeyCode.I))
            NetworkPlayer.localPlayer.CmdApplyDamage(this.gameObject, 15f, this.gameObject);

        healthContent.fillAmount = Mathf.Lerp(healthContent.fillAmount, currentHealth / maxHealth, Time.deltaTime * Mathf.Exp(healthContent.fillAmount) * 4f);
    }

    public void SetMaxHealth(int weaponId)
    {
        maxHealth = GameManager.instance.weaponDatabase.allWeapons[weaponId].healthPercentage;
        currentHealth = maxHealth;
        if (GameManager.instance.isInOnlineMultiplayer && NetworkPlayer.localPlayer != null)
            NetworkPlayer.localPlayer.CmdSetHealth(GameManager.instance.GetPlayerId(this.gameObject), maxHealth);
        RefreshDamageDisplay();
    }

    void OnParry()
    {
        if (currentParryState == ParryState.None)
            StartCoroutine(Parry());
    }

    void IHealthEntity.DoDamage(float damageAmount, GameObject playerThatShot)
    {
        if (currentHealth <= 0 || isInvincible) //if we're playing the death anim, we dont take damage
            return;

        if (currentParryState != ParryState.IsParrying)
        {
            currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth + damageAmount, 0, maxHealth);
        }

        TakeHitEffect(damageAmount, playerThatShot);

        if (currentHealth <= 0)
            Die(playerThatShot);
    }

    public void TakeHitEffect(float damageAmount, GameObject playerThatShot)
    {
        if (currentParryState != ParryState.IsParrying)
        {
            RefreshDamageDisplay();
            CustomFunctions.HitPause();
            CustomFunctions.PlaySound(getHitSound);
            DisplayDamageOnMap(damageAmount, false);
            StopCoroutine(HitEffectSpriteBlink());
            StartCoroutine(HitEffectSpriteBlink());
        }
        else
        {
            ResetParry();
            CustomFunctions.PlaySound(parryHitSound);
            RefreshDamageDisplay();
            DisplayDamageOnMap(damageAmount, true);
            CustomFunctions.HitPause();
        }

        //print("Take hit effect with current health " + currentHealth);

    }

    void Die(GameObject playerThatShot)
    {
        currentLives--;
        playerAnim.Play("PlayerDie");
        RefreshDamageDisplay();
        if (currentLifeHolderHealthUI == null)
            currentLifeHolderHealthUI = currentPlayerHealthUI.transform.GetChild(4).gameObject;

        currentLifeHolderHealthUI.transform.GetChild(currentLives).gameObject.SetActive(false);

        GameManager.instance.connectedPlayers[GameManager.instance.GetPlayerId(this.gameObject)].numberOfDeaths++;
        if (playerThatShot != null)
            GameManager.instance.connectedPlayers[GameManager.instance.GetPlayerId(playerThatShot.gameObject)].numberOfKills++;
    }

    public void Respawn()
    {
        if (currentLives > 0)
        {
            transform.position = SpawnPointsManager.instance.GetPlayerRespawnPosition(GameManager.instance.GetPlayerId(this.gameObject));
            currentHealth = maxHealth;
            if (GameManager.instance.isInOnlineMultiplayer)
                NetworkPlayer.localPlayer.CmdSetHealth(GameManager.instance.GetPlayerId(this.gameObject), maxHealth);
            playerAnim.Play("Idle");
        }
        else
        {
            print("current lives is " + currentLives);
            currentHealth = 0f;
            this.gameObject.SetActive(false);
            ResultScreenManager.instance.playersWhoDiedOrder.Add(GameManager.instance.GetPlayerId(this.gameObject));
        }
        RefreshDamageDisplay();

        GameManager.instance.ShowResultScreenIfNecessary();
    }

    public void RefreshDamageDisplay()
    {
        if (currentPlayerHealthUI == null)
            currentPlayerHealthUI = GameManager.instance.playerHealthUI[GameManager.instance.GetPlayerId(this.gameObject)];

        currentPlayerHealthUI.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.FloorToInt(currentHealth) + "<size=25>." + GetDecimalValue(currentHealth) + "</size><size=20>HP</size>";
        currentPlayerHealthUI.GetComponentInChildren<TextMeshProUGUI>().color = GetPercentageColor();
        healthContent.color = GetPercentageColor();
    }

    void DisplayDamageOnMap(float damageAmount, bool heal = false)
    {
        GameObject dmgDisplay = Instantiate(damageDisplayPrefab);
        dmgDisplay.transform.position = this.transform.position;
        string damageDisplayContent = "";
        if (heal == false)
        {
            damageDisplayContent = "<color=#F35959>- " + damageAmount + "</color>";
        }
        else
        {
            damageDisplayContent = "<color=#68F669>+ " + damageAmount + "</color>";
        }

        dmgDisplay.GetComponentInChildren<TextMeshProUGUI>().text = damageDisplayContent;
        Destroy(dmgDisplay, 0.3f);
    }

    Color GetPercentageColor()
    {
        float ratio = currentHealth / maxHealth;
        Color output = new Color(0.48f, 0.67f, 0.36f);

        if (ratio > 0.33f && ratio < 0.66f)
            output = Color.yellow;
        else if (ratio <= 0.33f)
            output = Color.red;

        return output;
    }

    int GetDecimalValue(float input)
    {
        float output = input % 1;
        output = output * 10f;
        output = Mathf.RoundToInt(output);
        return (int)output;
    }

    IEnumerator HitEffectSpriteBlink()
    {
        playerRenderer.enabled = false;
        yield return new WaitForSeconds(0.1f);
        playerRenderer.enabled = true;
        yield return new WaitForSeconds(0.1f);
        playerRenderer.enabled = false;
        yield return new WaitForSeconds(0.1f);
        playerRenderer.enabled = true;
    }

    public void SetNumberOfLives()
    {
        if (currentPlayerHealthUI == null)
            currentPlayerHealthUI = GameManager.instance.playerHealthUI[GameManager.instance.GetPlayerId(this.gameObject)];
        if (currentLifeHolderHealthUI == null)
            currentLifeHolderHealthUI = currentPlayerHealthUI.transform.GetChild(4).gameObject;

        for (int i = 0; i < GameManager.instance.numberOfLivesThisGame; i++)
        {
            GameObject icon = Instantiate(healthIconPrefab, currentLifeHolderHealthUI.transform);
        }

        currentLives = GameManager.instance.numberOfLivesThisGame;
    }

    IEnumerator Parry()
    {
        if (CanParry())
        {
            if (currentParryState == ParryState.None)
            {
                playerAnim.Play("Parry");
                CustomFunctions.PlaySound(parryBubbleSound);
                currentParryState = ParryState.IsParrying;
                parryCircle.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                while (parryCircle.transform.localScale.x > 0)
                {
                    parryCircle.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f) * Mathf.Exp(1.5f - parryCircle.transform.localScale.x);
                    yield return new WaitForSeconds(0.01f);
                }
            }

            if (parryCircle.transform.localScale.x <= 0)
            {
                //print("DONE");
                currentParryState = ParryState.ParryCooldown;
                if (parryWasSuccessful == false)
                    yield return new WaitForSeconds(0.2f);

                StopParry();
            }
        }

        yield break;
    }

    void ResetParry()
    {
        parryCircle.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        parryWasSuccessful = true;
    }

    public void StopParry()
    {
        currentParryState = ParryState.None;
        parryCircle.transform.localScale = Vector3.zero;
        if (currentHealth > 0)
            playerAnim.Play("Idle");
    }

    bool CanParry()
    {
        if (currentHealth <= 0 || playerMotor.wallSliding == true)
        {
            return false;
        }

        return true;
    }

    void IHealthEntity.OnDie()
    {
        throw new NotImplementedException();
    }
}

public interface IHealthEntity
{
    void DoDamage(float damageAmount, GameObject playerThatShot);
    void OnDie();
}
