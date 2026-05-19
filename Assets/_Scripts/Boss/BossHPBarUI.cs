using UnityEngine;
using UnityEngine.UI;

public class BossHPBarUI : MonoBehaviour
{
    [Header("HPバー")]
    [SerializeField] private Slider hpSlider;

    [Header("対象ボス")]
    [SerializeField] private BossHP bossHP;

    [Header("HPバー全体")]
    [SerializeField] private GameObject rootUI;

    void Start()
    {
        if (bossHP == null) return;

        // 最初は非表示
        rootUI.SetActive(false);

        // イベント登録
        bossHP.OnHPChanged += UpdateHPBar;
        bossHP.OnDead += HideUI;

        // 初期値
        UpdateHPBar(
            bossHP.GetCurrentHP(),
            bossHP.GetMaxHP()
        );
    }

    void UpdateHPBar(int current, int max)
    {
        hpSlider.value = (float)current / max;
    }

    // HPバー表示
    public void ShowUI()
    {
        rootUI.SetActive(true);
    }

    // HPバー非表示
    void HideUI()
    {
        rootUI.SetActive(false);
    }

    private void OnDestroy()
    {
        if (bossHP == null) return;

        bossHP.OnHPChanged -= UpdateHPBar;
        bossHP.OnDead -= HideUI;
    }
}