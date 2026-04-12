using UnityEngine;

public class EnemyJump : MonoBehaviour
{
    [SerializeField, Header("移動速度")]
    private float moveSpeed = 2f;
    [SerializeField, Header("ジャンプ力")]
    private float jumpForce = 5f;
    [SerializeField, Header("ジャンプ間隔（秒）")]
    private float jumpInterval = 2f;

    [SerializeField, Header("地面判定用レイヤー")]
    private LayerMask groundLayer;
    [SerializeField, Header("地面判定の位置")]
    private Transform groundCheck;
    [SerializeField, Header("地面判定の半径")]
    private float groundCheckRadius = 0.1f;

    [Header("カメラ外消滅")]
    [SerializeField] private float DestroyEnemyWidth = -0.3f;
    [SerializeField] private float DestroyEnemyWidth2 = 1.3f;
    [SerializeField] private float DestroyEnemyHight = -0.3f;
    [SerializeField] private float DestroyEnemyHight2 = 1.3f;

    Rigidbody2D rb;
    SpriteRenderer sr;

    private Camera mainCamera;

    int moveDir = -1;
    float jumpTimer;
    bool isGrounded;

    bool isActive = true;

    // スポナー通知
    public delegate void EnemyDestroyed(EnemyJump enemy);
    public event System.Action<EnemyJump> OnDestroyed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        jumpTimer = jumpInterval;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!isActive) return;

        CheckOutOfCamera();

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
        if (!isActive) return;

        // 横移動
        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void Reverse()
    {
        moveDir *= -1;
        sr.flipX = moveDir > 0;
    }

    void CheckOutOfCamera()
    {
        if (mainCamera == null) return;

        Vector3 viewPos =
            mainCamera.WorldToViewportPoint(transform.position);

        if (viewPos.x < DestroyEnemyWidth || viewPos.x > DestroyEnemyWidth2 ||
            viewPos.y < DestroyEnemyHight || viewPos.y > DestroyEnemyHight2)
        {
            DestroyEnemy();
        }
    }

    public void DestroyEnemy()
    {
        OnDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    void SetActiveState(bool state)
    {
        sr.enabled = state;
        rb.simulated = state;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = state;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy"))
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
}
