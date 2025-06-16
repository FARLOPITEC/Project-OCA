using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartupAnimation : MonoBehaviour
{
    public GameObject logo;  
    public GameObject appName; 

   
    public float fadeInDuration = 0.5f;
    public float holdDuration = 0.5f;
    public float fadeOutDuration = 0.5f;
    public float moveDistance = 100f;

    

    void Start()
    {
        logo.SetActive(false);
        //appName.SetActive(false);

        StartLogoAnimation();
        Application.targetFrameRate = 60;
    }

    void StartLogoAnimation()
    {
        logo.SetActive(true);

        CanvasGroup logoCanvasGroup = logo.GetComponent<CanvasGroup>();
        if (logoCanvasGroup == null) logoCanvasGroup = logo.AddComponent<CanvasGroup>();
        logoCanvasGroup.alpha = 0;

        RectTransform logoRect = logo.GetComponent<RectTransform>();
        Vector2 initialLogoPosition = logoRect.anchoredPosition;
        logoRect.anchoredPosition = initialLogoPosition + new Vector2(0, -moveDistance);

        
        LeanTween.alphaCanvas(logoCanvasGroup, 1, fadeInDuration).setEase(LeanTweenType.easeOutQuad);
        LeanTween.move(logoRect, initialLogoPosition, fadeInDuration).setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() =>
            {
                LeanTween.delayedCall(holdDuration, StartLogoFadeOut);
            });
    }

    void StartLogoFadeOut()
    {
        CanvasGroup logoCanvasGroup = logo.GetComponent<CanvasGroup>();
        RectTransform logoRect = logo.GetComponent<RectTransform>();
        Vector2 targetLogoPosition = logoRect.anchoredPosition + new Vector2(0, moveDistance);

        // Desaparición rápida del logo
        LeanTween.alphaCanvas(logoCanvasGroup, 0, fadeOutDuration).setEase(LeanTweenType.easeInQuad);
        LeanTween.move(logoRect, targetLogoPosition, fadeOutDuration).setEase(LeanTweenType.easeInBack)
            .setOnComplete(LoadNextScene);
    }

    /*void StartAppNameAnimation()
    {
        //appName.SetActive(true);

        CanvasGroup appNameCanvasGroup = appName.GetComponent<CanvasGroup>();
        if (appNameCanvasGroup == null) appNameCanvasGroup = appName.AddComponent<CanvasGroup>();
        appNameCanvasGroup.alpha = 0;

        LeanTween.alphaCanvas(appNameCanvasGroup, 1, fadeInDuration).setEase(LeanTweenType.easeOutQuad);

        LeanTween.delayedCall(holdDuration, () =>
        {
           
            LeanTween.alphaCanvas(appNameCanvasGroup, 0, fadeOutDuration).setEase(LeanTweenType.easeInQuad)
                .setOnComplete(LoadNextScene);
        });
    } */

    void LoadNextScene()
    {
        SceneManager.LoadScene("MenuUsuarios");
    }
}
