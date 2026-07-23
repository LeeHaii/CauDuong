using Pixyz.Toolbox.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pixyz
{
    public class OptimizeSDKUtils
    {
        public static void ExportMeshes(uint scene, string filePath)
        {
            //PiXYZScene::instance()->getRootOccurrence()->addChild(scene);
            Core.Native.NativeInterface.Save(filePath);
            Scene.Native.NativeInterface.SetParent(scene, 0, true, 0);
            Scene.Native.NativeInterface.ResetPartTransform(scene);
        }

        public static uint BakeMaterials(Scene.Native.OccurrenceList sourceOccurrences, uint targetOccurrence, int resolution, int padding, bool applyMaterial, Algo.Native.BakingMethod method, Algo.Native.BakeMapList maps, Geom.Native.Matrix4List sourceMatrices, Geom.Native.Matrix4 targetMatrix)
        {
            // Create normals/tangents/UV on the source to properly bake normals(/!\ don't override)
            Algo.Native.NativeInterface.CreateNormals(sourceOccurrences, 180, false, false);
            Algo.Native.NativeInterface.MapUvOnAABB(sourceOccurrences, false, 0.1f, 0, false);
            Algo.Native.NativeInterface.CreateTangents(sourceOccurrences, -1, 0, false);

            Scene.Native.OccurrenceList targetOccurrences = new Scene.Native.OccurrenceList() { list = new uint[] { targetOccurrence } };
            
            Algo.Native.NativeInterface.RemoveDegeneratedPolygons(targetOccurrences, 0.001);
            Algo.Native.NativeInterface.CreateNormals(targetOccurrences, 180, true, false);
            // Creates some UVs for the baking process
            Algo.Native.NativeInterface.MapUvOnAABB(targetOccurrences, false, 0.1f, 0, true);

            Algo.Native.NativeInterface.RepackUV(targetOccurrences, 0, true, resolution, padding * 2, true, 3, true);
            // Maximizes space taken in the UVs for maximum useful texture density
            Algo.Native.NativeInterface.NormalizeUV(targetOccurrences, 0, -1, false, false, false);
            Algo.Native.NativeInterface.CreateTangents(targetOccurrences, -1, 0, true);

            Algo.Native.getPixelValueList callbackList = new Algo.Native.getPixelValueList(0);
            Material.Native.ImageList images = Algo.Native.NativeInterface.BakeMaps(targetOccurrences, sourceOccurrences, maps, 0, resolution, padding, true, "", new Algo.Native.CustomBakeMapList(0), -1, method, 0, false, 0.0, callbackList);

            uint materialId = Material.Native.NativeInterface.CreateMaterial("Baked", "PBR");

            for(int index = 0; index < maps.length; index++)
            {
                var map = maps[index];
                switch(map.type)
                {
                    case Algo.Native.MapType.Diffuse:
                        Core.Native.NativeInterface.SetProperty(materialId, "albedo", "TEXTURE([[1,1],[0,0]," + images[index].ToString() + ", 0])");
                        break;
                    case Algo.Native.MapType.Normal:
                        uint normalTangent = Algo.Native.NativeInterface.ConvertNormalMap(targetOccurrences, images[index], 0, true, false, true, true, false, resolution, padding);
                        Core.Native.NativeInterface.SetProperty(materialId, "normal", "TEXTURE([[1,1],[0,0]," + normalTangent + ", 0])");
                        Algo.Native.NativeInterface.OrientNormalMap(normalTangent);
                        break;
                    case Algo.Native.MapType.Roughness:
                        Core.Native.NativeInterface.SetProperty(materialId, "roughness", "TEXTURE([[1,1],[0,0]," + images[index].ToString() + ", 0])");
                        break;
                    case Algo.Native.MapType.Metallic:
                        Core.Native.NativeInterface.SetProperty(materialId, "metallic", "TEXTURE([[1,1],[0,0]," + images[index].ToString() + ", 0])");
                        break;
                    case Algo.Native.MapType.AO:
                        Core.Native.NativeInterface.SetProperty(materialId, "ao", "TEXTURE([[1,1],[0,0]," + images[index].ToString() + ", 0])");
                        break;
                    case Algo.Native.MapType.Emissive:
                        Core.Native.NativeInterface.SetProperty(materialId, "emissive", "TEXTURE([[1,1],[0,0]," + images[index].ToString() + ", 0])");
                        break;
                    default:
                        break;
                }
            }

            if(applyMaterial)
            {
                Polygonal.Native.MeshDefinition mesh = Polygonal.Native.NativeInterface.GetMeshDefinition(Scene.Native.NativeInterface.GetPartMesh(Scene.Native.NativeInterface.GetComponent(targetOccurrence, Scene.Native.ComponentType.Part, true)));
                if (mesh.id==0) return materialId;

                Material.Native.MaterialDefinition material = Material.Native.NativeInterface.GetMaterialDefinition(materialId);

                if (material.id == 0) return materialId;

                Scene.Native.NativeInterface.SetOccurrenceMaterial(targetOccurrence, materialId);
                Scene.Native.NativeInterface.TransferMaterialsOnPatches(targetOccurrence);
            }

            return materialId;
        }

        public static uint CombineMeshes(Scene.Native.OccurrenceList occurrenceList, bool forceUVGeneration, int resolution)
        {
            Scene.Native.OccurrenceList merged = Scene.Native.NativeInterface.MergeParts(occurrenceList, Scene.Native.MergeHiddenPartsMode.MergeSeparately);

            Algo.Native.NativeInterface.CopyUV(merged, 0, 1);
            Algo.Native.NativeInterface.MapUvOnAABB(merged, true, 1.0, 1, forceUVGeneration);
            Algo.Native.NativeInterface.CreateTangents(merged, -1, 1, forceUVGeneration);
            bool uniformRatioRepack = true;
            Algo.Native.NativeInterface.RepackUV(merged, 1, false, resolution, 0, uniformRatioRepack, 3, true);
            Algo.Native.NativeInterface.NormalizeUV(merged, 1, 1, false, true, false);

            Algo.Native.BakeMap diffuse = new Algo.Native.BakeMap();
            diffuse.type = Algo.Native.MapType.Diffuse;
            Algo.Native.BakeMap normal = new Algo.Native.BakeMap();
            diffuse.type = Algo.Native.MapType.Normal;
            Algo.Native.BakeMap metallic = new Algo.Native.BakeMap();
            diffuse.type = Algo.Native.MapType.Metallic;
            Algo.Native.BakeMap roughness = new Algo.Native.BakeMap();
            diffuse.type = Algo.Native.MapType.Roughness;
            Algo.Native.BakeMap opacity = new Algo.Native.BakeMap();
            diffuse.type = Algo.Native.MapType.Opacity;
            Algo.Native.BakeMap ao = new Algo.Native.BakeMap();
            diffuse.type = Algo.Native.MapType.AO;
            Algo.Native.BakeMapList mapsToBake = new Algo.Native.BakeMapList(new Algo.Native.BakeMap[] {
                        diffuse,
                        normal,
                        metallic,
                        roughness,
                        opacity,
                        ao
                    });

            Algo.Native.getPixelValueList callbackList = new Algo.Native.getPixelValueList(0);
            Material.Native.ImageList maps = Algo.Native.NativeInterface.BakeMaps(merged, new Scene.Native.OccurrenceList(), mapsToBake, 1, resolution, 1, true, "", new Algo.Native.CustomBakeMapList(0), -1, Algo.Native.BakingMethod.RayOnly, 0.1, false, 0.0, callbackList);

            Algo.Native.NativeInterface.ConvertNormalMap(merged, maps[1], 1, true, false, true, true, true, -1, 1);
            Algo.Native.NativeInterface.SwapUvChannels(merged, 0, 1);
            Algo.Native.NativeInterface.RemoveUV(merged, 1);
            Geom.Native.Point2 tiling = new Geom.Native.Point2() { x = 1, y = 1 };
            Geom.Native.Point2 offset = new Geom.Native.Point2() { x = 1, y = 1 };

            Material.Native.MaterialDefinition matDef = new Material.Native.MaterialDefinition()
            {
                name = "Merged",
                id = 0,
                albedo = new Material.Native.ColorOrTexture() { texture = new Material.Native.Texture() { image = maps[0], channel = 0, offset = offset, tilling = tiling }, _type = Material.Native.ColorOrTexture.Type.TEXTURE },
                normal = new Material.Native.ColorOrTexture() { texture = new Material.Native.Texture() { image = maps[1], channel = 0, offset = offset, tilling = tiling }, _type = Material.Native.ColorOrTexture.Type.TEXTURE },
                metallic = new Material.Native.CoeffOrTexture() { texture = new Material.Native.Texture() { image = maps[2], channel = 0, offset = offset, tilling = tiling }, _type = Material.Native.CoeffOrTexture.Type.TEXTURE },
                roughness = new Material.Native.CoeffOrTexture() { texture = new Material.Native.Texture() { image = maps[3], channel = 0, offset = offset, tilling = tiling }, _type = Material.Native.CoeffOrTexture.Type.TEXTURE },
                opacity = new Material.Native.CoeffOrTexture() { texture = new Material.Native.Texture() { image = maps[4], channel = 0, offset = offset, tilling = tiling }, _type = Material.Native.CoeffOrTexture.Type.TEXTURE },
                ao = new Material.Native.CoeffOrTexture() { texture = new Material.Native.Texture() { image = maps[5], channel = 0, offset = offset, tilling = tiling }, _type = Material.Native.CoeffOrTexture.Type.TEXTURE }
            };

            var mat = Material.Native.NativeInterface.CreateMaterialFromDefinition(matDef);
            Core.Native.NativeInterface.SetProperty(merged[0], "Material", mat.ToString());

            uint outputMesh = Scene.Native.NativeInterface.GetPartMesh(Scene.Native.NativeInterface.GetComponent(merged[0], Scene.Native.ComponentType.Part, true));

            Scene.Native.NativeInterface.TransferMaterialsOnPatches(merged[0]);
            Algo.Native.NativeInterface.DeletePatches(merged, true);

            return outputMesh;
        }

        public static UnityEngine.Material CreateImpostorMaterial(int iImpostorType, int iRenderOn, Algo.Native.OctahedralImpostor octahedralIMP, int atlasSize, Texture2D albedo, Texture2D normal, Texture2D depth, Texture2D ao, Texture2D roughness)
        {

            OctahedronImpostorType impostorType = (OctahedronImpostorType)iImpostorType;
            RenderImpostorOn renderOn = (RenderImpostorOn)iRenderOn;

            Shader shader = Shader.Find("Pixyz/PxzImpostor");

            UnityEngine.Material material = new UnityEngine.Material(shader);

            //shader properties
            material.SetTexture(Shader.PropertyToID("_PxzImposterAlbedoTexture"), albedo);
            material.SetTexture(Shader.PropertyToID("_PxzImposterNormalTexture"), normal);
            material.SetTexture(Shader.PropertyToID("_PxzImposterDepthTexture"), depth);
            material.SetTexture(Shader.PropertyToID("_PxzImposterRoughnessTexture"), roughness);
            material.SetTexture(Shader.PropertyToID("_PxzImposterAOTexture"), ao);

            if (impostorType == OctahedronImpostorType.HemiOctahedron)
                material.SetFloat("_FullOcta", 0);
            if (renderOn == RenderImpostorOn.OrientedBoundingBox || renderOn == RenderImpostorOn.CustomMesh)
                material.SetFloat("_PreciseMesh", 1);

            material.SetInt("_PxzImposterFramesCount", atlasSize);
            material.SetFloat("_OctahedronDiameter", (float)(octahedralIMP.Radius * 2.0));
            material.enableInstancing = true;
            return material;
        }
        public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
        {
            byte[] _bytes = _texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(_fullPath, _bytes);
            Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
        }
    }

}
