using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class BossMonster : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 350f;
    [SerializeField] private float _current;
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject[] monsters;
    [SerializeField] private Collider2D _bCollider;
    [SerializeField] private GameObject platForms;
    [SerializeField] private TextMeshProUGUI _text;
    private bool isDead;
    private float _healthPercent;
    public HealthBar healthBar;

    public bool getDead()
    {
        return isDead;
    }
    public float getHealth()
    {
        return _current;
    }

    public float getMaxHealth()
    {
        return _maxHealth;
    }
    void Start()
    {
        _anim = GetComponent<Animator>();
        _current = _maxHealth;
        healthBar.SetMaxHealth(_maxHealth);
        _bCollider = GetComponent<Collider2D>();
        _bCollider.enabled = false;
    }

    public void TakeDamage(float damage)
    {
        _current -= damage;
        ScreenShakeController.instance.StartShake(0.2f, 0.4f);
        healthBar.SetHealth(_current);
        _anim.SetTrigger("Hit");

        if (_current <= 0)
        {
            Die();
            isDead = true;
        }
    }

    void Die()
    {
        _anim.SetBool("Dead", true);
        _bCollider.enabled = false;
        StartCoroutine(Death());
        Debug.Log("Boss Dead");
    }
    // Update is called once per frame
    void Update()
    {
        _healthPercent = (_current / _maxHealth) * 100;
        _healthPercent = Mathf.Max(0, _healthPercent);
        _anim.SetFloat("CurrentHP", _healthPercent);
        _text.text = _healthPercent.ToString("F1") + "%";
        if (monsters.All(m => m.GetComponent<BossSmallEnemies>().isDead))
        {
            _anim.SetBool("isLowered", true);
            _bCollider.enabled = true;
            platForms.SetActive(true);
        }

    }

    private IEnumerator Death()
    {
        ScreenShakeController.instance.StartShake(3f, 0.5f);
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
