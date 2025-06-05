using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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

    public Image imagenCanvas;
    public GameObject contenedorIconos;
    int contador = 0;

    public List<Material> materiales;
    List<ColorJugadores> coloresJu;

    //Vector3[] posicionesTablero;
    //public GameObject prefabFicha;
    //int posicionActual = 0;
    //GameObject[] fichass;

    Dictionary<string, Jugador3D> jugadores=new Dictionary<string, Jugador3D>();
    Dictionary<int, Vector3> posicionesTablero= new Dictionary<int, Vector3>();
    Dictionary<string, GameObject> fichas= new Dictionary<string, GameObject>();
    Dictionary<int, string> turnos= new Dictionary<int, string>();
    Dictionary<int, GameObject> iconos = new Dictionary<int, GameObject>();

    int jugadorActual=1;
    int filas = 11;
    int columnas = 11;

    List<int> tableroEsquina;



        //Imagen en partes ---------------------------------------------------


    public Texture2D fullImage; // La imagen completa


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

    Dictionary<int, Vector3> rotacionesDado = new Dictionary<int, Vector3>
        {
            {1, new Vector3(90, 0, 0)},
            {2, new Vector3(0, 0, 0)},
            {3, new Vector3(0, 0, -90)},
            {4, new Vector3(0, 0, 90)},
            {5, new Vector3(0, 0, 180)},
            {6, new Vector3(-90, 0, 0)}
        };

    public float anguloRotacion = -90f;

    bool block=false;

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
    //List<GameObject> casillas2D = new List<GameObject>();
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
    public GameObject noCamera;




    ////Pruebas------------------------------------------------------------------
    //Dicciopinta****************************************************************
    bool puedeDibujar = false;
    bool puedeDibujarTiempo = false;
    int contadorClicks = 0;
    public RawImage lienzoDibujo;
    public TMP_Text tiempoContador;


    ////BBDD------------------------------------------------------------------
    ClaseManagerBBDD managerBBDD;
    List<Jugador> jugBBDD;

    // Start is called before the first frame update
    void Start()
    {
        //Dado
        posicionInicial = dado.transform.position;
        rotacionInicial = dado.transform.rotation;



        rb = dado.GetComponent<Rigidbody>();
        rb.useGravity = false;

        //Lienzo
        CrearLienzo();

        GenerarMapaEspiralAGrid();

        //Tablero y Fichas
        GenerarTablero3D(filas, columnas);



        //BBDD
        string databasePath = Path.Combine(Application.persistentDataPath, "JuegOcaBBDD.db");
        managerBBDD = new ClaseManagerBBDD(databasePath);

        jugadores = new Dictionary<string, Jugador3D>();
        CargarJugadores();
        CargarColores();

        //Tablero 2D
        GenerarTablero2D();


        PersonalizarFicha();
        MarcoJugador();

        ////Configuracion
        //Tablero2D
        MoverFicha2DAlFinal(fichas2D[turnos[jugadorActual]]);
        MoverFicha2DAlFinal(fichasBTN2D[turnos[jugadorActual]]);
        
        MoverFicha2DAlFinal(fichas[jugadores[turnos[jugadorActual]].Ficha]);
        //TurnoMaterial(jugadorActual);
        TurnoCamara(fichas[jugadores[turnos[jugadorActual]].Ficha]);
        VerificarSuperposicion();
        

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
        }
        else if (filas * columnas < 9 * 9)
        {
            tableroEsquina = new List<int>() { 6, 12, 18, 23, 28, 32, 36, 39, 42, 44, 46, 47 };
            porcentaje = new float[] { 0.47f, 0.8f, 0.99f, 0.0f };
            bordTableroPequeño.SetActive(true);
        }
        else
        {
            tableroEsquina = new List<int>() { 8, 16, 24, 31, 38, 44, 50, 55, 60, 64, 68, 71, 74, 76, 78, 79 };
            porcentaje = new float[] { 0.39f, 0.69f, 0.88f, 0.0f };
            bordTableroMediano.SetActive(true);
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

            if (i == 0)
            {
                casilla = Instantiate(prefabCasSal, posicionCasilla, rotacionO);

            }
            else if (i==(filas * columnas)-1) {
                casilla = Instantiate(prefabCasSal, posicionCasilla, rotacionO);
            }
            else {
                casilla = Instantiate(prefabCasNorm, posicionCasilla, rotacionO);
            }

            if (filas * columnas > 9 * 9)
            {
                if (i == 10 || i == 20 || i == 30 || i == 39 || i == 48 || i == 56 || i == 64 || i == 71 || i == 78 || i == 84 || i == 90 || i == 95 || i == 100 || i == 104 || i == 108 || i == 111 || i == 114 || i == 116 || i == 118 || i == 119)
                {
                    Quaternion rotacion45 = Quaternion.Euler(0f, 0f, 45f);
                    rotacionCasilla *= rotacion45;
                    casilla = Instantiate(prefabCasEsq, posicionCasilla, rotacionO);
                    contenedorCorazones.transform.position = new Vector3(25.5f, 20.4f, 15.3f);

                }
            }
            else if (filas * columnas < 9 * 9)
            {
                if (i == 6 || i == 12 || i == 18 || i == 23 || i == 28 || i == 32 || i == 36 || i == 39 || i == 42 || i == 44 || i == 46 || i == 47)
                {
                    Quaternion rotacion45 = Quaternion.Euler(0f, 0f, 45f);
                    rotacionCasilla *= rotacion45;
                    casilla = Instantiate(prefabCasEsq, posicionCasilla, rotacionO);
                    contenedorCorazones.transform.position = new Vector3(5015.14602f, 20.70971f, 4.148824f);
                }
            }
            else {
                if (i == 8 || i == 16 || i == 24 || i == 31 || i == 38 || i == 44 || i == 50 || i == 55 || i == 60 || i == 64 || i == 68 || i == 71 || i == 74 || i == 76 || i == 78 || i == 79)
                {
                    Quaternion rotacion45 = Quaternion.Euler(0f, 0f, 45f);
                    rotacionCasilla *= rotacion45;
                    casilla = Instantiate(prefabCasEsq, posicionCasilla, rotacionO);
                    contenedorCorazones.transform.position = new Vector3(20.4f, 20.4f, 10.2f);
                }
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
                    AsignarTextura(casilla, i, fullImage, filas);
                }
                else if (i < filas * columnas * porcentaje[1])
                {
                    textComponent.color = Color.blue;
                    AsignarTextura(casilla, i, fullImage, filas);
                }

                else if (i < filas * columnas * porcentaje[2])
                {
                    textComponent.color = Color.yellow;
                    AsignarTextura(casilla, i, fullImage, filas);
                }
                else if (i < filas * columnas * porcentaje[3])
                {
                    textComponent.color = Color.red;
                    AsignarTextura(casilla, i, fullImage, filas);
                }
                else if (i == (filas * columnas) - 1 || i == 0)
                {

                }
                else
                {
                    textComponent.color = Color.black;
                    AsignarTextura(casilla, i, fullImage, filas);
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
                        Debug.Log(textoTMP.text + "   " + turnos[jugador]);
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
                        Debug.Log(textoTMP.text + "   " + turnos[jugador]);
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
                //Debug.LogError("FichasOpacas: turnos[i]: " + turnos[i] + " ju.Nombre: " + jugadores[turnos[i]].Nombre);
                //FichasOpacas(fichas[jugadores[turnos[i]].Ficha]);
                ColorMaterialOpacoIns(i);
                //CambiarMaterialGlobal(i);
                //fichas[jugadores[turnos[i]].Ficha].GetComponent<Renderer>().material = ColorMaterialOpaco(jugadores[turnos[i]].ColorIcono);
                //fichas[jugadores[turnos[i]].Ficha].GetComponent<Renderer>().material.SetFloat("_Mode", fichas[jugadores[turnos[i]].Ficha].GetComponent<Renderer>().material.GetFloat("_Mode"));
            }
            else {
                //Debug.LogError("FichasTraslucidas: turnos[i]: " + turnos[i] + " ju.Nombre: " + jugadores[turnos[i]].Nombre);
                //FichasTraslucidas(fichas[jugadores[turnos[i]].Ficha]);
                ColorMaterialTraslucidoIns(i);
                //fichas[jugadores[turnos[i]].Ficha].GetComponent<Renderer>().material = ColorMaterialTraslucido(jugadores[turnos[i]].ColorIcono);
                //fichas[jugadores[turnos[i]].Ficha].GetComponent<Renderer>().material.SetFloat("_Mode", fichas[jugadores[turnos[i]].Ficha].GetComponent<Renderer>().material.GetFloat("_Mode"));
                //CambiarMaterialGlobal(i);

            }
        }

    }

    void TurnoCamara(GameObject gameObject)
    {

        for (int i = 1; i <= turnos.Count; i++)
        {
            if (turnos[i].Equals(turnos[jugadorActual]))
            {
                MoverFicha2DAlFinal(fichas[jugadores[turnos[jugadorActual]].Ficha]);

                TurnoMaterial(jugadorActual);
                VerificarSuperposicion();
                fichas[jugadores[turnos[i]].Ficha].transform.Find("CameraFicha").gameObject.SetActive(true);

                

            }
            else
            {
                fichas[jugadores[turnos[i]].Ficha].transform.Find("CameraFicha").gameObject.SetActive(false);
            }
            
            
        }

    }

    void VerificarSuperposicion()
    {
        
        foreach (KeyValuePair<string,Jugador3D> jugador in jugadores)
        {
            hayOtraFichaEnMismaCasilla = false;

            // Comprobar si hay otra ficha en la misma casilla
            foreach (KeyValuePair<string, Jugador3D> otroJugador in jugadores)
            {
                if (jugador.Value != otroJugador.Value && jugador.Value.Posicion == otroJugador.Value.Posicion)
                {
                    hayOtraFichaEnMismaCasilla = true;
                    break;
                }
            }

            // Si hay otra ficha, hacerla transparente; si no, volver a visible
            if (hayOtraFichaEnMismaCasilla)
            {
                hayOtraFichaEnMismaCasilla = true;
                if (jugador.Value.Nombre.Equals(turnos[jugadorActual]))
                {
                    //Debug.Log(jugador.Value.Nombre);
                    HacerVisible(fichas[jugador.Value.Ficha]);
                }
                else
                {
                    HacerInvisible(fichas[jugador.Value.Ficha]);
                }

            }
            else {
                hayOtraFichaEnMismaCasilla = false;
                HacerVisible(fichas[jugador.Value.Ficha]);
            }
            
        }
        
    }

    void ComprobarCoincidencias(int jugador) {
        for (int i=1;i<jugadores.Count;i++) {
            Debug.Log("jugadores.Count: "+ jugadores.Count+ " jugadores[turnos[i]].Nombre: " + jugadores[turnos[i]].Nombre+" i: "+i);
                if (jugadores[turnos[i]] != jugadores[turnos[jugador]] && jugadores[turnos[i]].Posicion == jugadores[turnos[jugador]].Posicion)
                {
                   if(!turnos[i].Equals(turnos[jugadorActual]))HacerInvisible(fichas[jugadores[turnos[i]].Ficha]);
                }
                else {
                    if (turnos[i].Equals(turnos[jugadorActual])) HacerVisible(fichas[jugadores[turnos[i]].Ficha]);
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
                    //nuevaFicha.GetComponent<Renderer>().material = ColorElegidoMaterial(ju.Value.ColorIcono);
                    

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

                    //nuevaFicha.SetActive(false);
                    nuevaFicha.transform.SetParent(casillas.transform, false);

                    
                    


                }
            }));
            //fichas.Add(ju.Value.Nombre, ju.Value.Posicion);
            cont++;
        }
        
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
                PopupDado.SetActive(true);
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
        if (popupTablero2D.activeSelf)
        {
            TurnoCamara(fichas[jugadores[turnos[jugadorActual]].Ficha]);

            Boton2D.SetActive(true);
            popupTablero2D.SetActive(false);
            Boton3D.SetActive(false);
            noCamera.SetActive(false);

        }
        else {
            DesactivarCamaras();
            
            Boton2D.SetActive(false);
            popupTablero2D.SetActive(true);
            Boton3D.SetActive(true);
            noCamera.SetActive(true);

        }

    }

    void FisicasDado() {
        //Debug.Log("Rotación del dado: " + dado.transform.rotation.eulerAngles);


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
            PopupDado.SetActive(false);
            //fichas.GetValueOrDefault(jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugadorActual)).Ficha).SetActive(true);
            //fichas[jugadores[turnos[jugadorActual]].Ficha].transform.Find("CamaraFicha").gameObject.SetActive(true);
            
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
                Debug.Log("El resultado del dado es: " + NumeroActual);
                

            }
        }
        block = false;
    }
  

    public void MoverAFicha(int resultadoDado)
    {
        StartCoroutine(MoverFicha2DPasoAPaso(jugadorActual, resultadoDado));
        StartCoroutine(MoverFichaPasoAPaso(jugadorActual, resultadoDado));

    }

    IEnumerator MoverFichaPasoAPaso( int jugador, int resultadoDado)
    {
        

        int nuevaPosicion = jugadores[turnos[jugador]].Posicion + resultadoDado;
        if (posicionesTablero.ContainsKey(nuevaPosicion))
        {
            while (jugadores[turnos[jugador]].Posicion < nuevaPosicion)
            {
                int siguienteCasilla = jugadores[turnos[jugador]].Posicion + 1;
                //ComprobarCoincidencias(siguienteCasilla);

                if (!posicionesTablero.ContainsKey(siguienteCasilla)) break; // Detener si la siguiente casilla no existe

                /*Vector3 posicionInicial = posicionesTablero[jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugador)).Posicion];
                Vector3 posicionDestino = posicionesTablero[siguienteCasilla];*/

                
                Vector3 posicionInicial = casillas.transform.TransformPoint(posicionesTablero[jugadores[turnos[jugador]].Posicion]);
                Vector3 posicionDestino = casillas.transform.TransformPoint(posicionesTablero[siguienteCasilla]);


                float t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime * velocidadMovimiento;

                    // Movimiento con efecto de golpe en cada casilla
                    float salto = Mathf.Sin(t * Mathf.PI) * alturaSalto;
                    fichas[jugadores[turnos[jugadorActual]].Ficha].transform.position = Vector3.Lerp(posicionInicial, posicionDestino, t) + Vector3.up * salto;

                    yield return null;
                }

                jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugador)).Posicion++;
                yield return new WaitForSeconds(pausaEntreCasillas); // Pausa antes de seguir avanzando
                                                                     // Si la casilla es una esquina, rotar la cámara
                
                if (tableroEsquina.Contains(jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugadorActual)).Posicion))
                {
                    
                    StartCoroutine(RotarCamaraSuavemente(anguloRotacion));

                }
            }
            //fichas.GetValueOrDefault(jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugadorActual)).Ficha).SetActive(false);
            //fichas[jugadores[turnos[jugadorActual]].Ficha].transform.Find("CamaraFicha").gameObject.SetActive(false);
            
            jugadorActual = siguienteJugador(jugador);
            TurnoCamara(fichas[jugadores[turnos[jugadorActual]].Ficha]);

            MoverFicha2DAlFinal(fichas2D[turnos[jugadorActual]]);
            MoverFicha2DAlFinal(fichasBTN2D[turnos[jugadorActual]]);
            MarcoJugador();
            //fichas.GetValueOrDefault(jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugadorActual)).Ficha).SetActive(true);
            //fichas[jugadores[turnos[jugadorActual]].Ficha].transform.Find("CamaraFicha").gameObject.SetActive(true);
        }
        block=false;
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
            
            fichas2D.Add(ju.Nombre, fichaPrefab2D);


            //Boton Tablero 2D

            GameObject fichaBTNCasilla2D = Instantiate(prefabsBotonFicha2D);
            Image ColorBotonBTN2D = fichaBTNCasilla2D.gameObject.GetComponentInChildren<Image>();
            ColorBotonBTN2D.color = color;

            Image ImagenBTNIcono2D = fichaBTNCasilla2D.transform.Find("ImageJugador").gameObject.GetComponentInChildren<Image>();
            CambiarImagenJugador(ImagenBTNIcono2D.gameObject, ju.RutaImagen);

            fichaBTNCasilla2D.transform.SetParent(contenedorBotonFichas2D.transform, false);
            fichasBTN2D.Add(ju.Nombre, fichaBTNCasilla2D);

            StartCoroutine(ObtenerPosicionCasilla(casillas2D[110], botonCasillas2D[110], (posicion,posicion2) =>
            {
                fichaPrefab2D.GetComponent<RectTransform>().anchoredPosition = posicion;
                fichaBTNCasilla2D.GetComponent<RectTransform>().anchoredPosition = posicion2;
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
        
        if (indice == 0 || indice==120) {
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
        
        if (posicionCasilla==0 || posicionCasilla == 120) {
            GameObject salida2D = Instantiate(prefabsCasilla2DSalida);
            GameObject salidaBTN2D = Instantiate(prefabsCasilla2DSalida);
            if (posicionCasilla == 0) { casillas2D.Add(110, salida2D); botonCasillas2D.Add(110, salidaBTN2D); }
            if (posicionCasilla == 120) { casillas2D.Add(60, salida2D); botonCasillas2D.Add(60, salidaBTN2D); }
            //Debug.Log("indice: " + posicionCasilla + " CalcularPosicionEnGrid(posicionCasilla): " + CalcularPosicionEnGrid(posicionCasilla));

        } else {
            GameObject casilla2D = Instantiate(prefabsCasilla2D);
            GameObject casillaBTN2D = Instantiate(casillaBotonTablero2D);
            //Debug.Log("numeroCasilla: "+ numeroCasilla+ " posicionCasilla: "+ posicionCasilla);
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

            //Debug.Log("CalcularPosicionEnGrid(posicionCasilla): " + CalcularPosicionEnGrid(posicionCasilla)+ " posicionCasilla: " + posicionCasilla);
            //Debug.Log("indice: " + posicionCasilla + " CalcularPosicionEnGrid(posicionCasilla): " + CalcularPosicionEnGrid(posicionCasilla));
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
            //Debug.Log($"x {x} z {z} posicionGrid {posicionGrid} indice {i}");

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
    }


    int CalcularPosicionEnGrid(int indiceEspiral)
    {
        return mapaEspiralAGrid[indiceEspiral]; // Devuelve la posición correcta en el Grid
    }



    void GenerarTablero2D() {
        foreach (int clave in casillas2D.Keys.OrderBy(k => k))
        {
            
            

            //Debug.LogError("clave: "+ clave);
            casillas2D[clave].transform.SetParent(contenedorCasilla2D.transform, false);
            Vector2 posicionInicial = casillas2D[clave].GetComponent<RectTransform>().anchoredPosition;


            botonCasillas2D[clave].transform.SetParent(contenedorBotonTablero2D.transform, false);
        }



        IEnumerator ObtenerPosicionesConRetraso()
        {
            yield return new WaitForEndOfFrame(); 
            foreach (Transform hijo in contenedorCasilla2D.transform)
            {
                //Debug.Log($"Casilla: {hijo.gameObject.name}, Posición UI: {hijo.GetComponent<RectTransform>().anchoredPosition}");
            }
        }
        StartCoroutine(ObtenerPosicionesConRetraso());

        IEnumerator ObtenerPosicionesConRetrasoBTN()
        {
            yield return new WaitForEndOfFrame();
            foreach (Transform hijo in contenedorBotonTablero2D.transform)
            {
                //Debug.Log($"Casilla: {hijo.gameObject.name}, Posición UI: {hijo.GetComponent<RectTransform>().anchoredPosition}");
            }
        }
        StartCoroutine(ObtenerPosicionesConRetrasoBTN());

        AñadirFicha2D();

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



    public void AccionImagen() {

        CambiarImagen(contador, fullImage,5);
        contador++;
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

    //Cambio de 2D a 3D y viceversa

    void CambioDeDimension() {
        //popupTablero2D.

        //popup.


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
        jugBBDD = managerBBDD.SelectAll<Jugador>();
        if (jugBBDD == null)
        {
            Jugador3D jugador1 = new Jugador3D("Carmelo","rojo", "C:/Users/carma/Downloads/Corazon2.png", "FichaCorazon", 0);
            Jugador3D jugador2 = new Jugador3D("Fran", "verde","C:/Users/carma/Downloads/Corazon2.png", "FichaCylinder", 0);
            Jugador3D jugador3 = new Jugador3D("Seryi","fuxia", "C:/Users/carma/Downloads/Corazon2.png", "FichaCylinder", 0);

            jugadores.Add(jugador1.Nombre, jugador1);
            jugadores.Add(jugador2.Nombre, jugador2);
            jugadores.Add(jugador3.Nombre, jugador3);
        }
        else {
            foreach (Jugador ju in jugBBDD)
            {
                Jugador3D jugador3D = new Jugador3D(ju.Nombre,ju.ColorIcono, ju.RutaImagen, ju.Ficha, 0);
                jugadores.Add(jugador3D.Nombre, jugador3D);

                AñadirIcono(ju.Nombre, ju.RutaImagen, ju.ColorIcono,cont);

                cont++;

            }
        }
        
    }

    void CargarColores() {

        coloresJu = managerBBDD.SelectAll<ColorJugadores>();
    }
       
}


