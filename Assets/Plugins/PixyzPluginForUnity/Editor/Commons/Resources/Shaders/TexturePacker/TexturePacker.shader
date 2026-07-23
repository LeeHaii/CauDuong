Shader "Pixyz/TexturePacker"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        _RedTex("RedTexture", 2D) = "white" {}
        _RedTexChan("RedTextureChan", Int) = 0
        _RedEnable("RedEnable", Int) = 0

        _GreenTex("GreenTexture", 2D) = "white" {}
        _GreenTexChan("GreenTextureChan", Int) = 2
        _GreenEnable("GreenEnable", Int) = 0

        _BlueTex("BlueTexture", 2D) = "white" {}
        _BlueTexChan("BlueTextureChan", Int) = 1
        _BlueEnable("BlueEnable", Int) = 0

        _AlphaTex("AlphaTexture", 2D) = "white" {}
        _AlphaTexChan("AlphaTextureChan", Int) = 3
        _AlphaEnable("AlphaEnable", Int) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Name "Pack"
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                sampler2D _RedTex;
                sampler2D _BlueTex;
                sampler2D _GreenTex;
                sampler2D _AlphaTex;

                int _RedTexChan;
                int _BlueTexChan;
                int _GreenTexChan;
                int _AlphaTexChan;

                int _RedEnable;
                int _GreenEnable;
                int _BlueEnable;
                int _AlphaEnable;

                int _unpack;

                struct appdata
                {
                   float4 vertex : POSITION;
                   float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                   float2 uv : TEXCOORD0;
                   float4 vertex : SV_POSITION;
                };

                v2f vert(appdata inVert)
                {
                   v2f outVert;
                   outVert.vertex = UnityObjectToClipPos(inVert.vertex);
                   outVert.uv = inVert.uv;
                   return outVert;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                   float4 outColor = float4(0,0,0,1);

                   if (_RedEnable == 1)
                      outColor.r = tex2D(_RedTex, i.uv)[_RedTexChan];
                   if (_GreenEnable == 1)
                      outColor.g = tex2D(_BlueTex, i.uv)[_BlueTexChan];
                   if (_BlueEnable == 1)
                      outColor.b = tex2D(_GreenTex, i.uv)[_GreenTexChan];
                   if (_AlphaEnable == 1)
                      outColor.a = tex2D(_AlphaTex, i.uv)[_AlphaTexChan];

                   return outColor;
                }
                ENDCG
            }

            Name "UnPack"
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                sampler2D _MainTex;

                int _RedTexChan;
                int _BlueTexChan;
                int _GreenTexChan;
                int _AlphaTexChan;

                int _RedEnable;
                int _GreenEnable;
                int _BlueEnable;
                int _AlphaEnable;

                struct appdata
                {
                   float4 vertex : POSITION;
                   float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                   float2 uv : TEXCOORD0;
                   float4 vertex : SV_POSITION;
                };

                struct PixelOutput {
                   float4 col0 : SV_Target0;
                   float4 col1 : SV_Target1;
                   float4 col2 : SV_Target2;
                   float4 col3 : SV_Target3;
                };

                float4 assignChannelValue(int chan, float value)
                {
                   float4 color = float4(0, 0, 0, 1);
                   if (chan == 0)
                   {
                      color.r = value;
                   }
                   else if (chan == 1)
                   {
                      color.g = value;
                   }
                   else if (chan == 2)
                   {
                      color.b = value;
                   }
                   else
                   {
                      color.a = value;
                   }
                   return color;
                }

                v2f vert(appdata inVert)
                {
                   v2f outVert;
                   outVert.vertex = UnityObjectToClipPos(inVert.vertex);
                   outVert.uv = inVert.uv;
                   return outVert;
                }

                PixelOutput frag(v2f i)
                {
                      float4 packColor = tex2D(_MainTex, i.uv);
                      PixelOutput outColors;

                      if (_RedEnable == 1)
                         outColors.col0 = assignChannelValue(_RedTexChan, packColor.r);
                      else
                         outColors.col0 = float4(1, 0, 0, 1);

                      if (_GreenEnable == 1)
                         outColors.col1 = assignChannelValue(_GreenTexChan, packColor.b);
                      else
                         outColors.col1 = float4(0, 1, 0, 1);

                      if (_BlueEnable == 1)
                         outColors.col2 = assignChannelValue(_BlueTexChan, packColor.g);
                      else
                         outColors.col2 = float4(0, 0, 1, 1);

                      if (_AlphaEnable == 1)
                         outColors.col3 = assignChannelValue(_AlphaTexChan, packColor.a);
                      else
                         outColors.col3 = float4(0, 0, 0, 1);

                      return outColors;
                }
                ENDCG
            }
        }
}
