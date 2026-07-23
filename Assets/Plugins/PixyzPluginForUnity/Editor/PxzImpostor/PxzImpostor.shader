// Upgrade NOTE: replaced 'defined FOG_COMBINED_WITH_WORLD_POS' with 'defined (FOG_COMBINED_WITH_WORLD_POS)'

Shader "Pixyz/PxzImpostor"
{
   Properties
   {
       [NoScaleOffset] _PxzImpostorAlbedoOcclusion("Impostor Albedo & Occlusion ", 2D) = "white" {}
       [NoScaleOffset] _PxzImpostorSpecularRoughness("Impostor Specular & Smoothness ", 2D) = "white" {}
       [NoScaleOffset] _PxzImpostorNormals("Impostor Normal", 2D) = "white" {}
       [NoScaleOffset] _PxzImpostorEmission("Impostor Emission", 2D) = "black" {}
       [NoScaleOffset] _PxzImpostorDepth("Impostor Depth", 2D) = "white" {}
       _PxzImpostorFramesCount("FramesXY", Float) = 0
       [HideInInspector]_OctahedronDiameter("Impostor Size", Float) = 1
       [Toggle] _FullOcta("Full Octahedron", Float) = 1
       _Pxz_Clip("Impostor Clip", Range(0.0 , 1.0)) = 0.75
       _Linear("Linear Search Steps", Float) = 30
       _Binary("Binary Search Steps", Float) = 1
       _ShadowBias("Shadow Bias", Range(0.0, 10)) = 0.8
       [Toggle] _PreciseMesh("PreciseMesh", Float) = 0
       [HideInInspector] _PrecisionDepth("_PrecisionDepth", Float) = 1
   }
    SubShader
    {
        CGINCLUDE
          #define UNITY_SAMPLE_FULL_SH_PER_PIXEL 1
        ENDCG


        Tags { "RenderType" = "Opaque" "DisableBatching" = "True"}
        LOD 200
        ZWrite On

        
	// ------------------------------------------------------------
	// Surface shader code generated out of a CGPROGRAM block:
	

	// ---- forward rendering base pass:
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma target 3.0 LOD_FADE_CROSSFADE
#pragma target 3.0
#pragma multi_compile_instancing
#pragma multi_compile __ LOD_FADE_CROSSFADE
#pragma multi_compile_fog
#pragma multi_compile_fwdbase
#include "HLSLSupport.cginc"
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_SH
#define UNITY_INSTANCED_LIGHTMAPSTS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: <when no other keywords are defined>
#if !defined(INSTANCING_ON) && !defined(LOD_FADE_CROSSFADE)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_TSPACE
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD6; // SH
  #endif
  UNITY_LIGHTING_COORDS(7,8)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD6; // SH
  #endif
  UNITY_FOG_COORDS(7)
  UNITY_SHADOW_COORDS(8)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_TSPACE
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  float4 lmap : TEXCOORD6;
  UNITY_LIGHTING_COORDS(7,8)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  float4 lmap : TEXCOORD6;
  UNITY_FOG_COORDS(7)
  UNITY_SHADOW_COORDS(8)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
   
  #endif
  UNITY_TRANSFER_FOG(o, o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
  UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);
  o.Normal = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o);
  IN.pos.xy = surfIN.viewPos.xy; // fetch back clipPos.xy
  float3 worldPos = surfIN.worldPos;
  outDepth = surfIN.outDepth;

  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
  
  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;
  //float3 worldN;
  //worldN.x = dot(_unity_tbn_0, o.Normal);
  //worldN.y = dot(_unity_tbn_1, o.Normal);
  //worldN.z = dot(_unity_tbn_2, o.Normal);
  //worldN = normalize(worldN);
  //o.Normal = worldN;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.worldViewDir = worldViewDir;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  //#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    //giInput.ambient = IN.sh;
  //#else
    giInput.ambient.rgb = 0.0;
  //#endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingStandard_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingStandard (o, worldViewDir, gi);
  c.rgb += o.Emission;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: INSTANCING_ON 
#if defined(INSTANCING_ON) && !defined(LOD_FADE_CROSSFADE)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_TSPACE
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD6; // SH
  #endif
  UNITY_LIGHTING_COORDS(7,8)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD6; // SH
  #endif
  UNITY_FOG_COORDS(7)
  UNITY_SHADOW_COORDS(8)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_TSPACE
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  float4 lmap : TEXCOORD6;
  UNITY_LIGHTING_COORDS(7,8)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  float4 lmap : TEXCOORD6;
  UNITY_FOG_COORDS(7)
  UNITY_SHADOW_COORDS(8)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);
  o.Normal = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o);
  IN.pos.xy = surfIN.viewPos.xy; // fetch back clipPos.xy
  float3 worldPos = surfIN.worldPos;
  outDepth = surfIN.outDepth;

