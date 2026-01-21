using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [Header("ライフ管理")]
    [SerializeField] private int maxHP = 100;
    private int currentHP;

    [Header("HPバー（Slider）")]
    [SerializeField] private Slider hpSlider;

    [Header("無敵時間")]
    [SerializeField] private float invincibleTime = 1.5f;
    private bool isInvincible = false;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
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
    {
        if (isInvincible) return;

        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        if (hpSlider != null)
        {
            hpSlider.value = currentHP;
        }

        StartCoroutine(InvincibleCoroutine());

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("プレイヤー死亡");
        // ゲームオーバー処理など
    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        float timer = 0f;
        while (timer < invincibleTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
    }
}
