
Shader "Space Conqueror/Force Field"
{
    Properties
    {
        //base attributes
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _Color ("Color", Color) = (1,1,1,1)

        //force field attributes
        _FresnelPower("Fresnel Power", Range(0, 10)) = 3
        _ColoredPercent("Colored Percent", Range(0, 2.5)) = 1
        
        //outline attributes
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 4)) = 0.25

    }
    SubShader
    {
        // This Pass Renders the outline
        Tags { "RenderType"="Geometry" "Queue"="Transparent" }
        LOD 200
        Cull back

        Pass{
            ZWrite Off
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f{
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
            };

            fixed4 _OutlineColor;
            half _OutlineWidth;

            v2f vert(appdata input){
                input.vertex += float4(input.normal * _OutlineWidth, 1);

                v2f output;

                output.pos = UnityObjectToClipPos(input.vertex);
                output.normal = mul(unity_ObjectToWorld, input.normal);

                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                return _OutlineColor;
            }

            ENDCG
        }
        
        
        // This Pass Renders the main texture
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #ifndef SHADER_API_D3D11
                #pragma target 3.0
            #else
                #pragma target 4.0
            #endif

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 position : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)
            
            v2f vert (appdata vert)
            {
                v2f output;

                output.position = UnityObjectToClipPos(vert.vertex);
                output.uv = vert.uv;
                
                return output;
            }
            
            fixed4 frag (v2f input) : SV_Target
            {
                return tex2D(_MainTex,input.uv);
            }
            ENDCG
        }
        
        
        // This Pass Renders the field
	    Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent+1" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        Cull Back
        Lighting Off
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #ifndef SHADER_API_D3D11
                #pragma target 3.0
            #else
                #pragma target 4.0
            #endif

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float rim : TEXCOORD1;
                float4 position : SV_POSITION;
            };

            fixed4 _Color;
            half _FresnelPower;
            half _ColoredPercent;
            
            /*// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)*/
            
            fixed3 viewDir;
            v2f vert (appdata vert)
            {
                v2f output;

                output.position = UnityObjectToClipPos(vert.vertex);
                output.uv = vert.uv;

                viewDir = normalize(ObjSpaceViewDir(vert.vertex));
                output.rim = 1.0 - _ColoredPercent*saturate(dot(viewDir, vert.normal));
                //rim defines zone of colored pixels

                return output;
            }
            
            fixed4 pixel;
            fixed4 frag (v2f input) : SV_Target
            {
                pixel = _Color * pow(_FresnelPower, input.rim);
                pixel = lerp(0, pixel, input.rim);
                return clamp(pixel, 0, _Color);
            }
            ENDCG
        }
        // End Fields Pass
    	
        
   
    }
    FallBack "Diffuse"
}
