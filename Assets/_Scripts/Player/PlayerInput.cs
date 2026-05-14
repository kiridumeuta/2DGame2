using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // 横移動入力
    public float MoveInput { get; private set; }

    // ジャンプ入力
    public bool JumpPressed { get; private set; }

    // インベントリ
    public bool ResetInventoryPressed { get; private set; }
    public bool ShowInventoryPressed { get; private set; }

    void Update()
    {
        // 横移動
        MoveInput = 0f;

        if (Input.GetKey(KeyCode.D))
            MoveInput = 1f;

        if (Input.GetKey(KeyCode.A))
            MoveInput = -1f;

        // 単発入力
        JumpPressed = Input.GetKeyDown(KeyCode.Space);

        ResetInventoryPressed = Input.GetKeyDown(KeyCode.R);
        ShowInventoryPressed = Input.GetKeyDown(KeyCode.I);
    }
}
