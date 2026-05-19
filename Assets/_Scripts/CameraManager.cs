using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField, Header("追従対象")]
    private Transform target;
    [SerializeField, Header("追従の滑らかさ")]
    private float smoothSpeed = 0.1f;

    [SerializeField, Header("相対位置")]
    private Vector3 offset;

    [SerializeField, Header("デッドゾーンの大きさ（幅・高さ）")]
    private Vector2 deadZoneSize = new Vector2(5f, 3f);

    [SerializeField, Header("カメラ移動範囲（最小）")]
    private Vector2 minCamPos;

    [SerializeField, Header("カメラ移動範囲（最大）")]
    private Vector2 maxCamPos;

    [Header("カメラ固定")]
    [SerializeField] private bool isLocked = false;

    private Vector3 lockedPosition;

    void LateUpdate()
    {
        // =========================
        // カメラ固定中
        // =========================
        if (isLocked)
        {
            Vector3 camPos = transform.position;

            Vector3 targetPos = new Vector3(
                lockedPosition.x,
                lockedPosition.y,
                camPos.z
            );

            transform.position = Vector3.Lerp(
                camPos,
                targetPos,
                smoothSpeed
            );

            return;
        }

        if (target == null) return;

        // 現在のカメラ位置（Zは維持）
        Vector3 camPos2 = transform.position;
        Vector3 targetPos2 = target.position + offset;

        // デッドゾーン（境界）
        float left = camPos2.x - deadZoneSize.x / 2f;
        float right = camPos2.x + deadZoneSize.x / 2f;
        float bottom = camPos2.y - deadZoneSize.y / 2f;
        float top = camPos2.y + deadZoneSize.y / 2f;

        Vector3 newPos = camPos2;

        // --- X 軸方向 ---
        if (targetPos2.x < left)
            newPos.x = targetPos2.x + deadZoneSize.x / 2f;
        else if (targetPos2.x > right)
            newPos.x = targetPos2.x - deadZoneSize.x / 2f;

        // --- Y 軸方向 ---
        if (targetPos2.y < bottom)
            newPos.y = targetPos2.y + deadZoneSize.y / 2f;
        else if (targetPos2.y > top)
            newPos.y = targetPos2.y - deadZoneSize.y / 2f;

        // Zはそのまま（2Dなら -10 固定）
        newPos.z = camPos2.z;

        // カメラ範囲Clampを追加
        newPos.x = Mathf.Clamp(newPos.x, minCamPos.x, maxCamPos.x);
        newPos.y = Mathf.Clamp(newPos.y, minCamPos.y, maxCamPos.y);

        // 滑らかに追従
        transform.position = Vector3.Lerp(camPos2, newPos, smoothSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        // デッドゾーンを Scene ビューに可視化
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector3(
                (minCamPos.x + maxCamPos.x) / 2f,
                (minCamPos.y + maxCamPos.y) / 2f,
                transform.position.z
            ),
            new Vector3(
                maxCamPos.x - minCamPos.x,
                maxCamPos.y - minCamPos.y,
                0
            )
        );
    }

    public void LockCamera(Vector3 pos)
    {
        isLocked = true;

        lockedPosition = pos;
    }

    public void UnlockCamera()
    {
        isLocked = false;
    }
}