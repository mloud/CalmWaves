Shader "Custom/UIBlurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 1.0
        _Horizontal ("Horizontal Pass", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
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

            sampler2D _MainTex;
            float _BlurSize;
            float _Horizontal;
            float2 _MainTex_TexelSize; // Unity will set this to (1/width, 1/height)

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texOffset = _MainTex_TexelSize * _BlurSize;

                // Gaussian weights for 5 samples
                float weight[5] = {0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216};

                // Apply horizontal or vertical blur based on _Horizontal value
                float2 offset = _Horizontal > 0.5 ? float2(texOffset.x, 0.0) : float2(0.0, texOffset.y);

                // Sample the texture with Gaussian weights
                fixed4 color = tex2D(_MainTex, i.uv) * weight[0];
                for (int j = 1; j < 5; ++j)
                {
                    color += tex2D(_MainTex, i.uv + offset * j) * weight[j];
                    color += tex2D(_MainTex, i.uv - offset * j) * weight[j];
                }

                return color;
            }
            ENDCG
        }
    }
}