#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;
  //float3 worldN;
  //worldN.x = dot(_unity_tbn_0, o.Normal);
  //worldN.y = dot(_unity_tbn_1, o.Normal);
  //worldN.z = dot(_unity_tbn_2, o.Normal);
  //worldN = normalize(worldN);
  //o.Normal = worldN;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.worldViewDir = worldViewDir;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingStandard_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingStandard (o, worldViewDir, gi);
  c.rgb += o.Emission;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: LOD_FADE_CROSSFADE 
#if defined(LOD_FADE_CROSSFADE) && !defined(INSTANCING_ON)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_TSPACE
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD6; // SH
  #endif
  UNITY_LIGHTING_COORDS(7,8)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD6; // SH
  #endif
  UNITY_FOG_COORDS(7)
  UNITY_SHADOW_COORDS(8)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_TSPACE
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  float4 lmap : TEXCOORD6;
  UNITY_LIGHTING_COORDS(7,8)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  float4 lmap : TEXCOORD6;
  UNITY_FOG_COORDS(7)
  UNITY_SHADOW_COORDS(8)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);
  o.Normal = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o);
  IN.pos.xy = surfIN.viewPos.xy; // fetch back clipPos.xy
  float3 worldPos = surfIN.worldPos;
  outDepth = surfIN.outDepth;

#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;
 /* float3 worldN;
  worldN.x = dot(_unity_tbn_0, o.Normal);
  worldN.y = dot(_unity_tbn_1, o.Normal);
  worldN.z = dot(_unity_tbn_2, o.Normal);
  worldN = normalize(worldN);
  o.Normal = worldN;*/

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.worldViewDir = worldViewDir;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingStandard_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingStandard (o, worldViewDir, gi);
  c.rgb += o.Emission;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: LOD_FADE_CROSSFADE INSTANCING_ON 
#if defined(LOD_FADE_CROSSFADE) && defined(INSTANCING_ON)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_TSPACE
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD6; // SH
  #endif
  UNITY_LIGHTING_COORDS(7,8)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD6; // SH
  #endif
  UNITY_FOG_COORDS(7)
  UNITY_SHADOW_COORDS(8)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_TSPACE
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  float4 lmap : TEXCOORD6;
  UNITY_LIGHTING_COORDS(7,8)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
  float4 lmap : TEXCOORD6;
  UNITY_FOG_COORDS(7)
  UNITY_SHADOW_COORDS(8)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);
  o.Normal = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o);
  IN.pos.xy = surfIN.viewPos.xy; // fetch back clipPos.xy
  float3 worldPos = surfIN.worldPos;
  outDepth = surfIN.outDepth;

#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;
  //float3 worldN;
  //worldN.x = dot(_unity_tbn_0, o.Normal);
  //worldN.y = dot(_unity_tbn_1, o.Normal);
  //worldN.z = dot(_unity_tbn_2, o.Normal);
  //worldN = normalize(worldN);
  //o.Normal = worldN;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.worldViewDir = worldViewDir;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingStandard_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingStandard (o, worldViewDir, gi);
  c.rgb += o.Emission;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif


