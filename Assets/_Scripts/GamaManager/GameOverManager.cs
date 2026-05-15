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
            // fadePanel.alpha を 0 → 1 に徐々に変化
            // 0 = 完全透明、1 = 完全に黒
            fadePanel.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);

            // 毎フレーム経過時間を加算
            timer += Time.deltaTime;

            // 次のフレームまで待機
            yield return null;
        }

        // 念のため最後に完全に暗転状態にする
        fadePanel.alpha = 1f;

        // 指定したゲームオーバーシーンを読み込む
        SceneManager.LoadScene(gameOverSceneName);
    }
}
