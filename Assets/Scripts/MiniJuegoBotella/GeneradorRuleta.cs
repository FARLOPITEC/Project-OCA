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
    private Coroutine corutinaDifuminado;

    [Header("Difuminado al mostrar carta")]
    public GameObject difuminado;

    public float duracionGiro = 3f;

    private bool girando = false;
    private List<GameObject> sectores = new List<GameObject>();
    private GameObject cartaActiva;

    void Start()
    {
        CrearRuleta();
        difuminado.SetActive(false); // Asegurarse de que el difuminado est� desactivado al inicio
    }

    public void CrearRuleta()
    {
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

        // Apagar difuminado justo al comenzar el giro (antes de animar la carta fuera)
        if (difuminado.activeSelf)
        {
            if (corutinaDifuminado != null)
            {
                StopCoroutine(corutinaDifuminado);
                corutinaDifuminado = null;
            }
            StartCoroutine(DesactivarDifuminadoConDelay());
        }


        if (cartaActiva != null)
        {
            RectTransform rect = cartaActiva.GetComponent<RectTransform>();
            if (rect != null)
            {
                LeanTween.moveX(rect, Screen.width, 0.4f).setEaseInExpo().setOnComplete(() =>
                {
                    Destroy(cartaActiva);
                    cartaActiva = null;
                    StartCoroutine(Girar());
                    
                });
                return;
            }
            else
            {
                Destroy(cartaActiva);
                cartaActiva = null;
            }
        }

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

        Vector2 direccion = botella.up;
        float anguloBotella = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        anguloBotella = (450f - anguloBotella) % 360f;

        int cantidad = SeleccionMinijuegos.minijuegosSeleccionados.Count;
        float anguloPorSector = 360f / cantidad;
        int index = Mathf.FloorToInt(anguloBotella / anguloPorSector);

        MostrarCarta(index);
        girando = false;
    }

    private void MostrarCarta(int index)
    {
       

        //Destruir carta anterior si existe
        if (cartaActiva != null)
        {
            Destroy(cartaActiva);
            cartaActiva = null;
        }



        foreach (Transform child in contenedorCartas)
            Destroy(child.gameObject);

        var data = SeleccionMinijuegos.minijuegosSeleccionados[index];

        cartaActiva = Instantiate(data.cartaPrefab, contenedorCartas);
        RectTransform rect = cartaActiva.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(-Screen.width, 0);
            rect.localScale = Vector3.one;

            LeanTween.moveX(rect, 0f, 0.5f).setEaseOutExpo();

            MostrarConDelay();
        }

        if (data.archivoCSV != null)
        {
            string[] lineas = data.archivoCSV.text.Split('\n');
            string frase = lineas[Random.Range(0, lineas.Length)].Trim();

            var txt = cartaActiva.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
                txt.text = frase;
        }

        Button btn = cartaActiva.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                RectTransform r = cartaActiva.GetComponent<RectTransform>();
                if (r != null)
                {
                    LeanTween.moveX(r, Screen.width, 0.4f).setEaseInExpo().setOnComplete(() =>
                    {
                        if (corutinaDifuminado != null)
                        {
                            StopCoroutine(corutinaDifuminado);
                            corutinaDifuminado = null;
                        }
                        StartCoroutine(DesactivarDifuminadoConDelay());

                        Destroy(cartaActiva);
                        cartaActiva = null;
                    });
                }
            });
        }
    }

    public void MostrarConDelay()
    {
        if (corutinaDifuminado != null)
        {
            StopCoroutine(corutinaDifuminado);
            corutinaDifuminado = null;
        }

        corutinaDifuminado = StartCoroutine(MostrarDespuesDeDelay());
    }

    IEnumerator MostrarDespuesDeDelay()
    {
        yield return new WaitForSeconds(0.1f);  // Puedes ajustar el delay si quieres
        difuminado.SetActive(true);
    }

    IEnumerator DesactivarDifuminadoConDelay()
    {
        yield return new WaitForSeconds(0.2f);
        difuminado.SetActive(false);
    }

}
