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

    void LateUpdate()
    {
        if (target == null) return;

        // 現在のカメラ位置（Zは維持）
        Vector3 camPos = transform.position;
        Vector3 targetPos = target.position + offset;

        // デッドゾーン（境界）
        float left = camPos.x - deadZoneSize.x / 2f;
        float right = camPos.x + deadZoneSize.x / 2f;
        float bottom = camPos.y - deadZoneSize.y / 2f;
        float top = camPos.y + deadZoneSize.y / 2f;

        Vector3 newPos = camPos;

        // --- X 軸方向 ---
        if (targetPos.x < left)
            newPos.x = targetPos.x + deadZoneSize.x / 2f;
        else if (targetPos.x > right)
            newPos.x = targetPos.x - deadZoneSize.x / 2f;

        // --- Y 軸方向 ---
        if (targetPos.y < bottom)
            newPos.y = targetPos.y + deadZoneSize.y / 2f;
        else if (targetPos.y > top)
            newPos.y = targetPos.y - deadZoneSize.y / 2f;

        // Zはそのまま（2Dなら -10 固定）
        newPos.z = camPos.z;

        // カメラ範囲Clampを追加
        newPos.x = Mathf.Clamp(newPos.x, minCamPos.x, maxCamPos.x);
        newPos.y = Mathf.Clamp(newPos.y, minCamPos.y, maxCamPos.y);

        // 滑らかに追従
        transform.position = Vector3.Lerp(camPos, newPos, smoothSpeed);
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
}
