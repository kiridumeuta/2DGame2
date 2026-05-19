using UnityEngine;

public class Boss1 : MonoBehaviour
{
    [Header("移動")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("距離設定")]
    [SerializeField] private float approachDistance = 8f; // 近づく距離
    [SerializeField] private float retreatDistance = 3f;  // 下がる距離

    [Header("ジャンプ")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpInterval = 3f;

    [Header("地面判定")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("起動設定")]
    [SerializeField] private bool activateWhenVisible = true;

    [Header("突進")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashCooldown = 4f;
    [SerializeField] private float dashStopTime = 1f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Camera mainCamera;

    private Transform player;

    private bool isGrounded;
    private bool isActive = false;
    private bool isDashing = false;

    private float jumpTimer;
    private float dashCooldownTimer;
    private float dashTimer;
    private float dashStopTimer;


    private int moveDir = 0;
    private int dashDir;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        mainCamera = Camera.main;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        dashCooldownTimer = dashCooldown;

        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        jumpTimer = jumpInterval;

        // 最初から動かすかどうか
        if (!activateWhenVisible)
        {
            isActive = true;
        }
    }

    void Update()
    {
        // 画面内に入ったら起動
        if (!isActive && activateWhenVisible)
        {
            CheckVisible();
            return;
        }

        if (player == null) return;

        // 地面判定
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // プレイヤー方向
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        // 向き変更
        sr.flipX = dir < 0;

        // =========================
        // 突進後硬直
        // =========================
        if (dashStopTimer > 0f)
        {
            dashStopTimer -= Time.deltaTime;
            moveDir = 0;
            return;
        }

        // =========================
        // 突進中
        // =========================
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            moveDir = dashDir;

            if (dashTimer <= 0f)
            {
                isDashing = false;
                dashStopTimer = dashStopTime;
            }

            return;
        }

        // =========================
        // 突進クールタイム
        // =========================
        dashCooldownTimer -= Time.deltaTime;

        if (dashCooldownTimer <= 0f)
        {
            StartDash((int)dir);
            dashCooldownTimer = dashCooldown;
            return;
        }

        // プレイヤーとの距離
        float distance = Vector2.Distance(transform.position, player.position);

        // 行動決定
        if (distance > approachDistance)
        {
            // プレイヤーへ近づく
            moveDir = (int)dir;
        }
        else if (distance < retreatDistance)
        {
            // 近すぎたら下がる
            moveDir = -(int)dir;
        }
        else
        {
            // 適正距離なら停止
            moveDir = 0;
        }

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

        float speed = moveSpeed;

        // 突進中は速度アップ
        if (isDashing)
        {
            speed = dashSpeed;
        }

        rb.linearVelocity = new Vector2(
            moveDir * speed,
            rb.linearVelocity.y
        );
    }

    void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void CheckVisible()
    {
        if (mainCamera == null) return;

        Vector3 viewPos =
            mainCamera.WorldToViewportPoint(transform.position);

        // 画面内に入ったら起動
        if (viewPos.x > 0f && viewPos.x < 1f &&
            viewPos.y > 0f && viewPos.y < 1f)
        {
            isActive = true;
        }
    }

    void StartDash(int dir)
    {
        isDashing = true;

        dashDir = dir;

        dashTimer = dashDuration;
    }

    private void OnDrawGizmosSelected()
    {
        // 地面判定
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }

        // 距離確認用
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, approachDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}
