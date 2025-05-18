using UnityEngine;
using UnityEngine.UI;

public class BotellaGiratoria : MonoBehaviour
{
    [Header("Referencia a la imagen de la botella")]
    public RectTransform botella;

    [Header("Cartas que se mostrarán según el ángulo")]
    public GameObject[] cartas; // Deben ser exactamente 6

    [Header("Duración del giro (en segundos)")]
    public float duracionGiro = 3f;

    private bool girando = false;

    // Método público para asignar en el OnClick del botón
    public void GirarBotella()
    {
        if (girando) return;
        girando = true;
        OcultarTodasLasCartas();

        int vueltas = Random.Range(3, 5); // número de vueltas completas
        float anguloExtra = Random.Range(0f, 360f); // ángulo adicional aleatorio
        float anguloFinal = vueltas * 360f + anguloExtra;

        LeanTween.rotateZ(botella.gameObject, anguloFinal, duracionGiro)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                float anguloDetenido = botella.eulerAngles.z % 360f;
                MostrarCartaPorAngulo(anguloDetenido);
                girando = false;
            });
    }

    // Determina qué carta mostrar según el ángulo final
    void MostrarCartaPorAngulo(float angulo)
    {
        if (angulo >= 360f) angulo = 359.99f;

        int cartaIndex = Mathf.FloorToInt(angulo / 60f);
        cartaIndex = Mathf.Clamp(cartaIndex, 0, cartas.Length - 1);

        cartas[cartaIndex].SetActive(true);
        Debug.Log($"Ángulo final: {angulo}° → Mostrando Carta {cartaIndex + 1}");
    }

    // Oculta todas las cartas antes de mostrar la nueva
    void OcultarTodasLasCartas()
    {
        foreach (GameObject carta in cartas)
        {
            carta.SetActive(false);
        }
    }
}
