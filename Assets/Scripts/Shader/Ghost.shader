Shader "Unlit/Ghost"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}
        _Minimum("선명도", range(0, 1)) = 0.05
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float2  uv          : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                float2  uv          : TEXCOORD0;
                half2   lightingUV  : TEXCOORD1;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            half4 _MainTex_ST;

            TEXTURE2D(_ShapeLightTexture0);
            SAMPLER(sampler_ShapeLightTexture0);
            half2 _ShapeLightBlendFactors0;
            half4 _ShapeLightMaskFilter0;
            half4 _ShapeLightInvertedFilter0;

            float _Minimum;

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(v.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);

                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 TestLightShared(in SurfaceData2D sur, InputData2D input){
                half alpha = sur.alpha;
                half4 color = half4(sur.albedo, alpha);
                const half4 mask = sur.mask;
                const half2 lightingUV = input.lightingUV;

                half4 shapeLight0 = SAMPLE_TEXTURE2D(_ShapeLightTexture0, sampler_ShapeLightTexture0, lightingUV);

                if (any(_ShapeLightMaskFilter0))
                {
                    half4 processedMask = (1 - _ShapeLightInvertedFilter0) * mask + _ShapeLightInvertedFilter0 * (1 - mask);
                    shapeLight0 *= dot(processedMask, _ShapeLightMaskFilter0);
                }

                half4 shapeLight0Modulate = shapeLight0 * _ShapeLightBlendFactors0.x;
                half4 shapeLight0Additive = shapeLight0 * _ShapeLightBlendFactors0.y;
                
                half4 finalOutput = _HDREmulationScale * (color * shapeLight0Modulate + shapeLight0Additive);

                finalOutput.a = alpha;
                finalOutput = lerp(color, finalOutput, _UseSceneLighting);

                return max(_Minimum, finalOutput);
            }

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                const half4 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                SurfaceData2D surfaceData;
                InputData2D inputData;

                clip(main.a <= 0.0 ? -1:1);

                InitializeSurfaceData(main.rgb, main.a, surfaceData);
                InitializeInputData(i.uv, i.lightingUV, inputData);
                
                return TestLightShared(surfaceData, inputData);
            }
            ENDHLSL
        }
    }
}
