using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("移動速度(右)")]
    private float MoveRight = 0.01f;
    [SerializeField, Header("移動速度(左)")]
    private float MoveLeft = -0.01f;

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

    [SerializeField, Header("ライフ管理")] private int maxHP = 3;     // 最大ライフ数
    private int currentHP;                      // 現在のライフ
    [SerializeField] private GameObject[] hearts;   // シーンに配置した 3つのハート

    [SerializeField, Header("無敵時間")]
    private float invincibleTime = 1.5f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    // 二段ジャンプ
    private bool canDoubleJump = false;

    [SerializeField, Header("少し前に地面にいた場合のジャンプ許可")] private float coyoteTime = 0.1f;
    private float coyoteCounter;


    Animator animator;
    Rigidbody2D RB2D;

    void Start()
    {
        animator = GetComponent<Animator>();
        RB2D = GetComponent<Rigidbody2D>();

        currentHP = maxHP;
        UpdateHearts();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckGround();
        PlayerMove();
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

    private void PlayerMove()
    {
        float move = 0f;

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(MoveRight, 0f, 0);
            move = Mathf.Abs(MoveRight);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(MoveLeft, 0f, 0);
            move = Mathf.Abs(MoveLeft);
        }
        else
        {
            move = 0f;
        }

        bool isWalking = (move > 0f) && isGrounded;
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

            //animator.SetBool("Jump", true);  // ← ジャンプ開始時に1回だけtrue
            animator.SetTrigger("DoubleJump"); // ← Trigger！

            canDoubleJump = false;  // もう二段ジャンプ不可
        }
    }

    private void CheckGround()
    {
        //// 足元に地面（Layer） があるか？
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        //// 地上に戻ったら二段ジャンプリセット
        //if (isGrounded)
        //{
        //    canDoubleJump = true;
        //    animator.SetBool("Jump", false); // ← 着地した瞬間だけ false
        //}

        bool groundedNow = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // ★ Animator へ地上/空中の状態を送る
        animator.SetBool("IsGround", groundedNow);

        // 二段ジャンプのリセット
        if (groundedNow)
        {
            canDoubleJump = true;
        }

        isGrounded = groundedNow;

    }

    // ライフを減らす処理
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;   // ←無敵中なら何もせず終了！

        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        UpdateHearts();

        // 無敵化開始
        StartCoroutine(InvincibleCoroutine());

        if (currentHP == 0)
        {
            Die();
        }
    }

    // ハート表示を更新
    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < currentHP);
        }
    }

    private void Die()
    {
        Debug.Log("プレイヤー死亡");
        // リスポーン処理やゲームオーバーなどを書く
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // プレイヤーの足が敵の上より上かを判定
            if (groundCheck.position.y > collision.transform.position.y)
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
                TakeDamage(1); // ライフを減らす
            }
        }
    }

    //元の処理
    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        // Enemyタグの相手に当たったか？
        if (collision.collider.CompareTag("Enemy"))
        {
            if (isInvincible) return;   // ←無敵なら処理しない

            // プレイヤーの足が敵の上側より上にあるかを判定
            if (groundCheck.position.y > collision.collider.transform.position.y)
            {
                // 敵オブジェクトを消す
                Destroy(collision.collider.gameObject);
                // 反動で跳ね返る
                RB2D.linearVelocity = new Vector2(RB2D.linearVelocity.x, BoundJump);

                // 強ジャンプ受付を開始
                jumpBufferCounter = jumpBufferTime;

                // 踏んだ場合は空中扱いなので二段ジャンプは1回にしておく
                //   こうすると踏んだ後にも空中ジャンプ（実質二段目）が可能
                canDoubleJump = true;
            }
            else
            {
                Debug.Log("敵に当たりました");
                TakeDamage(1);   // ライフを1つ減らす
            }
        }
    }*/

    //強ジャンプ処理
    private void StrongBoundJump()
    {
        RB2D.linearVelocity = new Vector2(RB2D.linearVelocity.x, SuperBoundJump);
    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        float timer = 0f;

        while (timer < invincibleTime)
        {
            // 透明・表示を切り替える（点滅）
            spriteRenderer.enabled = !spriteRenderer.enabled;

            // 点滅速度
            yield return new WaitForSeconds(0.1f);

            timer += 0.1f;
        }

        // 最後に表示を ON に戻す
        spriteRenderer.enabled = true;

        isInvincible = false;
    }
}
