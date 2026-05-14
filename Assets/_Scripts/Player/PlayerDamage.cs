using System.Collections;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [Header("–³“GŽžŠÔ")]
    [SerializeField] private float invincibleTime = 1.5f;

    private bool isInvincible = false;

    private int defaultLayer;
    private int invincibleLayer;

    private SpriteRenderer spriteRenderer;
    private PlayerHP playerHP;
    private AudioSource audioSource;

    [Header("SE")]
    [SerializeField] private AudioClip damageSE;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHP = GetComponent<PlayerHP>();
        audioSource = GetComponent<AudioSource>();

        defaultLayer = gameObject.layer;
        invincibleLayer = LayerMask.NameToLayer("PlayerInvincible");
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        audioSource.PlayOneShot(damageSE);

        playerHP.TakeDamage(damage);

        if (playerHP.currentHP > 0)
        {
            StartCoroutine(InvincibleCoroutine());
        }
    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        gameObject.layer = invincibleLayer;

        float timer = 0f;
        while (timer < invincibleTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        spriteRenderer.enabled = true;

        gameObject.layer = defaultLayer;

        isInvincible = false;
    }
}
