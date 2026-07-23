using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Pixyz.Commons.Utilities;
using Pixyz.Commons.Extensions;

namespace Pixyz.ImportSDK
{
	public static partial class Conversions
	{
		#region MaterialExtensions

		private static void SetTexture(this UnityEngine.Material material, string property, Material.Native.Texture textureExtract, ref Dictionary<uint, Texture2D> textureMap)
		{
			if (!material.HasProperty(property))
				return;
			material.SetTextureOffset(property, new Vector2((float)textureExtract.offset.x, (float)textureExtract.offset.y));
			material.SetTextureScale(property, new Vector2((float)textureExtract.tilling.x, (float)textureExtract.tilling.y));
			material.SetTexture(property, GetTexture(textureExtract, ref textureMap));
		}

		public static bool TryGetTexture(this UnityEngine.Material material, string texturePropertyName, out Material.Native.Texture texture, ref Dictionary<Texture2D, uint> texMap)
		{
			if (!material.HasProperty(texturePropertyName))
			{
				texture = null;
				return false;
			}
			Texture2D texture2D = material.GetTexture(texturePropertyName) as Texture2D;
			if (!texture2D)
			{
				texture = null;
				return false;
			}
			if (!texMap.ContainsKey(texture2D))
			{
				Debug.LogError($"Texture '{texture2D.name}' haven't been converted to a Native PiXYZ texture");
				texture = null;
				return false;
			}
			texture = new Material.Native.Texture();
			texture.offset = new Geom.Native.Point2();
			Vector2 offset = material.GetTextureOffset(texturePropertyName);
			texture.offset.x = offset.x;
			texture.offset.y = offset.y;
			texture.tilling = new Geom.Native.Point2();
			Vector2 tilling = material.GetTextureScale(texturePropertyName);
			texture.tilling.x = tilling.x;
			texture.tilling.y = tilling.y;
			texture.image = texMap[texture2D];
			return true;
		}

		public static Color TryGetColor(this UnityEngine.Material material, string colorPropertyName)
		{
			if (!material.HasProperty(colorPropertyName))
			{
				return Color.white;
			}
			return material.GetColor(colorPropertyName);
		}

		public static float TryGetFloat(this UnityEngine.Material material, string floatPropertyName, float defaultValue = 0f)
		{
			if (!material.HasProperty(floatPropertyName))
			{
				return defaultValue;
			}
			return material.GetFloat(floatPropertyName);
		}

		#endregion

		#region TextureUtilities

		private static Texture2D GetTexture(Material.Native.Texture textureExtract, ref Dictionary<uint, Texture2D> textureMap)
		{
			if (!textureMap.ContainsKey(textureExtract.image))
			{
				if (textureExtract.image == 0)
				{
					return null;
				}
				else
				{
					textureMap.Add(textureExtract.image, ConvertImageDefinition(Material.Native.NativeInterface.GetImageDefinition(textureExtract.image)));
				}

			}
			return textureMap[textureExtract.image];
		}

		private static Texture2D CreateSpecGlossMap(Texture2D specular, Texture2D roughness)
		{
			if (specular == null || roughness == null)
				return null;
			if (specular.width != roughness.width || specular.height != roughness.height)
			{
				Debug.LogError("Specular/metallic map and roughness map must be of the same size");
				return specular;
			}

			Texture2D specGloss = new Texture2D(specular.width, specular.height, TextureFormat.RGBA32, false);
			specGloss.name = specular.name;

			Color[] colorsSpecular = specular.GetPixels();
			Color[] colorsRoughness = roughness.GetPixels();

			for (int i = 0; i < colorsSpecular.Length; i++)
			{
				Color colorSpecular = colorsSpecular[i];
				Color colorRoughness = colorsRoughness[i];
				colorsSpecular[i] = new Color(colorSpecular.r, colorSpecular.g, colorSpecular.b, 1f - colorRoughness.r);
			}

			specGloss.SetPixels(colorsSpecular);
			specGloss.Apply();
			return specGloss;
		}
		#endregion

		#region ImageDefinition <> Texture2D

