using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VentanaTablero3D : MonoBehaviour
{
    public GameObject prefabCasNorm;
    public GameObject prefabCasEsq;
    public GameObject prefabCasSal;
    public GameObject casillas;

    public Material materialCasillaSalEnt;
    public Material materialCasillaTierra;
    public Material materialCasillaRoca;
    public Material materialCasillaRuinas;
    public Material materialCasillaTemplo;

    public Image imagenCanvas;
    int contador = 0;

    //Vector3[] posicionesTablero;
    //public GameObject prefabFicha;
    int posicionActual = 0;
    //GameObject[] fichass;

    Dictionary<string, Jugador3D> jugadores=new Dictionary<string, Jugador3D>();
    Dictionary<int, Vector3> posicionesTablero= new Dictionary<int, Vector3>();
    Dictionary<string, GameObject> fichas= new Dictionary<string, GameObject>();
    Dictionary<int, string> turnos= new Dictionary<int, string>();

    int jugadorActual=1;
   
    
    //Imagen en partes ---------------------------------------------------


    public Texture2D fullImage; // La imagen completa
 


    // Start is called before the first frame update
    void Start()
    {
        jugadores=new Dictionary<string, Jugador3D>();
        //Casillas0_11();
        Jugador3D jugador1 = new Jugador3D("Carmelo", "C:/Users/carma/Downloads/Corazon2.png", "FichaCorazon", 0);
        Jugador3D jugador2 = new Jugador3D("Fran", "C:/Users/carma/Downloads/Corazon2.png", "FichaCylinder", 0);
        Jugador3D jugador3 = new Jugador3D("Seryi", "C:/Users/carma/Downloads/Corazon2.png", "FichaCylinder", 0);

        jugadores.Add(jugador1.Nombre, jugador1);
        jugadores.Add(jugador2.Nombre, jugador2);
        jugadores.Add(jugador3.Nombre, jugador3);

        

        GenerarTablero3D(11,11);
        PersonalizarFicha();
    }

    // Update is called once per frame
    void Update()
    {
        
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

        for (int i = 0; i < filas * columnas; i++)
        {
            
            // Calcular la posición escalada según el tamaño del cubo
            Vector3 posicionCasilla = new Vector3(x * tamano, 0f, z * tamano);

            posicionesTablero.Add(i, new Vector3(x * tamano, 6.1f, z * tamano));

            

            // Establecer la rotación según la dirección actual
            Quaternion rotacionCasilla;
            if (dx == 1 && dz == 0) // Moviendo hacia la derecha
            {
                rotacionCasilla = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (dx == 0 && dz == 1) // Moviendo hacia abajo
            {
                rotacionCasilla = Quaternion.Euler(0f, -90f, 0f);
            }
            else if (dx == -1 && dz == 0) // Moviendo hacia la izquierda
            {
                rotacionCasilla = Quaternion.Euler(0f, 180f, 0f);
            }
            else // Moviendo hacia arriba
            {
                rotacionCasilla = Quaternion.Euler(0f, 90f, 0f);
            }

            // Instanciar la casilla con la posición y rotación definidas
            
            GameObject casilla = null;
            rotacionCasilla = Quaternion.Euler(0f, 0f, 0f);

            if (i == 0)
            {
                casilla = Instantiate(prefabCasSal, posicionCasilla, rotacionCasilla);
            }
            else if (i == 10 || i == 20 || i == 30 || i == 39 || i == 48 || i == 56 || i == 64 || i == 71 || i == 78 || i == 84 || i == 90 || i == 95 || i == 100 || i == 104 || i == 108 || i == 111 || i == 114 || i == 116 || i == 118 || i == 119) {
                casilla = Instantiate(prefabCasEsq, posicionCasilla, rotacionCasilla);
            }
            else if (i==(filas * columnas)-1) {
                casilla = Instantiate(prefabCasSal, posicionCasilla, rotacionCasilla);
            }
            else {
                casilla = Instantiate(prefabCasNorm, posicionCasilla, rotacionCasilla);
            }

            TextMeshPro textComponent = casilla.GetComponentInChildren<TextMeshPro>();

            if (textComponent != null)
            {
                textComponent.SetText(i.ToString()); // Cambia el texto aquí
                if (i >= 1 && i < 40)
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



    void PersonalizarFicha() {
        int cont = 1;
        
        foreach(KeyValuePair<string, Jugador3D> ju in jugadores) {
            StartCoroutine(GenerarFicha(ju, (nuevaFicha) =>
            {
                if (nuevaFicha != null)
                {
                    Transform textFront = nuevaFicha.transform.Find("NombreFront");
                    Transform textBack = nuevaFicha.transform.Find("NombreBack");
                    TextMeshPro textComponentFront = textFront.GetComponent<TextMeshPro>();
                    TextMeshPro textComponentBack = textBack.GetComponent<TextMeshPro>();

                    textComponentFront.SetText(ju.Value.Nombre);
                    textComponentBack.SetText(ju.Value.Nombre);

                    Transform fotoObj = nuevaFicha.transform.Find("FotoObj");
                    StartCoroutine(CambiarImagenFicha(fotoObj, ju.Value.RutaIcono));

                    turnos.Add(cont, ju.Value.Nombre);
                    fichas.Add(ju.Value.Ficha + " " + cont, nuevaFicha);
                    ju.Value.Ficha = ju.Value.Ficha + " " + cont;

                    nuevaFicha.transform.SetParent(casillas.transform, false);
                }
            }));
            //fichas.Add(ju.Value.Nombre, ju.Value.Posicion);
            cont++;
        }
        
    }
    public void tirarDado() {

        int dad = dado();
        //Debug.Log("Número de dado: " + dad);

        movimientoFicha(dad,jugadorActual);

        

    }
    void movimientoFicha(int numeroDado,int jugador) {

        
        
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

    }

    int dado() {

        return (int)UnityEngine.Random.Range(1,7);
        
    }

    IEnumerator AnimarMovimiento(int jugador,int posicionAct, int nuevaPosicion)
    {
        GameObject ficha = fichas.GetValueOrDefault(jugadores.GetValueOrDefault(turnos.GetValueOrDefault(jugador)).Ficha);
        while (Vector3.Distance(ficha.transform.position, posicionesTablero[nuevaPosicion]) > 0.1f)
        {
            ficha.transform.position = Vector3.MoveTowards(ficha.transform.position, posicionesTablero[nuevaPosicion], 5f * Time.deltaTime);
            yield return null; // Esperar al siguiente frame
            
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




    void AsignarTextura(GameObject casilla, int indice, Texture2D fullImage, int gridSize)
    {
        // Calcula las dimensiones de la imagen dividida
        int gridSizeX = gridSize;
        int gridSizeY = gridSize;

        float posicionX = posicionesTablero[indice].x / 5.1f; // Coordenada X en la cuadrícula
        float posicionY = posicionesTablero[indice].z / 5.1f; // Coordenada Y en la cuadrícula

        float cellWidth = fullImage.width / gridSizeX;
        float cellHeight = fullImage.height / gridSizeY;

        Debug.Log("indice: " + indice + " posicionX : " + posicionX + " posicionY: " + posicionY + "Posiciones : x:" + posicionesTablero[indice].x + " z:" + posicionesTablero[indice].z);

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

        Debug.Log("indice: " + indice + " posicionX : " + posicionX + " posicionY: " + posicionY+ "Posiciones : x:" + posicionesTablero[contador].x+" z:" + posicionesTablero[contador].z);
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

        Debug.Log("Imagen cargada correctamente.");

        Texture2D cellTexture = new Texture2D(imagenFicha.width, imagenFicha.height);
        cellTexture.SetPixels(imagenFicha.GetPixels());
        cellTexture.Apply();

        Material topMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        topMaterial.mainTexture = cellTexture;

        MeshRenderer renderer = prefab.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = topMaterial;
            Debug.Log("Material aplicado correctamente.");
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
       
}


