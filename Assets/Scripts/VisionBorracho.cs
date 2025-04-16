using UnityEngine;
using TMPro; // si estás usando TextMeshPro
public class VisionBorracho : MonoBehaviour
{
    public TextMeshProUGUI textMain;
    public TextMeshProUGUI textGhost;

    public float wobbleAmount = 2f;
    public float speed = 5f;

    void Update()
    {
        float x = Mathf.Sin(Time.time * speed) * wobbleAmount;
        float y = Mathf.Cos(Time.time * speed) * wobbleAmount;
        textGhost.rectTransform.anchoredPosition = new Vector2(x, y);
    }
}
