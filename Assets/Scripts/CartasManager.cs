using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CartasManager : MonoBehaviour
{
    public GameObject carta;
    public TextMeshProUGUI textoCarta;
    public TextAsset archivoCSV;
    public GameObject difuminado; // Opcional, para efectos visuales
    public Button botonMostrarCarta; // Botón para mostrar la carta
    private bool sombraActiva = false;

    private List<string> frases = new List<string>();
    private RectTransform cartaRect;

    private float duracion = 0.4f;
    private Vector2 centro = Vector2.zero;
    private Vector2 derecha = new Vector2(2000, 0);
    private Vector2 izquierda = new Vector2(-2000, 0);

    private bool bloqueado = false;
    [Header("Botón al que se le aplicará la sombra")]
    public Button boton;

    [Header("Sombra Inicial (al iniciar la escena)")]
    public Color32 sombraInicialColor = new Color32(0, 0, 0, 128);
    public Vector2 sombraInicialDireccion = new Vector2(0f, -10f);

    [Header("Sombra Personalizada (para aplicar después)")]
    public Color32 sombraPersonalizadaColor = new Color32(203, 0, 209, 80);  // ejemplo: rosa
    public Vector2 sombraPersonalizadaDireccion = new Vector2(0f, -10f);

    private Shadow sombraActual;

    private void Start()
    {
        CargarCSV();
        cartaRect = carta.GetComponent<RectTransform>();
        carta.SetActive(false);
        difuminado.SetActive(false); // Asegurarse de que el difuminado esté desactivado al inicio

        AplicarSombra(sombraInicialColor, sombraInicialDireccion);

    }

    private void CargarCSV()
    {
        frases.Clear();

        if (archivoCSV != null)
        {
            string[] lineas = archivoCSV.text.Split('\n');
            foreach (string linea in lineas)
            {
                string frase = linea.Trim();
                if (!string.IsNullOrEmpty(frase))
                    frases.Add(frase);
            }
        }
        else
        {
            Debug.LogError("No se asignó el archivo CSV en: " + gameObject.name);
        }
    }

    public void MostrarCarta()
    {
        if (bloqueado || frases.Count == 0) return;
        StartCoroutine(AnimarCarta());
    }

    private IEnumerator AnimarCarta()
    {
        bloqueado = true;

        int index = Random.Range(0, frases.Count);
        textoCarta.text = frases[index];

        if (carta.activeSelf)
        {
            LeanTween.move(cartaRect, derecha, duracion).setOnComplete(() =>
            {
                cartaRect.anchoredPosition = izquierda;
                LeanTween.move(cartaRect, centro, duracion);
            });
        }
        else
        {
            carta.SetActive(true);
            QuitarSombra(); // Quitar sombra al mostrar la carta
            MostrarConDelay();
            //AplicarSombra(new Color32(203, 0, 209, 80), new Vector2(0f, -10f));


            cartaRect.anchoredPosition = izquierda;
            LeanTween.move(cartaRect, centro, duracion);
        }

        yield return new WaitForSeconds(1f);
        bloqueado = false;
    }

    public void MostrarConDelay()
    {
        StartCoroutine(MostrarDespuesDeDelay());
    }

    IEnumerator MostrarDespuesDeDelay()
    {
        yield return new WaitForSeconds(0.25f);  // Espera medio segundo
        difuminado.SetActive(true);        // Muestra el objeto
    }

    /// <summary>
    /// Aplica una sombra con los parámetros indicados
    /// </summary>
    public void AplicarSombra(Color32 color, Vector2 direccion)
    {
        if (boton == null) return;

        // Obtener o añadir sombra
        sombraActual = boton.GetComponent<Shadow>();
        if (sombraActual == null)
            sombraActual = boton.gameObject.AddComponent<Shadow>();

        // Aplicar propiedades
        sombraActual.effectColor = color;
        sombraActual.effectDistance = direccion;
    }

    /// <summary>
    /// Quita la sombra del botón
    /// </summary>
    public void QuitarSombra()
    {
        if (boton == null) return;

        sombraActual = boton.GetComponent<Shadow>();
        if (sombraActual != null)
        {
            // En lugar de destruirla, la desactivamos visualmente
            sombraActual.effectColor = new Color32(0, 0, 0, 0);
        }
    }

    public void AplicarSombraPersonalizada()
    {
        AplicarSombra(sombraPersonalizadaColor, sombraPersonalizadaDireccion);
    }
    public void RestaurarSombraInicial()
    {
        AplicarSombra(sombraInicialColor, sombraInicialDireccion);
    }


}


