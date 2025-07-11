Shader "Custom/OhajikiShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 5)) = 1
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _AlphaMax ("Alpha Max", Range(0, 5)) = 1
    }

    SubShader
    {
        Tags {
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "Queue" = "Transparent"
        }
        LOD 100
        
        Pass
        {
            Tags{ "LightMode" = "UniversalGBuffer" }
            ZWrite On
            ColorMask 0
        }
        
        // ŠePass‚Åcbuffer‚ª•Ï‚í‚ç‚È‚¢‚æ‚¤‚É‚±‚±‚É’è‹`‚·‚é
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float _Alpha;
        half _RimPower;
        float4 _RimColor;
        float4 _OutlineColor;
        CBUFFER_END
        ENDHLSL
        
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" "Queue" = "Transparent"}
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Assets/Shaders/wavenoise.hlsl"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float fogFactor: TEXCOORD1;
                float3 posWS : TEXCOORD2;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD3;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);    // <-
                o.posWS = TransformObjectToWorld(v.vertex);     // <-
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
       ;        VertexNormalInputs normalInput = GetVertexNormalInputs(v.normal, v.tangent);
                o.normal = normalInput.normalWS;
                
                o.fogFactor = ComputeFogFactor(o.vertex.z);     // <-
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                half3 viewDir = GetWorldSpaceViewDir(i.posWS);
                half NdotV = max(dot(normalize(viewDir), i.normal), 0);
                
                float2 p0, p1, p2;
                float2 i0, i1, i2;

                float2 p = i.uv * 32.0 - 16.0;
                hexgrid(p, p0, i0, p1, i1, p2, i2);

                float2 g;
                float2 d = float3(0.0, 0.0, 0.0);
                float n = 0.5 + 0.5*psrdnoise(p*0.05+d, float2(0.0,0.0), 0.2, g);

                float A = length(g);
                A = max(0.0, A-0.3);
                g = normalize(g);

                float3 ph = hashphase(float3(i0.x, i1.x, i2.x), float3(i0.y, i1.y, i2.y));

                float alpha = 1.0;

                float w0 = wavelet(p, p0, g, ph.x + alpha);
	            float w1 = wavelet(p, p1, g, ph.y + alpha);
	            float w2 = wavelet(p, p2, g, ph.z + alpha);

                float4 col;
                float rim = saturate(abs(dot(i.normal, float3(0, 1, 0))) * NdotV + 0.3);
                col.a = saturate(_Alpha);

                float f = 0.3*A*(w0 + w1 + w2) + 0.1;

                float3 bg = float3(0.0, n*0.5, n);
                float3 fg = float3(1.0, 1.0, 1.0);
                
                float3 mixcolor = lerp(bg, fg, f);
                col.rgb = MixFog(col.rgb, i.fogFactor); // <-
                col.rgb = mixcolor * _RimColor;
                return col;
            }
            ENDHLSL
        }
        
    }
}