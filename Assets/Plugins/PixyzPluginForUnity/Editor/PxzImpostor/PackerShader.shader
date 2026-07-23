Shader "Pixyz/Packer"
{
   Properties
   {
       _MainTex("Texture", 2D) = "white" {}
       _ExtraTexture("_ExtraTexture", 2D) = "white" {}
   }
      SubShader
   {
       Tags { "RenderType" = "Opaque" }

       Pass //0 Pack depth in alpha of emissive
       {

           CGPROGRAM
           #pragma vertex vert_img // basic vertex shader from UnityCG.cginc
           #pragma fragment frag
           // make fog work
           #pragma multi_compile_fog

           #include "UnityCG.cginc"


           uniform sampler2D _MainTex;
           uniform sampler2D _ExtraTexture;

           fixed4 frag(v2f_img i) : SV_Target
           {
              float depth = tex2D(_ExtraTexture, i.uv).r;
              float3 emissive = tex2D(_MainTex, i.uv).rgb;

              float c = 0;
              if (depth != 0)
                 c = 1.0 - depth;
              float4 color = fixed4(emissive, c);
              return color;
           }
           ENDCG
       }
       Pass //1 fix albedo
       {

           CGPROGRAM
           #pragma vertex vert_img // basic vertex shader from UnityCG.cginc
           #pragma fragment frag
           // make fog work
           #pragma multi_compile_fog

           #include "UnityCG.cginc"


           uniform sampler2D _MainTex;
           uniform sampler2D _ExtraTexture;

           fixed4 frag(v2f_img i) : SV_Target
           {
              float3 specular = tex2D(_ExtraTexture, i.uv).rgb;
              float4 albedo = tex2D(_MainTex, i.uv);
              albedo.rgb = albedo.rgb / (1 - specular);
              return albedo;

           }
           ENDCG
       }

       Pass //2 pack roughness in alpha of normal map
       {

           CGPROGRAM
           #pragma vertex vert_img // basic vertex shader from UnityCG.cginc
           #pragma fragment frag
           // make fog work
           #pragma multi_compile_fog

           #include "UnityCG.cginc"


           uniform sampler2D _MainTex;
           uniform sampler2D _ExtraTexture;

           fixed4 frag(v2f_img i) : SV_Target
           {
              float smoothness = tex2D(_ExtraTexture, i.uv).a;
              float3 normal = tex2D(_MainTex, i.uv).rgb;
              float4 finalFrag = fixed4(normal, smoothness);
              return finalFrag;

           }
           ENDCG
       }
       Pass //3 Fix Emission when light scene is toggled off 
       {
           CGPROGRAM
           #pragma target 3.0
           #pragma vertex vert_img
           #pragma fragment frag
           #include "UnityCG.cginc"

           uniform sampler2D _MainTex;

           float4 frag(v2f_img i) : SV_Target
           {
              float4 emission = tex2D(_MainTex, i.uv);

              emission.rgb = -log2(emission.rgb);

              return emission;
           }
           ENDCG
       }
       Pass //4 Copy depth from Z-Buffer
       {

           CGPROGRAM
           #pragma vertex vert_img // basic vertex shader from UnityCG.cginc
           #pragma fragment frag
           // make fog work
           #pragma multi_compile_fog

           #include "UnityCG.cginc"


           uniform sampler2D _MainTex;
           uniform sampler2D _ExtraTexture;

           fixed4 frag(v2f_img i) : SV_Target
           {
              float depth = tex2D(_ExtraTexture, i.uv).r;
              float c = 0;
              if (depth != 0)
                 c = 1.0 - depth;
              float4 color = fixed4(c, c, c, 1.0);
              return color;
           }
           ENDCG
       }

       Pass // Copy value from _ExtraTexture in alpha channel 5
       {
           CGPROGRAM
           #pragma target 3.0
           #pragma vertex vert_img // basic vertex shader from UnityCG.cginc
           #pragma fragment frag
           #include "UnityCG.cginc"

           uniform sampler2D _MainTex;
           uniform sampler2D _ExtraTexture;

           fixed4 frag(v2f_img i) : SV_Target
           {
              float alpha = tex2D(_ExtraTexture, i.uv).a;
              fixed4 finalColor = (float4(tex2D(_MainTex, i.uv).rgb , alpha));
              return finalColor;
           }
           ENDCG
       }

   }
}
