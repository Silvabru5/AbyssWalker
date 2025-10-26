using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossBatMovement : MonoBehaviour
{
    [SerializeField] private Transform[] _targets;
    [SerializeField] private Transform _enemyView;
    [SerializeField] private float _viewRange;

    [SerializeField] private float moveFreq;
    [SerializeField] private float moveAmplitude;
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask _playerLayer;

    private Transform movePos;
    private Transform endPos;
    private SpriteRenderer spriteRenderer;
    private float lastTValue;
    private bool isChasing;
    private EnemyAI ai;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Set a random amount of movement speed so that bats do not overlap 
        moveSpeed = Random.Range(0.05f, 0.15f);
        // Find all points with tag "BatPoint"
        Transform[] allPoints = GameObject.Find("Positions").GetComponentsInChildren<Transform>();

        // Exclude the parent itself
        allPoints = System.Array.FindAll(allPoints, t => t != allPoints[0]);

        int pairIndex = Random.Range(0, allPoints.Length / 2);
        movePos = allPoints[pairIndex * 2];
        endPos = allPoints[pairIndex * 2 + 1];

        spriteRenderer = GetComponent<SpriteRenderer>();
        //ai = GetComponent<EnemyAI>();

        //ai.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (!isChasing)
        {
            // Horizontal movement using PingPong
            float t = Mathf.PingPong(Time.time * moveSpeed, 1f);
            Vector2 basePos = Vector2.Lerp(movePos.position, endPos.position, t);

            // Vertical sine-wave motion
            float sineOffset = Mathf.Sin(Time.time * moveFreq) * moveAmplitude;

            // Combine both
            Vector2 sineMove = new Vector2(basePos.x, basePos.y + sineOffset);

            transform.position = sineMove;
            // Determine direction (for flipping)
            bool movingRight = t > lastTValue;
            spriteRenderer.flipX = !movingRight;

            lastTValue = t;
        }
        CheckForPlayer();

    }
    void CheckForPlayer()
    {
        // check if the player is in the radius
        Collider2D foundPlayer = Physics2D.OverlapCircle(_enemyView.position, _viewRange, _playerLayer);

        if (foundPlayer != null && foundPlayer.GetComponent<isHero>())
        {
            //Enable AI behaviour and stop the sine wave movement
            isChasing = true;
            ai.enabled = true;

        }

    }

    //private void OnDrawGizmos() // Drawing view to see ranges
    //{
    //    if (_enemyView == null)
    //    {
    //        return;
    //    }
    //    Gizmos.DrawWireSphere(_enemyView.position, _viewRange);
    //}
}
