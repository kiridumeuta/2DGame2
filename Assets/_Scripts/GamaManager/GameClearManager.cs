using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameClearManager : MonoBehaviour
{
    [Header("暗転用パネル")]
    [SerializeField] private CanvasGroup fadePanel;

    [Header("暗転時間")]
    [SerializeField] private float fadeDuration = 3f;

    [Header("暗転の最大濃さ")]
    [SerializeField] private float maxFadeAlpha = 0.5f;

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
            fadePanel.alpha = Mathf.Lerp(0f, maxFadeAlpha, timer / fadeDuration);

            timer += Time.deltaTime;
            yield return null;
        }

        fadePanel.alpha = maxFadeAlpha;

        // クリアシーンを重ねる
        SceneManager.LoadScene(gameClearSceneName, LoadSceneMode.Additive);
    }
}
