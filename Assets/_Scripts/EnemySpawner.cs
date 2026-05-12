using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField, Header("湧かせる敵")]
    private GameObject enemyPrefab;
    [SerializeField, Header("スポーン間隔(秒)")]
    private float spawnInterval = 5f;
    [SerializeField, Header("スポーン範囲(X方向ランダム)")]
    private float spawnRangeX = 1f;
    [SerializeField, Header("画面上の存在数上限")]
    private int maxEnemies = 10;

    private float timer;
    private int currentEnemies = 0;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // 最大数チェック
        if (currentEnemies >= maxEnemies) return;

        // ★ スポナーがカメラ内なら湧かせない
        if (IsSpawnerInCamera()) return;

        GameObject enemyObj =
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        /*Vector3 spawnPos = transform.position;
        // X座標をランダムにずらす場合
        spawnPos.x += Random.Range(-spawnRangeX, spawnRangeX);
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);*/

        // 敵が消えたときにカウントを減らす
        Enemy1 enemy = enemyObj.GetComponent<Enemy1>();
        if (enemy != null)
        {
            // インスタンスのイベントに登録
            enemy.OnDestroyed += HandleEnemyDestroyed;
        }

        currentEnemies++;
    }

    bool IsSpawnerInCamera()
    {
        if (mainCamera == null) return false;

        Vector3 viewPos =
            mainCamera.WorldToViewportPoint(transform.position);

        // カメラ内判定（0～1の範囲）
        return viewPos.x >= 0f && viewPos.x <= 1f &&
               viewPos.y >= 0f && viewPos.y <= 1f &&
               viewPos.z > 0f; // カメラの前にあるか
    }

    /*Vector3 GetSpawnPosition()

    {
        if (mainCamera == null) return Vector3.zero;

        // X方向にカメラ内外を確認
        float spawnPosX = Random.Range(-spawnRangeX, spawnRangeX);

        // ビューポート座標に変換
        Vector3 viewPos = mainCamera.WorldToViewportPoint(new Vector3(spawnPosX, 0f, 0f));

        // 画面内なら湧かせない
        if (viewPos.x > 0f && viewPos.x < 1f) return Vector3.zero;

        return new Vector3(spawnPosX, transform.position.y, transform.position.z);
    }*/

    private void OnEnemyDestroyed(Enemy1 enemy)
    {
        currentEnemies--;
    }

    private void OnDrawGizmos()
    {
        // Scene上でスポーン範囲を可視化
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.left * spawnRangeX, transform.position + Vector3.right * spawnRangeX);
    }

    private void HandleEnemyDestroyed(Enemy1 enemy)
    {
        currentEnemies--;
    }
}
