using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // 歩き状態
    public void SetWalk(bool isWalking)
    {
        animator.SetBool("Walk", isWalking);
    }

    // 地面判定
    public void SetGround(bool isGrounded)
    {
        animator.SetBool("IsGround", isGrounded);
    }

    // ジャンプ開始
    public void PlayJump()
    {
        animator.SetTrigger("JumpStart");
    }

    // 二段ジャンプ
    public void PlayDoubleJump()
    {
        animator.SetTrigger("DoubleJump");
    }

    // 落下
    public void PlayFall()
    {
        animator.SetTrigger("Fall");
    }
}
