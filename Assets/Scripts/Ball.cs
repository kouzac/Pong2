using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public class Ball : NetworkBehaviour
{
    private Rigidbody2D rb;
    public float speed = 5f; // Velocidad inicial de la bola

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnStartServer()
    {
        LaunchBall();
    }

    private void LaunchBall()
    {
        float angle = Random.Range(0f, 360f);
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        rb.velocity = direction.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.CompareTag("Paddle") || collision.gameObject.CompareTag("Boundary"))
            {
                Vector2 currentVelocity = rb.velocity;
                Vector2 collisionNormal = collision.contacts[0].normal;
                Vector2 newDirection = Vector2.Reflect(currentVelocity, collisionNormal);
                rb.velocity = newDirection.normalized * speed;
            }
        }
    }
}