		public static Material.Native.ImageDefinition ConvertTexture(Texture2D texture, bool isNormalMap = false)
		{
			Material.Native.ImageDefinition imageDefinition = new Material.Native.ImageDefinition();

			imageDefinition.name = texture.name;
			imageDefinition.width = texture.width;
			imageDefinition.height = texture.height;
			imageDefinition.data = new Core.Native.ByteList();

			switch (texture.format)
			{
				case TextureFormat.R8:
					imageDefinition.bitsPerComponent = 8;
					imageDefinition.componentsCount = 1;
					imageDefinition.data.list = texture.GetRawTextureData();
					break;
				case TextureFormat.R16:
					imageDefinition.bitsPerComponent = 8;
					imageDefinition.componentsCount = 2;
					imageDefinition.data.list = texture.GetRawTextureData();
					break;
				case TextureFormat.RGB24:
					imageDefinition.bitsPerComponent = 8;
					imageDefinition.componentsCount = 3;
					imageDefinition.data.list = texture.GetRawTextureData();
					break;
				case TextureFormat.RGBA32:
				default:
					Texture2D tempTex = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
					Color32[] pixels = TextureUtilities.GetColor32(texture, isNormalMap);

					tempTex.SetPixels32(pixels);
					tempTex.Apply();

					byte[] rawData = tempTex.GetRawTextureData();

					imageDefinition.data.list = new byte[rawData.Length];
					imageDefinition.bitsPerComponent = 8;
					imageDefinition.componentsCount = 4;

					System.Array.Copy(rawData, imageDefinition.data.list, rawData.Length);

					GameObject.DestroyImmediate(tempTex);

					break;
			}

			return imageDefinition;
		}

		public static Texture2D ConvertImageDefinition(Material.Native.ImageDefinition imageDefinition)
		{
			TextureFormat format = TextureFormat.Alpha8;
			switch (imageDefinition.componentsCount)
			{
				case 1:
					if (imageDefinition.bitsPerComponent == 8)
						format = TextureFormat.R8;
					else if (imageDefinition.bitsPerComponent == 16)
						format = TextureFormat.R16;
					break;
				case 2:
					if (imageDefinition.bitsPerComponent == 8)
						format = TextureFormat.RG16;
					break;
				case 3:
					if (imageDefinition.bitsPerComponent == 8)
						format = TextureFormat.RGB24;
					break;
				case 4:
					if (imageDefinition.bitsPerComponent == 8)
						format = TextureFormat.RGBA32;
					break;
				default:
					break;
			}
			Texture2D texture = new Texture2D(imageDefinition.width, imageDefinition.height, format, false);
			texture.name = imageDefinition.name;

			if (imageDefinition.width <= 0 || imageDefinition.height <= 0)
			{
				Debug.LogWarning($"Texture '{texture.name}' is 0x0 and was ignored");
				return null;
			}
			if (imageDefinition.width > 16384 || imageDefinition.height > 16384)
			{
				Debug.LogWarning($"Texture '{texture.name}' is larger than 16384, which is not supported");
				return null;
			}
			try
			{
				if (imageDefinition.data != null && imageDefinition.data.length > 0)
				{
					if (format == TextureFormat.R8)
					{
						// Unity does not support well R8 format, convert it to RGBA32
						texture = new Texture2D(imageDefinition.width, imageDefinition.height, TextureFormat.RGBA32, false);
						var index = 0;
						for (int x = 0; x < texture.width; x++)
						{
							for (int y = 0; y < texture.height; y++)
							{
								byte value = imageDefinition.data[index++];
								Color32 col = new Color32(value, value, value, 255);
								texture.SetPixel(y, x, col);
							}
						}
					}
					else
						texture.LoadRawTextureData(imageDefinition.data);
				}
			}
			catch
			{
				Debug.LogWarning($"Texture data for '{texture.name}' is corrupted and couldn't be converted properly.");
			}
			texture.Apply();

			return texture;
		}
		#endregion

		#region MaterialDefinition <> Material


		private static void CombineAlbedoAndOpacity(Material.Native.Texture albedo, Material.Native.Texture opacity, Dictionary<uint, Texture2D> textures)
		{
			Texture2D albedotex = GetTexture(albedo, ref textures);
			Texture2D opacitytex = GetTexture(opacity, ref textures);

			Texture2D combined = null;
			if (albedotex != null && opacitytex != null)
			{
				combined = new Texture2D(albedotex.width, albedotex.height, TextureFormat.RGBA32, false);
				combined.name = albedotex.name;
				Color[] albedoColors = albedotex.GetPixels();
				Color[] opacityColors = opacitytex.GetPixels();
				for (int i = 0; i < albedoColors.Length; i++)
				{
					Color c = albedoColors[i];
					albedoColors[i] = new Color(c.r, c.g, c.b, opacityColors[i].r);
				}
				combined.SetPixels(albedoColors);
				combined.Apply();
			}
			else
			{
				if (albedotex == null)
					combined = opacitytex;
				else
					combined = albedotex;
			}

			textures[albedo.image] = combined;
		}

