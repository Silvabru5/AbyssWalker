using UnityEngine;

public class BossMonster : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 350f;
    [SerializeField] private float _current;
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject[] monsters;
    [SerializeField] private Collider2D _bCollider;
    [SerializeField] private GameObject platForms;
    void Start()
    {
        _anim = GetComponent<Animator>();
        _current = _maxHealth;
        _bCollider = GetComponent<Collider2D>();
        _bCollider.enabled = false;
    }

    public void TakeDamage(float damage)
    {
        _current -= damage;
        _anim.SetTrigger("Hit");

        if(_current <= 0)
        {
            Die();
        }
    }

    void Die()
    {
      //  _anim.SetBool("Dead", true);
        Debug.Log("Boss Dead");
    }
    // Update is called once per frame
    void Update()
    {
        _anim.SetFloat("CurrentHP", (_current / _maxHealth) * 100);
        if (monsters[0].GetComponent<BossSmallEnemies>().isDead == true && monsters[1].GetComponent<BossSmallEnemies>().isDead == true &&
            monsters[2].GetComponent<BossSmallEnemies>().isDead == true && monsters[3].GetComponent<BossSmallEnemies>().isDead == true)
        {
            _anim.SetBool("isLowered",true);
            _bCollider.enabled = true;
            platForms.SetActive(true);
        }
        
    }
}
