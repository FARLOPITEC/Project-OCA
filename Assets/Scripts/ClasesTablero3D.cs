using SQLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Ficha
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string RutaFicha { get; set; }

    // Constructor sin parámetros
    public Ficha() { }
    public Ficha(string nombre, string rutaFicha)
    {
        Nombre = nombre;
        RutaFicha = rutaFicha;
    }
}

public class Jugador3D
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nombre { get; set; }

    public string ColorIcono { get; set; }
    public string RutaIcono { get; set; }

    public string Ficha { get; set; }
    public int Posicion { get; set; }

    // Constructor sin parámetros
    public Jugador3D() { }
    public Jugador3D(string nombre, string colorIcono, string rutaIcono, string ficha, int posicion)
    {
        Nombre = nombre;
        ColorIcono = colorIcono;
        RutaIcono = rutaIcono;
        Ficha = ficha;
        Posicion = posicion;
    }
}

public class ClaseManagerBBDD
{

    private SQLiteConnection connection;

    public ClaseManagerBBDD(string databasePath)
    {
        connection = new SQLiteConnection(databasePath); // SQLite-net usa este constructor directamente
    }

    public void CreateTable<T>()
    {
        connection.CreateTable<T>(); // Método propio de SQLite-net para crear tablas
    }
    public void Insert<T>(T objeto) where T : new()
    {
        connection.Insert(objeto); // Método genérico para insertar cualquier tipo de objeto en la base de datos
    }

    public List<T> SelectAll<T>() where T : new()
    {
        return connection.Table<T>().ToList(); // Devuelve todos los registros de la tabla T
    }

    public void DeletetAll<T>() where T : new()
    {
        connection.DeleteAll<T>(); // Borra todos los registros de la tabla sin eliminarla
    }




    public void CloseDatabase()
    {
        connection.Close();
    }
}

//Tablero 2D

public class Casilla2D
{

    public int Posicion { get; set; }
    public int Numero { get; set; }
    public Sprite Imagen { get; set; }

    public Casilla2D(int posicion, int numero, Sprite imagen)
    {
        Posicion = posicion;
        Numero = numero;
        Imagen = imagen;
    }
}