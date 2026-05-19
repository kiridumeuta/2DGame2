using UnityEngine;
using System;

public class BossHP : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int maxHP = 100;

    private int currentHP;

    // HP変更通知
    public event Action<int, int> OnHPChanged;

    // 死亡通知
    public event Action OnDead;

    void Start()
    {
        currentHP = maxHP;

        // 初期表示更新
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP < 0)
        {
            currentHP = 0;
        }

        // UI更新通知
        OnHPChanged?.Invoke(currentHP, maxHP);

        // 死亡
        if (currentHP <= 0)
        {
            Dead();
        }
    }

    void Dead()
    {
        CameraManager cam =
        Camera.main.GetComponent<CameraManager>();

        if (cam != null)
        {
            cam.UnlockCamera();
        }

        OnDead?.Invoke();

        Destroy(gameObject);
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }
}