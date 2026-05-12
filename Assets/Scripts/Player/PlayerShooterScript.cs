using UnityEngine;

public class PlayerShooterScript : MonoBehaviour
{
    [Header("武器一覧")]
    [SerializeField] private WeaponData[] weapons;

    [Header("現在装備ID")]
    [SerializeField] private string currentWeaponID = "gun";

    [Header("発射位置")]
    [SerializeField] private Transform firePoint;

    private WeaponData currentWeapon;

    private void Start()
    {
        // currentWaponIDに基づいて武器を装備する
        EquipWeapon(currentWeaponID);
    }

    void Update()
    {
        // currentWeaponがnullでなく、かつInventoryManagerにcurrentWeaponのweaponIDが存在する場合
        if (currentWeapon !=null && InventoryManager.Instance.HasItem(currentWeapon.weaponID))
        {
            // 武器を表示する
            currentWeapon.gunObject.SetActive(true);

            // Fキーが押されたらShoot()を呼び出す
            if (Input.GetKeyDown(KeyCode.F))
            {
                Shoot();
            }
        }
        // 武器未所持の場合は
        else if ((currentWeapon != null))
        {
            // 武器を非表示にする
            currentWeapon.gunObject.SetActive(false);
        }
    }

    // 武器を装備するメソッド
    public void EquipWeapon(string weaponID)
    {
        // すべての武器を非表示にする
        foreach (var weapon in weapons)
        {
            if(weapon.gunObject != null)
            {
                weapon.gunObject.SetActive(false);
            }
        }

        // 指定のweaponIDに一致する武器を検索
        foreach (var weapon in weapons)
        {
            // weaponIDが一致する場合
            if (weapon.weaponID == weaponID)
            {
                // 現在の武器を更新
                currentWeapon = weapon;
                // 現在の武器IDを更新
                currentWeaponID = weaponID;
                // 武器を表示する
                break;
            }
        }
    }

    // 弾を発射するメソッド
    private void Shoot()
    {
        // bulletCountの分だけループして弾を生成する
        for (int i =0; i<currentWeapon.bulletCount; i++)
        {
            // 弾の角度の初期値を0に設定
            float angleOffset = 0f;

            // 複数の弾を発射する場合は
            if(currentWeapon.bulletCount > 1)
            {
                // 中央を基準にして、spreadAngleに基づいて角度を計算する
                angleOffset = ((i - (currentWeapon.bulletCount - 1) / 2f) * currentWeapon.spreadAngle);
            }

            // firePointの回転にangleOffsetを加算して、弾の回転を計算する
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, angleOffset);

            // 弾Prefabを生成する
            GameObject bullet = Instantiate(currentWeapon.bulletPrefab, firePoint.position, rotation);

            // 生成した弾のRigidbody2Dコンポーネントを取得する
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            // 弾の速度を設定する（firePointの右方向にcurrentWeapon.bulletSpeedの速度で発射する）
            rb.linearVelocity = bullet.transform.right * currentWeapon.bulletSpeed;
        }
    }
}
