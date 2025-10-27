using System.Collections;
using TMPro;
using UnityEngine;

public class BossSmallEnemies : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _healthPercent;
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject _spawner;
    [SerializeField] private Transform _pivot;
   
    private Collider2D _col;
    public HealthBar healthBar;
    public bool isDead = false;
     void Start()
    {
        _anim = GetComponent<Animator>();
        _currentHealth = _maxHealth;
        healthBar.SetMaxHealth(_maxHealth);
        _anim.SetFloat("CurrentHP", 100);
        _col = GetComponent<Collider2D>();
        _spawner = GameObject.Find("SpawnPoints");
    }

    public float  GetCurrenthealth()
    {
        return _currentHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        healthBar.SetHealth(_currentHealth);
        _anim.SetTrigger("isHit");

        if(_currentHealth <= 0)
        {
            isDead = true;
   //         SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_VATS_DESTROYED);
            Die();
        }
       // else
     //       SoundManager.PlaySound(SoundTypeEffects.NECROMANCER_VATS_TAKES_DAMAGE);

    }

    void Die()
    {
        _anim.SetBool("isDead", isDead);
        _col.enabled = false;
        Debug.Log("Enemy Died");
        _spawner.GetComponent<SpawnPoints>().Spawn(_pivot.position);
    }
    // Update is called once per frame
    void Update()
    {
        _healthPercent = (_currentHealth / _maxHealth) * 100;
        _anim.SetFloat("CurrentHP",_healthPercent);
    }

}
