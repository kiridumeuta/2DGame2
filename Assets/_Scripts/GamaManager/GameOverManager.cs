using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // CanvasGroupを使ってUIパネルの透明度(alpha)を操作する
    [Header("暗転用パネル")]
    [SerializeField] private CanvasGroup fadePanel;

    // 暗転が完了するまでにかかる時間（秒）
    [Header("暗転時間")]
    [SerializeField] private float fadeDuration = 3f;

    // 暗転の最大濃さ（0 = 完全透明、1 = 完全に黒）
    [Header("暗転の最大濃さ")]
    [SerializeField] private float maxFadeAlpha = 0.5f;

    // 暗転後に読み込むゲームオーバー画面のシーン名
    [Header("ゲームオーバーシーン名")]
    [SerializeField] private string gameOverSceneName = "GameOverScene";

    // ゲームオーバー中かどうか
    public bool IsGameOver { get; private set; } = false;

    // プレイヤー死亡時に呼ぶ
    public void TriggerGameOver()
    {
        if (IsGameOver)
        {
            // すでにゲームオーバー処理が始まっている場合は何もしない
            return;
        }

        IsGameOver = true;

        // コルーチン開始
        // 徐々に暗転
        StartCoroutine(FadeOutAndLoadScene());
    }

    // 暗転してからゲームオーバーシーンへ移動するコルーチン
    private IEnumerator FadeOutAndLoadScene()
    {
        // 経過時間を記録する変数
        float timer = 0f;

        // fadeDuration秒になるまで繰り返す
        while (timer < fadeDuration)
        {
            // fadePanelのalphaを0からmaxFadeAlphaまで徐々に変化させる
            fadePanel.alpha = Mathf.Lerp(0f, maxFadeAlpha, timer / fadeDuration);

            // 毎フレーム経過時間を加算
            timer += Time.deltaTime;

            // 次のフレームまで待機
            yield return null;
        }

        // 最終的にalphaをmaxFadeAlphaに設定して完全に暗転させる
        fadePanel.alpha = maxFadeAlpha;

        // ゲームオーバーシーンを重ねる
        SceneManager.LoadScene(gameOverSceneName, LoadSceneMode.Additive);
    }
}
