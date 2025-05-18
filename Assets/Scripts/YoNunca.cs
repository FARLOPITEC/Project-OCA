using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModoYoNuncaManager : MonoBehaviour
{
    [Header("Carta Yo Nunca")]
    public GameObject cartaYoNunca;
    public TextMeshProUGUI txtCartaYoNunca;

    private List<string> frasesYoNunca = new List<string>();
    private RectTransform cartaYoNuncaRect;

    private float duracion = 0.4f;
    private Vector2 centro = Vector2.zero;
    private Vector2 derecha = new Vector2(2000, 0);
    private Vector2 izquierda = new Vector2(-2000, 0);

    private bool bloqueado = false;

    private void Start()
    {
        CargarCSV("CSV/YoNunca", frasesYoNunca);
        cartaYoNuncaRect = cartaYoNunca.GetComponent<RectTransform>();
        cartaYoNunca.SetActive(false);
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

    public void MostrarYoNunca()
    {
        if (bloqueado || frasesYoNunca.Count == 0) return;
        StartCoroutine(AnimarCartaYoNunca());
    }

    private IEnumerator AnimarCartaYoNunca()
    {
        bloqueado = true;

        int index = Random.Range(0, frasesYoNunca.Count);
        txtCartaYoNunca.text = frasesYoNunca[index];

        if (cartaYoNunca.activeSelf)
        {
            LeanTween.move(cartaYoNuncaRect, derecha, duracion).setOnComplete(() =>
            {
                cartaYoNuncaRect.anchoredPosition = izquierda;
                LeanTween.move(cartaYoNuncaRect, centro, duracion);
            });
        }
        else
        {
            cartaYoNunca.SetActive(true);
            cartaYoNuncaRect.anchoredPosition = izquierda;
            LeanTween.move(cartaYoNuncaRect, centro, duracion);
        }

        yield return new WaitForSeconds(1f);
        bloqueado = false;
    }
}
