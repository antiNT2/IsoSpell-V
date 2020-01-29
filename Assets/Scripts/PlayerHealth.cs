using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentLives = 4;
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    GameObject currentPlayerHealthUI;
    GameObject currentLifeHolderHealthUI;
    SpriteRenderer playerRenderer;
    [SerializeField]
    AudioClip getHitSound;
    [SerializeField]
    GameObject healthIconPrefab;
    Animator playerAnim;

    private void Start()
    {
        currentPlayerHealthUI = GameManager.instance.playerHealthUI[GameManager.instance.GetPlayerId(this.gameObject)];
        playerRenderer = GetComponent<SpriteRenderer>();
        playerAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            DoDamage(15f);
    }

    public void SetMaxHealth(int weaponId)
    {
        maxHealth = GameManager.instance.weaponDatabase.allWeapons[weaponId].healthPercentage;
        currentHealth = maxHealth;
        RefreshDamageDisplay();
    }

    public void DoDamage(float damageAmount)
    {
        if (currentHealth <= 0) //if we're playing the death anim, we dont take damage
            return;

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);
        RefreshDamageDisplay();
        CustomFunctions.HitPause();
        CustomFunctions.PlaySound(getHitSound);
        StopCoroutine(HitEffectSpriteBlink());
        StartCoroutine(HitEffectSpriteBlink());

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        currentLives--;
        /* if (currentLives > 0)
         {
             playerAnim.Play("PlayerDie");
         }
         else
         {
             currentHealth = 0f;
             this.gameObject.SetActive(false);
         }*/
        playerAnim.Play("PlayerDie");
        RefreshDamageDisplay();
        if (currentLifeHolderHealthUI == null)
            currentLifeHolderHealthUI = currentPlayerHealthUI.transform.GetChild(4).gameObject;

        currentLifeHolderHealthUI.transform.GetChild(currentLives).gameObject.SetActive(false);
    }

    public void Respawn()
    {
        if (currentLives > 0)
        {
            transform.position = Vector2.zero;
            currentHealth = maxHealth;
            playerAnim.Play("Idle");
        }
        else
        {
            currentHealth = 0f;
            this.gameObject.SetActive(false);
        }
        RefreshDamageDisplay();
    }

    void RefreshDamageDisplay()
    {
        if (currentPlayerHealthUI == null)
            currentPlayerHealthUI = GameManager.instance.playerHealthUI[GameManager.instance.GetPlayerId(this.gameObject)];

        currentPlayerHealthUI.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.FloorToInt(currentHealth) + "<size=25>." + GetDecimalValue(currentHealth) + "</size><size=20>%</size>";
        currentPlayerHealthUI.GetComponentInChildren<TextMeshProUGUI>().color = GetPercentageColor();
    }

    Color GetPercentageColor()
    {
        float ratio = currentHealth / maxHealth;
        Color output = Color.green;

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
}
