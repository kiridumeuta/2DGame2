using UnityEngine;
using System.IO; // ファイル保存/読み込みのための名前空間
using System.Linq;
using Unity.VisualScripting; // FirstOrDefaultを使用するための名前空間

public class InventoryManager : MonoBehaviour
{
    // staticにすることで、InventoryManager.Instanceを使用してどこからでもアクセス可能なインスタンスを作成
    public static InventoryManager Instance;

    // 現在のインベントリのデータを保持するための変数
    public InventoryData inventory = new InventoryData();

    // JSONファイルの保存先パスを保持する変数
    private string savePath;

    private void Awake()
    {
        // シングルトンがまだ存在しない場合は、このインスタンスをシングルトンとして設定し、
        // シーンが切り替わっても破棄されないようにする
        if (Instance == null)
        {
            // このインスタンスをシングルトンとして設定
            Instance = this;
            // シーンが切り替わってもこのゲームオブジェクトを破棄しないようにする
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // すでにシングルトンが存在する場合は、このインスタンスを破棄する
            Destroy(gameObject);
            return;
        }

        // persistentDataPathは、アプリケーションがデータを保存するための適切な場所を提供するUnityのプロパティ
        // ようするに、PCやスマホ等のプラットフォームに応じて適切な場所が自動的に選択される
        savePath = Path.Combine(Application.persistentDataPath, "inventory.json");

        // 起動時に保存データを読み込む
        LoadInventory();
    }

    // インベントリにアイテムを追加するメソッド→
    // id：識別ID
    // name：表示名
    // amount：増加数（デフォルトは1）
    public void AddItem(string id, string name, bool stackable, int amount = 1)
    {
        // すでに同じIDのアイテムがインベントリに存在するかどうかを確認
        ItemData item = inventory.items.FirstOrDefault(i => i.itemID == id);

        // もし持っていたら
        if (item != null)
        {
            // 既存のアイテムが複数所持可能であれば、数量を増やす
            if (stackable)
            {
                item.count += amount;
            }
            else
            {
                // スタック不可のアイテムは重複して持てないので、何もしない
                Debug.Log(name + "は既に所持しています。");
                return;
            }
        }
        else
        {
            // まだ持っていなかったら、新しいアイテムをインベントリに追加
            inventory.items.Add(new ItemData
            {
                itemID = id,
                itemName = name,
                count = amount,
                stackable = stackable
            });
        }

        // 追加後に保存
        SaveInventory();
    }

    // 保存
    public void SaveInventory()
    {
        // InventoryデータをJSON形式の文字列に変換して、指定したパスに保存する
        // trueを渡すことで、JSONを見やすい形式（インデント付き）で保存することができる
        string json = JsonUtility.ToJson(inventory, true);
        // ファイルに書き込む
        File.WriteAllText(savePath, json);
        // 保存場所確認
        Debug.Log("保存先：" + savePath);
    }

    // 読み込み
    public void LoadInventory()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);

                // JSONが空の場合は新しいインベントリを作成
                if (string.IsNullOrEmpty(json))
                {
                    inventory = new InventoryData();
                    return;
                }

                inventory = JsonUtility.FromJson<InventoryData>(json);

                // JSONの形式が正しくない場合や、InventoryDataに変換できない場合は新しいインベントリを作成
                if (inventory == null)
                {
                    inventory = new InventoryData();
                }
            }
            catch
            {
                Debug.LogError("JSON読み込み失敗：新規インベントリを作成");
                inventory = new InventoryData();
            }
        }

        // 保存ファイルが存在するかどうかを確認
        if (File.Exists(savePath))
        {
            // JSON文字列をファイルから読み込む
            string json = File.ReadAllText(savePath);
            // JSON文字列をInventoryDataオブジェクトに変換する
            inventory = JsonUtility.FromJson<InventoryData>(json);
        }
        else
        {
            // 保存ファイルが無ければ新規作成
            inventory = new InventoryData();
        }
    }

    // インベントリをリセットするメソッド
    public void ResetInventory()
    {
        inventory = new InventoryData();

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        Debug.Log("インベントリをリセットしました。");
    }

    // 所持アイテムをコンソールに表示するメソッド
    public void ShowInventory()
    {
        foreach (ItemData item in InventoryManager.Instance.inventory.items)
        {
            Debug.Log($"アイテムID: {item.itemID}, アイテム名: {item.itemName}, 数量: {item.count}");
        }
    }
}
