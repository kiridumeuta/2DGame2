using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f; // 弾の寿命

    [SerializeField] private GameObject explosionPrefab; // 爆発エフェクトのプレハブ

    void Start()
    {
        Destroy(gameObject, lifeTime); // 一定時間後に弾を破壊
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Wall"))
        {
            Destroy(gameObject); // 壁か床に衝突したら弾を破壊
        }
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject); // 衝突したら弾を破壊

            if(explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity); // 爆発エフェクトを生成
            }

            Enemy1 enemy = collision.GetComponent<Enemy1>();
            EnemyJump enemyjump = collision.GetComponent<EnemyJump>();
            if (enemy != null)
            {
                enemy.DestroyEnemy(); // スポナーに通知される
            }
            if (enemyjump != null)
            {
                enemyjump.DestroyEnemy(); // スポナーに通知される
            }
        }
    }
}
