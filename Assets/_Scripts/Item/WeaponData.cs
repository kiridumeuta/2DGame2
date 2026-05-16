using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public string weaponID; // 武器の種類
    public string weaponName; // 武器の名前
    public GameObject gunObject; // 武器の見た目
    public GameObject bulletPrefab; // 生成する弾のプレハブ
    public float bulletSpeed = 10f; // 弾の速度
    public int bulletCount = 1; // 1回の発射で生成する弾の数
    public float spreadAngle = 0; // 弾の散らばり角度（単位は度）
}
