using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador3D
{

    public string Nombre { get; set; }
    public string RutaIcono { get; set; }

    public string Ficha { get; set; }
    public int Posicion { get; set; }

    public Jugador3D(string nombre, string rutaIcono, string ficha, int posicion)
    {
        Nombre = nombre;
        RutaIcono = rutaIcono;
        Ficha = ficha;
        Posicion = posicion;
    }
}
