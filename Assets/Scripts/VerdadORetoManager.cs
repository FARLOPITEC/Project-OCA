using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
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

    private void Start()
    {
        CargarCSV("CSV/verdad", preguntasVerdad);
        CargarCSV("CSV/reto", preguntasReto);

        // Ocultamos ambas cartas al inicio
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
        if (preguntasVerdad.Count == 0) return;

        cartaVerdad.SetActive(true);
        cartaReto.SetActive(false);

        int index = Random.Range(0, preguntasVerdad.Count);
        txtCartaVerdad.text = preguntasVerdad[index];
    }

    public void MostrarReto()
    {
        if (preguntasReto.Count == 0) return;

        cartaVerdad.SetActive(false);
        cartaReto.SetActive(true);

        int index = Random.Range(0, preguntasReto.Count);
        txtCartaReto.text = preguntasReto[index];
    }
}
