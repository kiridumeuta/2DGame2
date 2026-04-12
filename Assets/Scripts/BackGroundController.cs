using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    [Header("追従するカメラ")]
    [SerializeField] private Transform cameraTransform;

    [Header("移動倍率（0〜1）")]
    [SerializeField] private float parallaxFactor = 0.5f;

    private Vector3 lastCameraPos;

    void Start()
    {
        if (cameraTransform != null)
            lastCameraPos = cameraTransform.position;
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // カメラの移動量
        Vector3 delta = cameraTransform.position - lastCameraPos;

        // 背景を「割合」で移動
        transform.position += new Vector3(
            delta.x * parallaxFactor,
            delta.y * parallaxFactor,
            0f
        );

        // カメラ位置を更新
        lastCameraPos = cameraTransform.position;
    }
}
