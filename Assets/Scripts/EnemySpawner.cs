using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // list of enemies (Skeleton, Spider, Undead)
    public Transform[] spawnPoints;   // Spawn locations
    public float spawnRate = 3f;      // Time between spawns
    public int maxEnemies = 10;       // Max enemies allowed on screen

    private float nextSpawnTime = 0f;

    void Update()
    {
        if (Time.time >= nextSpawnTime && CountEnemies() < maxEnemies)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length); // Pick a random enemy
        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);  // Pick a random spawn location

        Instantiate(enemyPrefabs[randomEnemyIndex], spawnPoints[randomSpawnIndex].position, Quaternion.identity);
    }

    int CountEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}
