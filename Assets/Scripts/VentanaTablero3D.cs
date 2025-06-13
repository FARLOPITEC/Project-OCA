using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SeleccionMinijuegos;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

public class VentanaTablero3D : MonoBehaviour
{

    ////Tablero 3D----------------------------------------------------------------
    public GameObject prefabCasNorm;
    public GameObject prefabCasEsq;
    public GameObject prefabCasSal;
    public GameObject casillas;
    public GameObject bordTableroGrande;
    public GameObject bordTableroMediano;
    public GameObject bordTableroPequeño;
    public GameObject contenedorCorazones;
    public GameObject popupPreCarga;
    public GameObject popupTurno;
    public GameObject popupGanador;
    public GameObject popupOtraPartida;
    public GameObject camaraPopup;
    public GameObject camaraReferencia;

    public Image imagenCanvas;
    public GameObject contenedorIconos;
    int contador = 0;

    public List<Material> materiales;
    List<ColorJugadores> coloresJu;

    Dictionary<string, Jugador3D> jugadores=new Dictionary<string, Jugador3D>();
    Dictionary<string, Jugador3D> jugadoresIniciales = new Dictionary<string, Jugador3D>();
    Dictionary<int, Vector3> posicionesTablero= new Dictionary<int, Vector3>();
    Dictionary<string, GameObject> fichas= new Dictionary<string, GameObject>();
    Dictionary<int, string> turnos= new Dictionary<int, string>();
    Dictionary<int, GameObject> iconos = new Dictionary<int, GameObject>();

    int jugadorActual=1;
    public int filas = 11;
    public int columnas = 11;

    List<int> tableroEsquina;



        //Imagen en partes ---------------------------------------------------


    public Texture2D[] imagenesTableros; // La imagen completa
    int numeroTablero;


        //Movimiento fichas---------------------------------------------------
    public float velocidadMovimiento = 5f; // Velocidad del desplazamiento
    public float alturaSalto = 0.5f; // Altura del pequeño golpe en cada casilla
    public float pausaEntreCasillas = 0.2f; // Tiempo de espera antes de avanzar a la siguiente casilla

    //Movimiento Dado
    public GameObject PopupDado;
    public GameObject dado;
    Rigidbody rb;

    public Text texto;
    public Cara[] caras;
    public int NumeroActual;

    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    float tiempoQuieto = 0f; // Acumulador de tiempo en reposo
    float tiempoNecesario = 1f; // Tiempo mínimo en reposo antes de resetear

    public float anguloRotacion = -90f;

    bool block=false;
    bool blockBotonVista = false;

    bool hayOtraFichaEnMismaCasilla = false;

    ////Tablero 2D------------------------------------------------------------
    //GeneraTablero-------------------------------------------------------
    Dictionary<string, GameObject> fichas2D = new Dictionary<string, GameObject>();
    public GameObject contenedorFichas2D;

    public GameObject popupTablero2D;

    public GameObject prefabsCasilla2D;
    public GameObject contenedorCasilla2D;
    public GameObject prefabsCasilla2DSalida;
    public GameObject prefabsFicha2D;

    Dictionary<int,GameObject> casillas2D = new Dictionary<int,GameObject>();
    Dictionary<int, int> mapaEspiralAGrid = new Dictionary<int, int>();

    ////Cambio De Tablero--------------------------------------------------------
    Dictionary<int, GameObject> botonCasillas2D = new Dictionary<int, GameObject>();

    Dictionary<string, GameObject> fichasBTN2D = new Dictionary<string, GameObject>();
    public GameObject contenedorBotonFichas2D;

    public GameObject contenedorBotonTablero2D;
    public GameObject casillaBotonTablero2D;
    public GameObject prefabsBotonFicha2D;

    public GameObject Boton2D;
    public GameObject Boton3D;
    public Camera camaraBoton3D;
    public GameObject noCamera;




    ////Pruebas------------------------------------------------------------------
    //Dicciopinta****************************************************************
    bool puedeDibujar = false;
    bool puedeDibujarTiempo = false;
    int contadorClicks = 0;
    public RawImage lienzoDibujo;
    public TMP_Text tiempoContador;

    GameObject cartaActiva;


    ////BBDD------------------------------------------------------------------

    List<Jugador> jugBBDD;

    bool hayPartida=false;
    int contAutoGuardado = 0;
    bool primeraCarga = true;


    void Awake()
    {
        if (primeraCarga)
        {
            // Inicialización de sqlite-net y otros elementos, solo la primera vez.
            try
            {
                ClaseManagerBBDD.Instance.Conectar(Path.Combine(Application.persistentDataPath, "JuegOcaBBDD.db"));
                Debug.Log("Base de datos SQLite conectada correctamente.");
            }
            catch (Exception e)
            {
                Debug.LogError("Error al conectar con la base de datos: " + e.Message);
            }

            CrearMinijuegos();
            CargarConfigMinijugegos();
            GenerarMapaEspiralAGrid();
            // Tablero y Fichas
            GenerarTablero3D(filas, columnas);

            // Marcamos que ya se realizó la primera carga.
            primeraCarga = false;
        }
    }

  

    // Start se ejecuta después de Awake y OnEnable.
    void Start()
    {
        // Configuración de acuerdo a la escena anterior.
        if (DatosEscena.EscenaAnterior.Equals("MenuUsuarios"))
        {
            popupPreCarga.SetActive(false);
            popupTablero2D.SetActive(false);
        }
        else
        {
            StartCoroutine(MovimientoHada());
        }

        // Dado
        posicionInicial = dado.transform.position;
        rotacionInicial = dado.transform.rotation;

        rb = dado.GetComponent<Rigidbody>();
        rb.useGravity = false;

        // Lienzo
        CrearLienzo();

        jugadores = new Dictionary<string, Jugador3D>();
        CargarJugadores();
        CargarColores();

        // Ficha y Jugadores
        PersonalizarFicha();
        MarcoJugador();

        // Tablero 2D
        GenerarTablero2D();
        AñadirFicha2D();
        MoverFicha2DAlFinal(fichas2D[turnos[jugadorActual]]);
        MoverFicha2DAlFinal(fichasBTN2D[turnos[jugadorActual]]);

        //// Configuración Tablero2D
        MoverFicha2DAlFinal(fichas[jugadores[turnos[jugadorActual]].Ficha]);
        TurnoCamara(fichas[jugadores[turnos[jugadorActual]].Ficha]);
        VerificarSuperposicion(jugadores[turnos[jugadorActual]].Posicion, jugadorActual);
    }






// Update is called once per frame

void Update()
    {
        ActualizarDibujoWin();


        //PararDado();
    }

    //TABLERO 3D -------------------------------------------------------------------------------------

