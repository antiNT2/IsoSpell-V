using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;

    [SerializeField]
    GameObject zombiePrefab;
    [SerializeField]
    float rate = 5f;

    private void Awake()
    {
        instance = this;
    }

    public void BeginSpawning()
    {
        if (PlayerPrefs.GetString("mode") == "zombie")
        {
            rate = rate / GameManager.instance.connectedPlayers.Count;
            StartCoroutine(SpawnRepeat());
        }
    }

    void Spawn()
    {
        GameObject spawnedZombie = Instantiate(zombiePrefab);
        spawnedZombie.transform.position = SpawnPointsManager.instance.GetRandomZombieSpawn();
    }

    IEnumerator SpawnRepeat()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(rate);
            rate = Mathf.Clamp(rate -= 0.2f, 1f, 10f);
        }
    }
}
