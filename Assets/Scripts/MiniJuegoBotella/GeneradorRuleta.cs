using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneradorRuleta : MonoBehaviour
{
    public RectTransform botella;
    public RectTransform ruletaTransform;
    public GameObject sectorPrefab;
    public RectTransform contenedorCartas;

    public float duracionGiro = 3f;

    private bool girando = false;
    private List<GameObject> sectores = new List<GameObject>();
    private GameObject cartaActiva;

    void Start()
    {
        CrearRuleta();
    }

    public void CrearRuleta()
    {
        // Elimina sectores anteriores
        foreach (var sector in sectores)
        {
            Destroy(sector);
        }
        sectores.Clear();

        int cantidad = SeleccionMinijuegos.minijuegosSeleccionados.Count;
        if (cantidad == 0) return;

        float anguloSector = 360f / cantidad;

        for (int i = 0; i < cantidad; i++)
        {
            var data = SeleccionMinijuegos.minijuegosSeleccionados[i];
            GameObject sector = Instantiate(sectorPrefab, ruletaTransform);
            sector.transform.localRotation = Quaternion.Euler(0, 0, -i * anguloSector);
            sector.GetComponent<Image>().fillAmount = 1f / cantidad;
            sector.GetComponent<Image>().color = data.color;
            sectores.Add(sector);
        }
    }

    public void GirarBotella()
    {
        if (girando) return;
        StartCoroutine(Girar());
    }

    private IEnumerator Girar()
    {
        girando = true;

        float anguloInicial = botella.eulerAngles.z;
        float anguloFinal = Random.Range(0f, 360f) + 360f * 5;
        float tiempo = 0f;

        while (tiempo < duracionGiro)
        {
            float t = tiempo / duracionGiro;
            float angulo = Mathf.Lerp(anguloInicial, anguloFinal, Mathf.SmoothStep(0f, 1f, t));
            botella.rotation = Quaternion.Euler(0f, 0f, angulo);
            tiempo += Time.deltaTime;
            yield return null;
        }

        botella.rotation = Quaternion.Euler(0f, 0f, anguloFinal % 360f);

        yield return new WaitForSeconds(0.5f);

        float anguloBotella = (360f - botella.eulerAngles.z + 90f) % 360f;
        int cantidad = SeleccionMinijuegos.minijuegosSeleccionados.Count;
        float anguloPorSector = 360f / cantidad;
        int index = Mathf.FloorToInt(anguloBotella / anguloPorSector);

        if (index < 0 || index >= cantidad)
        {
            Debug.LogWarning("Índice fuera de rango: " + index);
            girando = false;
            yield break;
        }

        MostrarCarta(index);
        girando = false;
    }

    private void MostrarCarta(int index)
    {
        if (cartaActiva != null)
        {
            Destroy(cartaActiva);
        }

        var data = SeleccionMinijuegos.minijuegosSeleccionados[index];
        cartaActiva = Instantiate(data.cartaPrefab, contenedorCartas);

        RectTransform rect = cartaActiva.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }

        TextAsset csv = data.archivoCSV;
        if (csv != null)
        {
            string[] lineas = csv.text.Split('\n');
            string frase = lineas[Random.Range(0, lineas.Length)].Trim();

            TextMeshProUGUI txt = cartaActiva.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                txt.text = frase;
            }
        }
    }

}