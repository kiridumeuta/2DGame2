using System.Collections;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameOverManager gameOverManager;  // GameOverManagerをインスペクターから参照
    [SerializeField] private GameClearManager gameClearManager;  // GameClearManagerをインスペクターから参照

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
    private float jumpBufferCounter = 0f;   // 内部カウンタ

    [SerializeField, Header("地面レイヤー")]
    private LayerMask groundLayer;
    [SerializeField, Header("地面判定の位置")]
    private Transform groundCheck;
    [SerializeField, Header("判定半径")]
    private float groundCheckRadius = 0.2f;

    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    // 二段ジャンプ
    private bool canDoubleJump = false;

    private bool hasBouncedThisFrame = false;

    //落下検知用
    private bool isFall = false;

    float moveInput;

    private PlayerInput playerInput;
    private PlayerHP playerHP;
    private PlayerDamage playerDamage;
    private PlayerCollision playerCollision;
    private PlayerAnimationController animController;


    Rigidbody2D RB2D;

    [Header("SE")]
    [SerializeField] private AudioClip jumpSE;   // ジャンプした音
    [SerializeField] private AudioClip doublejumpSE;  // 二段ジャンプした音
    [SerializeField] private AudioClip stompSE;   // 敵を踏んだ音
    [SerializeField] private AudioClip damageSE;  // ダメージ音

    private AudioSource audioSource;

    public bool HasBouncedThisFrame => hasBouncedThisFrame;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerDamage = GetComponent<PlayerDamage>();
        playerCollision = GetComponent<PlayerCollision>();
        animController = GetComponent<PlayerAnimationController>();

        RB2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHP = GetComponent<PlayerHP>();
        audioSource = GetComponent<AudioSource>();

        // シーン内の GameOverManager を探す
        if (gameOverManager == null)
        {
            Debug.LogError("GameOverManagerが設定されていません");
        }
        // シーン内の GameClearManager を探す
        if (gameClearManager == null)
        {
            Debug.LogError("GameClearManagerが設定されていません");
        }
    }

    void Update()
    {
        // 毎フレームリセット
        hasBouncedThisFrame = false;

        moveInput = playerInput.MoveInput;

        CheckGround();
        PlayerJump();
        PlayerFall();

        // 踏んだ後の受付時間を減らす
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;

            // 強ジャンプ入力があったら
            if (playerInput.JumpPressed)
            {
                StrongBoundJump();
                jumpBufferCounter = 0; // 受付終了
            }
        }

        // Rキーでインベントリリセット
        if (playerInput.ResetInventoryPressed)
        {
            InventoryManager.Instance.ResetInventory();
        }

        // Iキーでインベントリ表示
        if (playerInput.ShowInventoryPressed)
        {
            InventoryManager.Instance.ShowInventory();
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
        animController.SetWalk(isWalking);
    }

    private void PlayerJump()
    {

        // 1段目ジャンプ（地上）
        if (playerInput.JumpPressed && isGrounded)
        {
            audioSource.PlayOneShot(jumpSE);

            RB2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

            animController.PlayJump(); // ← Trigger に変更

            canDoubleJump = true;  // 空中でもう1回OK
            return;
        }

        // 2段目ジャンプ（空中）
        if (playerInput.JumpPressed && !isGrounded && canDoubleJump)
        {
            // 一度速度をリセットすると綺麗な二段ジャンプになる
            RB2D.linearVelocity = new Vector2(RB2D.linearVelocity.x, 0f);

            audioSource.PlayOneShot(doublejumpSE);

            RB2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

            animController.PlayDoubleJump(); // ← Trigger

            canDoubleJump = false;  // もう二段ジャンプ不可
        }
    }

    private void PlayerFall()
    {
        // 落下中かどうかを Animator に送る
        if (!hasBouncedThisFrame && RB2D.linearVelocity.y < -0.1f && isFall)
        {
            animController.PlayFall();
            isFall = false;
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

        // Animator へ地上/空中の状態を送る
        animController.SetGround(groundedNow);

        // 二段ジャンプのリセット
        if (groundedNow)
        {
            canDoubleJump = true;
        }

        //落下検知リセット
        if (groundedNow)
        {
            isFall = true;
        }

        isGrounded = groundedNow;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerCollision.HandleTriggerEnter(collision);
    }

    public void SetBouncedThisFrame(bool value)
    {
        hasBouncedThisFrame = value;
    }

    public void ResetDoubleJump()
    {
        canDoubleJump = true;
    }

    public void StartJumpBuffer(float time)
    {
        jumpBufferCounter = time;
    }

    //強ジャンプ処理
    private void StrongBoundJump()
    {
        RB2D.linearVelocity = new Vector2(RB2D.linearVelocity.x, SuperBoundJump);
    }
}
