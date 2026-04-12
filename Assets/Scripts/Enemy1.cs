using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    [SerializeField, Header("移動速度")]
    private float moveSpeed = 2f;
    [SerializeField, Header("カメラ外で消滅(X)")]
    private float DestroyEnemyWidth = -0.3f;
    [SerializeField, Header("カメラ外で消滅(-X)")]
    private float DestroyEnemyWidth2 = 1.3f;
    [SerializeField, Header("カメラ外で消滅(Y)")]
    private float DestroyEnemyHight = -0.3f;
    [SerializeField, Header("カメラ外で消滅(-Y)")]
    private float DestroyEnemyHight2 = 1.3f;

    Rigidbody2D rb;
    SpriteRenderer sr;

    int defaultLayer;
    int noPushLayer;

    // ← 追加：移動方向（-1 = 左、1 = 右）
    int moveDir = -1;

    private bool isActive = true; // 表示・動作中かどうか

    private Camera mainCamera;

    // 敵が破壊されたときに通知
    public delegate void EnemyDestroyed(Enemy1 enemy);
    public event System.Action<Enemy1> OnDestroyed;


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

        if (!isActive) return; // 非アクティブなら動作停止
    }

    void FixedUpdate()
    {
        //rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
        if (!isActive) return;

        Vector2 newPos = rb.position + Vector2.right * moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    void CheckOutOfCamera()
    {
        if (mainCamera == null) return;

        // カメラのビューポート座標に変換（0~1の範囲）
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        // ビューポート外なら削除
        if (viewPos.x < DestroyEnemyWidth || viewPos.x > DestroyEnemyWidth2 ||
            viewPos.y < DestroyEnemyHight || viewPos.y > DestroyEnemyHight2)
        {
            DestroyEnemy();
        }
    }

    void SetActiveState(bool state)
    {
        // レンダラーの表示・非表示
        sr.enabled = state;

        // Rigidbody2Dの動きを止める
        rb.simulated = state;

        // 必要ならコライダーも無効化
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = state;
    }

    public void DestroyEnemy()
    {
        OnDestroyed?.Invoke(this); // スポナーに通知
        Destroy(gameObject);       // 自分を破壊
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy"))
        {
            // 衝突点の高さをチェック
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.y < 0.1f) // 横からの衝突なら反転
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

        // めり込み防止（おまじない）
        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
    }
}
