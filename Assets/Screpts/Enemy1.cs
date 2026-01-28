using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    [SerializeField, Header("移動速度")]
    private float moveSpeed = 2f;
    [SerializeField, Header("カメラ外で消滅(X)")]
    private float DestroyEnemyWidth = 2f;
    [SerializeField, Header("カメラ外で消滅(Y)")]
    private float DestroyEnemyHight = 2f;

    Rigidbody2D rb;
    SpriteRenderer sr;

    int defaultLayer;
    int noPushLayer;

    // ← 追加：移動方向（-1 = 左、1 = 右）
    int moveDir = -1;

    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        defaultLayer = gameObject.layer;
        noPushLayer = LayerMask.NameToLayer("EnemyNoPush");

        mainCamera = Camera.main;// 追加
    }

    void Update()
    {
        CheckOutOfCamera();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
    }

    void CheckOutOfCamera()
    {
        if (mainCamera == null) return;

        // カメラのビューポート座標に変換（0~1の範囲）
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        // 画面外なら削除
        if (viewPos.x < -0.3f || viewPos.x > 1.3f || viewPos.y < -0.3f || viewPos.y > 1.3f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 壁に当たったら反転
        if (collision.gameObject.CompareTag("Wall") ||
            collision.gameObject.CompareTag("Enemy"))
        {
            Reverse();
        }
    }

    void Reverse()
    {
        moveDir *= -1;
        sr.flipX = moveDir > 0;

        // めり込み防止（おまじない）
        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.layer = noPushLayer;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.layer = defaultLayer;
        }
    }
}
