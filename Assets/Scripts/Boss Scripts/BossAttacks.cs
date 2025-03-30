using System.Collections;
using System.Linq;
using UnityEngine;

public class BossAttacks : MonoBehaviour
{
    //Flying Skull Variables
    [SerializeField] private GameObject[] _skullPrefabs;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Transform _transform;
    [SerializeField] private GameObject[] monsters;
    [SerializeField] private GameObject[] firepillarsObjects;
    private BossMonster boss;
    private float spawnTime = 5f;
    private Animator _anim;
    private bool _spawning;
    private bool _triggeredWall = false;
    private float spellDelay=4f;
    int randomIndex = 0;
    int randomSpawn = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _anim = GetComponent<Animator>();
       boss  = GetComponent<BossMonster>();
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
        if (!boss.getDead())
        {
            if (!_spawning)
            {
                StartCoroutine(Spawner());
            }
            else if (!_triggeredWall && monsters.All(m => m.GetComponent<BossSmallEnemies>().isDead))
            {
                _triggeredWall = true; // Prevent multiple activations
                StartCoroutine(FireWall());
                if (boss.getHealth() < boss.getMaxHealth() * 0.5f)
                {
                    spawnTime = 2.5f;
                    spellDelay = 2f;
                }
            }

        }
    }

   
    private IEnumerator Spawner()
    {
        _spawning = true;
        yield return new WaitForSeconds(spawnTime);
        SpawnEnemies();  
        _spawning = false; 

    }

    private IEnumerator FireWall()
    {
        _anim.SetTrigger("Casting");
        yield return new WaitForSeconds(spellDelay);
        foreach(GameObject pilar in firepillarsObjects)
        {
            pilar.SetActive(true);
            yield return new WaitForSeconds(1f);
            pilar.SetActive(false);
        }
        _triggeredWall=false;
    }
}
