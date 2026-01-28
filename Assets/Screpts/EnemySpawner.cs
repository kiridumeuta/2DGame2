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

    void Start()
    {

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

        Vector3 spawnPos = transform.position;
        // X座標をランダムにずらす場合
        spawnPos.x += Random.Range(-spawnRangeX, spawnRangeX);
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // 敵が消えたときにカウントを減らす
        Enemy1 enemy = enemyObj.GetComponent<Enemy1>();
        if (enemy != null)
        {
            // インスタンスのイベントに登録
            enemy.OnDestroyed += HandleEnemyDestroyed;
        }

        currentEnemies++;
    }

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
