using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador
{
    public string Nombre { get; set; }
    public string RutaImagen { get; set; }

    public Color ColorIcono { get; set; }

    public string Ficha { get; set; }
    public int Posicion { get; set; }

    public Jugador(string nombre, string rutaImagen, Color colorIcono, string ficha, int posicion)
    {
        Nombre = nombre;
        RutaImagen = rutaImagen;
        ColorIcono = colorIcono;
        Ficha = ficha;
        Posicion = posicion;
    }
}
