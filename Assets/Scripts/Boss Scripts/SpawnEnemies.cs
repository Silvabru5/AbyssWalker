using System.Collections;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private Animator _anim;
    private Collider2D _col;
    public bool isDead = false;
    void Start()
    {
        _anim = GetComponent<Animator>();
        _currentHealth = _maxHealth;
        _anim.SetFloat("CurrentHP", 100);
        _col = GetComponent<Collider2D>();

        _anim.SetTrigger("Death");
        StartCoroutine(RespawnEnemy());
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _anim.SetTrigger("Hurt");
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        _anim.SetTrigger("Death");
        _col.enabled = false;
        Debug.Log("Enemy Died");
        StartCoroutine(RespawnEnemy());
     
    }

    // Update is called once per frame
    void Update()
    {
        _anim.SetFloat("CurrentHP", (_currentHealth / _maxHealth) * 100);

    }

    private IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(8f);
        isDead = false;
        _anim.SetTrigger("Recover");
        _col.enabled = true;
        _currentHealth = _maxHealth;
    }

}
