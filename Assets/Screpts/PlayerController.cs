using System.Collections;
using Unity.VisualScripting;
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

    [Header("無敵時間")]
    [SerializeField] private float invincibleTime = 1.5f;

    private bool isInvincible = false;

    int defaultLayer;
    int invincibleLayer;

    [SerializeField, Header("地面レイヤー")]
    private LayerMask groundLayer;
    [SerializeField, Header("地面判定の位置")]
    private Transform groundCheck;
    [SerializeField, Header("判定半径")]
    private float groundCheckRadius = 0.2f;

    [SerializeField] private Collider2D footCollider;

    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    // 二段ジャンプ
    private bool canDoubleJump = false;

    private bool hasBouncedThisFrame = false;

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

        defaultLayer = gameObject.layer;
        invincibleLayer = LayerMask.NameToLayer("PlayerInvincible");

        // シーン内の GameOverManager を探す
        if (gameOverManager == null)
        {
            Debug.LogError("GameOverManagerが設定されていません。インスペクターで設定してください。");
        }
    }

    void Update()
    {
        // 毎フレームリセット
        hasBouncedThisFrame = false;

        moveInput = 0f;

        if (Input.GetKey(KeyCode.D)) moveInput = 1f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;

        CheckGround();
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

            animator.SetTrigger("JumpStart"); // ← Trigger に変更

            canDoubleJump = true;  // 空中でもう1回OK
            return;
        }

        // 2段目ジャンプ（空中）
        if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && canDoubleJump)
        {
            // 一度速度をリセットすると綺麗な二段ジャンプになる
            RB2D.linearVelocity = new Vector2(RB2D.linearVelocity.x, 0f);
            RB2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

            animator.SetTrigger("DoubleJump"); // ← Trigger

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

        // ★ Animator へ地上/空中の状態を送る
        animator.SetBool("IsGround", groundedNow);

        // 二段ジャンプのリセット
        if (groundedNow)
        {
            canDoubleJump = true;
        }

        isGrounded = groundedNow;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // プレイヤーが落下中かを判定
            if (!hasBouncedThisFrame && RB2D.linearVelocity.y < 0f)
            {
                // 敵を消す
                //Destroy(collision.gameObject);

                // 反動で跳ね返る
                RB2D.linearVelocity = new Vector2(RB2D.linearVelocity.x, BoundJump);

                // 強ジャンプ受付を開始
                jumpBufferCounter = jumpBufferTime;

                // 空中ジャンプリセット
                canDoubleJump = true;

                // 敵を消す処理 ←ここを DestroyEnemy() に置き換える
                Enemy1 enemy = collision.GetComponent<Enemy1>();
                if (enemy != null)
                {
                    enemy.DestroyEnemy(); // ← スポナーに通知される
                }

                hasBouncedThisFrame = true; // このフレームではもうダメージを受けない
            }
            else if (!hasBouncedThisFrame)
            {
                Debug.Log("敵に当たりました");
                TakeDamage(30); // ライフを減らす
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

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        playerHP.TakeDamage(damage);

        StartCoroutine(InvincibleCoroutine());
    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        // レイヤー変更
        gameObject.layer = invincibleLayer;

        float timer = 0f;
        while (timer < invincibleTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        spriteRenderer.enabled = true;

        // レイヤー戻す
        gameObject.layer = defaultLayer;

        isInvincible = false;
    }
}
