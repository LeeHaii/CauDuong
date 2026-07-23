using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Pixyz.Commons.Utilities
{
	public static class ShaderUtilities
	{
		public static Shader GetPixyzLineShader()
		{
			Shader shader = Shader.Find("Pixyz/Simple Lines");
			if (shader != null)
				return shader;

			return GetDefaultShader();
		}
		public static Shader GetPixyzSplatsShader()
		{
			Shader shader = Shader.Find("Pixyz/Splats");
			if (shader != null)
				return shader;

			return GetDefaultShader();
		}

		public static Shader GetDefaultShader()
		{
			Shader shader = null;
#if UNITY_2019_1_OR_NEWER
			if (GraphicsSettings.renderPipelineAsset)
			{
				shader = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.defaultShader;
			}
#else
		if (Rendering.GraphicsSettings.renderPipelineAsset)
		{
			shader = Rendering.GraphicsSettings.renderPipelineAsset.GetDefaultShader();
		}
#endif
			if (shader == null)
				shader = Shader.Find("Universal Render Pipeline/Lit");
            
            if (shader == null)
                shader = Shader.Find("Standard");
                
			return shader;
		}


		private static Dictionary<string, string> VredToShaderGraph = new Dictionary<string, string>()
		{
			{ "UMetallicPaintMaterial", "Shader Graphs/CarPaint" },
			{ "UBrushedMetalMaterial", "Shader Graphs/Metal" },
			{ "UEnvironmentSwitchMaterial", "" },
			{ "USphereEnvironmentMaterial", "" },
			{ "UCarbonMaterial", "Shader Graphs/Metal" },
			{ "UCarbonMaterial2D", "Shader Graphs/CarbonFiber" },
			{ "UChromeMaterial", "Shader Graphs/Metal" },
			{ "UFlipflopMaterial", "Shader Graphs/CarPaint" },
			{ "UGlassMaterial", "Shader Graphs/GlassSimple" },
			{ "UPhongMaterial", "Shader Graphs/CarPaint" },
			{ "UPlasticMaterial", "Shader Graphs/PlasticTransparentSimple" },
			{ "UReflectivePlasticMaterial", "Shader Graphs/PlasticNormal" },
			{ "UTireMaterial", "Shader Graphs/Rubber" },
			{ "UUnicolorPaintMaterial", "Shader Graphs/CarPaint" },
			{ "UVelvetMaterial", "Shader Graphs/FabricSuede" },
			{ "UWovenClothMaterial", "Shader Graphs/FabricSuede" },
			{ "UTriplanarMaterial", "Shader Graphs/FabricSuede" },
			{ "UShadowMaterial", "" },
		};
		private static Dictionary<string, Dictionary<string, string>> VredPropToKeyword = new Dictionary<string, Dictionary<string, string>>()
		{
			{  "Shader Graphs/CarPaint", new Dictionary<string, string>()
			{
				{ "baseColor", "Color_F141EB95" },
				{ "flakeColor", "Color_CF7B3627" }
			}
			},
			{  "Shader Graphs/Rubber", new Dictionary<string, string>()
			{
				{ "ambienceColor", "Color_9A170B2D" },
			}
			},
			{  "Shader Graphs/PlasticTransparentSimple", new Dictionary<string, string>()
			{
				{ "ambienceColor", "Color_38879BCF" },
				{ "seeThrough", "Color_38879BCF_alpha" }
			}
			},
			{  "Shader Graphs/GlassSimple", new Dictionary<string, string>()
			{
				{ "ambienceColor", "Color_DCF1FFF3" },
			}
			},
			{  "Shader Graphs/CarbonFiber", new Dictionary<string, string>()
			{
				{ "ambienceColor", "Color_DCF1FFF3" },
			}
			},
			{  "Shader Graphs/Metal", new Dictionary<string, string>()
			{
				{ "ambienceColor", "Color_AD8F67CE" },
			}
			},
			{  "Shader Graphs/FabricSuede", new Dictionary<string, string>()
			{
				{ "ambienceColor", "Color_4F4ED3A9" },
				{ "diffuseComponent_image", "Texture2D_D9FF89B8" }
			}
			},
			{  "Shader Graphs/Leather", new Dictionary<string, string>()
			{
				{ "ambienceColor", "Color_7A8D5E5E" },
				{ "diffuseComponent_image", "Texture2D_4D3C9E50" }
			}
			}
		};
		public static Shader GetCustomShader(string name)
		{
			Shader shader = null;
			if (VredToShaderGraph.ContainsKey(name))
			{
				shader = Shader.Find(VredToShaderGraph[name]);
			}
			return shader;
		}

		public static string GetShaderKeyword(string name, string property)
		{
			VredPropToKeyword.TryGetValue(name, out Dictionary<string, string> maps);
			if (maps != null)
			{
				maps.TryGetValue(property, out string keyword);
				return keyword;
			}
			return null;
		}
	}
}