using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsManager : MonoBehaviour
{
    public Transform player1Respawn;
    public Transform player2Respawn;
    public Transform player3Respawn;
    public Transform player4Respawn;

    public Transform[] zombieRandomSpawns;

    public static SpawnPointsManager instance;

    private void Start()
    {
        instance = this;
    }

    private void OnEnable()
    {
        GameManager.instance.SetAllPlayerRespawnPosition();
    }

    public Vector3 GetPlayerRespawnPosition(int playerId)
    {
        if (playerId == 0)
        {
            return player1Respawn.transform.position;
        }
        if (playerId == 1)
        {
            return player2Respawn.transform.position;
        }
        if (playerId == 2)
        {
            return player3Respawn.transform.position;
        }
        if (playerId == 3)
        {
            return player4Respawn.transform.position;
        }

        return Vector3.zero;
    }

    public Vector3 GetRandomZombieSpawn()
    {
        return zombieRandomSpawns[Random.Range(0, zombieRandomSpawns.Length)].position;
    }
}
