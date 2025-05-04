using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ficha
{
    public string Nombre { get; set; }
    public string RutaFicha { get; set; }

    public Ficha(string nombre, string rutaFicha)
    {
        Nombre = nombre;
        RutaFicha = rutaFicha;
    }
}
