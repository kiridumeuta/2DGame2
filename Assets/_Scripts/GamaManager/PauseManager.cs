using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("ポーズメニューUI")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("GameOverManager参照")]
    [SerializeField] private GameOverManager gameOverManager;

    private bool isPaused = false;

    void Start()
    {
        // 開始時は非表示
        pauseMenuUI.SetActive(false);

        // 念のため通常速度
        Time.timeScale = 1f;
    }

    void Update()
    {
        // ゲームオーバー中ならポーズ禁止
        if (gameOverManager != null && gameOverManager.IsGameOver)
        {
            return;
        }

        // Pキーで切り替え
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        // UI表示
        pauseMenuUI.SetActive(true);

        // 時間停止
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        // UI非表示
        pauseMenuUI.SetActive(false);

        // 時間再開
        Time.timeScale = 1f;
    }
}
