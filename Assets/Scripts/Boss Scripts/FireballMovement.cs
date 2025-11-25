using UnityEngine;
/*
 * Author: Adrian Agius
 * File: FireballMovement.cs
 * Description: Fire attack for boss 1.
 * 
 */
public class FireballMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime); // Move fireball forward
    }
}
