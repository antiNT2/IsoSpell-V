using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    GameObject currentPlayerHealthUI;
    SpriteRenderer playerRenderer;
    [SerializeField]
    AudioClip getHitSound;

    private void Start()
    {
        currentPlayerHealthUI = GameManager.instance.playerHealthUI[GameManager.instance.GetPlayerId(this.gameObject)];
        playerRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            DoDamage(1.5f);
    }

    public void SetMaxHealth(int weaponId)
    {
        maxHealth = GameManager.instance.weaponDatabase.allWeapons[weaponId].healthPercentage;
        currentHealth = maxHealth;
        RefreshDamageDisplay();
    }

    public void DoDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
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
        currentHealth = maxHealth;
        transform.position = Vector2.zero;
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
}
