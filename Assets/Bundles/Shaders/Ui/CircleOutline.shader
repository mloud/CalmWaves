Shader "UI/CircleOutline"
{
    Properties
    {
        _Radius ("Radius", Range(0, 1)) = 0.4
        _Thickness ("Thickness", Range(0, 0.5)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            float _Radius;
            float _Thickness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color; // Pass the vertex color to the fragment shader
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Centered UV coordinates
                float2 uv = i.uv - 0.5;

                // Calculate distance from the center
                float dist = length(uv);

                // Outline calculation with hard edge
                float outline = (dist < _Radius - _Thickness || dist > _Radius + _Thickness) ? 0 : 1;

                // Use the UI component’s color with outline factor
                return fixed4(i.color.rgb, i.color.a * outline);
            }
            ENDCG
        }
    }
}