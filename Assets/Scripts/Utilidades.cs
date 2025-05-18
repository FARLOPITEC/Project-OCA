using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utilidades : MonoBehaviour
{
    // Start is called before the first frame update
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

    public void cambarEscena3()
    {
        SceneManager.LoadScene("");
    }

    public void cambarEscena4()
    {
        SceneManager.LoadScene("");
    }

    public void cambarEscena5()
    {
        SceneManager.LoadScene("");

    }

}
