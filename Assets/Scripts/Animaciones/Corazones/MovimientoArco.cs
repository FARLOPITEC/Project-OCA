using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoArco : MonoBehaviour
{

    public float swingAmplitude = 1f;  // Qu� tan lejos se mueve de un lado a otro
    public float swingSpeed = 2f;  // Velocidad del vaiv�n
    public float tiltAmount = 15f; // Cu�nto se inclina al moverse

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // Guarda la posici�n inicial
    }

    void Update()
    {
        // Movimiento de vaiv�n en el propio espacio del objeto
        float swing = Mathf.Sin(Time.time * swingSpeed) * swingAmplitude;

        // Aplicar el movimiento en su propio eje (transform.right mueve seg�n su rotaci�n)
        transform.position = startPosition + transform.right * swing;

        // Simular inclinaci�n sin afectar el giro completo
        float rotationZ = Mathf.Sin(Time.time * swingSpeed) * tiltAmount;
        //transform.rotation = Quaternion.Euler(0, 0, rotationZ);



        if (gameObject.name.Contains("Izq"))
        {
            if (gameObject.name.Contains("1")) {
                transform.rotation = Quaternion.Euler(0, 0, rotationZ);
            }
            else if (gameObject.name.Contains("2"))
            {
                transform.rotation = Quaternion.Euler(0, 70, rotationZ);
            }
            else if (gameObject.name.Contains("3"))
            {
                transform.rotation = Quaternion.Euler(0, 0, rotationZ);
            }
            else if (gameObject.name.Contains("4"))
            {
                transform.rotation = Quaternion.Euler(0, 70, rotationZ);
            }
            
        }
        else if(gameObject.name.Contains("Der"))
        {
            if (gameObject.name.Contains("1"))
            {
                transform.rotation = Quaternion.Euler(0, 70, rotationZ);
            }
            else if (gameObject.name.Contains("2"))
            {
                transform.rotation = Quaternion.Euler(0, 0, rotationZ);
            }
            else if (gameObject.name.Contains("3"))
            {
                transform.rotation = Quaternion.Euler(0, 70, rotationZ);
            }
            else if (gameObject.name.Contains("4"))
            {
                transform.rotation = Quaternion.Euler(0, 0, rotationZ);
            }
            

        }
        
    }
}



