using UnityEngine;

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

    Rigidbody2D RB2D;

    private bool isGrounded;
    // 二段ジャンプ
    private bool canDoubleJump = false;

    void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
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
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(MoveRight, 0f, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(MoveLeft, 0f, 0);
        }
    }
    private void PlayerJump()
    {
        // 1段目ジャンプ（地上）
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            RB2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            canDoubleJump = true;  // 空中でもう1回OK
            return;
        }

        // 2段目ジャンプ（空中）
        if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && canDoubleJump)
        {
            // 一度速度をリセットすると綺麗な二段ジャンプになる
            RB2D.linearVelocity = new Vector2(RB2D.linearVelocity.x, 0f);
            RB2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

            canDoubleJump = false;  // もう二段ジャンプ不可
        }
    }

    private void CheckGround()
    {
        // 足元に地面（Layer） があるか？
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 地上に戻ったら二段ジャンプリセット
        if (isGrounded)
        {
            canDoubleJump = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Enemyタグの相手に当たったか？
        if (collision.collider.CompareTag("Enemy"))
        {
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
                // 横などからぶつかった時の処理
            }
        }
    }

    //強ジャンプ処理
    private void StrongBoundJump()
    {
        RB2D.linearVelocity = new Vector2(RB2D.linearVelocity.x, SuperBoundJump);
    }
}