ENDCG

}

	// ---- forward rendering additive lights pass:
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardAdd" }
		ZWrite Off Blend One One

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma target 3.0 LOD_FADE_CROSSFADE
#pragma target 3.0
#pragma multi_compile_instancing
#pragma multi_compile __ LOD_FADE_CROSSFADE
#pragma multi_compile_fog
#pragma skip_variants INSTANCING_ON
#pragma multi_compile_fwdadd_fullshadows
#include "HLSLSupport.cginc"
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_SH
#define UNITY_INSTANCED_LIGHTMAPSTS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: <when no other keywords are defined>
#if !defined(INSTANCING_ON) && !defined(LOD_FADE_CROSSFADE)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 tSpace0 : TEXCOORD0;
  float3 tSpace1 : TEXCOORD1;
  float3 tSpace2 : TEXCOORD2;
  float3 worldPos : TEXCOORD3;
  float4 custompack0 : TEXCOORD4; // localRayDir outDepth
  float2 custompack1 : TEXCOORD5; // frameOcta2D
  float3 custompack2 : TEXCOORD6; // viewPos
  UNITY_LIGHTING_COORDS(7,8)
  UNITY_FOG_COORDS(9)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
  o.tSpace1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
  o.tSpace2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);
  o.worldPos.xyz = worldPos;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);
  o.Normal = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o); 
  IN.pos.xy = surfIN.viewPos.xy; // fetch back clipPos.xy
  outDepth = surfIN.outDepth;
  float3 worldPos = surfIN.worldPos;

  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
  
  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;
  //float3 worldN;
  //worldN.x = dot(_unity_tbn_0, o.Normal);
  //worldN.y = dot(_unity_tbn_1, o.Normal);
  //worldN.z = dot(_unity_tbn_2, o.Normal);
  //worldN = normalize(worldN);
  //o.Normal = worldN;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingStandard (o, worldViewDir, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: LOD_FADE_CROSSFADE 
#if defined(LOD_FADE_CROSSFADE) && !defined(INSTANCING_ON)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 tSpace0 : TEXCOORD0;
  float3 tSpace1 : TEXCOORD1;
  float3 tSpace2 : TEXCOORD2;
  float3 worldPos : TEXCOORD3;
  float4 custompack0 : TEXCOORD4; // localRayDir outDepth
  float2 custompack1 : TEXCOORD5; // frameOcta2D
  float3 custompack2 : TEXCOORD6; // viewPos
  UNITY_LIGHTING_COORDS(7,8)
  UNITY_FOG_COORDS(9)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
  o.tSpace1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
  o.tSpace2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);
  o.worldPos.xyz = worldPos;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;
  // call surface function
  surf(surfIN, o);
  IN.pos.xy = surfIN.viewPos.xy; // fetch back clipPos.xy
  outDepth = surfIN.outDepth;
  float3 worldPos = surfIN.worldPos;

#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
     fixed4 c = 0;
  //float3 worldN;
  //worldN.x = dot(_unity_tbn_0, o.Normal);
  //worldN.y = dot(_unity_tbn_1, o.Normal);
  //worldN.z = dot(_unity_tbn_2, o.Normal);
  //worldN = normalize(worldN);
  //o.Normal = worldN;


  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingStandard (o, worldViewDir, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif


ENDCG

}

	// ---- deferred shading pass:
	Pass {
		Name "DEFERRED"
		Tags { "LightMode" = "Deferred" }

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma target 3.0 LOD_FADE_CROSSFADE
#pragma target 3.0
#pragma multi_compile_instancing
#pragma multi_compile __ LOD_FADE_CROSSFADE
#pragma exclude_renderers nomrt
#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
#pragma multi_compile_prepassfinal
#include "HLSLSupport.cginc"
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_SH
#define UNITY_INSTANCED_LIGHTMAPSTS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: <when no other keywords are defined>
#if !defined(INSTANCING_ON) && !defined(LOD_FADE_CROSSFADE)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
#ifndef DIRLIGHTMAP_OFF
  float3 viewDir : TEXCOORD6;
#endif
  float4 lmap : TEXCOORD7;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD8; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD8;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  float3 viewDirForLight = UnityWorldSpaceViewDir(worldPos);
  #ifndef DIRLIGHTMAP_OFF
  o.viewDir.x = dot(viewDirForLight, worldTangent);
  o.viewDir.y = dot(viewDirForLight, worldBinormal);
  o.viewDir.z = dot(viewDirForLight, worldNormal);
  #endif
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN, out float outDepth : SV_Depth,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);
  o.Normal = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o);
  IN.pos.xy = surfIN.viewPos.xy; // fetch back clipPos.xy
  float3 worldPos = surfIN.worldPos;
  outDepth = surfIN.outDepth;

