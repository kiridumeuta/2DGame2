using UnityEngine;

public class Item1Script : MonoBehaviour
{
    // アイテムID
    [SerializeField] private string itemID = "gun";
    // アイテム名
    [SerializeField] private string itemName = "銃";
    // 取得数
    [SerializeField] private int count = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // InventoryManagerに追加
        InventoryManager.Instance.AddItem(itemID, itemName, false, count);

        // アイテム削除
        Destroy(gameObject);
    }
}