    void GenerarTablero3D(int filas, int columnas)
    {
        
        //Debug.Log("filas: "+ filas+ " columnas: "+ columnas);
        float tamano = 5.1f; // Tamaño de cada cubo
        int x = 0, z = 0; // Posición inicial
        int dx = 1, dz = 0; // Dirección inicial

        int leftBound = 0, rightBound = columnas - 1;
        int topBound = 0, bottomBound = filas - 1;

        //posicionesTablero = new Vector3[filas * columnas];
        float[] porcentaje = null;

        if (filas * columnas > 9 * 9)
        {
            tableroEsquina = new List<int>() { 10, 20, 30, 39, 48, 56, 64, 71, 78, 84, 90, 95, 100, 104, 108, 111, 114, 116, 118, 119 };
            porcentaje = new float[] { 0.33f, 0.59f, 0.79f, 0.92f };
            bordTableroGrande.SetActive(true);
            contenedorCorazones.transform.position = new Vector3(5025.5f, 20.4f, 15.3f);

            camaraBoton3D.transform.localPosition = new Vector3(25.5f, 24.1f, -23.8f);


        }
        else if (filas * columnas < 9 * 9)
        {
            tableroEsquina = new List<int>() { 6, 12, 18, 23, 28, 32, 36, 39, 42, 44, 46, 47 };
            porcentaje = new float[] { 0.47f, 0.8f, 0.99f, 0.0f };
            bordTableroPequeño.SetActive(true);
            contenedorCorazones.transform.position = new Vector3(5015.14602f, 20.70971f, 4.148824f);
            camaraBoton3D.transform.localPosition = new Vector3(15.3f, 24.1f, -23.8f);
        }
        else
        {
            tableroEsquina = new List<int>() { 8, 16, 24, 31, 38, 44, 50, 55, 60, 64, 68, 71, 74, 76, 78, 79 };
            porcentaje = new float[] { 0.39f, 0.69f, 0.88f, 0.0f };
            bordTableroMediano.SetActive(true);
            contenedorCorazones.transform.position = new Vector3(5020.4f, 20.4f, 10.2f);
            camaraBoton3D.transform.localPosition = new Vector3(20.4f, 24.1f, -23.8f);
        }
        for (int i = 0; i < filas * columnas; i++)
        {
            
            // Calcular la posición escalada según el tamaño del cubo
            Vector3 posicionCasilla = new Vector3(x * tamano, 0f, z * tamano);

            posicionesTablero.Add(i, new Vector3(x * tamano, 5.1f, z * tamano));

            

            // Establecer la rotación según la dirección actual
            Quaternion rotacionCasilla;
            if (dx == 1 && dz == 0) // Moviendo hacia la derecha
            {
                //rotacionCasilla = Quaternion.Euler(90f, 0f, 0f);
                rotacionCasilla = Quaternion.Euler(90f, 90f, 0f);
            }
            else if (dx == 0 && dz == 1) // Moviendo hacia abajo
            {
                //rotacionCasilla = Quaternion.Euler(90f, -90f, 0f);
                rotacionCasilla = Quaternion.Euler(90f, 0f, 0f);
            }
            else if (dx == -1 && dz == 0) // Moviendo hacia la izquierda
            {
                //rotacionCasilla = Quaternion.Euler(90f, 180f, 0f);
                rotacionCasilla = Quaternion.Euler(90f, -90f, 0f);
            }
            else // Moviendo hacia arriba
            {
                //rotacionCasilla = Quaternion.Euler(90f, 90f, 0f);
                rotacionCasilla = Quaternion.Euler(90f, 180f, 0f);
            }

            // Instanciar la casilla con la posición y rotación definidas
            
            GameObject casilla = null;
            Quaternion rotacionO = Quaternion.Euler(0f, 0f, 0f);

            if (tableroEsquina.Contains(i))
            {
                Quaternion rotacion45 = Quaternion.Euler(0f, 0f, 45f);
                rotacionCasilla *= rotacion45;
                casilla = Instantiate(prefabCasEsq, posicionCasilla, rotacionO);
            }else if (i == 0)
            {
                casilla = Instantiate(prefabCasSal, posicionCasilla, rotacionO);

            }
            else if (i== (filas * columnas)-1) {
                casilla = Instantiate(prefabCasSal, posicionCasilla, rotacionO);
            }
            else {
                casilla = Instantiate(prefabCasNorm, posicionCasilla, rotacionO);
            }

            

            TextMeshPro textComponent = casilla.GetComponentInChildren<TextMeshPro>();
            if (textComponent != null)
            {
                
                textComponent.transform.rotation = rotacionCasilla;
            }
            else
            {
                //Debug.Log($"No se encontró TextMeshPro en {casilla.name}");
            }


            if (textComponent != null)
            {
                textComponent.SetText(i.ToString()); // Cambia el texto aquí


                if (i < filas * columnas * porcentaje[0])
                {
                    textComponent.color = Color.cyan;
                    AsignarTextura(casilla, i, imagenesTableros[numeroTablero], filas);
                }
                else if (i < filas * columnas * porcentaje[1])
                {
                    textComponent.color = Color.blue;
                    AsignarTextura(casilla, i, imagenesTableros[numeroTablero], filas);
                }

                else if (i < filas * columnas * porcentaje[2])
                {
                    textComponent.color = Color.yellow;
                    AsignarTextura(casilla, i, imagenesTableros[numeroTablero], filas);
                }
                else if (i < filas * columnas * porcentaje[3])
                {
                    textComponent.color = Color.red;
                    AsignarTextura(casilla, i, imagenesTableros[numeroTablero], filas);
                }
                else if (i == filas * columnas || i == 0)
                {

                }
                else
                {
                    textComponent.color = Color.black;
                    AsignarTextura(casilla, i, imagenesTableros[numeroTablero], filas);
                }

            }
            else {
                AsignarTextura(null, i, null, filas);
            }



            casilla.transform.SetParent(casillas.transform, false);


            // Avanzar a la siguiente posición
            x += dx;
            z += dz;

            // Cambiar dirección si se alcanza un borde
            if (dx == 1 && x > rightBound) // Derecha -> Abajo
            {
                dx = 0; dz = 1; x--; z++;
                topBound++;
            }
            else if (dz == 1 && z > bottomBound) // Abajo -> Izquierda
            {
                dx = -1; dz = 0; z--; x--;
                rightBound--;
            }
            else if (dx == -1 && x < leftBound) // Izquierda -> Arriba
            {
                dx = 0; dz = -1; x++; z--;
                bottomBound--;
            }
            else if (dz == -1 && z < topBound) // Arriba -> Derecha
            {
                dx = 1; dz = 0; z++; x++;
                leftBound++;
            }
            //Debug.Log("i:"+ i + " Count: " + (filas * columnas));

        }

    }
    //Icono Turno-----------------------------------------------------------------------------------------------
    void AñadirIcono(string nombre, string ruta,string hex,int cont)
    {
        GameObject iconoPrefab = Resources.Load<GameObject>("Prefabs/Escena Jugadores/MarcoIcono");

        GameObject icono = Instantiate(iconoPrefab, Vector3.zero, Quaternion.identity);

        Image IconoTablero = icono.transform.Find("IconoTablero").GetComponentInChildren<Image>();
        TMP_Text textNombre = icono.GetComponentInChildren<TMP_Text>();
        textNombre.text = nombre;


        Image textComponentBoton = IconoTablero.transform.Find("ButtonJugador").GetComponentInChildren<Image>();
        ColorUtility.TryParseHtmlString(hex, out Color color);
        textComponentBoton.color = color;

        Image ImagenIcono = textComponentBoton.transform.Find("ImageJugador").gameObject.GetComponentInChildren<Image>();
        CambiarImagenJugador(ImagenIcono.gameObject, ruta);
        iconos.Add(cont, icono);

        icono.transform.SetParent(contenedorIconos.transform, false);

    }

 
    void CambiarImagenJugador(GameObject prefab, string path)
    {
        if (path != null || path != "")
        {
            Image imagenPerfil = prefab.GetComponent<Image>();
            imagenPerfil.color = Color.white;
            

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


    //Movimiento fichas -----------------------------------------------------------------------------------------

    IEnumerator GenerarFicha(KeyValuePair<string, Jugador3D> ju, System.Action<GameObject> callback) {

        //Debug.Log("Nombre: "+ ju.Value.Nombre+ " Ficha: " + ju.Value.Ficha);
        GameObject fichaPrefab = Resources.Load<GameObject>("Fichas/" + ju.Value.Ficha);

        GameObject nuevaFicha = Instantiate(fichaPrefab, new Vector3(0, 5.1f, 0), fichaPrefab.transform.rotation);
        while (nuevaFicha == null)
        {
            yield return null;
        }
        callback?.Invoke(nuevaFicha);
    }

    string ColorHexToName(string color) {
        
        string nombre="";
        foreach (ColorJugadores coJu in coloresJu)
        {
            if (coJu.Hexadecimal.Equals(color))
            {
                nombre = coJu.Nombre;
            }

        }

        return nombre;
    }

    Material ColorElegidoMaterial(string color) {

        Material material=null;
        foreach (Material mat in materiales) {

            if (mat.name.Equals(ColorHexToName(color)))
            {
                material = mat;
            }
        
        }
        return material;
    }
    Material MaterialTaslucido(Material materialFicha) {
        materialFicha.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
        materialFicha.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        materialFicha.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        if (materialFicha.HasProperty("_BaseColor"))
        {
            Color colorActual = materialFicha.GetColor("_BaseColor");
            colorActual.a = 150f / 255f; // Convierte 187 al formato correcto
            materialFicha.SetColor("_BaseColor", colorActual);
        }
        materialFicha.SetFloat("_Surface", materialFicha.GetFloat("_Surface"));

        return materialFicha;
    }
    void ColorMaterialTraslucidoIns(int jugador)
    {
        Material materialFicha = fichas[jugadores[turnos[jugador]].Ficha].GetComponent<Renderer>().material;
        foreach (Material mat in materiales)
        {

            if (mat.name.Equals(ColorHexToName(jugadores[turnos[jugador]].ColorIcono)))
            {
                materialFicha = Instantiate(mat); 
            }
        }

        materialFicha = MaterialTaslucido(materialFicha);


        fichas[jugadores[turnos[jugador]].Ficha].GetComponent<Renderer>().material = materialFicha;
        foreach (Renderer planeMesh in fichas[jugadores[turnos[jugador]].Ficha].GetComponentsInChildren<Renderer>())
        {

            if (planeMesh.name.Equals("Plane"))
            {
                
                planeMesh.material = materialFicha;
            }
            else
            {

                if (!planeMesh.name.Equals("CameraFicha"))
                {

                    TMP_Text textoTMP = planeMesh.GetComponent<TMP_Text>(); // Obtener el componente antes de modificarlo
                    if (textoTMP != null)
                    {
                        //Debug.Log(textoTMP.text + "   " + turnos[jugador]);
                        Color colorTexto = textoTMP.color;
                        colorTexto.a = 100f / 255f;
                        textoTMP.color = colorTexto;
                    }
                    else
                    {

                        Material materialImage = planeMesh.GetComponent<Renderer>().material;

                        materialImage = MaterialTaslucido(materialImage);
                    }
                }
            }
        }

    }
    Material MaterialOpaco(Material material)
    {
        material.SetFloat("_Surface", 0); // 0 = Opaque, 1 = Transparent
        material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry; //  Cola de render estándar para opacos

        if (material.HasProperty("_BaseColor"))
        {
            Color colorActual = material.GetColor("_BaseColor");
            colorActual.a = 255f / 255f; // Convierte 187 al formato correcto
            material.SetColor("_BaseColor", colorActual);
        }
        material.SetFloat("_Surface", material.GetFloat("_Surface"));

        return material;
    }

    void ColorMaterialOpacoIns(int jugador)
    {
        Material material = fichas[jugadores[turnos[jugador]].Ficha].GetComponent<Renderer>().material;
        foreach (Material mat in materiales)
        {

            if (mat.name.Equals(ColorHexToName(jugadores[turnos[jugador]].ColorIcono)))
            {
                material = Instantiate(mat);
            }
        }

        material = MaterialOpaco(material);


        fichas[jugadores[turnos[jugador]].Ficha].GetComponent<Renderer>().material = material;
        foreach (Renderer planeMesh in fichas[jugadores[turnos[jugador]].Ficha].GetComponentsInChildren<Renderer>())
        {

            if (planeMesh.name.Equals("Plane"))
            {
                
                planeMesh.material = material;
            }
            else {

                if (!planeMesh.name.Equals("CameraFicha"))
                {

                    TMP_Text textoTMP = planeMesh.GetComponent<TMP_Text>(); // Obtener el componente antes de modificarlo
                    if (textoTMP != null)
                    {
                        Color colorTexto = textoTMP.color;
                        colorTexto.a = 255f / 255f;
                        textoTMP.color = colorTexto;
                    }
                    else
                    {

                        Material materialImage = planeMesh.GetComponent<Renderer>().material;

                        materialImage = MaterialOpaco(materialImage);

                    }
                }
            }
        }

    }
    void HacerInvisible(GameObject ficha) {
        ficha.SetActive(false);
    }

    void HacerVisible(GameObject ficha)
    {
        ficha.SetActive(true);
    }

    void TurnoMaterial(int jugador)
    {

        for (int i = 1; i <= turnos.Count;i++) {
            if (turnos[i].Equals(jugadores[turnos[jugador]].Nombre))
            {
                ColorMaterialOpacoIns(i);
            }
            else {
                ColorMaterialTraslucidoIns(i);
            }
        }

    }

    void TurnoCamara(GameObject gameObject)
    {

        for (int i = 1; i <= turnos.Count; i++)
        {
            if (turnos[i].Equals(turnos[jugadorActual]))
            {
                if(!fichas[jugadores[turnos[i]].Ficha].transform.Find("CameraFicha").gameObject.activeSelf) fichas[jugadores[turnos[i]].Ficha].transform.Find("CameraFicha").gameObject.SetActive(true);
                MoverFicha2DAlFinal(fichas[jugadores[turnos[jugadorActual]].Ficha]);

                TurnoMaterial(jugadorActual);
                //VerificarSuperposicion();

                

                

            }
            else
            {
                fichas[jugadores[turnos[i]].Ficha].transform.Find("CameraFicha").gameObject.SetActive(false);
            }
            
            
        }

    }

    void VerificarSuperposicion2()
    {
        int posicionJugadorActual = jugadores[turnos[jugadorActual]].Posicion; // Obtener posición del jugador actual

        foreach (KeyValuePair<string, Jugador3D> jugador in jugadores)
        {
            if (jugador.Value.Posicion == posicionJugadorActual) // Solo afecta a quienes coincidan con el jugador actual
            {
                if (jugador.Value.Nombre.Equals(turnos[jugadorActual]))
                {
                    HacerVisible(fichas[jugador.Value.Ficha]); // Mantener visible la ficha del jugador actual
                }
                else
                {
                    HacerInvisible(fichas[jugador.Value.Ficha]); // Ocultar solo las fichas que coinciden en su casilla con él
                }
            }
            else
            {
                HacerVisible(fichas[jugador.Value.Ficha]); // Si no están en la misma casilla, mantenerlas visibles
            }
        }
    }

    void VerificarSuperposicion3(int siguienteCasilla)
    {
        int posicionJugadorActual = jugadores[turnos[jugadorActual]].Posicion; // Obtener posición del jugador actual

        foreach (KeyValuePair<string, Jugador3D> jugador in jugadores)
        {
            // Si la ficha está en la siguiente casilla, hacerla invisible temporalmente
            if (jugador.Value.Posicion == siguienteCasilla)
            {
                if (!jugador.Value.Nombre.Equals(turnos[jugadorActual]))
                {
                    HacerInvisible(fichas[jugador.Value.Ficha]); // Ocultar solo las fichas que están en la siguiente casilla
                }
            }
            // Si la ficha ya ha sido alcanzada, volverla visible
            else if (jugador.Value.Posicion == posicionJugadorActual)
            {
                HacerVisible(fichas[jugador.Value.Ficha]); // La ficha vuelve a aparecer cuando el jugador la supera
            }
        }
    }

    void VerificarSuperposicion4(int siguienteCasilla)
    {
        // Asumimos que jugadorActual es el índice del jugador que se mueve.
        string currentPlayer = turnos[jugadorActual];
        int posActual = jugadores[currentPlayer].Posicion;

        // --- Fase 1: Ocultar fichas en la siguiente casilla (solo de otros jugadores) ---
        foreach (KeyValuePair<string, Jugador3D> jugador in jugadores)
        {
            // Si no es la ficha del jugador actual y se encuentra en la siguiente casilla...
            if (!jugador.Value.Nombre.Equals(currentPlayer) && jugador.Value.Posicion == siguienteCasilla)
            {
                HacerInvisible(fichas[jugador.Value.Ficha]);
            }
        }

        // --- Fase 2: Hacer visibles las fichas que estén en la casilla actual (de otros jugadores) ---
        foreach (KeyValuePair<string, Jugador3D> jugador in jugadores)
        {
            // Si no es el jugador actual y la ficha se encuentra en la misma posición que el jugador que se mueve...
            if (!jugador.Value.Nombre.Equals(currentPlayer) && jugador.Value.Posicion == posActual)
            {
                HacerVisible(fichas[jugador.Value.Ficha]);
            }
        }
    }
    void VerificarSuperposicion(int casilla, int jugador)
    {
        // Obtenemos el nombre del jugador que se está moviendo a partir del índice
        string currentPlayer = turnos[jugador];
        int posActual = jugadores[currentPlayer].Posicion;

        // Fase 1: Ocultar fichas de otros jugadores que se encuentren en la casilla objetivo
        foreach (KeyValuePair<string, Jugador3D> kvp in jugadores)
        {
            // Si NO es el jugador actual y su ficha está en la casilla "casilla"
            if (!kvp.Value.Nombre.Equals(currentPlayer) && kvp.Value.Posicion == casilla)
            {
                HacerInvisible(fichas[kvp.Value.Ficha]);
            }
        }

        // Fase 2: Volver visible las fichas que se encuentren en la casilla actual del jugador en movimiento
        foreach (KeyValuePair<string, Jugador3D> kvp in jugadores)
        {
            // Si el jugador NO es el actual y su posición coincide con la del jugador que hemos movido, la volvemos a mostrar
            if (!kvp.Value.Nombre.Equals(currentPlayer) && kvp.Value.Posicion == posActual)
            {
                HacerVisible(fichas[kvp.Value.Ficha]);
            }
        }
    }

    void GestionarVisibilidadFicha3(int posicionActual, int siguientePosicion)
    {
        foreach (KeyValuePair<string, Jugador3D> jugador in jugadores)
        {
            if (jugador.Value.Posicion == siguientePosicion)
            {
                if (!jugador.Key.Equals(turnos[jugadorActual])) { fichas[jugador.Value.Ficha].SetActive(false); } // Desactiva si hay ficha en la casilla destino
            }

            if (jugador.Value.Posicion == posicionActual)
            {
                if (!jugador.Key.Equals(turnos[jugadorActual])) { fichas[jugador.Value.Ficha].SetActive(true); }  // Activa la ficha del jugador que ha llegado
            }
        }
    }

    void GestionarVisibilidadFicha2(int posicionActual, int siguientePosicion)
    {
        foreach (KeyValuePair<string, Jugador3D> jugador in jugadores)
        {
            // Si el jugador ya está en la casilla destino, ocultamos su ficha
            if (jugador.Value.Posicion == siguientePosicion && !jugador.Key.Equals(turnos[jugadorActual]))
            {
                fichas[jugador.Value.Ficha].SetActive(false);
            }

            // Si el jugador deja la casilla, volvemos a activar su ficha
            if (jugador.Value.Posicion == posicionActual && !jugador.Key.Equals(turnos[jugadorActual]))
            {
                fichas[jugador.Value.Ficha].SetActive(true);
            }
        }
    }
   
    void PersonalizarFicha() {
        int cont = 1;
        
        foreach(KeyValuePair<string, Jugador3D> ju in jugadores) {
            StartCoroutine(GenerarFicha(ju, (nuevaFicha) =>
            {
                if (nuevaFicha != null)
                {
                    //Tablero 3D
                    Transform textFront = nuevaFicha.transform.Find("NombreFront");
                    Transform textBack = nuevaFicha.transform.Find("NombreBack");
                    TextMeshPro textComponentFront = textFront.GetComponent<TextMeshPro>();
                    TextMeshPro textComponentBack = textBack.GetComponent<TextMeshPro>();

                    Transform planoFront = textFront.transform.Find("Plane");
                    planoFront.GetComponent<Renderer>().material = ColorElegidoMaterial(ju.Value.ColorIcono);

                    Transform planoBack = textBack.transform.Find("Plane");
                    planoBack.GetComponent<Renderer>().material = ColorElegidoMaterial(ju.Value.ColorIcono);

                    textComponentFront.SetText(ju.Value.Nombre);
                    textComponentBack.SetText(ju.Value.Nombre);

                    Transform fotoObj = nuevaFicha.transform.Find("FotoObj");
                    StartCoroutine(CambiarImagenFicha(fotoObj, ju.Value.RutaIcono));

                    turnos.Add(cont, ju.Value.Nombre);
                    fichas.Add(ju.Value.Ficha + " " + cont, nuevaFicha);
                    ju.Value.Ficha = ju.Value.Ficha + " " + cont;

                    

                    Transform camaraTransform = nuevaFicha.transform.Find("CameraFicha");

                    camaraTransform.gameObject.SetActive(false);
                    if (cont == 1) camaraTransform.gameObject.SetActive(true);
                    nuevaFicha.transform.SetParent(casillas.transform, false);
                    if (hayPartida) {
                        
                        Vector3 posicionInicial = posicionesTablero[ju.Value.Posicion];
                        //Debug.Log("posicionInicial: "+ ju.Value.Posicion + " "+ posicionInicial);
                        nuevaFicha.transform.localPosition = posicionInicial;
                        //camaraBoton3D.transform.localPosition = new Vector3(25.5f, 24.1f, -23.8f);
                        // Aplicar la rotación correcta a cada ficha
                        Debug.LogError("ju.Value.Nombre: " + ju.Value.Nombre );
                        Quaternion rotacionFicha = CalcularRotacionFicha(ju.Value.Posicion);
                        nuevaFicha.transform.localRotation = rotacionFicha;
                    }
                   

                }
            }));

            cont++;
        }
        
    }


    Quaternion CalcularRotacionFicha(int posicion)
    {
        // Variables para simular el recorrido
        int x = 0, z = 0;         // Posición inicial (esquina inferior izquierda)
        int dx = 1, dz = 0;       // Movimiento inicial: a la derecha

        // Límites del tablero
        int left = 0, right = columnas - 1;
        int top = filas - 1, bottom = 0;

        // Rotación inicial (por ejemplo, -90° en X y 90° en Y)
        Quaternion currentRot = Quaternion.Euler(-90f, 90f, 0f);

        // Simulamos el recorrido desde la casilla 0 hasta la anterior a la posición deseada.
        // Se usan i < posicion, ya que queremos aplicar luego la actualización si la casilla destino es esquina.
        for (int i = 0; i < posicion; i++)
        {
            // Si el recorrido llega a una esquina, actualizamos la rotación de forma inmediata.
            if (tableroEsquina.Contains(i))
            {
                if (dx == 1 && dz == 0)
                {
                    // En movimiento a la derecha, la esquina indica que se gira hacia ARRIBA.
                    currentRot = Quaternion.Euler(-90f, 0f, 0f);
                }
                else if (dx == 0 && dz == 1)
                {
                    // En movimiento hacia arriba, la esquina indica giro hacia IZQUIERDA.
                    currentRot = Quaternion.Euler(-90f, -90f, 0f);
                }
                else if (dx == -1 && dz == 0)
                {
                    // En movimiento a la izquierda, la esquina indica giro hacia ABAJO.
                    currentRot = Quaternion.Euler(-90f, 180f, 0f);
                }
                else if (dx == 0 && dz == -1)
                {
                    // En movimiento hacia abajo, la esquina indica giro hacia DERECHA.
                    currentRot = Quaternion.Euler(-90f, 90f, 0f);
                }
                Debug.Log($"[Esquina] En casilla {i} se actualiza la rotación a: {currentRot}");
            }

            // Avanzamos la posición
            x += dx;
            z += dz;

            // Actualizamos dirección en función de los límites
            if (dx == 1 && x >= right)
            {
                dx = 0; dz = 1;  // Giro hacia arriba
                right--;
            }
            else if (dz == 1 && z >= top)
            {
                dx = -1; dz = 0; // Giro hacia la izquierda
                top--;
            }
            else if (dx == -1 && x <= left)
            {
                dx = 0; dz = -1; // Giro hacia abajo
                left++;
            }
            else if (dz == -1 && z <= bottom)
            {
                dx = 1; dz = 0;  // Giro hacia la derecha
                bottom++;
            }
        }

        // Si la casilla destino es una esquina, queremos que la ficha tenga ya la nueva rotación.
        // Esto se aplica para la esquina "incluso" si la posición coincide con uno de los índices.
        if (tableroEsquina.Contains(posicion))
        {
            if (dx == 1 && dz == 0)
                currentRot = Quaternion.Euler(-90f, 0f, 0f);
            else if (dx == 0 && dz == 1)
                currentRot = Quaternion.Euler(-90f, -90f, 0f);
            else if (dx == -1 && dz == 0)
                currentRot = Quaternion.Euler(-90f, 180f, 0f);
            else if (dx == 0 && dz == -1)
                currentRot = Quaternion.Euler(-90f, 90f, 0f);
            Debug.Log($"[Esquina destino] En la casilla {posicion} se fuerza la actualización de la rotación a: {currentRot}");
        }

        Debug.Log("Rotación final calculada: " + currentRot);
        return currentRot;
    }

    //Dado----------------------------------------------------------------------------
    public void tirarDado() {
        if (!block)
        {
            if (PopupDado.activeSelf)
            {
                block = true;

                FisicasDado();
                StartCoroutine(EsperarPararDado());
                
            }
            else {
                fichas[jugadores[turnos[jugadorActual]].Ficha].transform.Find("CameraFicha").gameObject.SetActive(false);
                
                blockBotonVista = true;
                PopupDado.SetActive(true);
                if (Boton3D.activeSelf) popupTablero2D.SetActive(false);
            }

        }

    }

    void DesactivarCamaras() {


        foreach (GameObject ficha in fichas.Values)
        {
            foreach (Camera camera in ficha.GetComponentsInChildren<Camera>()) {
                if (camera.name.Equals("CameraFicha"))
                {
                    camera.gameObject.SetActive(false);

                }
            }

            
        }
    }

    public void CambiarVistaTablero()
    {
        if (!blockBotonVista) {
            if (popupTablero2D.activeSelf)
            {
                TurnoCamara(fichas[jugadores[turnos[jugadorActual]].Ficha]);

                Boton2D.SetActive(true);
                popupTablero2D.SetActive(false);
                Boton3D.SetActive(false);
                noCamera.SetActive(false);

            }
            else
            {
                DesactivarCamaras();

                Boton2D.SetActive(false);
                popupTablero2D.SetActive(true);
                Boton3D.SetActive(true);
                noCamera.SetActive(true);

            }
        }
        

    }

    void FisicasDado() {

        rb.useGravity = true;
        // Reiniciar velocidad antes de lanzar
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Aplicar una fuerza en una dirección aleatoria
        Vector3 fuerza = new Vector3((int)UnityEngine.Random.Range(-5f, 5f), (int)UnityEngine.Random.Range(10f, 15f), (int)UnityEngine.Random.Range(-5f, 5f));
        rb.AddForce(fuerza, ForceMode.Impulse);

        // Aplicar un torque aleatorio para que ruede
        Vector3 torque = new Vector3((int)UnityEngine.Random.Range(-10f, 10f), (int)UnityEngine.Random.Range(-10f, 10f), (int)UnityEngine.Random.Range(-10f, 10f));
        rb.AddTorque(torque, ForceMode.Impulse);
    }

   

    void ResetearDado()
    {
        Rigidbody rb = dado.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false; // Desactiva la gravedad temporalmente

        dado.transform.position = posicionInicial;
        dado.transform.rotation = rotacionInicial;
    }


    IEnumerator EsperarPararDado()
    {
        yield return new WaitForFixedUpdate(); // Espera que Unity actualice físicas antes de evaluar

        // Espera hasta que el dado esté completamente quieto
        while (rb.velocity.magnitude >= 0.01f || rb.angularVelocity.magnitude >= 0.01f) 
        {
            yield return null; // Espera al siguiente frame
        }

        // Ahora el dado está quieto, acumulamos el tiempo
        tiempoQuieto = 0f;

        while (tiempoQuieto < tiempoNecesario)
        {
            tiempoQuieto += Time.deltaTime;
            yield return null; // Sigue esperando
        }

        // Cuando ha estado quieto el tiempo necesario, ejecutamos la lógica
        if (tiempoQuieto >= tiempoNecesario)
        {
            DetectarCaraSuperiorDef();
         
            if (Boton3D.activeSelf) popupTablero2D.SetActive(true);
        
            
            PopupDado.SetActive(false);
            
            TurnoCamara(fichas[jugadores[turnos[jugadorActual]].Ficha]);
            
            
            yield return new WaitForSeconds(0.5f);
            
            MoverAFicha(NumeroActual);

            

            ResetearDado(); // Resetea el dado después del tiempo necesario
            tiempoQuieto = 0f; // Resetea el contador para la próxima tirada

        }
    }


    void DetectarCaraSuperiorDef() {

        for (int i = 0; i < caras.Length; i++)
        {
            if (caras[i].TocaSuelo)
            {
                NumeroActual = 7 - caras[i].Numero;
               // Debug.Log("El resultado del dado es: " + NumeroActual);
                

            }
        }
        
    }
  

    public void MoverAFicha(int resultadoDado)
    {
        StartCoroutine(MoverFicha2DPasoAPaso(jugadorActual, resultadoDado));
        StartCoroutine(MoverFichaPasoAPaso(jugadorActual, resultadoDado));
        


    }

    IEnumerator MoverFichaPasoAPaso2(int jugador, int resultadoDado)
    {
        int nuevaPosicion = jugadores[turnos[jugador]].Posicion + resultadoDado;

        if (posicionesTablero.ContainsKey(nuevaPosicion))
        {
            while (jugadores[turnos[jugador]].Posicion < nuevaPosicion)
            {
                int siguienteCasilla = jugadores[turnos[jugador]].Posicion + 1;

                //VerificarSuperposicion(siguienteCasilla); // Hacer invisible si hay ficha en la siguiente casilla

                if (!posicionesTablero.ContainsKey(siguienteCasilla)) break; // Detener si la siguiente casilla no existe

                Vector3 posicionInicial = casillas.transform.TransformPoint(posicionesTablero[jugadores[turnos[jugador]].Posicion]);
                Vector3 posicionDestino = casillas.transform.TransformPoint(posicionesTablero[siguienteCasilla]);

                float t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime * velocidadMovimiento;

                    float salto = Mathf.Sin(t * Mathf.PI) * alturaSalto;
                    fichas[jugadores[turnos[jugadorActual]].Ficha].transform.position = Vector3.Lerp(posicionInicial, posicionDestino, t) + Vector3.up * salto;

                    yield return null;
                }

                jugadores[turnos[jugador]].Posicion++;

                //VerificarSuperposicion(jugadores[turnos[jugador]].Posicion); // Volver a hacer visible la ficha cuando la supera

                yield return new WaitForSeconds(pausaEntreCasillas);

                // Si la casilla es una esquina, rotar la cámara
                if (tableroEsquina.Contains(jugadores[turnos[jugadorActual]].Posicion))
                {
                    StartCoroutine(RotarCamaraSuavemente(anguloRotacion));
                }
            }

            MostrarPopupGanador();
            jugadorActual = siguienteJugador(jugador);

            MoverFicha2DAlFinal(fichas2D[turnos[jugadorActual]]);
            MoverFicha2DAlFinal(fichasBTN2D[turnos[jugadorActual]]);
            MarcoJugador();
        }

        MostrarCarta(RamdomMinijuego());
    }

    IEnumerator MoverFichaPasoAPaso(int jugador, int resultadoDado)
    {
        int nuevaPosicion = jugadores[turnos[jugador]].Posicion + resultadoDado;

        if (posicionesTablero.ContainsKey(nuevaPosicion))
        {
            while (jugadores[turnos[jugador]].Posicion < nuevaPosicion)
            {
                int siguienteCasilla = jugadores[turnos[jugador]].Posicion + 1;

                // Usamos el mismo 'jugador' para ambas llamadas a VerificarSuperposicion:
                //VerificarSuperposicion(siguienteCasilla, jugador);

                if (!posicionesTablero.ContainsKey(siguienteCasilla))
                    break;

                Vector3 posicionInicial = casillas.transform.TransformPoint(posicionesTablero[jugadores[turnos[jugador]].Posicion]);
                Vector3 posicionDestino = casillas.transform.TransformPoint(posicionesTablero[siguienteCasilla]);

                float t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime * velocidadMovimiento;

                    float salto = Mathf.Sin(t * Mathf.PI) * alturaSalto;
                    fichas[jugadores[turnos[jugador]].Ficha].transform.position =
                        Vector3.Lerp(posicionInicial, posicionDestino, t) + Vector3.up * salto;

                    yield return null;
                }

                jugadores[turnos[jugador]].Posicion++;

                // Verificamos nuevamente con la posición ya actualizada del mismo jugador
               // VerificarSuperposicion(jugadores[turnos[jugador]].Posicion, jugador);

                yield return new WaitForSeconds(pausaEntreCasillas);

                if (tableroEsquina.Contains(jugadores[turnos[jugador]].Posicion))
                {
                    StartCoroutine(RotarCamaraSuavemente(anguloRotacion));
                }
            }

            MostrarPopupGanador();
            jugadorActual = siguienteJugador(jugador);

            MoverFicha2DAlFinal(fichas2D[turnos[jugadorActual]]);
            MoverFicha2DAlFinal(fichasBTN2D[turnos[jugadorActual]]);
            MarcoJugador();
        }

        MostrarCarta(RamdomMinijuego());
    }

