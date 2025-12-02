using Unity.VisualScripting;
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

    [SerializeField, Header("地面レイヤー")]
    private LayerMask groundLayer;
    [SerializeField, Header("地面判定の位置")]
    private Transform groundCheck;
    [SerializeField, Header("判定半径")]
    private float groundCheckRadius = 0.2f;

    Rigidbody2D RB2D;

    private bool isGrounded;

    void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckGround();
        PlayerMove();
        PlayerJump();
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            RB2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }
    }

    private void CheckGround()
    {
        // 足元に地面（Layer） があるか？
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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
            }
            else
            {
                Debug.Log("敵に当たりました");
                // 横などからぶつかった時の処理
            }
        }
    }
}
