using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VerdadORetoManager : MonoBehaviour
{
    [Header("Cartas")]
    public GameObject cartaVerdad;
    public GameObject cartaReto;

    [Header("Textos")]
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

    private bool bloqueado = false;

    private void Start()
    {
        CargarCSV("CSV/verdad", preguntasVerdad);
        CargarCSV("CSV/reto", preguntasReto);

        cartaVerdadRect = cartaVerdad.GetComponent<RectTransform>();
        cartaRetoRect = cartaReto.GetComponent<RectTransform>();

        cartaVerdad.SetActive(false);
        cartaReto.SetActive(false);
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
    }

    public void MostrarReto()
    {
        if (bloqueado || preguntasReto.Count == 0) return;
        StartCoroutine(AnimarCarta("reto"));
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
}
