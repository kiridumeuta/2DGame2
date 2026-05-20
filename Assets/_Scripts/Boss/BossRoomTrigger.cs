using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    [SerializeField] private Transform cameraPoint;

    [SerializeField] private Vector2 bossRoomMin;

    [SerializeField] private Vector2 bossRoomMax;

    [SerializeField] private BossHPBarUI bossUI;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        if (collision.CompareTag("Player"))
        {
            activated = true;

            CameraManager cam = Camera.main.GetComponent<CameraManager>();

            if (cam != null)
            {
                cam.LockCamera(cameraPoint.position);

                // ボス部屋用カメラ範囲
                cam.SetCameraArea(
                    bossRoomMin,
                    bossRoomMax
                );
                /*
                // HPバー表示
                BossHPBarUI bossUI = FindAnyObjectByType<BossHPBarUI>();

                if (bossUI != null)
                {
                    bossUI.ShowUI();
                }*/
            }

            // HPバー表示
            if (bossUI != null)
            {
                bossUI.ShowUI();
            }
        }
    }
}