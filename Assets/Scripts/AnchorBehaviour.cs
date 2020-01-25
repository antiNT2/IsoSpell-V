using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorBehaviour : MonoBehaviour
{
    public float speed = 1f;
    public Vector2 direction;
    public Vector2 destination;
    public GameObject playerOwner;
    LineRenderer playerLineRenderer;

    public Action OnCollideWithObstacle;
    public Action OnArriveAtDestination;

    bool onArriveAtDestinationActivated;

    private void Start()
    {
        playerLineRenderer = playerOwner.GetComponent<LineRenderer>();
        playerLineRenderer.positionCount = 2;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        SetLineRenderer();

        if (Vector2.Distance(transform.position, destination) < 0.1f && onArriveAtDestinationActivated == false)
        {
            if (OnArriveAtDestination != null)
                OnArriveAtDestination();
            onArriveAtDestinationActivated = true;
        }
    }

    void SetLineRenderer()
    {
        if (onArriveAtDestinationActivated == true)
            return;

        playerLineRenderer.SetPosition(0, playerOwner.transform.position);
        playerLineRenderer.SetPosition(1, this.transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("COLLIDED with " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Obstacle")
        {
            if (OnCollideWithObstacle != null)
                OnCollideWithObstacle();
        }
    }
}
