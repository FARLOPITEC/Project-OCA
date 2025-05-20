using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoFicha : MonoBehaviour
{

    public Transform[] tilePositions; // Lista de posiciones de las casillas
    public float moveSpeed = 5f; // Velocidad del movimiento entre casillas
    public float bounceHeight = 0.5f; // Altura del pequeño rebote al llegar a cada casilla
    public float delayBetweenTiles = 0.2f; // Tiempo entre cada "golpe" en la casilla

    private int currentTileIndex = 0;

    public void MoveToTile(int targetIndex)
    {
        if (targetIndex >= tilePositions.Length) return; // Evitar que pase del límite
        StartCoroutine(MovePiece(targetIndex));
    }

    IEnumerator MovePiece(int targetIndex)
    {
        while (currentTileIndex < targetIndex)
        {
            Vector3 startPos = tilePositions[currentTileIndex].position;
            Vector3 targetPos = tilePositions[currentTileIndex + 1].position;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * moveSpeed;

                // Movimiento en arco (rebote pequeño)
                float arc = Mathf.Sin(t * Mathf.PI) * bounceHeight;
                transform.position = Vector3.Lerp(startPos, targetPos, t) + Vector3.up * arc;

                yield return null;
            }

            currentTileIndex++;
            yield return new WaitForSeconds(delayBetweenTiles); // Pausa en cada casilla
        }
    }
}

