using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SeleccionMinijuegos;

public class ManagerMenuTableros : MonoBehaviour
{
    

    List<ConfiguracionTablero> configuracionTableros;
    public Toggle[] opcionesTamaños = new Toggle[0];
    public Toggle[] opcionesMinijuegos = new Toggle[0];
    public Toggle[] opcionesMinijuegos18 = new Toggle[0];

    public GameObject popupConfiguracionTableros;
    public GameObject fondoDifuminado;

    string tablero;
    string tamaño;
    string minijuegos = "";
    string minijuegos18 = "";
    string continuarPartida = "N";

    // Start is called before the first frame update
    void Start()
    {

        // Inicialización de sqlite-net
        try
        {
            ClaseManagerBBDD.Instance.Conectar(Path.Combine(Application.persistentDataPath, "JuegOcaBBDD.db"));
            Debug.Log("Base de datos SQLite conectada correctamente.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error al conectar con la base de datos: " + e.Message);
        }

        ReiniciarTablaConfiguracion();

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Acciones Botones---------------------------------------------------------------------

    public void Jugar() {

        ToggleActivos();
        ConfiguracionTablero config = new ConfiguracionTablero(tablero, tamaño,minijuegos,minijuegos18,continuarPartida);
        ClaseManagerBBDD.Instance.Insert<ConfiguracionTablero>(config);
        popupConfiguracionTableros.SetActive(false);
        fondoDifuminado.SetActive(false);
        //managerBBDD.CloseDatabase();
        SceneManager.LoadScene("GenerarTablero");


    }

    public void SeleccionTablero()
    {
        popupConfiguracionTableros.SetActive(true);
        fondoDifuminado.SetActive(true);
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (clickedButton != null)
        {
            tablero = clickedButton.name;
            Debug.Log("tablero: "+ tablero);
        }

    }

    public void Volver()
    {
        popupConfiguracionTableros.SetActive(false);
        fondoDifuminado.SetActive(false);

    }

    public void VolverEscena()
    {
        SceneManager.LoadScene("MenuModosDeJuego");

    }



    void ToggleActivos() 
    {

        foreach (var toggle in opcionesTamaños)
        {
            if (toggle != null && toggle.isOn)
            {
                
                tamaño = toggle.GetComponentInChildren<Text>().text;
                
            }
        }

        foreach (var toggle in opcionesMinijuegos)
        {
            if (toggle != null && toggle.isOn)
            {

                string textoToggle = toggle.GetComponentInChildren<Text>().text;
                minijuegos += textoToggle + ":";

            }
            
        }
        int cont = 0;
        foreach (var toggle in opcionesMinijuegos18)
        {
            if (toggle != null && toggle.isOn)
            {

                minijuegos18 += cont + ":";

            }
            else {

                minijuegos18 += -1 + ":";
            }
           
            cont++;
        }

        
    }

    void ReiniciarTablaConfiguracion() {

        try {
            configuracionTableros = ClaseManagerBBDD.Instance.SelectAll<ConfiguracionTablero>();
            ClaseManagerBBDD.Instance.DeleteAll<ConfiguracionTablero>();
        } catch {
            ClaseManagerBBDD.Instance.CreateTable<ConfiguracionTablero>();
        }
        

    }


}
public class ConfiguracionTablero
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string tablero { get; set; }
    public string tamaño { get; set; }
    public string minijuegos { get; set; }
    public string minijuegos18 { get; set; }

    public string continuarPartida { get; set; }

    public ConfiguracionTablero()
    {
 
    }
    public ConfiguracionTablero(string tablero, string tamaño, string minijuegos, string minijuegos18, string continuarPartida)
    {
        this.tablero = tablero;
        this.tamaño = tamaño;
        this.minijuegos = minijuegos;
        this.minijuegos18 = minijuegos18;
        this.continuarPartida = continuarPartida;
    }
}