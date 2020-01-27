using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoVelocity : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 direction;

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
