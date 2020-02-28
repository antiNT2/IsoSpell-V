using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieHealth : MonoBehaviour, IHealthEntity
{
    public float currentHealth = 1f;
    public float maxHealth = 50f;
    public AudioClip getHitSound;
    [SerializeField]
    Image healthBarContent;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        healthBarContent.fillAmount = Mathf.Lerp(healthBarContent.fillAmount, currentHealth / maxHealth, Time.deltaTime * 15f);
    }

    void IHealthEntity.DoDamage(float damageAmount, GameObject playerThatShot)
    {
        if (currentHealth <= 0) //if we're playing the death anim, we dont take damage
            return;

        currentHealth -= damageAmount;
        CustomFunctions.HitPause();
        CustomFunctions.PlaySound(getHitSound);

        if (currentHealth <= 0)
            Die(playerThatShot);
    }

    void Die(GameObject playerThatShotThis)
    {
        if (playerThatShotThis != null)
            GameManager.instance.connectedPlayers[GameManager.instance.GetPlayerId(playerThatShotThis.gameObject)].numberOfKills++;

        GetComponent<IHealthEntity>().OnDie();
        Destroy(this.gameObject);
    }

    void IHealthEntity.OnDie()
    {
        Spawner.instance.OnZombieDeath(this.gameObject);
    }
}
