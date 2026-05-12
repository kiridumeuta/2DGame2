using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f; // ’e‚Ìõ–½

    void Start()
    {
        Destroy(gameObject, lifeTime); // ˆê’èŠÔŒã‚É’e‚ğ”j‰ó
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject); // Õ“Ë‚µ‚½‚ç’e‚ğ”j‰ó
        }
    }
}
