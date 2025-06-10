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

    private int currentLine = 0;
    private bool isTyping = false;
    private Vector3 originalCharPos;

    void Start()
    {
        // Guardar la posición final del personaje
        originalCharPos = characterImage.rectTransform.anchoredPosition;

        // Colocarlo fuera de pantalla a la izquierda
        characterImage.rectTransform.anchoredPosition = new Vector2(-Screen.width, originalCharPos.y);

        characterImage.gameObject.SetActive(false);
        bubbleImage.gameObject.SetActive(false);

        StartCoroutine(StartDialogue());
    }

    public IEnumerator StartDialogue()
    {
        // Activar imágenes
        characterImage.gameObject.SetActive(true);
        bubbleImage.gameObject.SetActive(true);

        // Deslizar personaje desde la izquierda
        yield return StartCoroutine(SlideInCharacter());

        // Mostrar texto
        yield return StartCoroutine(TypeLine(dialogueLines[currentLine]));
    }

    IEnumerator SlideInCharacter()
    {
        float elapsed = 0f;
        Vector2 startPos = characterImage.rectTransform.anchoredPosition;
        Vector2 endPos = originalCharPos;

        float oscillationAmplitude = 10f; // cuánto se mueve arriba y abajo (en píxeles)
        float oscillationFrequency = 5f;  // velocidad del balanceo

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);

            // Movimiento horizontal lineal
            float posX = Mathf.Lerp(startPos.x, endPos.x, t);

            // Movimiento vertical oscilatorio (senoidal)
            float posY = originalCharPos.y + Mathf.Sin(t * Mathf.PI * oscillationFrequency) * oscillationAmplitude;

            characterImage.rectTransform.anchoredPosition = new Vector2(posX, posY);

            yield return null;
        }

        // Al final, asegurar posición exacta sin oscilación
        characterImage.rectTransform.anchoredPosition = endPos;
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

    public void ShowNextLine()
    {
        if (isTyping) return;

        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            StartCoroutine(TypeLine(dialogueLines[currentLine]));
        }
        else
        {
            // Fin del diálogo
            characterImage.gameObject.SetActive(false);
            bubbleImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }
}