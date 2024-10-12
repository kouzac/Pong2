using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    // Este método se llama cuando la pelota colisiona con este límite
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificamos que la colisión sea con la bola
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();

            // Verificamos que el Rigidbody2D de la bola exista
            if (ballRb != null)
            {
                // Invertimos la dirección horizontal de la velocidad de la bola
                ballRb.velocity = new Vector2(-ballRb.velocity.x, ballRb.velocity.y);
                Debug.Log("Ball hit boundary and bounced!");
            }
        }
    }
}
