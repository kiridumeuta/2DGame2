using UnityEngine;

public class EnemyJump : MonoBehaviour
{
    [SerializeField, Header("移動速度")]
    private float moveSpeed = 2f;
    [SerializeField, Header("ジャンプ力")]
    private float jumpForce = 5f;
    [SerializeField, Header("ジャンプ間隔（秒）")]
    private float jumpInterval = 2f;

    Rigidbody2D rb;
    SpriteRenderer sr;

    int moveDir = -1;
    private float jumpTimer;

    [SerializeField, Header("地面判定用レイヤー")]
    private LayerMask groundLayer;
    [SerializeField, Header("地面判定の位置")]
    private Transform groundCheck;
    [SerializeField, Header("地面判定の半径")]
    private float groundCheckRadius = 0.1f;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        jumpTimer = jumpInterval;
    }

    void Update()
    {
        Debug.Log(isGrounded);
        // 地面判定
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // ジャンプタイマー
        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0f && isGrounded)
        {
            Jump();
            jumpTimer = jumpInterval;
        }
    }

    void FixedUpdate()
    {
        // 横移動
        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.y < 0.1f) // 横からぶつかったら反転
                {
                    Reverse();
                    break;
                }
            }
        }
    }

    void Reverse()
    {
        moveDir *= -1;
        sr.flipX = moveDir > 0;
    }

    void Jump()
    {
        // AddForce で上方向にジャンプ
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
