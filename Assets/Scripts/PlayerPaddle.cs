using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Transporting;

public class PlayerPaddle : NetworkBehaviour
{
 public float moveSpeed = 10f;   // Velocidad de la paleta
    public float boundaryX = 8f;    // Límites en el eje X para que la paleta no salga de la pantalla
    public Vector3 bottomPosition = new Vector3(0, -4.5f, 0);  // Posición para el jugador que aparece abajo
    public Vector3 topPosition = new Vector3(0, 4.5f, 0);      // Posición para el jugador que aparece arriba

    private static bool nextPlayerTop = false;  // Controla si el siguiente jugador aparece arriba o abajo
    private Vector3 syncedPosition;

    public override void OnStartClient()
    {
        base.OnStartClient(); // Llamar al método base

        if (base.IsOwner)
        {
            // Llamar al servidor para asignar la posición al jugador
            AssignPositionServerRpc();
        }
    }

    // Esta función es llamada por el servidor para asignar la posición
    [ServerRpc]
    private void AssignPositionServerRpc()
    {
        // Asignar posición al jugador basado en si es el siguiente en la parte superior o inferior
        Vector3 assignedPosition = nextPlayerTop ? topPosition : bottomPosition;

        // Actualizar la posición del jugador
        SetPositionObserversRpc(assignedPosition);

        // Cambiar para que el próximo jugador tenga la posición contraria
        nextPlayerTop = !nextPlayerTop;
    }

    // Esta función sincroniza la posición en todos los clientes
    [ObserversRpc]
    private void SetPositionObserversRpc(Vector3 assignedPosition)
    {
        transform.position = assignedPosition;
    }

    private void Update()
    {
        if (!base.IsOwner)
            return;

        HandleMovement();
    }

    private void HandleMovement()
    {
        // Obtener la entrada horizontal del jugador
        float moveInput = Input.GetAxis("Horizontal");

        // Calcular el desplazamiento en el eje X
        float moveAmount = moveInput * moveSpeed * Time.deltaTime;

        // Obtener la nueva posición de la paleta en el eje X
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Clamp(newPosition.x + moveAmount, -boundaryX, boundaryX);

        // Aplicar la nueva posición localmente
        transform.position = newPosition;

        // Si es el servidor, actualizar directamente la posición sincronizada
        if (base.IsServer)
        {
            syncedPosition = newPosition;
        }
        else
        {
            // Si es un cliente, solicitar al servidor que sincronice la posición
            MovePaddleServerRpc(newPosition);
        }
    }

    [ServerRpc]
    private void MovePaddleServerRpc(Vector3 newPosition)
    {
        syncedPosition = newPosition;
        MovePaddleObservers(newPosition);
    }

    [ObserversRpc]
    private void MovePaddleObservers(Vector3 newPosition)
    {
        if (!base.IsOwner)
        {
            transform.position = newPosition;
        }
    }

    private void FixedUpdate()
    {
        if (!base.IsOwner && base.IsServer)
        {
            transform.position = syncedPosition;
        }
    }
    
}
