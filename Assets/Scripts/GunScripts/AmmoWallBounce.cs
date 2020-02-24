using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoWallBounce : MonoBehaviour
{
    DamageZone ammoDamageZone;
    AmmoVelocity ammoVelocity;
    public AudioClip bounceSound;

    public int numberOfBouncesBeforeDestroying = 1;
    int currentNumberOfBounces = 0;

    private void Start()
    {
        ammoDamageZone = GetComponent<DamageZone>();
        ammoVelocity = GetComponent<AmmoVelocity>();
        ammoDamageZone.OnWallCollision += BounceAmmo;
    }

    void BounceAmmo(Collision2D collision)
    {
        if (currentNumberOfBounces < numberOfBouncesBeforeDestroying)
        {
            currentNumberOfBounces++;

            CustomFunctions.PlaySound(bounceSound);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - 180f);
        }
        else
        {
            ammoDamageZone.DestroyThis();
        }
    }
}
