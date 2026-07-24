Shader "CauDuong/IFC Double Sided"
{
    Properties
    {
        _Color ("Diffuse Color", Color) = (0.78, 0.8, 0.82, 1)
        _IfcSpecColor ("Specular Color", Color) = (0.04, 0.04, 0.04, 1)
        _Smoothness ("Smoothness", Range(0, 1)) = 0.45
        _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _SrcBlend ("Source Blend", Float) = 1
        [HideInInspector] _DstBlend ("Destination Blend", Float) = 0
        [HideInInspector] _ZWrite ("Depth Write", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        LOD 300
        Cull Off
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
