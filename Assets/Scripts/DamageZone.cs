using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float damage = 10f;
    public GameObject ignorePlayer;
    public bool destroyOnWallCollision = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print("collison " + collision.gameObject.name);
        if (collision.tag == "Player")
        {
            if (collision.gameObject != ignorePlayer)
                collision.GetComponent<PlayerHealth>().DoDamage(damage);
        }

        if (collision.tag == "Obstacle")
        {
            if (collision.gameObject != ignorePlayer)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
