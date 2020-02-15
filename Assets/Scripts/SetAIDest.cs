using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SetAIDest : MonoBehaviour
{
    IAstarAI ai;
    float initialMaxSpeed = 8f;

    void OnEnable()
    {
        ai = GetComponent<IAstarAI>();
        // Update the destination right before searching for a path as well.
        // This is enough in theory, but this script will also update the destination every
        // frame as the destination is used for debugging and may be used for other things by other
        // scripts as well. So it makes sense that it is up to date every frame.
        if (ai != null)
        {
            ai.onSearchPath += Update;
            initialMaxSpeed = ai.maxSpeed;
        }
    }

    void OnDisable()
    {
        if (ai != null) ai.onSearchPath -= Update;
    }

    /// <summary>Updates the AI's destination every frame</summary>
    void Update()
    {
        if (ai != null && GameManager.instance.isInWeaponSelection == false)
        {
            Vector3 destination = GetClosestPlayer();
            ai.destination = destination;

            //print(Vector2.Distance(destination, this.transform.position));

            if (Vector2.Distance(destination, this.transform.position) < 4f)
                ai.maxSpeed = 0f;
            else
                ai.maxSpeed = initialMaxSpeed;
        }
    }

    Vector3 GetClosestPlayer()
    {
        List<Transform> allPlayerTransform = new List<Transform>();
        List<ConnectedPlayer> allPlayers = GameManager.instance.connectedPlayers;

        List<float> allPlayersDistance = new List<float>();

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].playerObject.activeSelf == true)
                allPlayerTransform.Add(allPlayers[i].playerObject.transform);
        }

        for (int i = 0; i < allPlayerTransform.Count; i++)
        {
            allPlayersDistance.Add(Vector2.Distance(this.transform.position, allPlayerTransform[i].position));
        }

        Vector3 output = allPlayerTransform[allPlayersDistance.IndexOf(allPlayersDistance.Min())].position;
        return output;

    }
}
