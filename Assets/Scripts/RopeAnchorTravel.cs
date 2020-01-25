using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAnchorTravel : MonoBehaviour
{
   /* enum AnchorState
    {
        None,
        TravellingToDestination,
        ArrivedAtDestination
    }
    Vector2 direction;
    Rigidbody2D rb;
    float speed = 3000f;

    AnchorState currentState;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(currentState == AnchorState.TravellingToDestination)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("Collided with " + collision.gameObject);
        if (collision.gameObject.tag == "Obstacle")
        {
            currentState = AnchorState.ArrivedAtDestination;           
        }
    }

    void ResetAnchor()
    {
        rb.velocity = Vector2.zero;
        transform.localPosition = Vector3.zero;
        currentState = AnchorState.None;
    }

    public void SetDirection(Vector2 _direction)
    {
        if(currentState == AnchorState.TravellingToDestination)
        {
            ResetAnchor();
            return;
        }

        direction = _direction;
        GetComponent<SpriteRenderer>().enabled = true;
        currentState = AnchorState.TravellingToDestination;
    }*/
}
