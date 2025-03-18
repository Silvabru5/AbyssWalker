using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // List of enemy prefabs (Skeleton, Spider, Undead)
    public GameObject[] enemyPrefabs;

    // Spawn locations where enemies will appear
    public Transform[] spawnPoints;

    // Time interval (in seconds) between enemy spawns
    public float spawnRate = 3f;

    // Maximum number of enemies allowed on screen at a time
    public int maxEnemies = 10;

    // Tracks the time of the next enemy spawn
    private float nextSpawnTime = 0f;

    void Update()
    {
        // Check if it's time to spawn a new enemy and if the max enemy limit isn't reached
        if (Time.time >= nextSpawnTime && CountEnemies() < maxEnemies)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnRate; // Schedule the next spawn
        }
    }

    // Handles enemy spawning at random locations
    void SpawnEnemy()
    {
        // Ensure there are enemy prefabs and spawn points available before spawning
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        // Select a random enemy from the enemyPrefabs list
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);

        // Select a random spawn location from the spawnPoints list
        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);

        // Instantiate (spawn) the selected enemy at the chosen spawn point
        Instantiate(enemyPrefabs[randomEnemyIndex], spawnPoints[randomSpawnIndex].position, Quaternion.identity);
    }

    // Counts the number of active enemies in the scene
    int CountEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}
