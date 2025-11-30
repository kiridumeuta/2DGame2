using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float MoveRight = 0.01f;
    private float MoveLeft = -0.01f;

    private float jumpForce = 10f;

    Rigidbody2D RB2D;
    void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerMove();
        PlayerJump();
    }

    private void PlayerMove()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(MoveRight, 0f, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(MoveLeft, 0f, 0);
        }
    }
    private void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RB2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