		public static void ConvertMaterialExtract(Material.Native.MaterialDefinition materialExtract, ref UnityEngine.Material material, ref Dictionary<uint, Texture2D> textureMap)
		{
			material.name = materialExtract.name;

			float alpha = (materialExtract.opacity._type == Material.Native.CoeffOrTexture.Type.COEFF ? (float)materialExtract.opacity.coeff : 1f);

			material.SetFloat("_Cutoff", 0.5f);

			if (materialExtract.opacity._type == Material.Native.CoeffOrTexture.Type.TEXTURE)
			{
				CombineAlbedoAndOpacity(materialExtract.albedo.texture, materialExtract.opacity.texture, textureMap);
				material.SetFloat("_Mode", 1);
				material.SetOverrideTag("RenderType", "TransparentCutout");
				material.SetInt("_SrcBlend", (int)BlendMode.One);
				material.SetInt("_DstBlend", (int)BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.SetFloat("_Cutoff", 0.5f);
				material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, alpha)); // Standard
				material.SetColor("_BaseColor", new Color(1.0f, 1.0f, 1.0f, alpha)); // HDRP and LWRP
				material.EnableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

				// URP specific
				material.SetFloat("_AlphaClip", 1);
				material.SetFloat("_Surface", 0); // Opaque surface, but alpha clipping
			}
			else if (alpha < 1f)
			{
				material.SetFloat("_Mode", 3);
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_SrcBlend", (int)BlendMode.One);
				material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;

				// URP specific
				material.SetFloat("_Surface", 1); // Transparent
				material.SetFloat("_Blend", 0); // Alpha blend
				material.SetFloat("_AlphaClip", 0);
			}

			if (materialExtract.albedo._type == Material.Native.ColorOrTexture.Type.COLOR)
			{
				Color color = new Color((float)materialExtract.albedo.color.r, (float)materialExtract.albedo.color.g, (float)materialExtract.albedo.color.b, alpha);
				material.SetColor("_Color", color); // Standard
				material.SetColor("_BaseColor", color); // HDRP and LWRP
			}
			else if (materialExtract.albedo._type == Material.Native.ColorOrTexture.Type.TEXTURE)
			{
				material.SetTexture("_MainTex", materialExtract.albedo.texture, ref textureMap); // Standard
				material.SetTexture("_BaseMap", materialExtract.albedo.texture, ref textureMap); // HDRP and URP
			}

			if (materialExtract.normal._type == Material.Native.ColorOrTexture.Type.TEXTURE)
			{
				material.EnableKeyword("_NORMALMAP");
				material.SetTexture("_BumpMap", materialExtract.normal.texture, ref textureMap);
			}

			if (materialExtract.roughness._type == Material.Native.CoeffOrTexture.Type.TEXTURE)
			{
				material.EnableKeyword("_METALLICGLOSSMAP");
				Texture2D roughness = GetTexture(materialExtract.roughness.texture, ref textureMap);
				Texture2D specular = roughness;
				if (materialExtract.metallic._type == Material.Native.CoeffOrTexture.Type.TEXTURE)
				{
					specular = GetTexture(materialExtract.metallic.texture, ref textureMap);
				}
				else
				{
					//Unity does not handle transparency with metallic. If there is metallic the material will behave as an opaque mode
					float metallicCoeff = Mathf.Clamp((float)materialExtract.metallic.coeff, 0, 1);
					if (alpha < 1 && material.shader.name == ShaderUtilities.GetDefaultShader().name)
						material.SetFloat("_Metallic", 0);
					else
						material.SetFloat("_Metallic", metallicCoeff);
				}
				material.SetTexture("_MetallicGlossMap", CreateSpecGlossMap(specular, roughness));
			}
			else
			{
				material.SetFloat("_Glossiness", 1 - Mathf.Clamp((float)materialExtract.roughness.coeff, 0, 1));
				material.SetFloat("_Smoothness", 1 - Mathf.Clamp((float)materialExtract.roughness.coeff, 0, 1)); // URP specific

				if (materialExtract.metallic._type == Material.Native.CoeffOrTexture.Type.TEXTURE)
				{
					material.EnableKeyword("_METALLICGLOSSMAP");
					Texture2D metallic = GetTexture(materialExtract.metallic.texture, ref textureMap);
					material.SetTexture("_MetallicGlossMap", metallic);
				}
				else
				{
					//Unity does not handle transparency with metallic. If there is metallic the material will behave as an opaque mode
					float metallicCoeff = Mathf.Clamp((float)materialExtract.metallic.coeff, 0, 1);
					if (alpha < 1 && material.shader.name == ShaderUtilities.GetDefaultShader().name)
						material.SetFloat("_Metallic", 0);
					else
						material.SetFloat("_Metallic", metallicCoeff);
				}
			}

