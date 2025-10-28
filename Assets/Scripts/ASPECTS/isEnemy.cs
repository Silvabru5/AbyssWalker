using UnityEngine;

public class isEnemy : MonoBehaviour
{
    private float _maxHealth = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(_maxHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        _maxHealth -= damage;
    }

    public float getHealth()
    {
        return _maxHealth;
    }
}
