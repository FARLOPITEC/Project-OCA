using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utilidades : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject imgDifuminado;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void cambarEscenaMenuUsuarios()
    {
        SceneManager.LoadScene("MenuUsuarios");
    }

    public void cambarEscenaModosDeJuegos()
    {
        SceneManager.LoadScene("MenuModosDeJuego");
    }

    public void cambarEscenaMenuTableros()
    {
        SceneManager.LoadScene("MenuTableros");
    }

    public void cambarEscenaMenuMiniJuegos()
    {
        SceneManager.LoadScene("MenuMiniJuegos");
    }

    public void cambarEscenaMiniJuegoVerdadReto()
    {
        SceneManager.LoadScene("MiniJuegoVerdadReto");
    }

    public void cambarEscenaMiniJuegoYoNunca()
    {
        SceneManager.LoadScene("MiniJuegoYoNunca");
    }

    public void cambarEscenaMiniJuegoMasProbable()
    {
        SceneManager.LoadScene("MiniJuegoMasProbable");
    }

    public void cambarEscenaMiniJuegoQuienFue()
    {
        SceneManager.LoadScene("MiniJuegoQuienFue");
    }

    public void cambarEscenaMiniJuegoBotella()
    {
        SceneManager.LoadScene("MiniJuegoBotella");
    }

    public void cambarEscenaMiniJuego3Cartas()
    {
        SceneManager.LoadScene("MiniJuego3Cartas");
    }

    public void cambarEscena0()
    {
        SceneManager.LoadScene("");
    }
    public void cambarEscena1()
    {
        SceneManager.LoadScene("");
    }
    public void cambarEscena2()
    {
        SceneManager.LoadScene("");
    }
    public void cambarEscena3()
    {
        SceneManager.LoadScene("");
    }
    public void cambarEscena4()
    {
        SceneManager.LoadScene("");
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }





    public void MostrarPopup(GameObject popup)
    {
        if (popup != null)
            imgDifuminado.SetActive(true);
        popup.SetActive(true);
    }

    public void OcultarPopup(GameObject popup)
    {
        if (popup != null)
            imgDifuminado.SetActive(false);
        popup.SetActive(false);
    }

    public void OcultarPopupDerecha(GameObject popup)
    {
        if (popup != null)
        {
            RectTransform rect = popup.GetComponent<RectTransform>();
            Vector3 destino = rect.anchoredPosition + new Vector2(2000, 0); 

            LeanTween.move(rect, destino, 0.4f).setOnComplete(() =>
            {
                imgDifuminado.SetActive(false);
                popup.SetActive(false);
            });
        }
    }


}
