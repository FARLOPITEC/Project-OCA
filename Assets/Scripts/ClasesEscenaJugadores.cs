using SQLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string RutaImagen { get; set; }

    public string ColorIcono { get; set; }

    public string Ficha { get; set; }
    public int Posicion { get; set; }

    // Constructor sin parámetros
    public Jugador() { }
    public Jugador(string nombre, string rutaImagen, string colorIcono, string ficha, int posicion)
    {
        Nombre = nombre;
        RutaImagen = rutaImagen;
        ColorIcono = colorIcono;
        Ficha = ficha;
        Posicion = posicion;
    }
}

public class ColorJugadores
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Hexadecimal { get; set; }

    // Constructor sin parámetros
    public ColorJugadores() { }
    public ColorJugadores(string nombre, string hexadecimal)
    {
        Nombre = nombre;
        Hexadecimal = hexadecimal;

    }
}