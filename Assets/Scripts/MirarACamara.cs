using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirarACamar : MonoBehaviour
{

        public Transform camara; // Referencia a la c�mara

        void Update()
        {
            if (camara != null)
            {
                transform.LookAt(camara);
            }
        }
}

