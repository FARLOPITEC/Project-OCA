using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
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
    private string databasePath;
    private static ClaseManagerBBDD instance;

    public void Conectar(string databasePath)
    {
        // Guarda la ruta para poder reconectar más tarde si es necesario.
        this.databasePath = databasePath;

        // Si connection es nula o está cerrada (en este ejemplo suponemos que si se produce cualquier error al crear un comando, la conexión no está disponible)
        if (connection == null)
        {
            try
            {
                connection = new SQLiteConnection(databasePath);
                Debug.Log("Base de datos conectada en: " + databasePath);
            }
            catch (Exception e)
            {
                Debug.LogError("Error al conectar con la base de datos: " + e.Message);
            }
        }
        else
        {
            // Opcional: intenta verificar que la conexión sigue siendo válida.
            try
            {
                // Tratamos de crear un comando simple para probar la conexión. Si falla, se reabre.
                var cmd = connection.CreateCommand("SELECT 1");
                int resultado = cmd.ExecuteScalar<int>();
            }
            catch (Exception)
            {
                // Se detecta que la conexión no es válida, se reinicializa.
                try
                {
                    connection = new SQLiteConnection(databasePath);
                    Debug.Log("Base de datos reconectada en: " + databasePath);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error al reconectar con la base de datos: " + e.Message);
                }
            }
        }
    }

    public static ClaseManagerBBDD Instance
    {
        get
        {
            if (instance == null)
            {
                string dbPath = Path.Combine(Application.persistentDataPath, "JuegOcaBBDD.db");

                // Verifica si la base de datos existe; si no, cópiala antes de inicializar el Singleton
                if (!File.Exists(dbPath))
                {
                    Debug.Log("Base de datos no encontrada, copiando desde StreamingAssets...");
                    string sourcePath = Path.Combine(Application.streamingAssetsPath, "JuegOcaBBDD.db");

                    if (Application.platform == RuntimePlatform.Android)
                    {
                        UnityWebRequest www = UnityWebRequest.Get(sourcePath);
                        www.SendWebRequest();

                        while (!www.isDone) { } // Espera a que termine la descarga

                        if (www.result == UnityWebRequest.Result.Success)
                        {
                            File.WriteAllBytes(dbPath, www.downloadHandler.data);
                            Debug.Log("Base de datos copiada exitosamente.");
                        }
                        else
                        {
                            Debug.LogError("Error al copiar la base de datos: " + www.error);
                        }
                    }
                    else if (File.Exists(sourcePath))
                    {
                        File.WriteAllBytes(dbPath, File.ReadAllBytes(sourcePath));
                        Debug.Log("Base de datos copiada exitosamente.");
                    }
                    else
                    {
                        Debug.LogWarning("No hay archivo en StreamingAssets. Creando una base de datos nueva...");
                        SQLiteConnection conn = new SQLiteConnection(dbPath);
                        // Aquí podrías crear las tablas o lo que necesites.
                    }
                }

                instance = new ClaseManagerBBDD(dbPath);
            }
            return instance;
        }
    }

    private ClaseManagerBBDD(string databasePath)
    {
        this.databasePath = databasePath;
        connection = new SQLiteConnection(databasePath);
    }

    public void CreateTable<T>()
    {
        connection.CreateTable<T>();
    }

    public void DeleteTable<T>()
    {
        connection.DropTable<T>();
    }

    public void Insert<T>(T objeto) where T : new()
    {
        connection.Insert(objeto);
    }

    public List<T> SelectAll<T>() where T : new()
    {
        return connection.Table<T>().ToList();
    }

    public void DeleteAll<T>() where T : new()
    {
        connection.DeleteAll<T>();
    }

    public void CloseDatabase()
    {
        connection.Close();
        connection = null;
    }
}

//Singleton BBDD



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

//MiniJuegos------------------------------------------------------------------------------
public class MinijuegoTablero
{
    public string nombre;
    public string color;
    public string rutaArchivoCSV;

    public MinijuegoTablero(string nombre, string color, string rutaArchivoCSV)
    {
        this.nombre = nombre;
        this.color = color;
        this.rutaArchivoCSV = rutaArchivoCSV;
    }
}

public static class DatosEscena
{
    public static string EscenaAnterior { get; set; }
}

