using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform[] _spawnPoint;
    [SerializeField] private GameObject[] _enemies;
    [SerializeField] private List<GameObject> _spawnedEnemies = new List<GameObject>();

    public void Spawn(Vector2 spawnPos)
    {
        for (int i = 0; i < _spawnPoint.Length; i++)
        {
            Transform spawnPoint = _spawnPoint[i];
            bool hasLivingEnemy = false;

            // Check if a living enemy exists at this spawn point
            foreach (GameObject enemy in _spawnedEnemies)
            {
                if (enemy != null && enemy.activeInHierarchy && enemy.transform.position == spawnPoint.position)
                {
                    hasLivingEnemy = true;
                    break; // Exit loop since we found a live enemy
                }
            }

            // If no living enemy exists, spawn only one enemy and break out of loop
            if (!hasLivingEnemy)
            {
                GameObject newEnemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
                _spawnedEnemies.Add(newEnemy); // Keep track of spawned enemies
                Debug.Log("Spawned one enemy at: " + spawnPoint.position);
                break; // Exit the loop to ensure only one enemy spawns per call
            }
        }
    }
}
