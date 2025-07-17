Shader "Unlit/KonpeitouShader"
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
            #include "Assets/Shaders/polarcoords.hlsl"
            
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

             half4 tex(float2 st){
                float time = _Time * 0.2;
                float3 circ = float3(pol2xy(float2(time, 0.5)) + 0.5, 1.0);
                st.x / PI + 1.0;
                st.x += time;
                int deg = int(st.x);
                int ind1 = deg % 2;
                int ind2 = (deg + 1) % 2;

                half3 col1 = ind1 == 0 ? circ.rbg : circ.gbr;
                half3 col2 = ind2 == 0 ? circ.rbg : circ.gbr;
                half3 col = lerp(col1, col2, frac(st.x));

                return float4(lerp(circ.brg, col, st.x), 1.0);
                }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv = 2.0 * uv - float2(1.0, 1.0);
                uv = xy2pol(uv);

                return tex(uv);
            }
            ENDHLSL
        }
        
    }
}