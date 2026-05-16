using UnityEngine;

public class ItemScript : MonoBehaviour
{
    // アイテムID
    [SerializeField] private string itemID = "gun";
    // アイテム名
    [SerializeField] private string itemName = "銃";
    // 取得数
    [SerializeField] private int count = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // InventoryManagerに追加
            InventoryManager.Instance.AddItem(itemID, itemName, false, count);

            // 武器一覧更新
            PlayerShooterScript playerShooter = collision.GetComponent<PlayerShooterScript>();

            // 見つかった場合
            if (playerShooter != null)
            {
                // 武器一覧を更新
                playerShooter.RefreshOwnedWeapons();
            }

            // アイテム削除
            Destroy(gameObject);
        }
    }
}
