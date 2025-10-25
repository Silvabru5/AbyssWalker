using UnityEngine;

public class FireboltScript : MonoBehaviour
{

    public float lifetime = 3f;
    public int damage = 1;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            // need damage code here
        }
    }
}
