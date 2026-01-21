using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    [SerializeField, Header("ˆÚ“®‘¬“x")]
    private float moveSpeed = 2f;

    Rigidbody2D rb;
    int defaultLayer;
    int noPushLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultLayer = gameObject.layer;
        noPushLayer = LayerMask.NameToLayer("EnemyNoPush");
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.layer = noPushLayer;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.layer = defaultLayer;
        }
    }
}
