using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;

    private Vector2 vector2;

    public void InitiateProjectile(Vector2 _launchVector, float _projectileSpeed)
    {
        vector2 = _launchVector.normalized * _projectileSpeed;
        rb.AddForce(vector2);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Enemy"))
        {
            if (collision.collider.CompareTag("Player"))
            {
                SpidyMovement.instance.TakeDamage();
            }
            Destroy(this.gameObject);
        }
    }
}
