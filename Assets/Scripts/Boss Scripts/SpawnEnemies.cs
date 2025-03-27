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
        Respawn();
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _anim.SetTrigger("Hurt");

        if (_currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    void Die()
    {
        _anim.SetTrigger("Death");
        _col.enabled = false;
        Respawn();
        Debug.Log("Enemy Died");
    }

    public void Respawn()
    {
        _anim.SetTrigger("Death");
        _col.enabled = true;
        _currentHealth = _maxHealth;
        StartCoroutine(RespawnEnemy());
    }
    // Update is called once per frame
    void Update()
    {
        _anim.SetFloat("CurrentHP", (_currentHealth / _maxHealth) * 100);
    }

    private IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(4f);
        _anim.SetTrigger("Recover");
    }

}
