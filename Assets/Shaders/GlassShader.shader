Shader "lstwo/GlassShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,0.5)
        _Glossiness ("Smoothness", Range(0.0, 1.0)) = 0.9
        _Metallic ("Metallic", Range(0.0, 1.0)) = 0.0
        _Refraction ("Refraction Amount", Range(0.0, 1.0)) = 0.02
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "LightMode"="ForwardBase" }
        LOD 200

        GrabPass { "_GrabTexture" } // This creates a grab pass to sample the screen behind the object

        CGPROGRAM
        #pragma surface surf StandardSpecular alpha:fade

        sampler2D _MainTex;
        fixed4 _Color;
        half _Glossiness;
        half _Metallic;
        float _Refraction;

        sampler2D _GrabTexture; // The screen texture grabbed behind the object

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos; // Needed for screen-space UVs
            float3 viewDir;
            INTERNAL_DATA
        };

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            o.Specular = _Metallic;
            o.Smoothness = _Glossiness;

            // Calculate refraction using the view direction and normal
            float3 refracted = refract(normalize(IN.viewDir), o.Normal, 1.0 - _Refraction);

            // Use screen-space coordinates for refraction
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            screenUV += refracted.xy * _Refraction; // Apply refraction distortion
            fixed4 refractedColor = tex2D(_GrabTexture, screenUV); // Sample the distorted screen texture

            // Combine base color and refracted color
            o.Albedo = lerp(refractedColor.rgb, c.rgb, c.a);

            // Alpha for transparency
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
}
