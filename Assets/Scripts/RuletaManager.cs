using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BotellaGiratoria : MonoBehaviour
{
    public RectTransform botella;
    public float duracionGiro = 3f;

    public GameObject[] cartasEnEscena;

    public TextAsset[] archivosCSV;

    private GameObject cartaActiva;
    private bool girando = false;

    private Vector2 centro;
    private Vector2 izquierda;
    private Vector2 derecha;

    public void GirarBotella()
    {
        if (girando) return;
        StartCoroutine(FlujoGiro());
    }

    IEnumerator FlujoGiro()
    {
        girando = true;

        if (cartaActiva != null)
        {
            RectTransform cartaRect = cartaActiva.GetComponent<RectTransform>();

            LeanTween.move(cartaRect, derecha, 0.6f).setEaseInOutCubic();
            yield return new WaitForSeconds(0.6f);
            cartaActiva.SetActive(false);
        }

        int vueltas = Random.Range(3, 5);
        float anguloExtra = Random.Range(0f, 360f);
        float anguloFinal = vueltas * 360f + anguloExtra;

        bool giroTerminado = false;
        LeanTween.rotateZ(botella.gameObject, anguloFinal, duracionGiro).setEaseOutCubic().setOnComplete(() =>
        {
            giroTerminado = true;
        });

        yield return new WaitUntil(() => giroTerminado);
        yield return new WaitForSeconds(1f);

        float anguloDetenido = botella.eulerAngles.z % 360f;
        int index = Mathf.FloorToInt(anguloDetenido / 60f);
        index = Mathf.Clamp(index, 0, cartasEnEscena.Length - 1);

        MostrarCarta(index);

        girando = false;
    }

    void MostrarCarta(int index)
    {
        if (index < 0 || index >= cartasEnEscena.Length || index >= archivosCSV.Length)
        {
            Debug.LogError("Índice fuera de rango");
            return;
        }

        GameObject carta = cartasEnEscena[index];
        RectTransform cartaRect = carta.GetComponent<RectTransform>();

        carta.SetActive(true);

        centro = new Vector2(0f, cartaRect.anchoredPosition.y);
        izquierda = centro + Vector2.left * 1000f;
        derecha = centro + Vector2.right * 1000f;

        cartaRect.anchoredPosition = izquierda;
        LeanTween.move(cartaRect, centro, 0.6f).setEaseOutBack();

        string frase = ObtenerFraseAleatoria(archivosCSV[index]);
        TextMeshProUGUI texto = carta.GetComponentInChildren<TextMeshProUGUI>();
        if (texto != null) texto.text = frase;

        cartaActiva = carta;
    }

    string ObtenerFraseAleatoria(TextAsset csv)
    {
        if (csv == null) return "";
        string[] lineas = csv.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lineas.Length == 0) return "";
        return lineas[Random.Range(0, lineas.Length)];
    }
}
