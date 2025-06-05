using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class CaeYDesaparece : MonoBehaviour
{

    public GameObject prefab; // Prefab a instanciar
    public float fallSpeed = 2f; // Velocidad de caída
    public float rotationSpeed = 200f; // Velocidad de rotacion
    public float resetHeight = -5f; // Altura mínima antes de desaparecer
    public Material[] materials; // Array de materiales disponibles
    public Vector3[] spawnPoints; // Lista de puntos de aparición
    public float spawnDelay = 1f; // Tiempo de espera antes de instanciar cada objeto

    public GameObject contenedorPadre; // Este será el padre de los objetos instanciados


    private GameObject[] instances;

    void Awake()
    {
        instances = new GameObject[spawnPoints.Length];
        StartCoroutine(SpawnPrefabsWithDelay());
    }

    void Update()
    {
        for (int i = 0; i < instances.Length; i++)
        {
            if (instances[i] != null)
            {
                // Movimiento de caída
                instances[i].transform.position += Vector3.down * fallSpeed * Time.deltaTime;

                // Rotación sobre su eje Z (no usa Vector3.forward)
                instances[i].transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

                // Si toca el límite, destruir y reaparecer con un nuevo material
                if (instances[i].transform.position.y < resetHeight)
                {
                    Destroy(instances[i]);
                    StartCoroutine(SpawnPrefabDelayed(i, spawnPoints[i], spawnDelay));
                }
            }
        }
    }

    IEnumerator SpawnPrefabsWithDelay()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            yield return new WaitForSeconds(spawnDelay);
            instances[i] = SpawnPrefabAt(spawnPoints[i] + new Vector3(5000, 0, 0));
        }
    }

    IEnumerator SpawnPrefabDelayed(int index, Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);
        instances[index] = SpawnPrefabAt(position); // Se actualiza correctamente en la lista
    }

    GameObject SpawnPrefabAt(Vector3 position)
    {

        // Convertir spawnPoints[i] a coordenadas dentro del contenedor
        Vector3 spawnPosition = contenedorPadre.transform.TransformPoint(position);

        // Instanciar el objeto dentro del contenedor
        GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity, contenedorPadre.transform);

        // Ajustar localPosition para que el objeto aparezca correctamente dentro del contenedor
        instance.transform.localPosition = position;
        // Cambiar material aleatoriamente
        Renderer renderer = instance.GetComponentInChildren<Renderer>();

        if (renderer != null && materials.Length > 0)
        {
            renderer.material = materials[Random.Range(0, materials.Length)];


        }

        return instance;
    }
}



