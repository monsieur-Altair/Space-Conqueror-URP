Shader "Medieval usurper/Base Shader"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}

        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 4)) = 0.25
    }
    SubShader
    {
        
        Tags 
        { 
            "RenderType"="Geometry" 
            "Queue"="Transparent" 
            "RenderPipeline" = "UniversalRenderPipeline"
        }
        
        LOD 200
        Cull back

        Pass{
            Tags { "LightMode"="SRPDefaultUnlit" }
            ZWrite Off
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct Varyings{
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                half _OutlineWidth;
            CBUFFER_END            

            Varyings vert(Attributes input){
                input.vertex += float4(input.normal * _OutlineWidth, 1);

                Varyings output;

                output.pos = TransformObjectToHClip(input.vertex);
                output.normal = mul(unity_ObjectToWorld, input.normal);

                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                return _OutlineColor;
            }

            ENDHLSL
        }
        
        
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalRenderPipeline" 
        }
        
        Pass{
            Tags { "LightMode"="UniversalForward" }
            

            ZWrite On
            
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return color;
            }
            ENDHLSL
        }
    }
}
