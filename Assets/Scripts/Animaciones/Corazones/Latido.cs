using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Latido : MonoBehaviour
{
    public AnimationCurve beatCurve; // Se puede editar en el Inspector
    public float beatSpeed = 0.5f;
    public float scaleAmount = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PingPong(Time.time * beatSpeed, 1f);
        float scale = 2f + beatCurve.Evaluate(t) * scaleAmount;
        transform.localScale = new Vector3(scale, scale, scale);

    }


}
