Shader "Blit/Blit Overlay"
{
    Properties
    {
        _Opacity ("Overlay Opacity", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100

        Pass
        {
            Name "Blit"
            ZTest Off
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex FullscreenVert
            #pragma fragment Fragment
            #pragma multi_compile_fragment _ _LINEAR_TO_SRGB_CONVERSION
            #pragma multi_compile _ _USE_DRAW_PROCEDURAL

            #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Fullscreen.hlsl"

            float _Opacity;
            TEXTURE2D_X(_BaseTex);
            SAMPLER(sampler_BaseTex);
            TEXTURE2D_X(_OverlayTex);
            SAMPLER(sampler_OverlayTex);

            half4 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = input.uv;

                half4 baseCol = SAMPLE_TEXTURE2D_X(_BaseTex, sampler_BaseTex, uv);
                half4 overlayCol = SAMPLE_TEXTURE2D_X(_OverlayTex, sampler_OverlayTex, uv);

                #ifdef _LINEAR_TO_SRGB_CONVERSION
                sourceCol = LinearToSRGB(sourceCol);
                destCol = LinearToSRGB(destCol);
                #endif

                return baseCol + overlayCol;

                half4 col = lerp(baseCol, overlayCol, overlayCol.a);
                return lerp(baseCol, col, _Opacity);
            }
            ENDHLSL
        }
    }
}
