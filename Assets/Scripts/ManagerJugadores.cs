using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class ManagerJugadores : MonoBehaviour
{
    //Principal jugadores
    public GameObject contenedor;
    //Popup
    public TMP_Text tituloPopup;
    public GameObject popupJugador;
    public GameObject contenedorColores;
    public Image colorElegido;
    public Image imagenElegida;
    public GameObject imagenNoImagen;
    public TMP_InputField inputNombre;
    string ruta="";
    //Popup Advertencia
    public GameObject popupAdvertenvia;
    public TMP_Text tituloPopupAdvertencia;
    public TMP_Text problema;
    //Popup continuar partida
    public GameObject popupContinuarPartida;

    //Generales
    Dictionary<string, Jugador> jugadores = new Dictionary<string, Jugador>();
    Dictionary<string, string> strColoresHex = new Dictionary<string, string>();
    ColorJugadores colorJuBBDD = new();
    Dictionary<string, Color> colores = new Dictionary<string, Color>();
    Dictionary<string, GameObject> botonesColor = new Dictionary<string, GameObject>();
    int numJugadores = 1;

    //BBDD

    bool existeBaseDeDatos;
    void Awake()
    {
        Application.targetFrameRate = 60; // Reducir FPS para mejorar rendimiento y batería
        QualitySettings.vSyncCount = 0; // Desactivar VSync en móvil
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.READ_MEDIA_IMAGES"))
        {
            Debug.LogError("Permiso de lectura revocado, solicitando de nuevo...");
            Permission.RequestUserPermission("android.permission.READ_MEDIA_IMAGES");
        }
        else
        {
            Debug.Log("Permiso activo, debería poder acceder sin problemas.");
        }
        Debug.Log("Cargando SQLite...");


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

        
        List<Jugador> Jug = null;
        List<ConfiguracionTablero> config = null;
        try
        {
            Jug= ClaseManagerBBDD.Instance.SelectAll<Jugador>();
            
            
        }
        catch
        {
            ClaseManagerBBDD.Instance.CreateTable<Jugador>();

        }

        try
        {
            config = ClaseManagerBBDD.Instance.SelectAll<ConfiguracionTablero>();


        }
        catch
        {

        }

        if (config != null && config[0].continuarPartida.Equals("S"))
        {

            popupContinuarPartida.gameObject.SetActive(true);
        }
        else if(Jug!=null){
            ClaseManagerBBDD.Instance.DeleteAll<Jugador>();
        }
        


        try
        {
            StartCoroutine(GenerarColores());
        }
        catch (Exception e)
        {
            Debug.LogError("Error crítico antes del cierre: " + e.Message);
        }

    }







    // Update is called once per frame
    void Update()
    {
        
    }



    //Acciones botones-----------------------------------------------------------------------------------------------------------------------------
    //Principal
    public void AñadirJugador() {

        popupJugador.gameObject.SetActive(true);

        


    }

    public void AccionEmpezar()
    {
        if (jugadores.Count > 1)
        {
            StartCoroutine(CambiarEscena());
        }
        else
        {
            StartCoroutine(ProducirErrorJugadores());
        }
    }

    IEnumerator CambiarEscena()
    {

        if (jugadores.Count > 1)
        {
            StartCoroutine(GuardarJugadoresYCambiarEscena());
           
            //managerBBDD.CloseDatabase();
            yield return null;
            SceneManager.LoadScene("MenuModosDeJuego"); // Cambia por el nombre real de la escena
        }
        else {

            StartCoroutine(ProducirErrorJugadores());

        }

    }
    //Popup Jugador
    public void BotonVolver()
    {

        popupJugador.gameObject.SetActive(false);
        ReiniciarPopup();

    }



    public void BotonConfirmar()
    {

        Jugador jugador = new Jugador(inputNombre.text, ruta , ColorElegidoHex(colorElegido.color),"FichaCorazon", 0);
        ColorUtility.TryParseHtmlString("#FFFFFF", out Color blanco);

        


        Boolean encontrado=false;
        foreach (KeyValuePair<string, Jugador> ju in jugadores)
        {
            if (ju.Key==jugador.Nombre) { 
            
            encontrado = true;
            
            }
            
        }
        ColorUtility.TryParseHtmlString(jugador.ColorIcono, out Color colorJugador);
        if (!jugador.Nombre.Equals("") && jugador.RutaImagen!="" && colorJugador != blanco && !encontrado)
        {

            jugadores.Add(jugador.Nombre, jugador);
            AñadirIcono(jugador.Nombre,jugador.RutaImagen);
            numJugadores++;
            tituloPopup.text = "Jugador " + numJugadores;
            popupJugador.gameObject.SetActive(false);
            eliminarColor(colorElegido.color);
            ReiniciarPopup();



        }
        else {
            StartCoroutine(ProducirError(jugador,encontrado));
        }

    }

    public void BotonElegirImagen()
    {
        ElegirImagen((rutaSeleccionada) =>
        {
            if (!string.IsNullOrEmpty(rutaSeleccionada))
            {
                ruta = rutaSeleccionada;
                CambiarImagenJugador(imagenElegida.gameObject, ruta);
            }
        });
    }

    void ReiniciarPopup() {
        ColorUtility.TryParseHtmlString("#FFFFFF", out Color blanco);
        inputNombre.text = "";
        imagenElegida.sprite = null;
        ColorUtility.TryParseHtmlString("#6756C1", out Color lila);
        imagenElegida.color = lila;
        imagenNoImagen.SetActive(true);
        ruta = "";

        foreach (Transform child in contenedorColores.transform)
        {
            Destroy(child.gameObject); // Destruye cada hijo del contenedor

        }

        botonesColor = new Dictionary<string, GameObject>();
        colorElegido.color = blanco;
        AñadirBotonesColor();
    }
    //Principal Jugadores----------------------------------------------------------------------------------------------------------------------------------
    //Generar Icono principal
    void AñadirIcono(string nombre,string ruta) {
        GameObject iconoPrefab = Resources.Load<GameObject>("Prefabs/Escena Jugadores/Icono");

        GameObject icono = Instantiate(iconoPrefab, Vector3.zero, Quaternion.identity);


        TMP_Text textNombre = icono.GetComponentInChildren<TMP_Text>();
        textNombre.text =nombre;

        
        Image textComponentBoton = icono.transform.Find("ButtonJugador").GetComponentInChildren<Image>();
        textComponentBoton.color = colorElegido.color;

        Image ImagenIcono = textComponentBoton.transform.Find("ImageJugador").gameObject.GetComponentInChildren<Image>();
        CambiarImagenJugador(ImagenIcono.gameObject,ruta);

        icono.transform.SetParent(contenedor.transform, false);
    }

    //Popup Jugador----------------------------------------------------------------------------------------------------------------------------------
    //Generar Botones Colores Popup

    IEnumerator AñadirColores() {
        strColoresHex.Add("rojo", "#FF0008");
        strColoresHex.Add("verde", "#3EFF00");
        strColoresHex.Add("amarillo", "#F4FF00");
        strColoresHex.Add("fuxia", "#FF0086");
        strColoresHex.Add("lila", "#9700FF");
        strColoresHex.Add("azul", "#00D1FF");
        strColoresHex.Add("lima", "#00FF8C");
        strColoresHex.Add("naranja", "#FF7000");
        strColoresHex.Add("rosa", "#D76097");
        strColoresHex.Add("amarilloC", "#C1D760");
        Debug.Log("AñadirColores() terminó correctamente.");
        yield return null;
    }
    string ColorElegidoHex(Color color)
    {
        string strColor = "";
        foreach (KeyValuePair<string, Color> co in colores)
        {
            if (co.Value == color)
            {
                strColor = co.Key;

            }

        }


        return strColor;
    }
    IEnumerator GenerarColor2(string hex, string nombreColor) {

        ColorUtility.TryParseHtmlString(hex, out Color color);
        colores.Add(hex, color);

        yield return null;

    }

    IEnumerator GenerarColor(string hex, string nombreColor)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            colores.Add(hex, color);
            Debug.Log("Color generado: " + nombreColor + " -> " + color);
        }
        else
        {
            Debug.LogError("Error al convertir color: " + hex);
        }

        yield return null; // Espera un frame antes de seguir
    }

    IEnumerator GenerarColores()
    {
        yield return StartCoroutine(AñadirColores());

        

        Debug.Log("Cantidad de colores en strColoresHex antes del foreach: " + strColoresHex.Count);

        try
        {
            ClaseManagerBBDD.Instance.DeleteAll<ColorJugadores>();

        }
        catch
        {
            ClaseManagerBBDD.Instance.CreateTable<ColorJugadores>();
        }
        foreach (KeyValuePair<string, string> coH in strColoresHex)
        {
            if (colores == null)
            {
                colores = new Dictionary<string, Color>();
                Debug.LogWarning("¡colores estaba NULL, ahora está inicializado!");
            }

            yield return StartCoroutine(GenerarColor(coH.Value, coH.Key));

            
            if (!existeBaseDeDatos)
            {
                colorJuBBDD = new ColorJugadores(coH.Key, coH.Value);

                if (colorJuBBDD != null)
                {
                    ClaseManagerBBDD.Instance.Insert<ColorJugadores>(colorJuBBDD);
                }
                else
                {
                    Debug.LogError("colorJuBBDD es NULL antes de la inserción en la base de datos!");
                }
            }
        }

         

        AñadirBotonesColor();
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    void GeneraColores2() {


        ColorUtility.TryParseHtmlString("#FF0008", out Color rojo); // Azul
        colores.Add("rojo", rojo);

        ColorUtility.TryParseHtmlString("#3EFF00", out Color verde); // Azul
        colores.Add("verde", verde);

        ColorUtility.TryParseHtmlString("#F4FF00", out Color amarillo); // Azul
        colores.Add("amarillo", amarillo);

        ColorUtility.TryParseHtmlString("#FF0086", out Color fuxia); // Azul
        colores.Add("fuxia", fuxia);

        ColorUtility.TryParseHtmlString("#9700FF", out Color lila); // Azul
        colores.Add("lila", lila);

        ColorUtility.TryParseHtmlString("#00D1FF", out Color azul); // Azul
        colores.Add("azul", azul);

        ColorUtility.TryParseHtmlString("#00FF8C", out Color lima); // Azul
        colores.Add("lima", lima);

        ColorUtility.TryParseHtmlString("#FF7000", out Color naranja); // Azul
        colores.Add("naranja", naranja);

        ColorUtility.TryParseHtmlString("#D76097", out Color rosa); // Azul
        colores.Add("rosa", rosa);

        ColorUtility.TryParseHtmlString("#C1D760", out Color amarilloC); // Azul
        colores.Add("amarilloC", amarilloC);
        

    }
    void AñadirBotonesColor() {

        GameObject botonColor = Resources.Load<GameObject>("Prefabs/Escena Jugadores/" + "BotonColor");

        

        foreach (KeyValuePair<string, Color> co in colores)
        {
            GameObject nuevoBoton = Instantiate(botonColor, new Vector3(), Quaternion.Euler(0,0,0));
            Image imageComponentBoton = nuevoBoton.GetComponent<Image>();
            //ColorUtility.TryParseHtmlString(co.Value, out Color newColor);
            imageComponentBoton.color = co.Value;

            Button botonInstanciado = nuevoBoton.GetComponent<Button>();
            botonInstanciado.onClick.AddListener(() =>
            { 
                colorElegido.color = botonInstanciado.image.color;
            });
            
            
            botonesColor.Add(co.Key,nuevoBoton);


            nuevoBoton.transform.SetParent(contenedorColores.transform, false);

        }
        

    }

    void eliminarColor(Color color) {
        string colorABorrar = "";
        foreach (KeyValuePair<string, Color> co in colores) {
            if (co.Value == color) {
                colorABorrar = co.Key;

            }
        }
        
        
        
        colores.Remove(colorABorrar);

    }
    //Elegir Imagen Popup

    void ElegirImagen(Action<string> callback)
    {
        string ruta = "";
        //noImagen.gameObject.SetActive(false);

        NativeFilePicker.PickFile((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                ruta = path;
                CambiarImagenJugador(imagenElegida.gameObject, ruta);
                callback?.Invoke(path);
            }
            else
            {
                Debug.Log("No se seleccionó ningún archivo.");
                callback?.Invoke(null);
            }


        }, new string[] { "image/*" }// Prueba con un solo formato
);
        
    }

    void ElegirImagen2(Action<string> callback)
    {
        string ruta = "";

        NativeFilePicker.PickFile((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                ruta = path;
                CambiarImagenJugador(imagenElegida.gameObject, ruta);
                callback?.Invoke(path);
            }
            else
            {
                Debug.Log("No se seleccionó ninguna imagen.");
                callback?.Invoke(null);
            }

        }, new string[] { "image/jpeg", "image/png" }); // Usa formatos específicos
    }


    void CambiarImagenJugador(GameObject prefab, string path)
    {
        if (path != null || path != "")
        {
            Image imagenPerfil = prefab.GetComponent<Image>();
            imagenPerfil.color = Color.white;
            imagenNoImagen.SetActive(false);

            Texture2D texture = LoadTexture(path);

            if (texture != null)
            {
                imagenPerfil.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                AspectRatioFitter aspectRatioFitter = imagenPerfil.GetComponent<AspectRatioFitter>();
                if (aspectRatioFitter != null)
                {
                    aspectRatioFitter.aspectRatio = (float)texture.width / texture.height;

                }
            }
        }




    }

    private Texture2D LoadTexture(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError("El archivo no existe: " + path);
            return null;
        }

        byte[] fileData;
        try
        {
            fileData = System.IO.File.ReadAllBytes(path);
        }
        catch (Exception e)
        {
            Debug.LogError("Error al leer el archivo: " + e.Message);
            return null;
        }

        Texture2D texture = new Texture2D(2, 2);
        if (!texture.LoadImage(fileData))
        {
            Debug.LogError("Error al cargar la imagen: " + path);
            return null;
        }

        return texture;
    }

    //Popup Advertencia----------------------------------------------------------------------------------------------------------------------------------
    IEnumerator ProducirError(Jugador ju,Boolean encontrado) {
        
        ColorUtility.TryParseHtmlString("#FFFFFF", out Color blanco);
        ColorUtility.TryParseHtmlString(ju.ColorIcono, out Color colorJugador);
        Debug.Log("color: "+ ju.ColorIcono);
        if (ju.Nombre.Equals(""))
        {

            tituloPopupAdvertencia.text = "Error Nombre";
            problema.text = "No has introducido un nombre";
            

        }
        else if (ju.RutaImagen.Equals(""))
        {
            tituloPopupAdvertencia.text = "Error Imagen";
            problema.text = "No has elegido una imagen";


        }
        else if (colorJugador == blanco)
        {
            tituloPopupAdvertencia.text = "Error Color";
            problema.text = "No has elegido un color";


        }
        else if (encontrado)
        {
            tituloPopupAdvertencia.text = "Error Nombre";
            problema.text = "Ese nombre ya lo has elegido";


        }
        popupAdvertenvia.gameObject.SetActive(true);
        yield return new WaitForSeconds(2); 

        popupAdvertenvia.gameObject.SetActive(false); 
    }
    IEnumerator ProducirErrorJugadores()
    {

        tituloPopupAdvertencia.text = "Error Numero de Jugadores";
        problema.text = "Debes añadir como minimo 2 jugadores";


        popupAdvertenvia.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);

        popupAdvertenvia.gameObject.SetActive(false);
    }
    //BBDD----------------------------------------------------------------------------

    IEnumerator GuardarJugadoresYCambiarEscena()
    {
        AñadirJugadoresBBDD(); // Esperar a que termine

        //managerBBDD.CloseDatabase(); // Cerrar la base de datos después de la última inserción

        yield return null; // Esperar un frame antes de cambiar de escena

        SceneManager.LoadScene("MenuModosDeJuego");
    }

    void AñadirJugadoresBBDD()
    {
        foreach (KeyValuePair<string, Jugador> ju in jugadores)
        {
            ClaseManagerBBDD.Instance.Insert<Jugador>(ju.Value);
            // Espera un frame antes de la siguiente inserción
        }
    }


 


    //Continuar partida
    public void ConinuarPartidaAnterior() {
        DatosEscena.EscenaAnterior = SceneManager.GetActiveScene().name;
        //managerBBDD.CloseDatabase();
        SceneManager.LoadScene("GenerarTablero");
    }

    public void IniciarNueva() {
        ClaseManagerBBDD.Instance.DeleteAll<Jugador>();
        popupContinuarPartida.gameObject.SetActive(false); 
    }



}
