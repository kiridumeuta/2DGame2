using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("ポーズメニューUI")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("GameOverManager参照")]
    [SerializeField] private GameOverManager gameOverManager;

    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerShooterScript playerShooter;

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

        // プレイヤー操作停止
        player.SetControl(false);

        // 銃操作停止
        playerShooter.SetControl(false);

        // UI表示
        pauseMenuUI.SetActive(true);

        // 時間停止
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        // プレイヤー操作再開
        player.SetControl(true);

        // 銃操作再開
        playerShooter.SetControl(true);

        // UI非表示
        pauseMenuUI.SetActive(false);

        // 時間再開
        Time.timeScale = 1f;
    }

    // タイトルへ戻る
    public void GoToTitle()
    {
        // 時間を元に戻す
        Time.timeScale = 1f;

        SceneManager.LoadScene("StartScene");
    }

    // ステージ選択へ
    public void GoToStageSelect()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("StageSelectScene");
    }

    // ゲーム終了
    public void QuitGame()
    {
        Time.timeScale = 1f;

        Application.Quit();

        Debug.Log("ゲーム終了");
    }
}
