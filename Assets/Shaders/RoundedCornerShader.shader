Shader "Custom/RoundedCornerShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Asegúrate de que esta textura sea blanca por defecto
        _CornerRadius ("Corner Radius", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Front
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex; // Usa una textura
            fixed4 _MainTexColor; // Color de la textura
            float _CornerRadius;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Obtén el color de la textura
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Asegúrate de que el color sea visible (no blanco)
                if (texColor.a == 0) // Si la textura es transparente, devuelve un color negro
                    return fixed4(0, 0, 0, 0); 

                float2 uv = i.uv * 2 - 1; // Convertir a rango [-1, 1]
                float dist = max(abs(uv.x), abs(uv.y));
                float alpha = smoothstep(_CornerRadius, _CornerRadius + 0.1, dist);
                texColor.a *= 1 - alpha; // Aplicar esquinas redondeadas

                return texColor; // Devuelve el color de la textura con bordes redondeados
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
