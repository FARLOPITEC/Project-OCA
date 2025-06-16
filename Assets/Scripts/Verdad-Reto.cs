using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VerdadORetoManager : MonoBehaviour
{
    public GameObject cartaVerdad;
    public GameObject cartaReto;

    public TextMeshProUGUI txtCartaVerdad;
    public TextMeshProUGUI txtCartaReto;

    private List<string> preguntasVerdad = new List<string>();
    private List<string> preguntasReto = new List<string>();

    private RectTransform cartaVerdadRect;
    private RectTransform cartaRetoRect;

    private float duracion = 0.4f;
    private Vector2 centro = Vector2.zero;
    private Vector2 derecha = new Vector2(2000, 0);
    private Vector2 izquierda = new Vector2(-2000, 0);
    public GameObject difuminado; // Opcional, para efectos visuales

    //SHADOW PARA LOS BOTONES CUANDO APARECE EL DIFUMINADO
    [Header("Botón 1")]
    public Button boton1;
    public Color32 sombraPersonalizadaColor1 = new Color32(203, 0, 209, 80); // Ejemplo rosa

    [Header("Botón 2")]
    public Button boton2;
    public Color32 sombraPersonalizadaColor2 = new Color32(0, 150, 255, 80); // Ejemplo azul

    [Header("Sombra Inicial Común")]
    public Color32 sombraInicialColor = new Color32(0, 0, 0, 128);
    public Vector2 sombraInicialDireccion = new Vector2(0f, -10f);

    private Shadow sombraActual;

    private bool bloqueado = false;

    private void Start()
    {
        CargarCSV("CSV/verdad", preguntasVerdad);
        CargarCSV("CSV/reto", preguntasReto);

        cartaVerdadRect = cartaVerdad.GetComponent<RectTransform>();
        cartaRetoRect = cartaReto.GetComponent<RectTransform>();

        cartaVerdad.SetActive(false);
        cartaReto.SetActive(false);
        difuminado.SetActive(false); // Asegurarse de que el difuminado esté desactivado al inicio
        AplicarSombra(boton1, sombraInicialColor, sombraInicialDireccion);
        AplicarSombra(boton2, sombraInicialColor, sombraInicialDireccion);
    }

    private void CargarCSV(string rutaRecursos, List<string> listaDestino)
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

    public void MostrarVerdad()
    {
        if (bloqueado || preguntasVerdad.Count == 0) return;
        StartCoroutine(AnimarCarta("verdad"));
        StartCoroutine(Difuminado());
    }

    public void MostrarReto()
    {
        if (bloqueado || preguntasReto.Count == 0) return;
        StartCoroutine(AnimarCarta("reto"));
        StartCoroutine(Difuminado());
    }

    private IEnumerator AnimarCarta(string tipo)
    {
        bloqueado = true;

        if (tipo == "verdad")
        {
            int index = Random.Range(0, preguntasVerdad.Count);
            txtCartaVerdad.text = preguntasVerdad[index];

            if (cartaVerdad.activeSelf)
            {
                LeanTween.move(cartaVerdadRect, derecha, duracion).setOnComplete(() =>
                {
                    cartaVerdadRect.anchoredPosition = izquierda;
                    LeanTween.move(cartaVerdadRect, centro, duracion);
                });
            }
            else
            {
                if (cartaReto.activeSelf)
                {
                    LeanTween.move(cartaRetoRect, derecha, duracion).setOnComplete(() =>
                    {
                        cartaReto.SetActive(false);
                    });
                }

                cartaVerdad.SetActive(true);
                cartaVerdadRect.anchoredPosition = izquierda;
                LeanTween.move(cartaVerdadRect, centro, duracion);
            }
        }
        else if (tipo == "reto")
        {
            int index = Random.Range(0, preguntasReto.Count);
            txtCartaReto.text = preguntasReto[index];

            if (cartaReto.activeSelf)
            {
                LeanTween.move(cartaRetoRect, derecha, duracion).setOnComplete(() =>
                {
                    cartaRetoRect.anchoredPosition = izquierda;
                    LeanTween.move(cartaRetoRect, centro, duracion);
                });
            }
            else
            {
                if (cartaVerdad.activeSelf)
                {
                    LeanTween.move(cartaVerdadRect, derecha, duracion).setOnComplete(() =>
                    {
                        cartaVerdad.SetActive(false);
                    });
                }

                cartaReto.SetActive(true);
                cartaRetoRect.anchoredPosition = izquierda;
                LeanTween.move(cartaRetoRect, centro, duracion);
            }
        }

        yield return new WaitForSeconds(1f);
        bloqueado = false;
    }
    private IEnumerator Difuminado()
    {
        yield return new WaitForSeconds(0.25f);
        difuminado.SetActive(true);
    }

    private void AplicarSombra(Button boton, Color32 color, Vector2 direccion)
    {
        if (boton == null) return;

        Shadow sombra = boton.GetComponent<Shadow>();
        if (sombra == null)
            sombra = boton.gameObject.AddComponent<Shadow>();

        sombra.effectColor = color;
        sombra.effectDistance = direccion;
    }

    // Llamar cuando aparezca el difuminado (por ejemplo al abrir una ventana o popup)
    public void AplicarSombrasPersonalizadas()
    {
        AplicarSombra(boton1, sombraPersonalizadaColor1, sombraInicialDireccion);
        AplicarSombra(boton2, sombraPersonalizadaColor2, sombraInicialDireccion);
    }

    // Llamar cuando se cierre el difuminado (para restaurar sombra inicial)
    public void RestaurarSombrasIniciales()
    {
        AplicarSombra(boton1, sombraInicialColor, sombraInicialDireccion);
        AplicarSombra(boton2, sombraInicialColor, sombraInicialDireccion);
    }


}
