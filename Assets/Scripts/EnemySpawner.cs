using UnityEngine;

/*
    Author(s): Bruno Silva
    Description: Handles spawning of enemies during gameplay. Randomly selects 
                 enemy types and spawn points, enforces a maximum number of active
                 enemies. Works with EnemyHealth, which calls 
                 EnemyDied() upon death to increment progression counters.
    Date (last modification): 11/22/2025
*/


public class EnemySpawner : MonoBehaviour
{
    // list of enemy prefabs (skeleton, spider, undead)
    public GameObject[] enemyPrefabs;

    // spawn locations where enemies will appear
    public Transform[] spawnPoints;

    // how often to spawn new enemies (in seconds)
    public float spawnRate = 3f;

    // maximum number of enemies allowed at once
    public int maxEnemies = 10;

    // tracks when the next enemy should spawn
    private float nextSpawnTime = 0f;
    // counter for level progression
    public int numOfEnemiesDead = 0;

    // just increments this is called in enemyHealth when an enemy dies
    public void EnemyDied()
    {
        numOfEnemiesDead++;
    }
    void Update()
    {
        // check if it's time to spawn and if we're under the enemy limit
        if (Time.time >= nextSpawnTime && CountEnemies() < maxEnemies)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    // picks a random enemy and spawn point and creates the enemy in the scene
    void SpawnEnemy()
    {
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);

        GameObject selectedEnemy = enemyPrefabs[randomEnemyIndex];
        Transform selectedSpawnPoint = spawnPoints[randomSpawnIndex];

        // create the enemy at the spawn location
        Instantiate(selectedEnemy, selectedSpawnPoint.position, Quaternion.identity);
    }

    // counts all currently active enemies by checking how many have the EnemyHealth component
    int CountEnemies()
    {
        return GameObject.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None).Length;
    }
}