#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);

  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.worldViewDir = worldViewDir;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingStandard_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingStandard_Deferred (o, worldViewDir, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: INSTANCING_ON 
#if defined(INSTANCING_ON) && !defined(LOD_FADE_CROSSFADE)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
#ifndef DIRLIGHTMAP_OFF
  float3 viewDir : TEXCOORD6;
#endif
  float4 lmap : TEXCOORD7;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD8; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD8;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  float3 viewDirForLight = UnityWorldSpaceViewDir(worldPos);
  #ifndef DIRLIGHTMAP_OFF
  o.viewDir.x = dot(viewDirForLight, worldTangent);
  o.viewDir.y = dot(viewDirForLight, worldBinormal);
  o.viewDir.z = dot(viewDirForLight, worldNormal);
  #endif
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN, out float outDepth : SV_Depth,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);
  o.Normal = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o);
  IN.pos.xy = surfIN.viewPos.xy; // fetch back clipPos.xy
  float3 worldPos = surfIN.worldPos;
  outDepth = surfIN.outDepth;

#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);

  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.worldViewDir = worldViewDir;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingStandard_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingStandard_Deferred (o, worldViewDir, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: LOD_FADE_CROSSFADE 
#if defined(LOD_FADE_CROSSFADE) && !defined(INSTANCING_ON)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
#ifndef DIRLIGHTMAP_OFF
  float3 viewDir : TEXCOORD6;
#endif
  float4 lmap : TEXCOORD7;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD8; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD8;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  float3 viewDirForLight = UnityWorldSpaceViewDir(worldPos);
  #ifndef DIRLIGHTMAP_OFF
  o.viewDir.x = dot(viewDirForLight, worldTangent);
  o.viewDir.y = dot(viewDirForLight, worldBinormal);
  o.viewDir.z = dot(viewDirForLight, worldNormal);
  #endif
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN, out float outDepth : SV_Depth,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);
  o.Normal = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o);
  IN.pos.xy = surfIN.viewPos.xy; // fetch back clipPos.xy
  float3 worldPos = surfIN.worldPos;
  outDepth = surfIN.outDepth;

#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);

  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.worldViewDir = worldViewDir;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingStandard_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingStandard_Deferred (o, worldViewDir, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: LOD_FADE_CROSSFADE INSTANCING_ON 
#if defined(LOD_FADE_CROSSFADE) && defined(INSTANCING_ON)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
#ifndef DIRLIGHTMAP_OFF
  float3 viewDir : TEXCOORD6;
#endif
  float4 lmap : TEXCOORD7;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD8; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD8;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  float3 viewDirForLight = UnityWorldSpaceViewDir(worldPos);
  #ifndef DIRLIGHTMAP_OFF
  o.viewDir.x = dot(viewDirForLight, worldTangent);
  o.viewDir.y = dot(viewDirForLight, worldBinormal);
  o.viewDir.z = dot(viewDirForLight, worldNormal);
  #endif
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN, out float outDepth : SV_Depth,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);
  o.Normal = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o);
  IN.pos.xy = surfIN.viewPos.xy; // fetch back clipPos.xy
  float3 worldPos = surfIN.worldPos;
  outDepth = surfIN.outDepth;

#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);

  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.worldViewDir = worldViewDir;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingStandard_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingStandard_Deferred (o, worldViewDir, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif


ENDCG

}

	// ---- shadow caster pass:
	Pass {
		Name "ShadowCaster"
		Tags { "LightMode" = "ShadowCaster" }
		ZWrite On ZTest LEqual

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma target 3.0 LOD_FADE_CROSSFADE
#pragma target 3.0
#pragma multi_compile_instancing
#pragma multi_compile __ LOD_FADE_CROSSFADE
#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
#pragma multi_compile_shadowcaster
#include "HLSLSupport.cginc"
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_SH
#define UNITY_INSTANCED_LIGHTMAPSTS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: <when no other keywords are defined>
#if !defined(INSTANCING_ON) && !defined(LOD_FADE_CROSSFADE)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // localRayDir outDepth
  float2 custompack1 : TEXCOORD3; // frameOcta2D
  float3 custompack2 : TEXCOORD4; // viewPos
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);


  // call surface function
  surf(surfIN, o);
  outDepth = surfIN.outDepth;
  float3 worldPos = surfIN.worldPos;
  IN.pos.xy = surfIN.viewPos.xy; //fetch back clipPos


#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif


  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: INSTANCING_ON 
