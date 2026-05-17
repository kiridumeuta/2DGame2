using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerShooterScript : MonoBehaviour
{
    [Header("武器一覧")]
    [SerializeField] private WeaponData[] allWeapons;

    [Header("発射位置")]
    [SerializeField] private Transform firePoint;

    private AudioSource audioSource;

    [Header("SE")]
    [SerializeField] private AudioClip shotSE;

    //[Header("現在武器表示UI")]
    //[SerializeField] private Text currentWeaponText;

    // 現在所持している武器一覧
    private List<WeaponData> ownedWeapons = new List<WeaponData>();

    // 現在装備している武器のID
    private int currentWeaponIndex = 0;

    // 現在装備している武器のデータ
    private WeaponData currentWeapon;

    // プレイヤーが右向きかどうか
    private bool isFacingRight = true;

    // プレイヤーSpriteRenderer
    private SpriteRenderer playerSpriteRenderer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // プレイヤー自身のSpriteRenderer取得
        playerSpriteRenderer = GetComponent<SpriteRenderer>();

        RefreshOwnedWeapons();

        // 初期装備
        if (ownedWeapons.Count > 0)
        {
            EquipWeapon(currentWeaponIndex);
        }
    }

    void Update()
    {
        // プレイヤーSpriteの向き確認
        if (playerSpriteRenderer != null)
        {
            // flipX=false → 右向き
            isFacingRight = !playerSpriteRenderer.flipX;
        }

        // 銃の向き更新
        UpdateWeaponDirection();

        // ←キーで前の武器
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousWeapon();
        }

        // →キーで次の武器
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextWeapon();
        }

        // 素手じゃない && Fキーで発射
        if (currentWeapon != null &&
            currentWeapon.weaponID != "none" &&
            Input.GetKeyDown(KeyCode.F))
        {
            Shoot();
        }
    }

    public void RefreshOwnedWeapons()
    {
        // 一旦空
        ownedWeapons.Clear();

        // 全武器チェック
        foreach (var weapon in allWeapons)
        {
            // 素手は常に所持
            if (weapon.weaponID == "none")
            {
                ownedWeapons.Add(weapon);
                Debug.Log("追加: " + weapon.weaponID);
            }
            // Inventoryにある武器だけ追加
            else if (InventoryManager.Instance.HasItem(weapon.weaponID))
            {
                ownedWeapons.Add(weapon);
                Debug.Log("追加: " + weapon.weaponID);
            }
            else
            {
                Debug.Log("未所持: " + weapon.weaponID);
            }
        }

        Debug.Log("総武器数: " + ownedWeapons.Count);

        // 現在Index調整
        if (currentWeaponIndex >= ownedWeapons.Count)
        {
            currentWeaponIndex = 0;
        }
    }

    private void NextWeapon()
    {
        if (ownedWeapons.Count == 0) return;

        currentWeaponIndex++;

        // 最後なら最初へ
        if (currentWeaponIndex >= ownedWeapons.Count)
        {
            currentWeaponIndex = 0;
        }

        EquipWeapon(currentWeaponIndex);
    }

    private void PreviousWeapon()
    {
        if (ownedWeapons.Count == 0) return;

        currentWeaponIndex--;

        // 最初より前なら最後へ
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = ownedWeapons.Count - 1;
        }

        EquipWeapon(currentWeaponIndex);
    }

    private void EquipWeapon(int index)
    {
        // 全武器非表示
        foreach (var weapon in allWeapons)
        {
            if (weapon.gunObject != null)
            {
                weapon.gunObject.SetActive(false);
            }
        }

        // 装備設定
        currentWeapon = ownedWeapons[index];

        // 武器表示
        if (currentWeapon.gunObject != null)
        {
            currentWeapon.gunObject.SetActive(true);
        }

        // UI更新
        //UpdateWeaponUI();

        // デバッグ表示
        Debug.Log("現在武器: " + currentWeapon.weaponName);
    }

    private void Shoot()
    {
        // 弾Prefabが無いなら終了
        if (currentWeapon.bulletPrefab == null) return;

        audioSource.PlayOneShot(shotSE);

        // 弾数分生成
        for (int i = 0; i < currentWeapon.bulletCount; i++)
        {
            float angleOffset = 0f;

            // 複数弾なら角度分散
            if (currentWeapon.bulletCount > 1)
            {
                angleOffset =
                    ((i - (currentWeapon.bulletCount - 1) / 2f)
                    * currentWeapon.spreadAngle);
            }

            // 回転計算
            Quaternion rotation =
                firePoint.rotation *
                Quaternion.Euler(0, 0, angleOffset);

            // 弾生成
            GameObject bullet =
                Instantiate(
                    currentWeapon.bulletPrefab,
                    firePoint.position,
                    rotation
                );

            // Rigidbody取得
            Rigidbody2D rb =
                bullet.GetComponent<Rigidbody2D>();

            // ベース方向（右 / 左）
            Vector2 baseDirection;

            if (isFacingRight)
            {
                baseDirection = Vector2.right;
            }
            else
            {
                baseDirection = Vector2.left;
            }

            // 拡散角度を方向に反映
            float finalAngle;

            // 右向きならそのまま
            if (isFacingRight)
            {
                finalAngle = angleOffset;
            }
            // 左向きなら180度反転
            else
            {
                finalAngle = 180f - angleOffset;
            }

            // 角度から方向ベクトル作成
            Vector2 shootDirection =
                Quaternion.Euler(0, 0, finalAngle) *
                Vector2.right;

            // 発射
            rb.linearVelocity =
                shootDirection *
                currentWeapon.bulletSpeed;
        }
    }

    // 武器の向きをプレイヤーに合わせる
    private void UpdateWeaponDirection()
    {
        // 武器が無いなら終了
        if (currentWeapon == null || currentWeapon.gunObject == null)
            return;

        // gunObject内の全SpriteRenderer取得
        SpriteRenderer[] gunSprites = currentWeapon.gunObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in gunSprites)
        {
            // プレイヤー向きに同期
            sprite.flipX = !isFacingRight;
        }

        // 銃本体の位置調整
        // プレイヤー右側 / 左側に持ち替える
        Transform gunTransform = currentWeapon.gunObject.transform;

        // 現在位置取得
        Vector3 gunPos = gunTransform.localPosition;

        if (isFacingRight)
        {
            // 右向きならXをプラス
            gunPos.x = Mathf.Abs(gunPos.x);
        }
        else
        {
            // 左向きならXをマイナス
            gunPos.x = -Mathf.Abs(gunPos.x);
        }

        // 位置反映
        gunTransform.localPosition = gunPos;

        // FirePoint位置調整
        // 銃口位置を左右で反転
        if (firePoint != null)
        {
            Vector3 firePos = firePoint.localPosition;

            if (isFacingRight)
            {
                // 右向き
                firePos.x = Mathf.Abs(firePos.x);
            }
            else
            {
                // 左向き
                firePos.x = -Mathf.Abs(firePos.x);
            }

            firePoint.localPosition = firePos;
        }
    }

    // 素手（Index0想定）へ強制装備
    public void ForceEquipDefaultWeapon()
    {
        // 武器があるなら最初（none）へ
        if (ownedWeapons.Count > 0)
        {
            currentWeaponIndex = 0;
            EquipWeapon(currentWeaponIndex);
        }
    }

    /*private void UpdateWeaponUI()
    {
        if (currentWeaponText != null && currentWeapon != null)
        {
            currentWeaponText.text = "武器： " + currentWeapon.weaponName;
        }
    }*/

    public WeaponData GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
