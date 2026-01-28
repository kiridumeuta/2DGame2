using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameOverManager gameOverManager;  // GameOverManagerをインスペクターから参照

    [SerializeField, Header("移動速度")]
    private float moveSpeed = 5f;

    [SerializeField, Header("ジャンプ力")]
    private float JumpForce = 14f;
    [SerializeField, Header("踏んだ後のジャンプ力")]
    private float BoundJump = 8f;

    [SerializeField, Header("強ジャンプ（踏んだ後）")]
    private float SuperBoundJump = 16f;
    [SerializeField, Header("強ジャンプ受付時間")]
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter = 0f;   // ←内部カウンタ

    [SerializeField, Header("地面レイヤー")]
    private LayerMask groundLayer;
    [SerializeField, Header("地面判定の位置")]
    private Transform groundCheck;
    [SerializeField, Header("判定半径")]
    private float groundCheckRadius = 0.2f;

    /*[SerializeField, Header("壁判定距離")]
    private float wallCheckDistance = 0.1f;
    [SerializeField]
    private LayerMask wallLayer;
    private bool isTouchingWall;*/

    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    // 二段ジャンプ
    private bool canDoubleJump = false;

    /*[SerializeField, Header("少し前に地面にいた場合のジャンプ許可")] private float coyoteTime = 0.1f;
    private float coyoteCounter;*/

    float moveInput;

    private PlayerHP playerHP;


    Animator animator;
    Rigidbody2D RB2D;

    void Start()
    {
        animator = GetComponent<Animator>();
        RB2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHP = GetComponent<PlayerHP>();
        // シーン内の GameOverManager を探す
        if (gameOverManager == null)
        {
            Debug.LogError("GameOverManagerが設定されていません。インスペクターで設定してください。");
        }
    }

    void Update()
    {
        moveInput = 0f;

        if (Input.GetKey(KeyCode.D)) moveInput = 1f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;

        CheckGround();
        /*CheckWall();*/
        PlayerJump();

        // 踏んだ後の受付時間を減らす
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;

            // 強ジャンプ入力があったら
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StrongBoundJump();
                jumpBufferCounter = 0; // 受付終了
            }
        }
    }

    void FixedUpdate()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        /*float targetX = moveInput * moveSpeed;

        if (!isGrounded && isTouchingWall)
        {
            targetX = 0f; // 空中で壁に張り付かない
        }*/

        RB2D.linearVelocity = new Vector2(moveInput * moveSpeed, RB2D.linearVelocity.y);

        // 向き
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        bool isWalking = (Mathf.Abs(moveInput) > 0f) && isGrounded;
        animator.SetBool("Walk", isWalking);
    }
    private void PlayerJump()
    {

        // 1段目ジャンプ（地上）
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            RB2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

            animator.SetTrigger("JumpStart"); // ← Trigger に変更！

            canDoubleJump = true;  // 空中でもう1回OK
            return;
        }

        // 2段目ジャンプ（空中）
        if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && canDoubleJump)
        {
            // 一度速度をリセットすると綺麗な二段ジャンプになる
            RB2D.linearVelocity = new Vector2(RB2D.linearVelocity.x, 0f);
            RB2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

            animator.SetTrigger("DoubleJump"); // ← Trigger！

            canDoubleJump = false;  // もう二段ジャンプ不可
        }
    }

    private void CheckGround()
    {
        bool groundedNow = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 上昇中なら必ず空中扱い
        if (RB2D.linearVelocityY > 0.05f)
        {
            groundedNow = false;
        }

        //bool groundedNow = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // ★ Animator へ地上/空中の状態を送る
        animator.SetBool("IsGround", groundedNow);

        // 二段ジャンプのリセット
        if (groundedNow)
        {
            canDoubleJump = true;
        }

        isGrounded = groundedNow;

    }

    /*void CheckWall()
    {
        Vector2 dir = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        float halfHeight = GetComponent<Collider2D>().bounds.extents.y;

        Vector2 posTop = new Vector2(transform.position.x, transform.position.y + halfHeight);
        Vector2 posMid = transform.position;
        Vector2 posBottom = new Vector2(transform.position.x, transform.position.y - halfHeight);

        isTouchingWall = Physics2D.Raycast(posTop, dir, wallCheckDistance, wallLayer)
                      || Physics2D.Raycast(posMid, dir, wallCheckDistance, wallLayer)
                      || Physics2D.Raycast(posBottom, dir, wallCheckDistance, wallLayer);

        // デバッグ用にRayを可視化
        Debug.DrawRay(posTop, dir * wallCheckDistance, Color.red);
        Debug.DrawRay(posMid, dir * wallCheckDistance, Color.green);
        Debug.DrawRay(posBottom, dir * wallCheckDistance, Color.blue);
    }*/


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // プレイヤーの足が敵の上より上かを判定
            if (RB2D.linearVelocity.y <= 0f && groundCheck.position.y > collision.transform.position.y)
            {
                // 敵を消す
                Destroy(collision.gameObject);

                // 反動で跳ね返る
                RB2D.linearVelocity = new Vector2(RB2D.linearVelocity.x, BoundJump);

                // 強ジャンプ受付を開始
                jumpBufferCounter = jumpBufferTime;

                // 空中ジャンプリセット
                canDoubleJump = true;
            }
            else
            {
                Debug.Log("敵に当たりました");
                playerHP.TakeDamage(30); // ライフを減らす
            }
        }
        // 奈落判定
        if (collision.CompareTag("Fall"))
        {
            if (gameOverManager != null)
            {
                // GameOverManager に処理を任せる
                gameOverManager.TriggerGameOver();

                // 操作を止める
                RB2D.linearVelocity = Vector2.zero;
                RB2D.simulated = false;
            }
        }
    }

    //強ジャンプ処理
    private void StrongBoundJump()
    {
        RB2D.linearVelocity = new Vector2(RB2D.linearVelocity.x, SuperBoundJump);
    }


}
