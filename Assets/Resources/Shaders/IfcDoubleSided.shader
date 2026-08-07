Shader "CauDuong/IFC Double Sided"
{
    Properties
    {
        _BaseColor ("Diffuse Color", Color) = (0.78, 0.8, 0.82, 1)
        _Color ("Legacy Diffuse Color", Color) = (0.78, 0.8, 0.82, 1)
        _IfcSpecColor ("Specular Color", Color) = (0.04, 0.04, 0.04, 1)
        _Smoothness ("Smoothness", Range(0, 1)) = 0.45
        _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _Surface ("Surface", Float) = 0
        [HideInInspector] _SrcBlend ("Source Blend", Float) = 1
        [HideInInspector] _DstBlend ("Destination Blend", Float) = 0
        [HideInInspector] _ZWrite ("Depth Write", Float) = 1
        [HideInInspector] _Cull ("Cull", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }
            Cull [_Cull]
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _Color;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                return _BaseColor;
            }
            ENDHLSL
        }
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        LOD 300
        Cull [_Cull]
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]

        CGPROGRAM
        #pragma surface Surface StandardSpecular fullforwardshadows addshadow keepalpha
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _IfcSpecColor;
        half _Smoothness;

        struct Input
        {
            float2 uv_MainTex;
        };

        void Surface(Input input, inout SurfaceOutputStandardSpecular output)
        {
            fixed4 surfaceSample = tex2D(_MainTex, input.uv_MainTex) * _Color;
            output.Albedo = surfaceSample.rgb;
            output.Specular = _IfcSpecColor.rgb;
            output.Smoothness = _Smoothness;
            output.Alpha = surfaceSample.a;
        }
        ENDCG
    }

    FallBack "Standard"
}