			if (materialExtract.ao._type == Material.Native.CoeffOrTexture.Type.COEFF)
				material.SetFloat("_OcclusionStrength", Mathf.Clamp((float)materialExtract.ao.coeff, 0, 1));
			else if (materialExtract.ao._type == Material.Native.CoeffOrTexture.Type.TEXTURE)
				material.SetTexture("_OcclusionMap", materialExtract.ao.texture, ref textureMap);

			if (materialExtract.emissive._type == Material.Native.ColorOrTexture.Type.COLOR)
			{
				Color color = new Color((float)materialExtract.emissive.color.r, (float)materialExtract.emissive.color.g, (float)materialExtract.emissive.color.b, alpha);
				if (color != Color.black)
				{
					material.EnableKeyword("_EMISSION");
					material.SetColor("_EmissionColor", color);
					material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
				}
			}
			else if (materialExtract.emissive._type == Material.Native.ColorOrTexture.Type.TEXTURE)
			{
				material.EnableKeyword("_EMISSION");
				material.SetColor("_EmissionColor", Color.white);
				material.SetTexture("_EmissionMap", materialExtract.emissive.texture, ref textureMap);
				material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
			}
		}

		public static void ConvertCustomMaterial(Material.Native.MaterialDefinition materialExtract, ref UnityEngine.Material material, ref Dictionary<uint, Texture2D> textureMap)
		{
			material.name = materialExtract.name;

			string[] properties = Core.Native.NativeInterface.ListProperties(materialExtract.id).list;

			foreach (var property in properties)
			{
				string keyword = ShaderUtilities.GetShaderKeyword(material.shader.name, property);
				if (keyword == null) continue;

				string value = Core.Native.NativeInterface.GetProperty(materialExtract.id, property);

				Material.Native.ShaderUniformType type = Material.Native.NativeInterface.GetUniformPropertyType(materialExtract.id, property);
				try
				{
					switch (type)
					{
						case Material.Native.ShaderUniformType.BOOLEAN:
							material.SetFloat(keyword, value == "True" ? 1f : 0f);
							break;
						case Material.Native.ShaderUniformType.COEFF:
							material.SetFloat(keyword, Mathf.Clamp(float.Parse(value), 0, 1));
							break;
						case Material.Native.ShaderUniformType.COLOR:
							value = value.Substring(1, value.Length - 2);
							string[] colorValues = value.Split(',');
							if (keyword.EndsWith("_alpha"))
							{
								keyword = keyword.Substring(0, keyword.Length - "_alpha".Length);
								Color color = TryGetColor(material, value);
								material.SetColor(keyword, new Color(color.r, color.g, color.b, float.Parse(colorValues[0])));
							}
							else
							{
								float alphaColor = TryGetColor(material, keyword).a;
								material.SetColor(keyword, new Color(float.Parse(colorValues[0]), float.Parse(colorValues[1]), float.Parse(colorValues[2]), alphaColor));
							}
							break;
						case Material.Native.ShaderUniformType.COLOR_ALPHA:
							value = value.Substring(1, value.Length - 2);
							string[] values = value.Split(',');
							if (keyword.EndsWith("_alpha"))
							{
								keyword = keyword.Substring(0, keyword.Length - "_alpha".Length);
								Color color = TryGetColor(material, keyword);
								material.SetColor(keyword, new Color(color.r, color.g, color.b, float.Parse(values[0])));
							}
							else
							{
								float alpha = TryGetColor(material, keyword).a;
								if (values.Length == 4) alpha = float.Parse(values[3]);
								material.SetColor(keyword, new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), alpha));
							}
							break;
						case Material.Native.ShaderUniformType.TEXTURE:
							uint image = int.Parse(value).ToUInt32();
							textureMap.TryGetValue(image, out Texture2D texture);
							if (texture == null)
							{
								texture = ConvertImageDefinition(Material.Native.NativeInterface.GetImageDefinition(image));
							}
							if (textureMap.ContainsKey(image))
								material.SetTexture(keyword, texture);
							textureMap.Add(image, texture);
							break;
						case Material.Native.ShaderUniformType.COLORORTEXTURE:
							image = int.Parse(value).ToUInt32();
							textureMap.TryGetValue(image, out Texture2D text2d);
							if (text2d == null)
							{
								texture = ConvertImageDefinition(Material.Native.NativeInterface.GetImageDefinition(image));
							}
							if (textureMap.ContainsKey(image))
								material.SetTexture(keyword, text2d);
							textureMap.Add(image, text2d);
							break;
						default:
							break;
					}
				}
				catch (System.Exception e) { Debug.LogError(e); }
			}
		}

		public static UnityEngine.Material ConvertMaterialExtract(Material.Native.MaterialDefinition materialExtract, ref Dictionary<uint, Texture2D> textureMap, Shader shader = null)
		{
			// Check custom materials
			if (materialExtract.id != 0 && Material.Native.NativeInterface.GetMaterialPatternType(materialExtract.id) == Material.Native.MaterialPatternType.CUSTOM)
			{
				var customMaterialPattern = Material.Native.NativeInterface.GetCustomMaterialPattern(materialExtract.id);
				shader = ShaderUtilities.GetCustomShader(Core.Native.NativeInterface.GetProperty(customMaterialPattern, "Name"));
				if (shader != null)
				{
					var mat = new UnityEngine.Material(shader);
					ConvertCustomMaterial(materialExtract, ref mat, ref textureMap);
					return mat;
				}
			}

			var material = new UnityEngine.Material(shader == null ? ShaderUtilities.GetDefaultShader() : shader); // todo: pick right shader

			ConvertMaterialExtract(materialExtract, ref material, ref textureMap);
			return material;
		}

		public static UnityEngine.Material[] ConvertMaterialExtracts(Material.Native.MaterialDefinitionList materialExtractList, ref Dictionary<uint, Texture2D> textureMap, Shader shader, bool useMaterialsInResources, Dictionary<string, UnityEngine.Material> materialsInResources)
		{
			var materials = new UnityEngine.Material[materialExtractList.length];
			for (int i = 0; i < materials.Length; i++)
			{
				if (useMaterialsInResources && materialsInResources.TryGetValue(materialExtractList[i].name, out UnityEngine.Material resourceMaterial))
                {
					materials[i] = resourceMaterial;
                }
				else
                {
					materials[i] = ConvertMaterialExtract(materialExtractList[i], ref textureMap, shader);
				}
			}
			return materials;
		}

		public static Material.Native.MaterialDefinition CreateStandardPixyzMaterial(string name, UnityEngine.Color baseColor)
		{
			Material.Native.MaterialDefinition nativeMaterial = new Material.Native.MaterialDefinition();
			nativeMaterial.name = name;

			Core.Native.ColorAlpha mainColor = baseColor.ToPiXYZColorAlpha();

			nativeMaterial.opacity = new Material.Native.CoeffOrTexture();
			nativeMaterial.opacity._type = Material.Native.CoeffOrTexture.Type.COEFF;
			nativeMaterial.opacity.coeff = mainColor.a;

			nativeMaterial.albedo = new Material.Native.ColorOrTexture();
			nativeMaterial.albedo._type = Material.Native.ColorOrTexture.Type.COLOR;
			nativeMaterial.albedo.color = mainColor.ColorAlphaToColor();

			nativeMaterial.normal = new Material.Native.ColorOrTexture();
			nativeMaterial.normal._type = Material.Native.ColorOrTexture.Type.COLOR;
			nativeMaterial.normal.color = Color.black.ToPiXYZColor();

			nativeMaterial.metallic = new Material.Native.CoeffOrTexture();
			nativeMaterial.metallic._type = Material.Native.CoeffOrTexture.Type.COEFF;
			nativeMaterial.metallic.coeff = 0.5;

			nativeMaterial.roughness = new Material.Native.CoeffOrTexture();
			nativeMaterial.roughness._type = Material.Native.CoeffOrTexture.Type.COEFF;
			nativeMaterial.roughness.coeff = 0.5;

			nativeMaterial.ao = new Material.Native.CoeffOrTexture();
			nativeMaterial.ao._type = Material.Native.CoeffOrTexture.Type.COEFF;
			nativeMaterial.ao.coeff = 0;

			nativeMaterial.emissive = new Material.Native.ColorOrTexture();
			nativeMaterial.emissive._type = Material.Native.ColorOrTexture.Type.COLOR;
			nativeMaterial.emissive.color = new Core.Native.Color() { r = 0, g = 0, b = 0 };

			return nativeMaterial;
		}

		public static Material.Native.MaterialDefinition ConvertMaterial(UnityEngine.Material material, ref Dictionary<Texture2D, uint> texMap)
		{
			Material.Native.MaterialDefinition materialExtract = new Material.Native.MaterialDefinition();
			materialExtract.name = material.name;
			materialExtract.id = material.GetInstanceID().ToUInt32();

			Color mainColor = Color.white;
			if (material.HasProperty("_Color"))
			{
				mainColor = material.GetColor("_Color");
			}
			else if (material.HasProperty("_BaseColor"))
			{
				mainColor = material.GetColor("_BaseColor");
			}

			materialExtract.opacity = new Material.Native.CoeffOrTexture();
			materialExtract.opacity._type = Material.Native.CoeffOrTexture.Type.COEFF;

			if (material.HasProperty("_Mode"))
			{
				if (material.GetFloat("_Mode") != 0)
					materialExtract.opacity.coeff = mainColor.a;
				else
					materialExtract.opacity.coeff = 1.0;
			}

			materialExtract.albedo = new Material.Native.ColorOrTexture();
			if (material.TryGetTexture("_MainTex", out Material.Native.Texture albedoTexture, ref texMap))
			{
				materialExtract.albedo._type = Material.Native.ColorOrTexture.Type.TEXTURE;
				materialExtract.albedo.texture = albedoTexture;
			}
			else
			{
				materialExtract.albedo._type = Material.Native.ColorOrTexture.Type.COLOR;
				materialExtract.albedo.color = mainColor.ToPiXYZColor();
			}

			materialExtract.normal = new Material.Native.ColorOrTexture();
			if (material.TryGetTexture("_BumpMap", out Material.Native.Texture normalTexture, ref texMap))
			{
				materialExtract.normal._type = Material.Native.ColorOrTexture.Type.TEXTURE;
				materialExtract.normal.texture = normalTexture;
			}
			else
			{
				materialExtract.normal._type = Material.Native.ColorOrTexture.Type.COLOR;
			}

			materialExtract.metallic = new Material.Native.CoeffOrTexture();
			if (material.TryGetTexture("_MetallicGlossMap", out Material.Native.Texture metallicTexture, ref texMap))
			{
				materialExtract.metallic._type = Material.Native.CoeffOrTexture.Type.TEXTURE;
				materialExtract.metallic.texture = metallicTexture;
			}
			else
			{
				materialExtract.metallic._type = Material.Native.CoeffOrTexture.Type.COEFF;
				materialExtract.metallic.coeff = material.TryGetFloat("_Metallic", 0.5f);
			}

			materialExtract.roughness = new Material.Native.CoeffOrTexture();
			materialExtract.roughness._type = Material.Native.CoeffOrTexture.Type.COEFF;
			if (material.HasProperty("_Glossiness"))
				materialExtract.roughness.coeff = 1.0 - material.TryGetFloat("_Glossiness", 0.5f);

			materialExtract.ao = new Material.Native.CoeffOrTexture();
			if (material.TryGetTexture("_OcclusionMap", out Material.Native.Texture occlusionTexture, ref texMap))
			{
				materialExtract.ao._type = Material.Native.CoeffOrTexture.Type.TEXTURE;
				materialExtract.ao.texture = occlusionTexture;
			}
			else
			{
				materialExtract.ao._type = Material.Native.CoeffOrTexture.Type.COEFF;
				materialExtract.ao.coeff = material.TryGetFloat("_OcclusionStrength", 1.0f);
			}

			if (material.TryGetTexture("_EmissionMap", out Material.Native.Texture emissiveTexture, ref texMap))
			{
				materialExtract.emissive._type = Material.Native.ColorOrTexture.Type.TEXTURE;
				materialExtract.emissive.texture = emissiveTexture;
			}
			else
			{
				materialExtract.emissive._type = Material.Native.ColorOrTexture.Type.COLOR;
				materialExtract.emissive.color = material.TryGetColor("_EmissionColor").ToPiXYZColor();
			}

			return materialExtract;
		}
		#endregion
	}
}
