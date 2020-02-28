using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WeaponInfo : ScriptableObject
{
    public string weaponName;
    public Sprite weaponIcon;
    public GameObject weaponPrefab;
    public AudioClip weaponShootSound;
    public float waitTimeBetweenShots;
    public float healthPercentage = 100f;
    public float damage = 10f;
    public int ammoCount = 8;
    public float reloadTime = 1f;
    public float ammoVelocity = 50f;
}
