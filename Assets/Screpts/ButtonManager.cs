using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    /// <summary>
    /// 指定したシーンに遷移する
    /// </summary>
    /// <param name="sceneName">遷移したいシーン名</param>

    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is empty!");
        }
    }

    /// <summary>
    /// 現在のシーンをリロード（リスタート用）
    /// </summary>
    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    /// <summary>
    /// タイトルシーンに戻る
    /// </summary>
    /// <param name="titleSceneName">タイトルシーン名</param>
    public void GoToTitle(string titleSceneName)
    {
        LoadScene(titleSceneName);
    }

    public void GoToMain()
    {
        SceneManager.LoadScene("Main"); // Mainシーンに遷移
    }

    public void QuitGame()
    {
        // 実行ファイル版ではゲームを終了
        Application.Quit();

        // エディター上では終了しないのでデバッグ用にログ
        Debug.Log("Game Quit (Editorでは動作しません)");
    }
}
