
Shader "Medieval usurper/Force Field"
{
    Properties
    {
        _FieldTex ("Field Texture", 2D) = "white" {}
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        _Percent("Field percent",Range(0.8,2))=1
        _FresnelPower("Fresnel Power", Range(0, 10)) = 3
    }
    SubShader
    {   
        Tags { 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True" 
            "Queue"="Transparent+1" 
            "RenderPipeline" = "UniversalRenderPipeline"
        }
       Blend SrcAlpha OneMinusSrcAlpha
       LOD 100
       Cull Back
       Lighting Off
       ZWrite On

        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float rim : TEXCOORD1;
                float4 position : SV_POSITION;
            };
            
            sampler2D _FieldTex;

            CBUFFER_START(UnityPerMaterial)
                float4 _FieldTex_ST;
                float4 _Color;
                half _FresnelPower;
                half2 _ScrollDirection;
                float3 viewDir;
                float4 pixel;
                float _Percent;
            CBUFFER_END
            
            

            
            Varyings vert (Attributes vert)
            {
                Varyings output;

                output.position = UnityObjectToClipPos(vert.vertex);
                output.uv = TRANSFORM_TEX(vert.uv, _FieldTex);

                viewDir = normalize(ObjSpaceViewDir(vert.vertex));
                output.rim = 1.0 - _Percent * saturate(dot(viewDir, vert.normal));

                return output;
            }

            float4 frag (Varyings input) : SV_Target
            {
                pixel = tex2D(_FieldTex, input.uv) * _Color * pow(_FresnelPower, input.rim);
                pixel = lerp(0, pixel, input.rim);
                
                return clamp(pixel, 0, _Color);
            }
            ENDHLSL
        }

    }
}