    IEnumerator RotarCamaraSuavemente(float anguloRot)
    {
        Transform ficha = fichas.GetValueOrDefault(jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugadorActual)).Ficha).transform;
        Quaternion rotacionInicial = ficha.transform.localRotation;

        Quaternion rotacionFinal;
        if (ficha.name.Contains("FichaCorazon"))
        {
            rotacionFinal = rotacionInicial * Quaternion.Euler(0, 0, anguloRot); // Solo rotación en Z
        }
        else
        {
            rotacionFinal = rotacionInicial * Quaternion.Euler(anguloRot, 0, 0); // Solo rotación en X
        }
        float tiempo = 0;
        while (tiempo < 1)
        {
            tiempo += Time.deltaTime * 2f; // Velocidad de rotación ajustable
            ficha.transform.localRotation = Quaternion.Lerp(rotacionInicial, rotacionFinal, tiempo);
            yield return null;
        }
    }

    //Ficha tablero 2D

    void AñadirFicha2D()
    {

        foreach (Jugador ju in jugBBDD)
        {
            ColorUtility.TryParseHtmlString(ju.ColorIcono, out Color color);

            ////Tablero 2D
            GameObject fichaPrefab2D = Instantiate(prefabsFicha2D);
            

            Image ColorBoton2D = fichaPrefab2D.gameObject.GetComponentInChildren<Image>();
            ColorUtility.TryParseHtmlString(ju.ColorIcono, out Color color2D);
            ColorBoton2D.color = color;

            Image ImagenIcono2D = fichaPrefab2D.transform.Find("ImageJugador").gameObject.GetComponentInChildren<Image>();
            CambiarImagenJugador(ImagenIcono2D.gameObject, ju.RutaImagen);


            fichaPrefab2D.transform.SetParent(contenedorFichas2D.transform, false);
            
            //Debug.Log("posicionFicha: "+ casillas2D[110].GetComponent<RectTransform>().anchoredPosition);
            fichas2D.Add(ju.Nombre, fichaPrefab2D);


            //Boton Tablero 2D

            GameObject fichaBTNCasilla2D = Instantiate(prefabsBotonFicha2D);
            
            Image ColorBotonBTN2D = fichaBTNCasilla2D.gameObject.GetComponentInChildren<Image>();
            ColorBotonBTN2D.color = color;

            Image ImagenBTNIcono2D = fichaBTNCasilla2D.transform.Find("ImageJugador").gameObject.GetComponentInChildren<Image>();
            CambiarImagenJugador(ImagenBTNIcono2D.gameObject, ju.RutaImagen);

            fichaBTNCasilla2D.transform.SetParent(contenedorBotonFichas2D.transform, false);
            
            //Debug.Log("posicionFichaBoton: " + botonCasillas2D[110].GetComponent<RectTransform>().anchoredPosition);
            fichasBTN2D.Add(ju.Nombre, fichaBTNCasilla2D);

            fichaPrefab2D.SetActive(false); 
            fichaBTNCasilla2D.SetActive(false); 

               StartCoroutine(ObtenerPosicionCasilla(casillas2D[CalcularPosicionEnGrid(ju.Posicion)], botonCasillas2D[CalcularPosicionEnGrid(ju.Posicion)], (posicion,posicion2) =>
               {
          
                   fichaPrefab2D.GetComponent<RectTransform>().anchoredPosition = posicion;
                   fichaBTNCasilla2D.GetComponent<RectTransform>().anchoredPosition = posicion2;
                   
 
                   fichaPrefab2D.SetActive(true);
                   fichaBTNCasilla2D.SetActive(true);
               }));

        }


    }

    IEnumerator ObtenerPosicionCasilla(GameObject casilla, GameObject casillaBoton, System.Action<Vector2, Vector2> callback)
    {
        yield return new WaitForEndOfFrame(); // Espera hasta que Unity actualice el Grid
        Vector2 posicionReal = casilla.GetComponent<RectTransform>().anchoredPosition;
        Vector2 posicionRealBoton = casillaBoton.GetComponent<RectTransform>().anchoredPosition;
        callback?.Invoke(posicionReal, posicionRealBoton); // Llama al callback con la posición
    }

    void MoverFicha2DAlFinal(GameObject ficha)
    {
        ficha.transform.SetSiblingIndex(ficha.transform.parent.childCount - 1);
    }

    IEnumerator MoverFicha2DPasoAPaso(int jugador, int resultadoDado)
    {
        int nuevaPosicion = jugadores[turnos[jugador]].Posicion + resultadoDado;

        if (casillas2D.ContainsKey(nuevaPosicion) && botonCasillas2D.ContainsKey(nuevaPosicion))
        {
            while (jugadores[turnos[jugador]].Posicion < nuevaPosicion)
            {
                int siguienteCasilla = jugadores[turnos[jugador]].Posicion + 1;
                

                if (!casillas2D.ContainsKey(siguienteCasilla)) break;

                // Obtener posiciones como Vector2 en el Canvas 2D
                Vector2 posicionInicial = casillas2D[mapaEspiralAGrid[jugadores[turnos[jugador]].Posicion]].GetComponent<RectTransform>().anchoredPosition;
                Vector2 posicionInicial2 = botonCasillas2D[mapaEspiralAGrid[jugadores[turnos[jugador]].Posicion]].GetComponent<RectTransform>().anchoredPosition;

                Vector2 posicionDestino = casillas2D[mapaEspiralAGrid[siguienteCasilla]].GetComponent<RectTransform>().anchoredPosition;
                Vector2 posicionDestino2 = botonCasillas2D[mapaEspiralAGrid[siguienteCasilla]].GetComponent<RectTransform>().anchoredPosition;

                float t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime * velocidadMovimiento;

                    // Mover suavemente la ficha en el tablero 2D sin interferencia del Layout
                    fichas2D[turnos[jugadorActual]].GetComponent<RectTransform>().anchoredPosition =
                        Vector2.Lerp(posicionInicial, posicionDestino, t);

                    fichasBTN2D[turnos[jugadorActual]].GetComponent<RectTransform>().anchoredPosition =
                        Vector2.Lerp(posicionInicial2, posicionDestino2, t);

                    yield return null;
                }


                
                yield return new WaitForSeconds(pausaEntreCasillas);
            }
        }
    }


    int siguienteJugador(int jugadorAct) {

        
        jugadorAct += 1;
        
        if (jugadorAct> jugadores.Count) {
            //jugadorActual = 1;
            jugadorAct = 1;
        }
        


        return jugadorAct;
    }

    void MarcoJugador() {

        //fichas.GetValueOrDefault(jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugadorActual)).Ficha).GetComponent<Image>().enabled=true;
            foreach (KeyValuePair<int,GameObject> ico in iconos)
            {

                if (ico.Key != jugadorActual)
                {
                    ico.Value.GetComponent<Image>().enabled = false;

                }
                else {
                    ico.Value.GetComponent<Image>().enabled = true;
                }
            }
    }



    void AsignarTextura(GameObject casilla, int indice, Texture2D fullImage, int gridSize)
    {
        //Debug.Log("indice: "+ indice+ " (filas*columnas)-1: "+ ((filas * columnas) - 1));
        if (indice == 0 || indice== (filas * columnas) - 1) {
            AsignarNumerosCasillas(indice, null);
        } else {
            // Calcula las dimensiones de la imagen dividida
            int gridSizeX = gridSize;
            int gridSizeY = gridSize;

            float posicionX = posicionesTablero[indice].x / 5.1f; // Coordenada X en la cuadrícula
            float posicionY = posicionesTablero[indice].z / 5.1f; // Coordenada Y en la cuadrícula

            float cellWidth = fullImage.width / gridSizeX;
            float cellHeight = fullImage.height / gridSizeY;

            //Debug.Log("indice: " + indice + " posicionX : " + posicionX + " posicionY: " + posicionY + "Posiciones : x:" + posicionesTablero[indice].x + " z:" + posicionesTablero[indice].z);

            // Obtén los píxeles correspondientes a esta sección
            Color[] pixels = fullImage.GetPixels(
                (int)(posicionX * cellWidth),
                (int)(posicionY * cellHeight),
                (int)cellWidth,
                (int)cellHeight);


            Texture2D cellTexture2D = new Texture2D((int)cellWidth, (int)cellHeight);
            cellTexture2D.SetPixels(pixels);
            cellTexture2D.Apply();


            pixels = RotarPixels180(pixels, (int)cellWidth, (int)cellHeight);

            // Crea una nueva textura para este fragmento
            Texture2D cellTexture = new Texture2D((int)cellWidth, (int)cellHeight);
            cellTexture.SetPixels(pixels);
            cellTexture.Apply();

            // Asegúrate de que el material inicial sea compatible con URP
            Material topMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit")); // Shader compatible con URP
            topMaterial.mainTexture = cellTexture;

            // Asigna el material al cubo
            MeshRenderer renderer = casilla.GetComponent<MeshRenderer>();
            renderer.material = topMaterial; // Aplica el material a todo el


            
            AsignarNumerosCasillas(indice, cellTexture2D);
        }


    }
 
    void AsignarNumerosCasillas(int posicionCasilla,Texture2D cellTexture)
    {
        //Debug.Log($"filas * columnas: {filas * columnas} - NumeroCasilla {posicionCasilla}");
        if (posicionCasilla==0 || posicionCasilla == (filas * columnas)-1) {
            GameObject salida2D = Instantiate(prefabsCasilla2DSalida);
            GameObject salidaBTN2D = Instantiate(prefabsCasilla2DSalida);

            if (posicionCasilla == 0) 
            {
                casillas2D.Add(CalcularPosicionEnGrid(posicionCasilla), salida2D);
                botonCasillas2D.Add(CalcularPosicionEnGrid(posicionCasilla), salidaBTN2D);
            }

            if (posicionCasilla == (filas * columnas) - 1)
            {
                casillas2D.Add(CalcularPosicionEnGrid(posicionCasilla), salida2D);
                botonCasillas2D.Add(CalcularPosicionEnGrid(posicionCasilla), salidaBTN2D); 
            }

        } else {
            GameObject casilla2D = Instantiate(prefabsCasilla2D);
            GameObject casillaBTN2D = Instantiate(casillaBotonTablero2D);

            casilla2D.GetComponentInChildren<TMP_Text>().text = posicionCasilla.ToString();
            casillaBTN2D.GetComponentInChildren<TMP_Text>().text = posicionCasilla.ToString();

            ColorNumeros(casilla2D, posicionCasilla);
            ColorNumeros(casillaBTN2D, posicionCasilla);

            // Crear un sprite directamente desde la textura generada
            Sprite casillaSprite = Sprite.Create(cellTexture,
                new Rect(0, 0, cellTexture.width, cellTexture.height),
                new Vector2(0.5f, 0.5f));

            // Asignarlo a la casilla en el tablero 2D
            Image casillaImage = casilla2D.GetComponent<Image>();
            casillaImage.sprite = casillaSprite;
            Image casillaBTNImage = casillaBTN2D.GetComponent<Image>();
            casillaBTNImage.sprite = casillaSprite;

            casillas2D.Add(CalcularPosicionEnGrid(posicionCasilla), casilla2D);
            botonCasillas2D.Add(CalcularPosicionEnGrid(posicionCasilla), casillaBTN2D);

        }
        
    }

    void ColorNumeros(GameObject casilla2D,int i) {

        float[] porcentaje = null;

        if (filas * columnas > 9 * 9)
        {
            porcentaje = new float[] { 0.33f, 0.59f, 0.79f, 0.92f };
        }
        else if (filas * columnas < 9 * 9)
        {
            porcentaje = new float[] { 0.47f, 0.8f, 0.99f, 0.0f };
        }
        else
        {
            porcentaje = new float[] { 0.39f, 0.69f, 0.88f, 0.0f };
        }
        
        if (i < filas * columnas * porcentaje[0])
        {
            casilla2D.GetComponentInChildren<TMP_Text>().color = Color.cyan;

        }
        else if (i < filas * columnas * porcentaje[1])
        {
            casilla2D.GetComponentInChildren<TMP_Text>().color = Color.blue;

        }

        else if (i < filas * columnas * porcentaje[2])
        {
            casilla2D.GetComponentInChildren<TMP_Text>().color = Color.yellow;

        }
        else if (i < filas * columnas * porcentaje[3])
        {
            casilla2D.GetComponentInChildren<TMP_Text>().color = Color.red;

        }
        else if (i == (filas * columnas) - 1 || i == 0)
        {

        }
        else
        {
            casilla2D.GetComponentInChildren<TMP_Text>().color = Color.black;

        }
        

    }

   
    void GenerarMapaEspiralAGrid()
    {
        int x = 0, z = filas - 1;
        int dx = 1, dz = 0;

        int leftBound = 0, rightBound = columnas - 1;
        int topBound = 0, bottomBound = filas - 2;


        for (int i = 0; i < filas * columnas; i++)
        {
            int posicionGrid = ((z * columnas) + x);


            if (!mapaEspiralAGrid.ContainsValue(posicionGrid)) // Evitar valores duplicados
            {
                mapaEspiralAGrid.Add(i, posicionGrid);
            }
            else
            {
                Debug.LogError($"¡Posición duplicada en el Grid! Índice {i} ya tiene {posicionGrid}");
            }

            // Avanzar a la siguiente posición
            x += dx;
            z += dz;


            if (dx == 1 && x >= rightBound) 
            {
                dx = 0; dz = -1; rightBound--;

            }
            else if (dz == 1 && z >= bottomBound)
            {
                dx = 1; dz = 0; bottomBound--;

            }
            else if (dx == -1 && x <= leftBound) 
            {
                dx = 0; dz = 1; leftBound++;

            }
            else if (dz == -1 && z <= topBound)
            {
                dx = -1; dz = 0; topBound++; 

            }

        }

        //Debug.Log(mapaEspiralAGrid.Count);
    }


    int CalcularPosicionEnGrid(int indiceEspiral)
    {
        return mapaEspiralAGrid[indiceEspiral]; // Devuelve la posición correcta en el Grid
    }

    void ConfigurarGridLayout()
    {
        GridLayoutGroup grid = contenedorCasilla2D.GetComponent<GridLayoutGroup>();
        GridLayoutGroup gridBTN = contenedorBotonTablero2D.GetComponent<GridLayoutGroup>();
        if (filas * columnas > 9 * 9) {
            
            grid.cellSize = new Vector2(91, 91); // Tamaño de cada celda
            grid.constraintCount = 11; // Número máximo de columnas o filas, según el constraint

            gridBTN.cellSize = new Vector2(31.81f, 31.81f); // Tamaño de cada celda
            gridBTN.constraintCount = 11; // Número máximo de columnas o filas, según el constraint

        } else if (filas * columnas < 9 * 9) {
            grid.cellSize = new Vector2(142.86f, 142.86f); // Tamaño de cada celda
            grid.constraintCount = 7; // Número máximo de columnas o filas, según el constraint

            gridBTN.cellSize = new Vector2(50, 50); // Tamaño de cada celda
            gridBTN.constraintCount = 7; // Número máximo de columnas o filas, según el constraint
        } else {
            grid.cellSize = new Vector2(111.12f, 111.12f); // Tamaño de cada celda
            grid.constraintCount = 9; // Número máximo de columnas o filas, según el constraint

            gridBTN.cellSize = new Vector2(38.89f, 38.89f); // Tamaño de cada celda
            gridBTN.constraintCount = 9; // Número máximo de columnas o filas, según el constraint
        }
    }
    void GenerarTablero2D() {

        ConfigurarGridLayout();

        foreach (int clave in casillas2D.Keys.OrderBy(k => k))
        {
            casillas2D[clave].transform.SetParent(contenedorCasilla2D.transform, false);
            Vector2 posicionInicial = casillas2D[clave].GetComponent<RectTransform>().anchoredPosition;

            botonCasillas2D[clave].transform.SetParent(contenedorBotonTablero2D.transform, false);
        }
    }


    void CambiarImagen(int indice, Texture2D fullImage, int gridSize) {
        // Calcula las dimensiones de la imagen dividida
        int gridSizeX = gridSize;
        int gridSizeY = gridSize;

        float posicionX = posicionesTablero[contador].x / 5.1f; // Coordenada X en la cuadrícula
        float posicionY = posicionesTablero[contador].z / 5.1f; // Coordenada Y en la cuadrícula

        float cellWidth = fullImage.width / gridSizeX;
        float cellHeight = fullImage.height / gridSizeY;

        // Obtén los píxeles correspondientes a esta sección
        Color[] pixels = fullImage.GetPixels(
            (int)(posicionX * cellWidth),
            (int)(posicionY * cellHeight),
            (int)cellWidth,
            (int)cellHeight);

        

        // Crea una nueva textura para este fragmento
        Texture2D cellTexture = new Texture2D((int)cellWidth, (int)cellHeight);
        cellTexture.SetPixels(pixels);
        cellTexture.Apply();

        //Debug.Log("indice: " + indice + " posicionX : " + posicionX + " posicionY: " + posicionY+ "Posiciones : x:" + posicionesTablero[contador].x+" z:" + posicionesTablero[contador].z);
        // Convierte la textura en un sprite
        Sprite fragmentSprite = Sprite.Create(cellTexture, new Rect(0, 0, cellWidth, cellHeight), new Vector2(0.5f, 0.5f));

        // Asigna el sprite al componente Image en el Canvas
        imagenCanvas.sprite = fragmentSprite;

        // Asegúrate de que el material inicial sea compatible con URP
        Material topMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit")); // Shader compatible con URP
        topMaterial.mainTexture = cellTexture;

    }


    IEnumerator CambiarImagenFicha(Transform prefab, string path)
    {
        

        Texture2D imagenFicha = LoadTexture(path);

        while (imagenFicha == null)
        {
            yield return null;
        }

        //Debug.Log("Imagen cargada correctamente.");

        Texture2D cellTexture = new Texture2D(imagenFicha.width, imagenFicha.height);
        cellTexture.SetPixels(imagenFicha.GetPixels());
        cellTexture.Apply();

        Material topMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        topMaterial.mainTexture = cellTexture;

        MeshRenderer renderer = prefab.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = topMaterial;
            //Debug.Log("Material aplicado correctamente.");
        }
        else
        {
            Debug.LogError("No se encontró MeshRenderer en el objeto.");
        }
    }

    Color[] RotarPixels180(Color[] pixels, int width, int height)
    {
        Color[] rotatedPixels = new Color[pixels.Length];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Nuevo índice: invertir horizontal y verticalmente
                int newIndex = (height - 1 - y) * width + (width - 1 - x);
                int oldIndex = y * width + x;
                rotatedPixels[newIndex] = pixels[oldIndex];
            }
        }
        return rotatedPixels;
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

    //Pruebas------------------------------------------------------------------------
    //Dicciopinta****************************************************************
    void ActualizarDibujoWin() {

        
        if (Input.GetMouseButtonDown(0) && contadorClicks < 1 && puedeDibujarTiempo) // Solo inicia si es un nuevo clic
        {
            puedeDibujar = true;
            contadorClicks++;
            //Debug.Log("Empieza a dibujar: puedeDibujar = " + puedeDibujar);
        }
        if (Input.GetMouseButtonUp(0) && contadorClicks > 0 && puedeDibujarTiempo) // Detiene el dibujo cuando se suelta
        {
            
            puedeDibujar = false;
            //ultimaPosicion = Vector2.zero; // Restablecer la última posición para evitar líneas inesperadas
            //Debug.Log("Termina dibujo: puedeDibujar = " + puedeDibujar);
        }


        if (puedeDibujar && puedeDibujarTiempo) // Dibuja solo si el botón sigue presionado
        {
            Debug.Log("Dentro puedeDibujar = " + puedeDibujar);
            Dibujar(Input.mousePosition);
        }
    }

    void ActualizarDibujoAndroid()
    {
        if (Input.touchCount > 0) // Hay al menos un toque activo
        {
            Touch toque = Input.GetTouch(0); // Primer toque en la pantalla

            if (toque.phase == TouchPhase.Began) // Empieza a dibujar
            {
                puedeDibujar = true;
            }

            if (toque.phase == TouchPhase.Ended) // Si levanta el dedo, ya no puede dibujar
            {
                puedeDibujar = false;
            }

            if (puedeDibujar)
            {
                Dibujar(toque.position); // Pasamos la posición del dedo
            }
        }

    }

    Vector2 ultimaPosicion; // Guarda la última posición del toque/mouse

    void Dibujar(Vector2 posicion)
    {
        if (lienzoDibujo == null)
        {
            Debug.LogError("Error: lienzoDibujo es null. Asegúrate de asignarlo desde el Inspector.");
            return;
        }

        Texture2D textura = lienzoDibujo.texture as Texture2D;
        if (textura == null)
        {
            //Debug.LogError("Error: la textura no es un Texture2D. Asignando una nueva...");
            //textura = new Texture2D(512, 512, TextureFormat.RGBA32, false);
            textura = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
            lienzoDibujo.texture = textura;
        }

        RectTransform rect = lienzoDibujo.rectTransform;
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, posicion, null, out localPos);

        float x = Mathf.Clamp01((localPos.x + rect.rect.width / 2) / rect.rect.width);
        float y = Mathf.Clamp01((localPos.y + rect.rect.height / 2) / rect.rect.height);

        int px = (int)(x * textura.width);
        int py = (int)(y * textura.height);

        // Si es la primera vez que se toca, solo dibuja un punto
        if (ultimaPosicion == Vector2.zero)
        {
            textura.SetPixel(px, py, Color.black);
        }
        else
        {
            // Dibujar una línea entre la última posición y la actual
            DibujarLineaSuaveGruesa(textura, (int)ultimaPosicion.x, (int)ultimaPosicion.y, px, py, Color.black,5);
        }

        textura.Apply();
        ultimaPosicion = new Vector2(px, py); // Guardar la posición actual como la última
    }

    void DibujarLineaSuaveGruesa(Texture2D textura, int x0, int y0, int x1, int y1, Color color, int grosor)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            for (int i = -grosor; i <= grosor; i++)
            {
                for (int j = -grosor; j <= grosor; j++)
                {
                    float distancia = Mathf.Sqrt(i * i + j * j); // Distancia del píxel al centro
                    float intensidad = Mathf.Clamp01(1.0f - (distancia / grosor)); // Suavizado con interpolación
                    Color colorMezclado = Color.Lerp(textura.GetPixel(x0 + i, y0 + j), color, intensidad);
                    textura.SetPixel(x0 + i, y0 + j, colorMezclado);
                }
            }

            if (x0 == x1 && y0 == y1) break;
            int e2 = err * 2;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
        textura.Apply();
    }


    void CrearLienzo() {
        int ancho = (int)lienzoDibujo.rectTransform.rect.width;
        int alto = (int)lienzoDibujo.rectTransform.rect.height;

        RenderTexture texturaDibujo = new RenderTexture(ancho, alto, 16);
        texturaDibujo.Create();
        lienzoDibujo.texture = texturaDibujo;

        //Debug.Log($"RenderTexture creado con tamaño: {ancho}x{alto}");

    }

    IEnumerator CuentaAtras(int segundos)
    {
        while (segundos >= 0)
        {
            tiempoContador.text ="Tiempo restante: " + segundos+"s";
            yield return new WaitForSeconds(1); // Espera 1 segundo
            segundos--;
        }
        puedeDibujarTiempo = false;
        Debug.Log("¡Tiempo terminado!");
    }

    public void AccionGo() {
        puedeDibujarTiempo = true;
        StartCoroutine(CuentaAtras(10)); // Inicia cuenta atrás de 10 segundos
        
    }


    //BBDD---------------------------------------------------------------------------

    void CargarJugadores() {

        int cont = 1;
        jugBBDD = ClaseManagerBBDD.Instance.SelectAll<Jugador>();
        if (jugBBDD != null){
            foreach (Jugador ju in jugBBDD)
            {
                Jugador3D jugador3D = new Jugador3D(ju.Nombre,ju.ColorIcono, ju.RutaImagen, ju.Ficha, ju.Posicion);
                jugadores.Add(jugador3D.Nombre, jugador3D);
                jugadoresIniciales.Add(jugador3D.Nombre, jugador3D);
                AñadirIcono(ju.Nombre, ju.RutaImagen, ju.ColorIcono,cont);

                cont++;

            }
        }
        //ClaseManagerBBDD.Instance.SelectAll<Jugador>();
    }


    void CargarColores() {

        coloresJu = ClaseManagerBBDD.Instance.SelectAll<ColorJugadores>();
    }

    IEnumerator MovimientoHada2()
    {
        Camera camaraComponente = camaraPopup.GetComponent<Camera>();
        if (camaraComponente != null)
        {
            //Debug.Log($"Cámara activa: {camaraComponente.enabled}");
           //Debug.Log($"RenderTexture asignada: {camaraComponente.targetTexture}");
        }
        else
        {
            //Debug.LogError("Error: El GameObject no tiene un componente Camera.");
        }


        // Posición y rotación relativa de la cámara de referencia
        Vector3 destinoFinal = camaraReferencia.transform.localPosition;
        Quaternion rotacionFinal = camaraReferencia.transform.localRotation;

        // Posiciones relativas dentro del padre
        Vector3 inicio = posicionesTablero[((filas * columnas) - 1)] + new Vector3(0, 5.1f, 0);
        Vector3 alturaFinal = posicionesTablero[((filas * columnas) - 1)] + new Vector3(0, 10f, 0);

        float suavizado = 1.5f;
        Vector3 velocidad = Vector3.zero;

        // Vista inicial girada 180° para que el hada mire en la dirección opuesta
        Quaternion rotInicial = Quaternion.Euler(90f, 180f, 0f);
        Quaternion rotFinal = Quaternion.Euler(40f, 180f, 0f);

        camaraPopup.transform.localRotation = rotInicial;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * suavizado;

            //  Asegurar que la interpolación usa `localPosition`
            camaraPopup.transform.localPosition = Vector3.Lerp(inicio, alturaFinal, t);

            //  Levantar la mirada progresivamente mientras sube
            camaraPopup.transform.localRotation = Quaternion.Lerp(rotInicial, rotFinal, t);

            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        //  Movimiento fluido de izquierda a derecha con el mismo sistema de coordenadas
        Quaternion rotIzquierda = Quaternion.Euler(40f, 130f, 0f);
        Quaternion rotDerecha = Quaternion.Euler(40f, 230f, 0f);

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * suavizado;
            camaraPopup.transform.localRotation = Quaternion.Lerp(rotFinal, rotIzquierda, t);
            yield return null;
        }

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * suavizado;
            camaraPopup.transform.localRotation = Quaternion.Lerp(rotIzquierda, rotDerecha, t);
            yield return null;
        }
        // Mantener la vista en `rotDerecha` mientras avanza a la salida
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * suavizado;
            t = Mathf.Clamp01(t);

            if (!DatosEscena.EscenaAnterior.Equals("MenuUsuarios"))
            {
                camaraPopup.transform.localPosition = Vector3.Lerp(alturaFinal, destinoFinal, t);
                camaraPopup.transform.localRotation = rotDerecha;
            }
            else
            {
                Vector3 objetivoLocal = fichas[jugadores[turnos[jugadorActual]].Ficha].transform.localPosition;
                camaraPopup.transform.localPosition = Vector3.Lerp(alturaFinal, objetivoLocal, t);
                camaraPopup.transform.LookAt(fichas[jugadores[turnos[jugadorActual]].Ficha].transform.position);
            }

            ultimaPosicion = camaraPopup.transform.localPosition; // Guardar última posición antes de la siguiente fase
            yield return null;
        }

        Camera camaraFicha = fichas[jugadores[turnos[jugadorActual]].Ficha].GetComponentInChildren<Camera>();
        //Vector3 objetivoCamara = camaraFicha.transform.TransformPoint(camaraFicha.transform.localPosition);  // Posición absoluta de la cámara de la ficha
        //Vector3 objetivoRelativo = fichas[jugadores[turnos[jugadorActual]].Ficha].transform.localPosition;

        //Quaternion rotacionObjetivo = Quaternion.LookRotation(objetivoRelativo - camaraPopup.transform.localPosition);

        // Convertir la posición local de `camaraFicha` al sistema de coordenadas del abuelo
        //Vector3 objetivoGlobal = fichas[jugadores[turnos[jugadorActual]].Ficha].transform.TransformPoint(camaraFicha.transform.localPosition);
        // Usar la orientación correcta según la escena
        //Vector3 direccionObjetivo = (objetivoGlobal - camaraPopup.transform.position).normalized;
        //Quaternion rotacionDestino = Quaternion.LookRotation(direccionObjetivo, camaraPopup.transform.up);
        //Quaternion rotacionDestino = Quaternion.LookRotation(direccionObjetivo, Vector3.up); // Asegurar eje correcto
        // Ajustar la dirección de la mirada correctamente hacia la ficha
         //Camera camaraFicha = fichas[jugadores[turnos[jugadorActual]].Ficha].GetComponentInChildren<Camera>();
        // La posición ya está en destinoFinal, ahora hacemos la transición sin retroceso
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * suavizado;
            t = Mathf.Clamp01(t);

            if (!DatosEscena.EscenaAnterior.Equals("MenuUsuarios"))
            {
                camaraPopup.transform.localPosition = Vector3.Lerp(ultimaPosicion, destinoFinal, t);
                camaraPopup.transform.localRotation = Quaternion.Lerp(rotDerecha, rotacionFinal, t);
            }
            else
            {
                // Dirección hacia la ficha
                Vector3 direccionFicha = fichas[jugadores[turnos[jugadorActual]].Ficha].transform.position - camaraPopup.transform.position;

                // Definir la dirección final hacia donde debe mirar al terminar el movimiento
                Vector3 direccionFinal = -direccionFicha; // Opuesta a la dirección inicial

                Quaternion rotacionInicial = camaraPopup.transform.localRotation;
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionFinal, Vector3.up); // Ajuste para que mire hacia donde venía
                                                                                                   // Definir correctamente `objetivoGlobal`
                Vector3 objetivoGlobal = fichas[jugadores[turnos[jugadorActual]].Ficha].transform.TransformPoint(camaraFicha.transform.localPosition);

                camaraPopup.transform.localPosition = Vector3.Lerp(ultimaPosicion, camaraPopup.transform.parent.InverseTransformPoint(objetivoGlobal), t);

                // Aplicamos Slerp para que gire progresivamente y al final mire hacia donde venía
                camaraPopup.transform.localRotation = Quaternion.Slerp(rotacionInicial, rotacionObjetivo, t);
            }

            yield return null;
        }




        popupTablero2D.gameObject.SetActive(false);
        popupPreCarga.gameObject.SetActive(false);

        MostrarPopupTurno();
    }
    IEnumerator MovimientoHada()
    {
        Camera camaraComponente = camaraPopup.GetComponent<Camera>();
        if (camaraComponente != null)
        {
            //Debug.Log($"Cámara activa: {camaraComponente.enabled}");
            //Debug.Log($"RenderTexture asignada: {camaraComponente.targetTexture}");
        }
        else
        {
            //Debug.LogError("Error: El GameObject no tiene un componente Camera.");
        }


        // Posición y rotación relativa de la cámara de referencia
        Vector3 destinoFinal = camaraReferencia.transform.localPosition;
        Quaternion rotacionFinal = camaraReferencia.transform.localRotation;

        // Posiciones relativas dentro del padre
        Vector3 inicio = posicionesTablero[((filas * columnas) - 1)] + new Vector3(0, 5.1f, 0);
        Vector3 alturaFinal = posicionesTablero[((filas * columnas) - 1)] + new Vector3(0, 10f, 0);

        float suavizado = 1.5f;
        Vector3 velocidad = Vector3.zero;

        // Vista inicial girada 180° para que el hada mire en la dirección opuesta
        Quaternion rotInicial = Quaternion.Euler(90f, 180f, 0f);
        Quaternion rotFinal = Quaternion.Euler(40f, 180f, 0f);

        camaraPopup.transform.localRotation = rotInicial;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * suavizado;

            //  Asegurar que la interpolación usa `localPosition`
            camaraPopup.transform.localPosition = Vector3.Lerp(inicio, alturaFinal, t);

            //  Levantar la mirada progresivamente mientras sube
            camaraPopup.transform.localRotation = Quaternion.Lerp(rotInicial, rotFinal, t);

            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        //  Movimiento fluido de izquierda a derecha con el mismo sistema de coordenadas
        Quaternion rotIzquierda = Quaternion.Euler(40f, 130f, 0f);
        Quaternion rotDerecha = Quaternion.Euler(40f, 230f, 0f);

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * suavizado;
            camaraPopup.transform.localRotation = Quaternion.Lerp(rotFinal, rotIzquierda, t);
            yield return null;
        }

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * suavizado;
            camaraPopup.transform.localRotation = Quaternion.Lerp(rotIzquierda, rotDerecha, t);
            yield return null;
        }
        // Mantener la vista en `rotDerecha` mientras avanza a la salida
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * suavizado;
            t = Mathf.Clamp01(t); // Asegurar que nunca avanza más allá del destino final

            // Mover hasta destinoFinal sin sobrepasarlo
            camaraPopup.transform.localPosition = Vector3.Lerp(alturaFinal, destinoFinal, t);

            // Mantener la vista fija en rotDerecha mientras avanza
            camaraPopup.transform.localRotation = rotDerecha;

            yield return null;
        }

        // La posición ya está en destinoFinal, ahora hacemos la transición sin retroceso
        t = 0;
        while (t < 1) // Reducimos a 1 para que la transición final sea más precisa
        {
            t += Time.deltaTime * suavizado;



            // Iniciamos desde la posición donde terminó el movimiento anterior
            camaraPopup.transform.localPosition = Vector3.Lerp(camaraPopup.transform.localPosition, destinoFinal, Mathf.Clamp01(t));

            // Transición natural a la rotación final
            camaraPopup.transform.localRotation = Quaternion.Lerp(rotDerecha, rotacionFinal, Mathf.Clamp01(t));


            yield return null;
        }



        popupTablero2D.gameObject.SetActive(false);
        popupPreCarga.gameObject.SetActive(false);

        MostrarPopupTurno();
    }
    //PopUp Turno

    IEnumerator ActivarPopupTurno()
    {
 

        if (jugadores[turnos[jugadorActual]].Posicion != (filas * columnas) - 1) {
            ColorUtility.TryParseHtmlString(jugadores[turnos[jugadorActual]].ColorIcono, out Color color2);

            popupTurno.transform.Find("Contenedor").GetComponent<Image>().color = color2;
            popupTurno.GetComponentInChildren<TMP_Text>().text = "¡¡Te toca\n" + jugadores[turnos[jugadorActual]].Nombre + "!!";

            popupTurno.SetActive(true);
            yield return new WaitForSeconds(2); // Espera 2 segundos
            popupTurno.SetActive(false);


            if (contAutoGuardado >= jugadores.Count) { GuardarPartida(); contAutoGuardado = 0; };
            contAutoGuardado++;
        }
    }
    void MostrarPopupTurno()
    {
        StartCoroutine(ActivarPopupTurno());
    }

    void MostrarPopupGanador()
    {
        foreach (KeyValuePair<string, Jugador3D> ju in jugadores) {
            if (ju.Value.Posicion >= (filas*columnas)-1) {
                ColorUtility.TryParseHtmlString(jugadores[turnos[jugadorActual]].ColorIcono, out Color color2);

                popupGanador.transform.Find("Contenedor").GetComponent<Image>().color = color2;
                //Debug.Log("ju.Value.Posicion: " + ju.Value.Posicion);
                popupGanador.GetComponentInChildren<TMP_Text>().text = "¡¡Has ganado\n" + jugadores[turnos[jugadorActual]].Nombre + "!!";
                popupGanador.SetActive(true);
                
            }
        }

    }

    public void ContinuarFinal()
    {
 
        popupGanador.SetActive(false);
        popupOtraPartida.SetActive(true);
    }



    public void OtraPartida() {

        GuardarPartidaInciales();



        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }


 
    public void Salir()
    {
        
        SceneManager.LoadScene("MenuTableros");

    }

    ////Minijuegos------------------------------------------------------------------------------------------------------------------------------------

    //Crear Minijuegos
    void CrearMinijuegos() {
        MinijuegoTablero miju1 = new MinijuegoTablero("Quien es mas probable", "#F700FF", "CSV/MasProbable");
        MinijuegoTablero miju2 = new MinijuegoTablero("Yo nunca", "#00FF10", "CSV/YoNunca");
        MinijuegoTablero miju3 = new MinijuegoTablero("Verdad", "#2E00FF", "CSV/verdad");
        MinijuegoTablero miju4 = new MinijuegoTablero("3 Palabras", "#1F552B", "CSV/3Palabras");
        MinijuegoTablero miju5 = new MinijuegoTablero("Reto", "#00FF81", "CSV/reto");
        MinijuegoTablero miju6 = new MinijuegoTablero("Beber", "#FFF700", "CSV/beber");
        MinijuegoTablero miju7 = new MinijuegoTablero("Quien fue", "#FFA200", "CSV/QuienFue");
        MinijuegoTablero miju8 = new MinijuegoTablero("Quien es mas probable +18", "#F700FF", "CSV/MasProbable18");
        MinijuegoTablero miju9 = new MinijuegoTablero("Yo nunca +18", "#00FF10", "CSV/YoNunca18");
        MinijuegoTablero miju10 = new MinijuegoTablero("Verdad +18", "#2E00FF", "CSV/verdad18");
        MinijuegoTablero miju11 = new MinijuegoTablero("3 Palabras +18", "#1F552B", "CSV/3Palabras18");
        MinijuegoTablero miju12 = new MinijuegoTablero("Hot", "#FF0000", "CSV/hot");

        miniJuegos.Add(miju1.nombre, miju1);
        miniJuegos.Add(miju2.nombre, miju2);
        miniJuegos.Add(miju3.nombre, miju3);
        miniJuegos.Add(miju4.nombre, miju4);
        miniJuegos.Add(miju5.nombre, miju5);
        miniJuegos.Add(miju6.nombre, miju6);
        miniJuegos.Add(miju7.nombre, miju7);
        miniJuegos.Add(miju8.nombre, miju8);
        miniJuegos.Add(miju9.nombre, miju9);
        miniJuegos.Add(miju10.nombre, miju10);
        miniJuegos.Add(miju11.nombre, miju11);
        miniJuegos.Add(miju12.nombre, miju12);

        int cont = 0;
        foreach (KeyValuePair<string, MinijuegoTablero> mj in miniJuegos) {

            
            cartas.Add(mj.Key, prefabsCartas[cont]);
            cont++;
        }

    }


    //PrefabsCartas
    Dictionary<int, string> numeroMJ = new Dictionary<int, string>();
    Dictionary<string, MinijuegoTablero> miniJuegos = new Dictionary<string, MinijuegoTablero>();
    List<ConfiguracionTablero> configMinijuegos = new List<ConfiguracionTablero>();
    List<ConfiguracionTablero> configMinijuegosIniciales = new List<ConfiguracionTablero>();
    Dictionary<string, List<string>> cartasCSV = new Dictionary<string, List<string>>();
    public GameObject[] prefabsCartas;
    Dictionary<string, GameObject> cartas = new Dictionary<string, GameObject>();
    public RectTransform contenedorCartas;
    public GameObject popupMinijuegos;
    public GameObject botonPopup;
    public TMP_Text tituloPopupMiniJuegos;

    //Mostras cartas
    string RamdomMinijuego()
    {
        return numeroMJ[Random.Range(0, numeroMJ.Count)];
    }
    private void MostrarCarta(string minijuego)
    {
        //Debug.Log("miniJuegos[minijuego].color: " + miniJuegos[minijuego].color);
        ColorUtility.TryParseHtmlString(miniJuegos[minijuego].color, out Color color1);
        botonPopup.GetComponent<Image>().color = color1;
        tituloPopupMiniJuegos.text = minijuego;

        popupMinijuegos.SetActive(true);

        
        foreach (Transform child in contenedorCartas)
            Destroy(child.gameObject);

        

        cartaActiva = Instantiate(cartas[miniJuegos[minijuego].nombre], contenedorCartas);
        RectTransform rect = cartaActiva.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(-Screen.width, 0);
            rect.localScale = Vector3.one;

            LeanTween.moveX(rect, 0f, 0.5f).setEaseOutExpo();
        }

        if (cartasCSV[miniJuegos[minijuego].nombre] != null)
        {
            string[] lineas = cartasCSV[miniJuegos[minijuego].nombre].ToArray();
            int numero= Random.Range(0, lineas.Length);
            string frase = lineas[numero].Trim();

            cartasCSV[miniJuegos[minijuego].nombre].RemoveAt(numero);

            var txt = cartaActiva.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
                txt.text = frase;
        }

    }

    IEnumerator DestruirCarta() {
        RectTransform r = cartaActiva.GetComponent<RectTransform>();
        if (r != null)
        {
            LeanTween.moveX(r, Screen.width, 0.4f).setEaseInExpo().setOnComplete(() =>
            {
                Destroy(cartaActiva);
                cartaActiva = null;
                fichas[jugadores[turnos[jugadorActual]].Ficha].SetActive(true);
                TurnoCamara(fichas[jugadores[turnos[jugadorActual]].Ficha]);
                popupMinijuegos.SetActive(false);
                block = false;
                blockBotonVista = false;
                MostrarPopupTurno();
            });
        }
        yield return null;
    }

    public void Continuar() {
        StartCoroutine(DestruirCarta());

    }
    //Cargar MiniJuegos
    void CargarCSV(string rutaRecursos, List<string> listaDestino)
    {
        TextAsset archivo = Resources.Load<TextAsset>(rutaRecursos);
        if (archivo != null)
        {
            string[] lineas = archivo.text.Split('\n');
            foreach (string linea in lineas)
            {
                string pregunta = linea.Trim();
                if (!string.IsNullOrEmpty(pregunta))
                    listaDestino.Add(pregunta);
            }
        }
        else
        {
            Debug.LogError("No se encontró el archivo en: " + rutaRecursos);
        }
    }

    void CargarConfigMinijugegos() {

        configMinijuegos = ClaseManagerBBDD.Instance.SelectAll<ConfiguracionTablero>();
        configMinijuegosIniciales = configMinijuegos;

        for (int i = 0; i < configMinijuegos.Count; i++) {
            //Debug.Log("tablero: " + configMinijuegos[i].tablero.ToUpper()+ " minijuegos: " + configMinijuegos[i].minijuegos + " minijuegos18: " + configMinijuegos[i].minijuegos18 + " tamaño: " + configMinijuegos[i].tamaño);
        }
        if (configMinijuegos[0].tamaño.Contains("grande")) { filas = 11;columnas = 11; Debug.Log("ToggleGrande"); }
        else if (configMinijuegos[0].tamaño.Contains("mediano")) { filas = 9; columnas = 9; Debug.Log("ToggleMediano "); }
        else if (configMinijuegos[0].tamaño.Contains("pequeño")) { filas = 7; columnas = 7; Debug.Log("TogglePequeño"); }


        if (configMinijuegos[0].tablero.ToUpper().Equals("CLASICO")) numeroTablero = 0;
            else if (configMinijuegos[0].tablero.ToUpper().Equals("ETILICO")) numeroTablero = 1;
            else if (configMinijuegos[0].tablero.ToUpper().Equals("HOT")) numeroTablero = 2;
            else if (configMinijuegos[0].tablero.ToUpper().Equals("PAREJA")) numeroTablero = 3;
            else if (configMinijuegos[0].tablero.ToUpper().Equals("GIGANTE")) numeroTablero = 4;

        if (configMinijuegos[0].continuarPartida.Equals("S")) {
            hayPartida = true;
        }

        jugadorActual= configMinijuegos[0].jugadorActual;

        string[] minijuegos = configMinijuegos[0].minijuegos.Split(':', StringSplitOptions.RemoveEmptyEntries);
        string[] minijuegos18 = configMinijuegos[0].minijuegos18.Split(':', StringSplitOptions.RemoveEmptyEntries);
        int cont = 0;
        foreach (string minijuego in minijuegos) {

            List<string> cvs = new List<string>();
            CargarCSV(miniJuegos[minijuego].rutaArchivoCSV,cvs);
            cartasCSV.Add(minijuego, cvs);
            numeroMJ.Add(cont,minijuego);
            cont++;
        }

        foreach (string mj18 in minijuegos18)
        {
            string nombre="";
            int opc =int.Parse(mj18);
            //Debug.LogError(mj18);
            if (opc == 0) nombre = "Quien es mas probable +18";
            else if (opc == 1) nombre = "Yo nunca +18";
            else if(opc == 2) nombre = "Verdad +18";
            else if(opc == 3) nombre = "3 Palabras +18";
            else if (opc == 4) nombre = "Hot";

            if (opc>0) {
                List<string> cvs = new List<string>();

                CargarCSV(miniJuegos[nombre].rutaArchivoCSV, cvs);
                cartasCSV.Add(nombre, cvs);

                numeroMJ.Add(cont, nombre);
                cont++;
            }

        }

    }



    //Cerrar Aplicacion
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            GuardarPartida();
        }
    }

    void GuardarPartida() {
        GuardarDatosPartida();
        GuardarConfiguracion();
    }
    void GuardarDatosPartida() {

        
        try
        {
            ClaseManagerBBDD.Instance.DeleteAll<Jugador>();
        }
        catch
        {
            ClaseManagerBBDD.Instance.CreateTable<Jugador>();
        }

        if (jugBBDD != null)
        {
            foreach (KeyValuePair<string,Jugador3D> ju in jugadores)
            {
                //Debug.Log(ju.Key+ ": "+ju.Value.Posicion);
                ClaseManagerBBDD.Instance.Insert<Jugador>(new Jugador(ju.Value.Nombre,ju.Value.RutaIcono,ju.Value.ColorIcono, ju.Value.Ficha.Substring(0, ju.Value.Ficha.LastIndexOf(" ")),ju.Value.Posicion));
            }

        }
        
    }

    void GuardarConfiguracion() {

        ClaseManagerBBDD.Instance.DeleteAll<ConfiguracionTablero>();
        configMinijuegos[0].jugadorActual=jugadorActual;
        configMinijuegos[0].continuarPartida = "S";
        ClaseManagerBBDD.Instance.Insert<ConfiguracionTablero>(configMinijuegos[0]);

    }
    void GuardarPartidaInciales()
    {
        GuardarDatosPartidaIniciales();
        GuardarConfiguracionInciales();
    }
    void GuardarDatosPartidaIniciales()
    {


        try
        {
            ClaseManagerBBDD.Instance.DeleteAll<Jugador>();
        }
        catch
        {
            ClaseManagerBBDD.Instance.CreateTable<Jugador>();
        }

        if (jugBBDD != null)
        {
            foreach (KeyValuePair<string, Jugador3D> ju in jugadoresIniciales)
            {
                //Debug.Log(ju.Key+ ": "+ju.Value.Posicion);
                ClaseManagerBBDD.Instance.Insert<Jugador>(new Jugador(ju.Value.Nombre, ju.Value.RutaIcono, ju.Value.ColorIcono, ju.Value.Ficha.Substring(0, ju.Value.Ficha.LastIndexOf(" ")), 0));
            }

        }

    }

    void GuardarConfiguracionInciales()
    {

        ClaseManagerBBDD.Instance.DeleteAll<ConfiguracionTablero>();
        configMinijuegos[0].jugadorActual = jugadorActual;
        configMinijuegos[0].continuarPartida = "S";
        ClaseManagerBBDD.Instance.Insert<ConfiguracionTablero>(configMinijuegosIniciales[0]);

    }

    void OnDestroy()
    {
        if (ClaseManagerBBDD.Instance != null)
        {
            ClaseManagerBBDD.Instance.CloseDatabase();
            
        }
    }
}


