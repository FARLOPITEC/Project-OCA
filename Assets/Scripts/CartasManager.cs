using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CartasManager : MonoBehaviour
{
    [Header("Elementos de la carta")]
    public GameObject carta;                  // Objeto de la carta
    public TextMeshProUGUI textoCarta;        // Texto dentro de la carta
    public TextAsset archivoCSV;              // CSV con las frases

    private List<string> frases = new List<string>();
    private RectTransform cartaRect;

    private float duracion = 0.4f;
    private Vector2 centro = Vector2.zero;
    private Vector2 derecha = new Vector2(2000, 0);
    private Vector2 izquierda = new Vector2(-2000, 0);

    private bool bloqueado = false;

    private void Start()
    {
        CargarCSV();
        cartaRect = carta.GetComponent<RectTransform>();
        carta.SetActive(false);
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
            cartaRect.anchoredPosition = izquierda;
            LeanTween.move(cartaRect, centro, duracion);
        }

        yield return new WaitForSeconds(1f);
        bloqueado = false;
    }
}
