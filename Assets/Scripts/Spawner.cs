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

    public bool isInZombieMode = false;

    public List<GameObject> allSpawnedZombies = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    public void BeginSpawning()
    {
        if (PlayerPrefs.GetString("mode") == "zombie")
            isInZombieMode = true;
        else
            isInZombieMode = false;

        if (isInZombieMode)
        {
            rate = rate / GameManager.instance.connectedPlayers.Count;
            StartCoroutine(SpawnRepeat());
        }
    }

    void Spawn()
    {
        if (allSpawnedZombies.Count >= 5)
            return;

        GameObject spawnedZombie = Instantiate(zombiePrefab);
        spawnedZombie.transform.position = SpawnPointsManager.instance.GetRandomZombieSpawn();

        allSpawnedZombies.Add(spawnedZombie);
    }

    public void OnZombieDeath(GameObject zombie)
    {
        allSpawnedZombies.Remove(zombie);
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
