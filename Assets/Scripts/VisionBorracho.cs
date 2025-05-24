using UnityEngine;
using TMPro; // si estás usando TextMeshPro
/*public class VisionBorracho : MonoBehaviour
{
    public TextMeshProUGUI textMain;
    public TextMeshProUGUI textGhost;

    public float wobbleAmount = 8f;
    public float speed = 8f;

    void Update()
    {
        float x = Mathf.Sin(Time.time * speed) * wobbleAmount;
        float y = Mathf.Cos(Time.time * speed) * wobbleAmount;
        textGhost.rectTransform.anchoredPosition = new Vector2(x, y);
    }
}*/


public class VisionBorracho : MonoBehaviour
{
    public TextMeshProUGUI textMain;
    public TextMeshProUGUI textGhost;

    /*public float maxOffset = 12f;
    public float speed = 2f;
    public float noiseScale = 1.5f;*/

    public float maxOffset = 20f;
    public float speed = 4f;
    public float noiseScale = 3f;

    private Vector2 originalPosGhost;
    private Vector2 originalPosMain;

    void Start()
    {
        if (textGhost != null)
            originalPosGhost = textGhost.rectTransform.anchoredPosition;

        if (textMain != null)
            originalPosMain = textMain.rectTransform.anchoredPosition;
    }

    void Update()
    {
        float time = Time.time * speed;

        // --- Texto Ghost: movimiento fuerte y transparente ---
        float xGhost = (Mathf.PerlinNoise(time * noiseScale, 0.0f) - 0.5f) * 2f * maxOffset;
        float yGhost = (Mathf.PerlinNoise(0.0f, time * noiseScale) - 0.5f) * 2f * maxOffset;
        textGhost.rectTransform.anchoredPosition = originalPosGhost + new Vector2(xGhost, yGhost);


        float alpha = 0.3f + Mathf.Sin(time * 2f) * 0.3f; // Oscila entre 0.0 y 0.6
        alpha = Mathf.Clamp01(alpha);
        Color ghostColor = textGhost.color;
        ghostColor.a = alpha;
        textGhost.color = ghostColor;



        // --- Texto Main: efectos sutiles ---
        float xMain = (Mathf.PerlinNoise(time * noiseScale + 100f, 0.0f) - 0.5f) * 3f;
        float yMain = (Mathf.PerlinNoise(0.0f, time * noiseScale + 100f) - 0.5f) * 3f;
        textMain.rectTransform.anchoredPosition = originalPosMain + new Vector2(xMain, yMain);

        float scale = 1f + Mathf.Sin(time * 0.5f) * 0.01f; // ±1% zoom
        textMain.rectTransform.localScale = new Vector3(scale, scale, 1f);

        float angle = Mathf.Sin(time * 0.7f) * 0.5f; // ±0.5 grados
        textMain.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}