#if defined(INSTANCING_ON) && !defined(LOD_FADE_CROSSFADE)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // localRayDir outDepth
  float2 custompack1 : TEXCOORD3; // frameOcta2D
  float3 custompack2 : TEXCOORD4; // viewPos
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);


  // call surface function
  surf(surfIN, o);
  outDepth = surfIN.outDepth;
  float3 worldPos = surfIN.worldPos;
  IN.pos.xy = surfIN.viewPos.xy; //fetch back clipPos


#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: LOD_FADE_CROSSFADE 
#if defined(LOD_FADE_CROSSFADE) && !defined(INSTANCING_ON)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // localRayDir outDepth
  float2 custompack1 : TEXCOORD3; // frameOcta2D
  float3 custompack2 : TEXCOORD4; // viewPos
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;


#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);


  // call surface function
  surf(surfIN, o);
  outDepth = surfIN.outDepth;
  float3 worldPos = surfIN.worldPos;
  IN.pos.xy = surfIN.viewPos.xy; //fetch back clipPos

#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: LOD_FADE_CROSSFADE INSTANCING_ON 
#if defined(LOD_FADE_CROSSFADE) && defined(INSTANCING_ON)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // localRayDir outDepth
  float2 custompack1 : TEXCOORD3; // frameOcta2D
  float3 custompack2 : TEXCOORD4; // viewPos
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;


#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);


  // call surface function
  surf(surfIN, o);
  outDepth = surfIN.outDepth;
  float3 worldPos = surfIN.worldPos;
  IN.pos.xy = surfIN.viewPos.xy; //fetch back clipPos

#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif


ENDCG

}

	// ---- meta information extraction pass:
	Pass {
		Name "Meta"
		Tags { "LightMode" = "Meta" }
		Cull Off

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma target 3.0 LOD_FADE_CROSSFADE
#pragma target 3.0
#pragma multi_compile_instancing
#pragma multi_compile __ LOD_FADE_CROSSFADE
#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
#pragma shader_feature EDITOR_VISUALIZATION

#include "HLSLSupport.cginc"
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_SH
#define UNITY_INSTANCED_LIGHTMAPSTS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: <when no other keywords are defined>
#if !defined(INSTANCING_ON) && !defined(LOD_FADE_CROSSFADE)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD6;
  float4 lightCoord : TEXCOORD7;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o); outDepth = surfIN.outDepth;
  float3 worldPos = surfIN.worldPos;
  IN.pos.xy = surfIN.viewPos.xy; //fetch clipPos back


#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: INSTANCING_ON 
#if defined(INSTANCING_ON) && !defined(LOD_FADE_CROSSFADE)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD6;
  float4 lightCoord : TEXCOORD7;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o); outDepth = surfIN.outDepth;
  float3 worldPos = surfIN.worldPos;
  IN.pos.xy = surfIN.viewPos.xy; //fetch clipPos back


#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: LOD_FADE_CROSSFADE 
#if defined(LOD_FADE_CROSSFADE) && !defined(INSTANCING_ON)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD6;
  float4 lightCoord : TEXCOORD7;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o); outDepth = surfIN.outDepth;
  float3 worldPos = surfIN.worldPos;
  IN.pos.xy = surfIN.viewPos.xy; //fetch clipPos back


