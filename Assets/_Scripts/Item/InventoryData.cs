using UnityEngine;
using System;
using System.Collections.Generic;

// JSON化するためのクラス
[Serializable]

public class InventoryData
{
    // インベントリ内のアイテムのリスト
    // 複数アイテムをまとめて管理するためのリスト
    public List<ItemData> items = new List<ItemData>();
}
