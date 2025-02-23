Shader "Custom/PlanarShift"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent-1"
            "RenderPipeline" = "UniversalPipeline"
        }

        Stencil
        {
            Ref 5
            Comp Always
            Pass Replace
        }

        //Blend DstColor Zero
        //Blend Zero SrcColor
        Blend OneMinusDstColor OneMinusSrcAlpha
        //Blend One Zero
        //Blend One One

        ZWrite off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            float3 _Color;
            float _Progress;

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
                float dist = distance(i.uv.xy, float2(0.5, 0.5));
                float prog = _Progress;
                float fwoom = saturate(1 - dist * 2 / prog);
                //clip(fwoom - 0.01);
                float value = saturate(saturate(dist * (100 - (prog * 600))) - dist * 2 / prog);
                value = saturate(sin(sin(dist * dist * 10000 * prog)));
                clip(value - 0.2);
                //value = saturate(1 - dist * 2 / prog);
                //clip(dist > 0.1);
                return value * -0.1; //float4(1, 1, 1, 1); //invLerp(-PI, PI, a) * 100;
            }

            
            ENDHLSL
        }
    }
}