using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("暗転用パネル")]
    [SerializeField] private CanvasGroup fadePanel;

    [Header("暗転時間")]
    [SerializeField] private float fadeDuration = 3f;

    [Header("ゲームオーバーシーン名")]
    [SerializeField] private string gameOverSceneName = "GameOverScene";

    // プレイヤー死亡時に呼ぶ
    public void TriggerGameOver()
    {
        StartCoroutine(FadeOutAndLoadScene());
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            fadePanel.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        fadePanel.alpha = 1f;

        SceneManager.LoadScene(gameOverSceneName);
    }
}
