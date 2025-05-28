using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeleccionMinijuegos : MonoBehaviour
{
    [System.Serializable]
    public class MinijuegoUI
    {
        public Toggle toggle;
        public string nombre;
        public Color color;
        public TextAsset archivoCSV;
        public GameObject cartaPrefab;
    }

    public List<MinijuegoUI> minijuegosUI;
    public string escenaRuleta = "MiniJuegoBotella";

    public static List<MinijuegoData> minijuegosSeleccionados = new List<MinijuegoData>();

    public void Jugar()
    {
        minijuegosSeleccionados.Clear();

        foreach (var mj in minijuegosUI)
        {
            if (mj.toggle != null && mj.toggle.isOn)
            {
                MinijuegoData nuevo = new MinijuegoData
                {
                    nombre = mj.nombre,
                    color = mj.color,
                    archivoCSV = mj.archivoCSV,
                    cartaPrefab = mj.cartaPrefab
                };

                minijuegosSeleccionados.Add(nuevo);
            }
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(escenaRuleta);
    }
}
