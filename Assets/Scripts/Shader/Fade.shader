Shader"Unlit/Fade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _Color ("Color", Color) = (0, 0, 0, 1)
        _Mode ("FadeMode", int) = 0
        _Fade ("Fade", range(0, 1.0)) = 0
        _X ("X", range(0, 1.0)) = 0
        _Y ("Y", range(0, 1.0)) = 0
    }
    SubShader
    {
        Tags {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                            UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                            UNITY_VERTEX_INPUT_INSTANCE_ID
    
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _Mode;
            float _Fade;
            float _X, _Y;
            float4 _Color;
            
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float4 col = _Color;
                if (_Mode == 0)
                    col.a = _Fade;
                else if (_Mode == 1)
                {
                    float range = sqrt(pow(i.uv.x - _X, 2) + pow(i.uv.y - _Y, 2) * 9 / 16);
                    //비율 때문에 일정한 크기로 줄어들지 않음 - 비율을 맞춤
                    if (range < (1.0 - _Fade) * sqrt(2))
                        col.a = 0;
                    else
                        col.a = 1;
                }
                        
                return col;
            }
            ENDHLSL
        }
    }
}
