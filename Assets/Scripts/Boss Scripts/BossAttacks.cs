using System.Collections;
using UnityEngine;

public class BossAttacks : MonoBehaviour
{
    //Flying Skull Variables
    [SerializeField] private GameObject[] _skullPrefabs;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Transform _transform;
    [SerializeField] private GameObject[] monsters;
    private Animator _anim;
    private bool _spawning;
    int randomIndex = 0;
    int randomSpawn = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _anim = GetComponent<Animator>();
       
    }

    void SpawnEnemies()
    {
        randomIndex = Random.Range(0, _spawnPoints.Length);
        randomSpawn = Random.Range(0,_skullPrefabs.Length);
        GameObject enemyPrefab = _skullPrefabs[randomIndex];
        Transform spawnPoint = _spawnPoints[randomSpawn];

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        EnemyMovement enemyMovement = newEnemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.SetTarget(_transform);    
        }
         
        _spawning = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (!_spawning)
        {
            StartCoroutine(Spawner());
        }
        if (monsters[0].GetComponent<BossSmallEnemies>().isDead == true && monsters[1].GetComponent<BossSmallEnemies>().isDead == true &&
            monsters[2].GetComponent<BossSmallEnemies>().isDead == true && monsters[3].GetComponent<BossSmallEnemies>().isDead == true)
        {

        }

    }

   
    private IEnumerator Spawner()
    {
        _spawning = true;
        yield return new WaitForSeconds(5f);
        SpawnEnemies();  
        _spawning = false; 

    }

}
