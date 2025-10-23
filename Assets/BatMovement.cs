using System;
using UnityEngine;
using static TreeEditor.TreeGroup;

public class BatMovement : MonoBehaviour
{
    [SerializeField] private Transform _target1;
    [SerializeField] private Transform _target2;
    [SerializeField] private Transform _enemyView;
    [SerializeField] private float _viewRange;

    [SerializeField] private float moveFreq;
    [SerializeField] private float moveAmplitude;
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask _playerLayer;

    private Vector2 movePos;
    private Vector2 endPos;
    private SpriteRenderer spriteRenderer;
    private float lastTValue;
    private bool isChasing;
    private EnemyAI ai;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movePos = _target1.position;
        endPos = _target2.position; 
        spriteRenderer = GetComponent<SpriteRenderer>();
        ai = GetComponent<EnemyAI>();

        ai.enabled = false;
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
            Vector2 basePos = Vector2.Lerp(movePos, endPos, t);

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

        Collider2D foundPlayer = Physics2D.OverlapCircle(_enemyView.position, _viewRange, _playerLayer);
      
        if (foundPlayer != null && foundPlayer.GetComponent<isHero>())
        {
            isChasing = true;
            ai.enabled = true;
         
        }
 
    }
    private void OnDrawGizmos()
    {
        if (_enemyView == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(_enemyView.position, _viewRange);
    }
}
