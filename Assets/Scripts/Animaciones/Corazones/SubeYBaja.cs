using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubeYBaja : MonoBehaviour
{


    public float fallHeight = 10f;  // Distancia que cae
    public float fallSpeed = 2f;   // Velocidad de ca�da
    public float rotationSpeed = 200f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position + new Vector3(5000, 0, 0); // Sumar 5000 en X
        transform.position = startPosition; // Aplicar la posici�n inicial desplazada

    }

    void Update()
    {
        // Movimiento de ca�da
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;


        // Movimiento de ca�da y regreso
        float newY = startPosition.y - Mathf.PingPong(Time.time * fallSpeed, fallHeight);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotaci�n sobre su eje Z (no usa Vector3.forward)
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}


