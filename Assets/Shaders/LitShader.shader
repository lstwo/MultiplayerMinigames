Shader "lstwo/LitShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.5
        _MetallicMap ("Metallic Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _SmoothnessMap ("Smoothness Map", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _ParallaxMap ("Height Map", 2D) = "black" {}
        _Parallax ("Height Scale", Range(0.005, 0.08)) = 0.02
        _ReflectionColor ("Reflection Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"RenderType"="Opaque"}
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows addshadow

        // Enable reflection probe support
        #pragma shader_feature _GLOSSYREFLECTIONS_ON

        sampler2D _MainTex;
        sampler2D _MetallicMap;
        sampler2D _SmoothnessMap;
        sampler2D _BumpMap;
        sampler2D _ParallaxMap;
        fixed4 _Color;
        half _Glossiness;
        half _Metallic;
        half _Parallax;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MetallicMap;
            float2 uv_SmoothnessMap;
            float2 uv_BumpMap;
            float2 uv_ParallaxMap;
            INTERNAL_DATA // Necessary for reflection probes
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo map
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // Metallic map
            half metallic = tex2D(_MetallicMap, IN.uv_MetallicMap).r;
            o.Metallic = metallic * _Metallic;

            // Smoothness map
            half smoothness = tex2D(_SmoothnessMap, IN.uv_SmoothnessMap).r;
            o.Smoothness = smoothness * _Glossiness;

            // Normal map
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

            // Parallax mapping (height mapping)
            #if _PARALLAXMAP
            float height = tex2D(_ParallaxMap, IN.uv_ParallaxMap).r;
            IN.uv_MainTex += height * _Parallax * IN.viewDir.xy;
            #endif
        }
        ENDCG
    }
    FallBack "Diffuse"
}
