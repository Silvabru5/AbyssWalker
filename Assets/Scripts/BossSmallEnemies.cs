using UnityEngine;

public class BossSmallEnemies : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private Animator _anim;
     void Start()
    {
        _anim = GetComponent<Animator>();
        _currentHealth = _maxHealth;
        _anim.SetFloat("CurrentHP", 100);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _anim.SetTrigger("isHit");

        if(_currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        _anim.SetBool("isDead", true);
        Debug.Log("Enemy Died");
    }
    // Update is called once per frame
    void Update()
    {
        _anim.SetFloat("CurrentHP", (_currentHealth/_maxHealth) * 100);
    }
}
