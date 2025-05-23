using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SeleccionMinijuegos : MonoBehaviour
{
    [System.Serializable]
    public class Minijuego
    {
        public Toggle toggle;
        public TextAsset archivoCSV;
        public Color color;
        public GameObject cartaPrefab;
    }

    public List<Minijuego> minijuegos;
    public string escenaRuleta = "MiniJuegoRuleta";

    public static List<TextAsset> seleccionCSVs = new List<TextAsset>();
    public static List<Color> seleccionColores = new List<Color>();
    public static List<GameObject> seleccionPrefabs = new List<GameObject>();

    public void Jugar()
    {
        seleccionCSVs.Clear();
        seleccionColores.Clear();
        seleccionPrefabs.Clear();

        foreach (var mj in minijuegos)
        {
            if (mj.toggle != null && mj.toggle.isOn)
            {
                seleccionCSVs.Add(mj.archivoCSV);
                seleccionColores.Add(mj.color);
                seleccionPrefabs.Add(mj.cartaPrefab);
            }
        }

        if (seleccionCSVs.Count > 0)
            SceneManager.LoadScene(escenaRuleta);
    }
}
