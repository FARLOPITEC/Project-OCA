using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class VentanaTablero3D : MonoBehaviour
{
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
    int filas = 7;
    int columnas = 7;

    List<int> tableroEsquina;



    //Imagen en partes ---------------------------------------------------


    public Texture2D fullImage; // La imagen completa

    //Movimiento fichas---------------------------------------------------
    public float velocidadMovimiento = 5f; // Velocidad del desplazamiento
    public float alturaSalto = 0.5f; // Altura del pequeño golpe en cada casilla
    public float pausaEntreCasillas = 0.2f; // Tiempo de espera antes de avanzar a la siguiente casilla

    //private int indiceCasillaActual = 0; // Casilla en la que está la ficha

   // private int currentTileIndex = 0;

    public float anguloRotacion = -90f;

    bool block=false;
    //Pruebas---------------------------------------------------------------
    //Dicciopinta*******************************************************
    bool puedeDibujar = false;
    bool puedeDibujarTiempo = false;
    int contadorClicks = 0;
    public RawImage lienzoDibujo;
    public TMP_Text tiempoContador;


    //BBDD------------------------------------------------------------------
    ClaseManagerBBDD managerBBDD;

    // Start is called before the first frame update
    void Start()
    {
        CrearLienzo();
        string databasePath = Path.Combine(Application.persistentDataPath, "JuegOcaBBDD.db");
        managerBBDD = new ClaseManagerBBDD(databasePath);

        jugadores =new Dictionary<string, Jugador3D>();
        //Casillas0_11();
        /* Jugador3D jugador1 = new Jugador3D("Carmelo", "C:/Users/carma/Downloads/Corazon2.png", "FichaCorazon", 0);
         Jugador3D jugador2 = new Jugador3D("Fran", "C:/Users/carma/Downloads/Corazon2.png", "FichaCylinder", 0);
         Jugador3D jugador3 = new Jugador3D("Seryi", "C:/Users/carma/Downloads/Corazon2.png", "FichaCylinder", 0);

         jugadores.Add(jugador1.Nombre, jugador1);
         jugadores.Add(jugador2.Nombre, jugador2);
         jugadores.Add(jugador3.Nombre, jugador3);*/
        
        CargarJugadores();
        CargarColores();


        GenerarTablero3D(filas, columnas);
        PersonalizarFicha();
        MarcoJugador();
    }

    // Update is called once per frame
    
    void Update()
    {
        ActualizarDibujoWin();
        

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
                /* if (i >= 1 && i < 40)
                 {
                     textComponent.color = Color.cyan;
                     AsignarTextura(casilla, i,fullImage, filas);
                 }
                 if (i>=40 && i<72) {
                     textComponent.color = Color.blue;
                     AsignarTextura(casilla, i,  fullImage, filas);
                 }
                 if (i>=72 && i<96) { 
                     textComponent.color = Color.yellow;
                     AsignarTextura(casilla, i,  fullImage, filas);
                 }
                 if (i>=96 && i<112) { 
                     textComponent.color = Color.red;
                     AsignarTextura(casilla, i,  fullImage, filas);
                 }
                 if (i>=112) { 
                     textComponent.color = Color.black;
                     AsignarTextura(casilla, i,  fullImage, filas);
                 }*/

                

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
                }else if (i == (filas * columnas)-1 )
                {
                    
                }
                else
                {
                    textComponent.color = Color.black;
                    AsignarTextura(casilla, i, fullImage, filas);
                }

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
            
            if (mat.name.Equals(ColorHexToName(color))) { 
                material = mat; 
            }
        
        }
        return material;
    }

    void PersonalizarFicha() {
        int cont = 1;
        
        foreach(KeyValuePair<string, Jugador3D> ju in jugadores) {
            StartCoroutine(GenerarFicha(ju, (nuevaFicha) =>
            {
                if (nuevaFicha != null)
                {
                    
                    nuevaFicha.GetComponent<Renderer>().material = ColorElegidoMaterial(ju.Value.ColorIcono);

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

                    nuevaFicha.transform.SetParent(casillas.transform, false);
                    nuevaFicha.SetActive(false);
                    if(cont == 1) nuevaFicha.SetActive(true); 

                }
            }));
            //fichas.Add(ju.Value.Nombre, ju.Value.Posicion);
            cont++;
        }
        
    }
    public void tirarDado() {
        if (!block)
        {
            block = true;
            //Debug.Log("Número de dado: " + dad);

            MoverAFicha();
        }


        

    }

    public void MoverAFicha()
    {
        StartCoroutine(MoverFichaPasoAPaso(jugadorActual));
    }

    IEnumerator MoverFichaPasoAPaso( int jugador)
    {
        
        int dad = dado();
        int nuevaPosicion = jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugador)).Posicion + dad;
        if (posicionesTablero.ContainsKey(nuevaPosicion))
        {
            while (jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugador)).Posicion < nuevaPosicion)
            {
                int siguienteCasilla = jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugador)).Posicion + 1;
                if (!posicionesTablero.ContainsKey(siguienteCasilla)) break; // Detener si la siguiente casilla no existe

                /*Vector3 posicionInicial = posicionesTablero[jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugador)).Posicion];
                Vector3 posicionDestino = posicionesTablero[siguienteCasilla];*/

                
                Vector3 posicionInicial = casillas.transform.TransformPoint(posicionesTablero[jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugador)).Posicion]);
                Vector3 posicionDestino = casillas.transform.TransformPoint(posicionesTablero[siguienteCasilla]);


                float t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime * velocidadMovimiento;

                    // Movimiento con efecto de golpe en cada casilla
                    float salto = Mathf.Sin(t * Mathf.PI) * alturaSalto;
                    fichas.GetValueOrDefault(jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugadorActual)).Ficha).transform.position = Vector3.Lerp(posicionInicial, posicionDestino, t) + Vector3.up * salto;

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
            fichas.GetValueOrDefault(jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugadorActual)).Ficha).SetActive(false);
            jugadorActual = siguienteJugador(jugador);
            MarcoJugador();
            fichas.GetValueOrDefault(jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugadorActual)).Ficha).SetActive(true);
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

    /*void movimientoFicha(int numeroDado,int jugador) {

        
        
    int nuevaPosicion = jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugador)).Posicion + numeroDado;
        


        if (nuevaPosicion >= posicionesTablero.Count)
        {

            nuevaPosicion = posicionesTablero.Count - 1; // Limitar al final del tablero

        }

        StartCoroutine(AnimarMovimiento(jugador, posicionActual, nuevaPosicion));
        // Mover la ficha a la nueva posición

        fichas.GetValueOrDefault(jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugadorActual)).Ficha).transform.position = posicionesTablero[nuevaPosicion];


        jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugadorActual)).Posicion = nuevaPosicion;
        posicionActual = nuevaPosicion; // Actualizar la posición actual

        jugadorActual = siguienteJugador(jugador);

    }*/

    int dado() {

        return (int)UnityEngine.Random.Range(1,7);
        
    }

    /*IEnumerator AnimarMovimiento(int jugador,int posicionAct, int nuevaPosicion)
    {
        GameObject ficha = fichas.GetValueOrDefault(jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugador)).Ficha);
        while (Vector3.Distance(ficha.transform.position, posicionesTablero[nuevaPosicion]) > 0.1f)
        {
            ficha.transform.position = Vector3.MoveTowards(ficha.transform.position, posicionesTablero[nuevaPosicion], 5f * Time.deltaTime);
            yield return null; // Esperar al siguiente frame
            
        }

    }*/

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

    //Añadir imagen fichas
    /*
    
    public void añadirImagen2()
    {
        //noImagen.gameObject.SetActive(false);
       
        NativeFilePicker.PickFile((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log("Path seleccionado: " + path);

                Texture2D texture = LoadTexture(path);

                CambiarImagenFicha(texture);
                
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
            else
            {
                Debug.LogError("No se seleccionó ningún archivo.");
            }
        }, new string[] { "image/*" }// Prueba con un solo formato
);
    }
    */
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

        Debug.Log($"RenderTexture creado con tamaño: {ancho}x{alto}");

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
        List<Jugador> jugBBDD = managerBBDD.SelectAll<Jugador>();
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