#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: LOD_FADE_CROSSFADE INSTANCING_ON 
#if defined(LOD_FADE_CROSSFADE) && defined(INSTANCING_ON)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: YES
// writes to emission: YES
// writes to occlusion: YES
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: YES
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        // Physically based Standard lighting model, and enable shadows on all light types
       //#pragma surface surf Standard fullforwardshadows vertex:vert dithercrossfade addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        #include "PxzImpostor.cginc"


        struct Input
        {
            float3 localRayDir;
            float2 frameOcta2D;
            float3 viewPos;
            float outDepth;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            PxzImpostorData impData;

            UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

            impData.vertex = v.vertex;

            PxzImpostorVertex(impData);

            ////vertex to frag
            v.vertex = impData.vertex;

            ////surface
            o.localRayDir = impData.localRayDir;
            o.frameOcta2D = impData.frameOcta2D;
            o.viewPos = impData.viewPos;
            o.outDepth = 0;
            o.worldPos = float3(0,0,0);
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // //#pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(inout Input IN, inout SurfaceOutputStandard o)
        {
           PxzImpostorData imp;
           imp.localRayDir = IN.localRayDir;
           imp.frameOcta2D = IN.frameOcta2D;
           imp.viewPos = IN.viewPos;


           fixed4 blendedAlbedo;
           float4 blendedNormal;
           float blendedSmoothness;
           float blendedOcclusion;
           float3 blendedEmission;
           PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

           o.Albedo = blendedAlbedo;
           o.Normal = blendedNormal.rgb;
           o.Smoothness = blendedSmoothness;
           o.Occlusion = blendedOcclusion;
           o.Emission = blendedEmission;

           float alphaCut = blendedNormal.a - _Pxz_Clip;
           clip(alphaCut);

        }
        
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 tSpace0 : TEXCOORD0;
  float4 tSpace1 : TEXCOORD1;
  float4 tSpace2 : TEXCOORD2;
  float4 custompack0 : TEXCOORD3; // localRayDir outDepth
  float2 custompack1 : TEXCOORD4; // frameOcta2D
  float3 custompack2 : TEXCOORD5; // viewPos
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD6;
  float4 lightCoord : TEXCOORD7;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xyz = customInputData.localRayDir;
  o.custompack1.xy = customInputData.frameOcta2D;
  o.custompack2.xyz = customInputData.viewPos;
  o.custompack0.w = customInputData.outDepth;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN, out float outDepth : SV_Depth) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_RECONSTRUCT_TBN(IN);
  #else
    UNITY_EXTRACT_TBN(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.localRayDir.x = 1.0;
  surfIN.frameOcta2D.x = 1.0;
  surfIN.viewPos.x = 1.0;
  surfIN.outDepth.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.localRayDir = IN.custompack0.xyz;
  surfIN.frameOcta2D = IN.custompack1.xy;
  surfIN.viewPos = IN.custompack2.xyz;
  surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
  SurfaceOutputStandard o;
#endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0, 0, 1);

  // call surface function
  surf(surfIN, o); outDepth = surfIN.outDepth;
  float3 worldPos = surfIN.worldPos;
  IN.pos.xy = surfIN.viewPos.xy; //fetch clipPos back


#ifndef USING_DIRECTIONAL_LIGHT
  fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
  fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif

  UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif


ENDCG

}
Pass
{
    Name "SCENESELECTIONPASS"
    Tags { "LightMode" = "SceneSelectionPass" }
    BlendOp Add
    Blend One Zero
    ZWrite On
    Cull Off

    CGPROGRAM
   // compile directives
   #pragma vertex vert_surf
   #pragma fragment frag_surf
   #pragma target 3.5
   #pragma multi_compile_instancing
   #pragma multi_compile_fog
   #pragma skip_variants INSTANCING_ON
   #pragma multi_compile_fwdadd_fullshadows
   #include "HLSLSupport.cginc"
   #define UNITY_INSTANCED_LOD_FADE
   #define UNITY_INSTANCED_SH
   #define UNITY_INSTANCED_LIGHTMAPSTS
   #include "UnityShaderVariables.cginc"
   #include "UnityShaderUtilities.cginc"
   #include "UnityCG.cginc"
   #include "Lighting.cginc"
   #include "UnityPBSLighting.cginc"
   #include "AutoLight.cginc"
   #include "UnityCG.cginc"
   #include "PxzImpostor.cginc"


   struct Input
   {
       float3 localRayDir;
       float2 frameOcta2D;
       float3 viewPos;
       float outDepth;
       float3 worldPos;
   };

   int _PassValue;
   int _ObjectId;

   void vert(inout appdata_full v, out Input o)
   {
       UNITY_INITIALIZE_OUTPUT(Input, o);

       PxzImpostorData impData;

       UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

       impData.vertex = v.vertex;

       PxzImpostorVertex(impData);

       ////vertex to frag
       v.vertex = impData.vertex;

       ////surface
       o.localRayDir = impData.localRayDir;
       o.frameOcta2D = impData.frameOcta2D;
       o.viewPos = impData.viewPos;
       o.outDepth = 0;
       o.worldPos = float3(0,0,0);
   }

   void surf(inout Input IN, inout SurfaceOutputStandard o)
   {
       PxzImpostorData imp;
       imp.localRayDir = IN.localRayDir;
       imp.frameOcta2D = IN.frameOcta2D;
       imp.viewPos = IN.viewPos;

       float3 dbg;
       fixed4 blendedAlbedo;
       float4 blendedNormal;
       float blendedSmoothness;
       float blendedOcclusion;
       float3 blendedEmission;
       PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedSmoothness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

       o.Albedo = blendedAlbedo;
       o.Normal = blendedNormal.rgb;
       o.Smoothness = blendedSmoothness;
       o.Occlusion = blendedOcclusion;
       o.Emission = blendedEmission;

       float alphaCut = blendedNormal.a - _Pxz_Clip;
       clip(alphaCut);
   }


   // vertex-to-fragment interpolation data
   struct v2f_surf {
       UNITY_POSITION(pos);
       float3 worldPos : TEXCOORD0;
       float4 custompack0 : TEXCOORD3; // localRayDir outDepth
       float2 custompack1 : TEXCOORD4; // frameOcta2D
       float3 custompack2 : TEXCOORD5; // viewPos
       UNITY_VERTEX_INPUT_INSTANCE_ID
       UNITY_VERTEX_OUTPUT_STEREO
   };

   // vertex shader
   v2f_surf vert_surf(appdata_full v) {
       UNITY_SETUP_INSTANCE_ID(v);
       v2f_surf o;
       UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
       UNITY_TRANSFER_INSTANCE_ID(v,o);
       UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
       Input customInputData;
       vert(v, customInputData);
       o.custompack0.xyz = customInputData.localRayDir;
       o.custompack1.xy = customInputData.frameOcta2D;
       o.custompack2.xyz = customInputData.viewPos;
       o.custompack0.w = customInputData.outDepth;
       o.pos = UnityObjectToClipPos(v.vertex);
       return o;
   }

   // fragment shader
   fixed4 frag_surf(v2f_surf IN, out float outDepth : SV_Depth) : SV_Target{
       UNITY_SETUP_INSTANCE_ID(IN);
   // prepare and unpack data
   Input surfIN;
   UNITY_INITIALIZE_OUTPUT(Input,surfIN);
   UNITY_INITIALIZE_OUTPUT(Input, surfIN);
   surfIN.localRayDir.x = 1.0;
   surfIN.frameOcta2D.x = 1.0;
   surfIN.viewPos.x = 1.0;
   surfIN.outDepth.x = 1.0;
   surfIN.localRayDir = IN.custompack0.xyz;
   surfIN.frameOcta2D = IN.custompack1.xy;
   surfIN.viewPos = IN.custompack2.xyz;
   surfIN.outDepth = IN.custompack0.w;
   float3 worldPos = IN.worldPos.xyz;


   #ifdef UNITY_COMPILER_HLSL
   SurfaceOutputStandard o = (SurfaceOutputStandard)0;
   #else
   SurfaceOutputStandard o;
   #endif
   o.Albedo = 0.0;
   o.Emission = 0.0;
   o.Alpha = 0.0;
   o.Occlusion = 1.0;
   fixed3 normalWorldVertex = fixed3(0,0,1);
   o.Normal = fixed3(0,0,1);
   // call surface function
   surf(surfIN, o);
   outDepth = surfIN.outDepth;
   UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
   return float4(_ObjectId, _PassValue, 1.0, 1.0);

}

ENDCG
}
Pass
{
    Name "ScenePickingPass"
    Tags { "LightMode" = "Picking" }
    BlendOp Add
    Blend One Zero
    ZWrite On
    Cull Off

    CGPROGRAM
   // compile directives
   #pragma vertex vert_surf
   #pragma fragment frag_surf
   #pragma target 3.5
   #pragma multi_compile_instancing
   #pragma multi_compile_fog
   #pragma skip_variants INSTANCING_ON
   #pragma multi_compile_fwdadd_fullshadows
   #include "HLSLSupport.cginc"
   #define UNITY_INSTANCED_LOD_FADE
   #define UNITY_INSTANCED_SH
   #define UNITY_INSTANCED_LIGHTMAPSTS
   #include "UnityShaderVariables.cginc"
   #include "UnityShaderUtilities.cginc"
   #include "UnityCG.cginc"
   #include "Lighting.cginc"
   #include "UnityPBSLighting.cginc"
   #include "AutoLight.cginc"
   #include "UnityCG.cginc"
   #include "PxzImpostor.cginc"

    float4 _SelectionID;

    struct Input
    {
        float3 localRayDir;
        float2 frameOcta2D;
        float3 viewPos;
        float outDepth;
        float3 worldPos;
    };

    void vert(inout appdata_full v, out Input o)
    {
        UNITY_INITIALIZE_OUTPUT(Input, o);

        PxzImpostorData impData;

        UNITY_INITIALIZE_OUTPUT(PxzImpostorData, impData);

        impData.vertex = v.vertex;

        PxzImpostorVertex(impData);

        ////vertex to frag
        v.vertex = impData.vertex;

        ////surface
        o.localRayDir = impData.localRayDir;
        o.frameOcta2D = impData.frameOcta2D;
        o.viewPos = impData.viewPos;
        o.outDepth = 0;
        o.worldPos = float3(0,0,0);
    }

    void surf(inout Input IN, inout SurfaceOutputStandard o)
    {
        PxzImpostorData imp;
        imp.localRayDir = IN.localRayDir;
        imp.frameOcta2D = IN.frameOcta2D;
        imp.viewPos = IN.viewPos;

        float3 dbg;
        fixed4 blendedAlbedo;
        float4 blendedNormal;
        float blendedRoughness;
        float blendedOcclusion;
        float3 blendedEmission;
        PxzImpostorFragment(imp, blendedAlbedo, blendedNormal, blendedRoughness, blendedOcclusion, blendedEmission, IN.outDepth, IN.worldPos);

        o.Albedo = blendedAlbedo;
        o.Normal = blendedNormal.rgb;
        o.Smoothness = 1 - blendedRoughness;
        o.Occlusion = blendedOcclusion;
        o.Emission = blendedEmission;

        float alphaCut = blendedNormal.a - _Pxz_Clip;
        clip(alphaCut);
    }


    // vertex-to-fragment interpolation data
    struct v2f_surf {
        UNITY_POSITION(pos);
        float3 worldPos : TEXCOORD0;
        float4 custompack0 : TEXCOORD3; // localRayDir outDepth
        float2 custompack1 : TEXCOORD4; // frameOcta2D
        float3 custompack2 : TEXCOORD5; // viewPos
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
    };

    // vertex shader
    v2f_surf vert_surf(appdata_full v) {
        UNITY_SETUP_INSTANCE_ID(v);
        v2f_surf o;
        UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
        UNITY_TRANSFER_INSTANCE_ID(v,o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        Input customInputData;
        vert(v, customInputData);
        o.custompack0.xyz = customInputData.localRayDir;
        o.custompack1.xy = customInputData.frameOcta2D;
        o.custompack2.xyz = customInputData.viewPos;
        o.custompack0.w = customInputData.outDepth;
        o.pos = UnityObjectToClipPos(v.vertex);
        return o;
    }

    // fragment shader
    fixed4 frag_surf(v2f_surf IN, out float outDepth : SV_DEPTH) : SV_Target{
        UNITY_SETUP_INSTANCE_ID(IN);
    // prepare and unpack data
    Input surfIN;
    UNITY_INITIALIZE_OUTPUT(Input,surfIN);
    UNITY_INITIALIZE_OUTPUT(Input, surfIN);
    surfIN.localRayDir.x = 1.0;
    surfIN.frameOcta2D.x = 1.0;
    surfIN.viewPos.x = 1.0;
    surfIN.outDepth.x = 1.0;
    surfIN.localRayDir = IN.custompack0.xyz;
    surfIN.frameOcta2D = IN.custompack1.xy;
    surfIN.viewPos = IN.custompack2.xyz;
    surfIN.outDepth = IN.custompack0.w;

#ifdef UNITY_COMPILER_HLSL
    SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
    SurfaceOutputStandard o;
#endif
    o.Albedo = 0.0;
    o.Emission = 0.0;
    o.Alpha = 0.0;
    o.Occlusion = 1.0;
    fixed3 normalWorldVertex = fixed3(0, 0, 1);
    o.Normal = fixed3(0, 0, 1);
    // call surface function
    surf(surfIN, o);
    outDepth = surfIN.outDepth;
    IN.pos.xy = surfIN.viewPos.xy; //fetch back clipPos.xy
    float3 worldPos = surfIN.worldPos;

    UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
    return _SelectionID;

}
ENDCG
}

	// ---- end of surface shader generated code

#LINE 103

    }
    FallBack "Diffuse"
}
