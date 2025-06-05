Shader "Custom/InteriorSphere" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Cull Front // Aquí es donde invertimos la visibilidad de las caras
        Pass {
            SetTexture [_MainTex] {
                Combine texture * primary
            }
        }
    }
}

