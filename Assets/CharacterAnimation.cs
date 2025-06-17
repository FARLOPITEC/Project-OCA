using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAnimation : MonoBehaviour
{
    [Header("UI Elements")]
    public Image characterImage;             // Imagen del personaje
    public Image bubbleImage;                // Imagen del bocadillo
    public TextMeshProUGUI dialogueText;     // Texto dentro del bocadillo

    [Header("Diálogo")]
    [TextArea(2, 5)]
    public string[] dialogueLines;           // Líneas de texto
    public float textSpeed = 0.05f;          // Velocidad del texto
    public float slideDuration = 0.5f;       // Duración de la animación
    public float waitBetweenLines = 15f;     // Tiempo entre frases

    private int currentLine = 0;
    private bool isTyping = false;
    private Vector3 originalCharPos;
    private Vector3 originalBubblePos;

    void Start()
    {
        // Guardar posiciones originales
        originalCharPos = characterImage.rectTransform.anchoredPosition;
        originalBubblePos = bubbleImage.rectTransform.anchoredPosition;

        // Colocar ambos fuera de pantalla a la izquierda
        characterImage.rectTransform.anchoredPosition = new Vector2(-Screen.width, originalCharPos.y);
        bubbleImage.rectTransform.anchoredPosition = new Vector2(-Screen.width, originalBubblePos.y);

        characterImage.gameObject.SetActive(false);
        bubbleImage.gameObject.SetActive(false);

        StartCoroutine(StartDialogue());
    }

    public IEnumerator StartDialogue()
    {
        // Activar personaje y bocadillo al mismo tiempo (pero sin texto aún)
        characterImage.gameObject.SetActive(true);
        bubbleImage.gameObject.SetActive(true);
        dialogueText.text = ""; // Asegurarse de que no se muestre texto antes de tiempo

        // Esperar a que termine el movimiento del personaje y bocadillo
        yield return StartCoroutine(SlideInCharacter());

        // Mostrar todas las líneas en bucle infinito
        while (true)
        {
            yield return StartCoroutine(TypeLine(dialogueLines[currentLine]));

            currentLine = (currentLine + 1) % dialogueLines.Length;

            yield return new WaitForSeconds(waitBetweenLines);
        }
    }

    IEnumerator SlideInCharacter()
    {
        float elapsed = 0f;
        Vector2 startCharPos = characterImage.rectTransform.anchoredPosition;
        Vector2 startBubblePos = bubbleImage.rectTransform.anchoredPosition;
        Vector2 endCharPos = originalCharPos;
        Vector2 endBubblePos = originalBubblePos;

        float oscillationAmplitude = 10f; // cuánto se mueve arriba y abajo (en píxeles)
        float oscillationFrequency = 5f;  // velocidad del balanceo

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);

            // Movimiento horizontal lineal
            float posXChar = Mathf.Lerp(startCharPos.x, endCharPos.x, t);
            float posXBubble = Mathf.Lerp(startBubblePos.x, endBubblePos.x, t);

            // Movimiento vertical oscilatorio (senoidal)
            float posYChar = originalCharPos.y + Mathf.Sin(t * Mathf.PI * oscillationFrequency) * oscillationAmplitude;
            float posYBubble = originalBubblePos.y + Mathf.Sin(t * Mathf.PI * oscillationFrequency) * oscillationAmplitude;

            characterImage.rectTransform.anchoredPosition = new Vector2(posXChar, posYChar);
            bubbleImage.rectTransform.anchoredPosition = new Vector2(posXBubble, posYBubble);

            yield return null;
        }

        // Asegurar posición exacta al terminar
        characterImage.rectTransform.anchoredPosition = endCharPos;
        bubbleImage.rectTransform.anchoredPosition = endBubblePos;
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }
}
