using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameClearManager : MonoBehaviour
{
    [Header("暗転用パネル")]
    [SerializeField] private CanvasGroup fadePanel;

    [Header("暗転時間")]
    [SerializeField] private float fadeDuration = 3f;

    [Header("ゲームクリアシーン名")]
    [SerializeField] private string gameClearSceneName = "GameClearScene";

    // プレイヤークリア時に呼ぶ
    public void TriggerGameClear()
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

        SceneManager.LoadScene(gameClearSceneName);
    }
}
