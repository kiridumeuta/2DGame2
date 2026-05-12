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
        if(collision.CompareTag("Wall"))
        {
            Destroy(gameObject); // •Ç‚©°‚ÉÕ“Ë‚µ‚½‚ç’e‚ğ”j‰ó
        }
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject); // Õ“Ë‚µ‚½‚ç’e‚ğ”j‰ó

            Enemy1 enemy = collision.GetComponent<Enemy1>();
            EnemyJump enemyjump = collision.GetComponent<EnemyJump>();
            if (enemy != null)
            {
                enemy.DestroyEnemy(); // ƒXƒ|ƒi[‚É’Ê’m‚³‚ê‚é
            }
            if (enemyjump != null)
            {
                enemyjump.DestroyEnemy(); // ƒXƒ|ƒi[‚É’Ê’m‚³‚ê‚é
            }
        }
    }
}
