Shader "Custom/NormalBullet"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent+1"
            "RenderPipeline" = "UniversalPipeline"
        }

        Stencil
        {
            Ref 5
            Comp Equal
            //Pass Replace
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"



            struct Attributes
            {
                float4 positionOS  : POSITION;
                float2 uv          : TEXCOORD0;
                float3 normal      : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 normal      : TEXCOORD1;
                float3 viewDir     : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;


            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = TransformObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - TransformObjectToWorld(v.positionOS.xyz));
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float height = i.positionHCS.y / _ScreenParams.y;
                float width = i.positionHCS.x / _ScreenParams.x;
                float4 col = tex2D(_MainTex, fmod(float2(width, height) + _Time.y, 1));
                float fresnel = dot(i.normal, i.viewDir);
                
                col *= 1 + pow(fresnel, 2) * 0.8;
                return col * _Color;
            }
            ENDHLSL
        }
    }
}