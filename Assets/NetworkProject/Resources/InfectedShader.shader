Shader "Custom/InfectedShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _RedAmount ("Red Channel", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float4 _BaseColor;
            float _RedAmount;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 col = _BaseColor;
                col.r = saturate(_RedAmount);
                return col;
            }
            ENDHLSL
        }
    }
}