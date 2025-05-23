using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GeneradorRuleta : MonoBehaviour
{
    public RectTransform botella;
    public Transform ruletaTransform;
    public GameObject sectorPrefab;

    public float radio = 300f;
    public float duracionGiro = 3f;

    private GameObject cartaActiva;
    private bool girando = false;

    private Vector2 centro = Vector2.zero;
    private Vector2 derecha = new Vector2(2000, 0);

    void Start()
    {
        GenerarRuleta();
    }

    void GenerarRuleta()
    {
        int total = SeleccionMinijuegos.seleccionCSVs.Count;
        if (total == 0) return;

        float anguloPorSector = 360f / total;
        float anguloActual = 0f;

        for (int i = 0; i < total; i++)
        {
            GameObject sector = Instantiate(sectorPrefab, ruletaTransform);
            sector.name = $"Sector_{i}";
            sector.GetComponent<Image>().color = SeleccionMinijuegos.seleccionColores[i];

            RectTransform rect = sector.GetComponent<RectTransform>();
            rect.localEulerAngles = new Vector3(0, 0, -anguloActual);
            rect.sizeDelta = new Vector2(2 * radio, 2 * radio);

            anguloActual += anguloPorSector;
        }
    }

    public void GirarBotella()
    {
        if (girando || SeleccionMinijuegos.seleccionCSVs.Count == 0) return;
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
            Destroy(cartaActiva);
        }

        int vueltas = Random.Range(3, 5);
        float anguloExtra = Random.Range(0f, 360f);
        float anguloFinal = vueltas * 360f + anguloExtra;

        bool terminado = false;
        LeanTween.rotateZ(botella.gameObject, anguloFinal, duracionGiro).setEaseOutCubic().setOnComplete(() =>
        {
            terminado = true;
        });

        yield return new WaitUntil(() => terminado);
        yield return new WaitForSeconds(0.5f);

        float anguloDetenido = botella.eulerAngles.z % 360f;
        int index = Mathf.FloorToInt(anguloDetenido / (360f / SeleccionMinijuegos.seleccionCSVs.Count));
        index = Mathf.Clamp(index, 0, SeleccionMinijuegos.seleccionCSVs.Count - 1);

        MostrarCarta(index);
        girando = false;
    }

    void MostrarCarta(int index)
    {
        GameObject carta = Instantiate(SeleccionMinijuegos.seleccionPrefabs[index], transform.parent);
        RectTransform cartaRect = carta.GetComponent<RectTransform>();

        carta.SetActive(true);
        cartaRect.anchoredPosition = derecha;
        LeanTween.move(cartaRect, centro, 0.6f).setEaseOutBack();

        string frase = ObtenerFraseAleatoria(SeleccionMinijuegos.seleccionCSVs[index]);
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
