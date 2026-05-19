using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    [SerializeField] private Transform cameraPoint;

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

                // HPÉoÅ[ï\é¶
                BossHPBarUI bossUI = FindAnyObjectByType<BossHPBarUI>();

                if (bossUI != null)
                {
                    bossUI.ShowUI();
                }
            }
        }
    }
}