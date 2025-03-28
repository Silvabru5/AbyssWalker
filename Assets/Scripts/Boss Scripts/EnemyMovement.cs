using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    private Transform _target;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    void Update()
    {
        if (_target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, _target.position, speed * Time.deltaTime);
        }
    }
}
