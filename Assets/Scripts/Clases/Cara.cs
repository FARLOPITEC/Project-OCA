using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cara : MonoBehaviour
{


    public int Numero;

    public bool TocaSuelo;

    void Start()
    {
        Numero = int.Parse(GetComponent<TMP_Text>().text);
    }

    void Update()
    {

    }

    // Constructor sin parámetros

   /* public Cara(int numero, bool tocaSuelo)
    {
        Numero = numero;
        TocaSuelo = tocaSuelo;
    }*/

    void OnTriggerEnter(Collider col)
    {
        //Debug.Log("col.gameObject.tag "+ col.gameObject.name);
        if (col.gameObject.name == "Suelo")
        {
            TocaSuelo = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        TocaSuelo = false;
    }
}
