Shader "Custom/Gradient"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"}

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        //Blend One Zero
        //Blend One One

        ZWrite off
        //Cull off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            float3 _Color;

            struct Attributes
            {
                float4 positionOS  : POSITION;
                float2 uv          : TEXCOORD0;
                float3 normal      : NORMAL;
                float4 color       : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                return float4(i.color.rgb, i.color.a * i.uv.x * i.uv.x); //invLerp(-PI, PI, a) * 100;
            }

            
            ENDHLSL
        }
    }
}