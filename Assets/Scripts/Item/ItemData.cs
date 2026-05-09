using UnityEngine;
using System;

// UnityのJsonUtilityを使用してアイテムデータをシリアライズ可能にするためのクラス
[Serializable]

public class ItemData
{
    public string itemID; // アイテムのID、例えば "potion" や "sword" などのプログラム内の識別用
    public string itemName; // アイテムの名前、例えば "ポーション" や "剣" などのUI表示用
    public int count; // アイテムの個数

    // true = 同じアイテムを複数持てる
    // false = 同じアイテムは1つしか持てない
    public bool stackable;
}
