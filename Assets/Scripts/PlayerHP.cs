using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [Header("ライフ管理")]
    [SerializeField] private int maxHP = 100;
    private int currentHP;

    [Header("HPバー（Slider）")]
    [SerializeField] private Slider hpSlider;
    /*
    [Header("無敵時間")]
    [SerializeField] private float invincibleTime = 1.5f;
    private bool isInvincible = false;*/

    private SpriteRenderer spriteRenderer;

    [Header("ゲームオーバーマネージャー")]
    [SerializeField] private GameOverManager gameOverManager; // Inspectorでセット

    /*int defaultLayer;
    int invincibleLayer;*/

    void Start()
    {/*
        defaultLayer = gameObject.layer;
        invincibleLayer = LayerMask.NameToLayer("PlayerInvincible");*/

        currentHP = maxHP;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }
    }

    // 外部から呼ばれるダメージ処理
    public void TakeDamage(int damage)
    {/*
        if (isInvincible) return;*/

        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        if (hpSlider != null)
        {
            hpSlider.value = currentHP;
        }
        /*
        StartCoroutine(InvincibleCoroutine());*/

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("プレイヤー死亡");
        // ゲームオーバー処理など
        // 1. プレイヤーを非表示にする
        gameObject.SetActive(false);

        // 2. 操作を無効化する場合は PlayerController を無効化
        GetComponent<PlayerController>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        // GameOverManager に通知
        if (gameOverManager != null)
        {
            gameOverManager.TriggerGameOver();
        }
    }

    private IEnumerator FadeOutAndGameOver()
    {
        // FadePanel を取得
        GameObject fadePanel = GameObject.Find("FadePanel");
        CanvasGroup cg = fadePanel.GetComponent<CanvasGroup>();

        float duration = 3f; // 3秒で暗転
        float timer = 0f;

        while (timer < duration)
        {
            cg.alpha = Mathf.Lerp(0, 1, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 1f;

        // ゲームオーバーシーンへ遷移
        SceneManager.LoadScene("GameOverScene"); // ← シーン名に合わせて変更
    }
    /*
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
    }*/
}
