using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaeYGira : MonoBehaviour
{
    public float fallSpeed = 10f;
    public float rotationSpeed = 200f;
    public float resetHeight = -5f;  // Altura m�nima antes de reiniciar
                                     // Start is called before the first frame update
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Movimiento de ca�da
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Rotaci�n sobre su eje Z (no usa Vector3.forward)
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Reiniciar cuando pase la altura m�nima
        if (transform.position.y < resetHeight)
        {
            transform.position = startPosition;
        }
    }


}
