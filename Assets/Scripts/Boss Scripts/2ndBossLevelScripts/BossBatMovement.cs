using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

/*
    Author(s): Bruno Silva
    Description: handles the boss bat’s idle movement using a horizontal ping-pong pattern combined
                 with a vertical sine-wave motion. also detects when the player enters a view radius,
                 switching the bat from idle movement to its ai-controlled chase state.
    Date (Last Modification): 11/22/2025
*/

public class BossBatMovement : MonoBehaviour
{
    [Header("target points (movement anchors)")]
    [SerializeField] private Transform[] _targets;
    [SerializeField] private Transform _enemyView;      // position used as the center for player detection
    [SerializeField] private float _viewRange;          // how far the bat can see the player

    [Header("movement settings")]
    [SerializeField] private float moveFreq;            // frequency of the sine-wave movement
    [SerializeField] private float moveAmplitude;       // height of the sine-wave movement
    [SerializeField] private float moveSpeed;           // base horizontal speed (randomized at runtime)
    [SerializeField] private LayerMask _playerLayer;    // layer used to detect the player

    private Transform movePos;                          // starting anchor for ping-pong movement
    private Transform endPos;                           // ending anchor for ping-pong movement
    private SpriteRenderer spriteRenderer;              // used to flip sprite depending on movement direction
    private float lastTValue;                           // tracks direction of ping-pong (left/right)
    private bool isChasing;                             // whether the bat is currently chasing the player
    private EnemyAI ai;                                 // reference to chase ai behavior (assigned in inspector)

    void Start()
    {
        // give each bat a slight variation in speed so they don’t perfectly overlap
        moveSpeed = Random.Range(0.05f, 0.15f);

        // find all child points under "Positions"
        Transform[] allPoints = GameObject.Find("Positions").GetComponentsInChildren<Transform>();

        // exclude the parent object itself (first element)
        allPoints = Array.FindAll(allPoints, t => t != allPoints[0]);

        // randomly pick a pair of points to move between
        int pairIndex = Random.Range(0, allPoints.Length / 2);
        movePos = allPoints[pairIndex * 2];
        endPos = allPoints[pairIndex * 2 + 1];

        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        // idle movement happens only if we are not chasing the player
        if (!isChasing)
        {
            // horizontal movement using ping-pong between two points
            float t = Mathf.PingPong(Time.time * moveSpeed, 1f);
            Vector2 basePos = Vector2.Lerp(movePos.position, endPos.position, t);

            // vertical sine wave motion for hovering effect
            float sineOffset = Mathf.Sin(Time.time * moveFreq) * moveAmplitude;

            // final movement combination
            Vector2 sineMove = new Vector2(basePos.x, basePos.y + sineOffset);
            transform.position = sineMove;

            // determine direction and flip sprite
            bool movingRight = t > lastTValue;
            spriteRenderer.flipX = !movingRight;

            lastTValue = t;
        }

        CheckForPlayer();
    }

    void CheckForPlayer()
    {
        // check if the player enters the detection radius
        Collider2D foundPlayer = Physics2D.OverlapCircle(_enemyView.position, _viewRange, _playerLayer);

        // ensure the collider belongs to the main player (using component check, not tags)
        if (foundPlayer != null && foundPlayer.GetComponent<isHero>())
        {
            isChasing = true;

            // if ai behavior is in use, enable it now
            if (ai != null)
                ai.enabled = true;
        }
    }
}
