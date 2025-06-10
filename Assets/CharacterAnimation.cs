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

    [Header("Di�logo")]
    [TextArea(2, 5)]
    public string[] dialogueLines;           // L�neas de texto
    public float textSpeed = 0.05f;          // Velocidad del texto
    public float slideDuration = 0.5f;       // Duraci�n de la animaci�n

    private int currentLine = 0;
    private bool isTyping = false;
    private Vector3 originalCharPos;

    void Start()
    {
        // Guardar la posici�n final del personaje
        originalCharPos = characterImage.rectTransform.anchoredPosition;

        // Colocarlo fuera de pantalla a la izquierda
        characterImage.rectTransform.anchoredPosition = new Vector2(-Screen.width, originalCharPos.y);

        characterImage.gameObject.SetActive(false);
        bubbleImage.gameObject.SetActive(false);

        StartCoroutine(StartDialogue());
    }

    public IEnumerator StartDialogue()
    {
        // Activar im�genes
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

        float oscillationAmplitude = 10f; // cu�nto se mueve arriba y abajo (en p�xeles)
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

        // Al final, asegurar posici�n exacta sin oscilaci�n
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
            // Fin del di�logo
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