Shader "lstwo/Skybox"
{
    Properties
    {
        _SkyColor ("Sky Color", Color) = (0.5, 0.8, 1.0, 1.0)
        _HorizonColor ("Horizon Color", Color) = (1.0, 0.5, 0.0, 1.0)
        _NightColor ("Night Color", Color) = (0.0, 0.0, 0.1, 1.0)
        _SunsetColor ("Sunset Color", Color) = (1.0, 0.3, 0.0, 1.0)
        _SunDirection ("Sun Direction", Vector) = (0.0, 1.0, 0.0)
        _BlendFactor ("Time Blend", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags {"Queue" = "Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _SkyColor;
            float4 _HorizonColor;
            float4 _NightColor;
            float4 _SunsetColor;
            float3 _SunDirection;
            float _BlendFactor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 normalizedDir = normalize(i.worldPos);
                float dotProduct = dot(normalizedDir, _SunDirection);

                // Time of day blending
                float4 skyColor = lerp(_SkyColor, _SunsetColor, _BlendFactor);
                float4 nightColor = lerp(_NightColor, _HorizonColor, _BlendFactor);

                // Blending between day and night
                float4 finalColor = lerp(nightColor, skyColor, saturate(dotProduct));

                return finalColor;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
