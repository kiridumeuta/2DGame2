using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private GameOverManager gameOverManager;
    [SerializeField] private GameClearManager gameClearManager;

    [Header("踏んだ後のジャンプ力")]
    [SerializeField] private float boundJump = 8f;

    [Header("強ジャンプ受付時間")]
    [SerializeField] private float jumpBufferTime = 0.2f;

    private Rigidbody2D rb2D;
    private AudioSource audioSource;
    private PlayerDamage playerDamage;
    private PlayerController playerController;

    [Header("SE")]
    [SerializeField] private AudioClip stompSE;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerDamage = GetComponent<PlayerDamage>();
        playerController = GetComponent<PlayerController>();

        if (gameOverManager == null)
        {
            Debug.LogError("GameOverManagerが設定されていません");
        }

        if (gameClearManager == null)
        {
            Debug.LogError("GameClearManagerが設定されていません");
        }
    }

    public void HandleTriggerEnter(Collider2D collision)
    {
        // 敵
        if (collision.CompareTag("Enemy"))
        {
            HandleEnemyCollision(collision);
        }

        // 奈落
        if (collision.CompareTag("Fall"))
        {
            HandleFallCollision();
        }

        // ゴール
        if (collision.CompareTag("Goal"))
        {
            HandleGoalCollision();
        }
    }

    private void HandleEnemyCollision(Collider2D collision)
    {
        // 落下中なら踏み
        if (!playerController.HasBouncedThisFrame && rb2D.linearVelocity.y < -0.1f)
        {
            audioSource.PlayOneShot(stompSE);

            // 跳ねる
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, boundJump);

            // PlayerController側へ通知
            playerController.StartJumpBuffer(jumpBufferTime);
            playerController.ResetDoubleJump();
            playerController.SetBouncedThisFrame(true);

            // 敵削除
            Enemy1 enemy = collision.GetComponent<Enemy1>();
            EnemyJump enemyJump = collision.GetComponent<EnemyJump>();

            if (enemy != null)
            {
                enemy.DestroyEnemy();
            }

            if (enemyJump != null)
            {
                enemyJump.DestroyEnemy();
            }
        }
        else if (!playerController.HasBouncedThisFrame)
        {
            Debug.Log("敵に当たりました");
            playerDamage.TakeDamage(30);
        }
    }

    private void HandleFallCollision()
    {
        if (gameOverManager != null)
        {
            gameOverManager.TriggerGameOver();

            rb2D.linearVelocity = Vector2.zero;
            rb2D.simulated = false;
        }
    }

    private void HandleGoalCollision()
    {
        if (gameClearManager != null)
        {
            gameClearManager.TriggerGameClear();

            rb2D.linearVelocity = Vector2.zero;
            rb2D.simulated = false;
        }
    }
}