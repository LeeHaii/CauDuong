using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Pixyz.OptimizeSDK.Runtime;
using Pixyz.OptimizeSDK.Editor.MeshProcessing;
using Pixyz.OptimizeSDK;
using Pixyz.Commons.Extensions;
using System.Collections.Generic;
namespace Pixyz.LODTools.Editor
{
    public class LODBuilder
    {
        public Action<bool> generationCompleted;
        private PixyzContext[] _contexts = null;
        private LODProcess _process = null;
        private int _currentLODIndex;
        public int currentIndex { get { return _currentLODIndex; } set { _currentLODIndex = value; indexChanged.Invoke(); } }
        public UnityEvent indexChanged = new UnityEvent();

        // Some rules such as point cloud decimation can automatically compute the threshold based on the applied optimization
        public Dictionary<uint, double> meshToThresholds = new Dictionary<uint, double>();

        public PixyzContext[] Contexts => _contexts;

        private Algo.Native.OctahedralImpostor octahedralIMP;
        private double[] _pointCloudsBounds;


        public async Task<PixyzContext[]> BuildLOD(PixyzContext sourceContext, LODProcess process, bool async = true)
        {
            if (_contexts != null && _contexts.Length > 0)
            {
                _contexts[0].Dispose();
            }
            indexChanged = new UnityEvent();
            _process = process;
            currentIndex = 1;
            _contexts = new PixyzContext[process.Rules.Count + 1];
            _contexts[0] = sourceContext;

            bool finishWithoutErrors = true;
            try
            {
                while(_currentLODIndex < _contexts.Length)
                {
                    if(async)
                    {
                        if (!await Task.Factory.StartNew(StartProcess))
                            break;
                    }
                    else
                    {
                        if (!StartProcess())
                            break;
                    }
                    ++currentIndex;
                }
            }
            catch(System.Exception e)
            {
                UnityEngine.Debug.LogError($"[LODGenerationError] {e.Message}\n{e.StackTrace}");
                finishWithoutErrors = false;
            }
            
            Core.Native.NativeInterface.SetCurrentThreadAsProcessThread();

            generationCompleted?.Invoke(finishWithoutErrors);

            return _contexts;
        }

        public bool StartProcess()
        {
            try
            {
                // Make sure Pixyz thread is the same as this one
                Core.Native.NativeInterface.SetCurrentThreadAsProcessThread();
                LODRule rule = _process.Rules[_currentLODIndex - 1];

                // Clone source context (LOD0 by default) (meshes, matrices) so we don't have to transfer them again from Unity to Pixyz
                PixyzContext srcContext = _contexts[_process.Sources[_currentLODIndex - 1]];
                PixyzContext context = srcContext.Clone() as PixyzContext;
                srcContext.LinkContext(context);
                _contexts[_currentLODIndex] = context;

                // Create scene from meshes and matrices
                uint scene = Scene.Native.NativeInterface.CreateSceneFromMeshes(context.pixyzMeshes, context.pixyzMatrices, true);
                Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { scene });

                // Assign this scene to an alternative tree so it does not interfere with other lod contexts
                uint tree = Scene.Native.NativeInterface.CreateAlternativeTree(currentIndex.ToString(), 0);
                Scene.Native.NativeInterface.SetParent(scene, tree, false, 0);

                if (rule.isDecimatePointCloudActivated)
                {
                    // Delete duplicated points
                    Polygonal.Native.TopologyCategoryMask topologyMask = new Polygonal.Native.TopologyCategoryMask();
                    topologyMask.connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD;
                    topologyMask.dimension = Polygonal.Native.TopologyDimensionMask.FACE;
                    Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001f, topologyMask);

                    double splatSize = 0.0035;

                    var partOccurrences = Scene.Native.NativeInterface.GetPartOccurrences(scene).list;

                    if (_pointCloudsBounds == null)
                    {
                        _pointCloudsBounds = new double[partOccurrences.Length];
                    }

                    for (int i = 0; i < partOccurrences.Length; i++)
                    {
                        var partOccurrenceList = new Scene.Native.OccurrenceList(new uint[] { partOccurrences[i] });
                        double previousPointCount = Scene.Native.NativeInterface.GetVertexCount(partOccurrenceList, false, false, true, false);
                        double currentPointCount = previousPointCount;

                        var bounds = Scene.Native.NativeInterface.GetAABB(partOccurrenceList);

                        if (_pointCloudsBounds[i] == default(double))
                        {
                            _pointCloudsBounds[i] = Math.Max(Math.Max(bounds.high.x - bounds.low.x, bounds.high.y - bounds.low.y), bounds.high.z - bounds.low.z);
                        }
                        double voxelSizePointCloud = _pointCloudsBounds[i];

                        // Takes maximum between : 
                        // - 1/200th (maximum we can allow for voxelization in decimation, for memory consideration)
                        // - The distance required for a LOD a 95%
                        double distance = Math.Max(0.005 * voxelSizePointCloud, (double)_currentLODIndex * (voxelSizePointCloud * splatSize) / 0.95);
                        double nextDistance = distance;

                        int k = 0;
                        while (currentPointCount > 0.5f * previousPointCount)
                        {
                            distance = nextDistance;
                            Algo.Native.NativeInterface.DecimatePointClouds(partOccurrenceList, distance);
                            currentPointCount = Scene.Native.NativeInterface.GetVertexCount(partOccurrenceList, false, false, true, false);
                            nextDistance = distance * (Math.Max(0, 10 * Math.Pow(currentPointCount / previousPointCount - 0.4, 3)) + 1);

                            if (++k > 3)
                            {
                                break;
                            }
                        }

                        double threshold = (voxelSizePointCloud * splatSize) / distance;
                        meshToThresholds.Add(Scene.Native.NativeInterface.GetPartMesh(Scene.Native.NativeInterface.GetComponent(partOccurrences[i], Scene.Native.ComponentType.Part, true)), threshold);
                    }

                    Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]); // MANDATORY
                    return true; // No possible other optimizations: return
                }

                if (rule.isRepairActivated)
                {
                    // Repair meshes (weld duplicated vertices, fix t-junctions...)
                    Algo.Native.NativeInterface.RepairMesh(occurrenceList, rule.repairParameters.tolerance, true, false);
                }   
                else
                {
                    // Weld duplicated vertices in any case to lighten the meshes
                    Polygonal.Native.TopologyCategoryMask topologyMask = new Polygonal.Native.TopologyCategoryMask();
                    topologyMask.connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD;
                    topologyMask.dimension = Polygonal.Native.TopologyDimensionMask.FACE;
                    Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001f, topologyMask);
                }

                if (rule.isBillboardActivated)
                {
                    // Create billboards
                    uint outputOccurrence = Algo.Native.NativeInterface.CreateBillboard(occurrenceList, rule.billoardParamters.resolution, rule.billoardParamters.XPositiveEnable, rule.billoardParamters.XNegativeEnable,
                        rule.billoardParamters.YPositiveEnable, rule.billoardParamters.YNegativeEnable, rule.billoardParamters.ZPositiveEnable, rule.billoardParamters.ZNegativeEnable, true, false);

                    context.pixyzMeshes = new Polygonal.Native.MeshList(new uint[] { Scene.Native.NativeInterface.GetPartMesh(Scene.Native.NativeInterface.GetComponent(outputOccurrence, Scene.Native.ComponentType.Part, true)) });
                    context.pixyzMatrices = new Geom.Native.Matrix4List(new Geom.Native.Matrix4[] { Conversions.Identity() });

                    return true; // No possible other optimizations: return
                }

                if (rule.isImpostorActivated)
                {
                    // Generate impostors

                    Geom.Native.Matrix4 toRightHand = Pixyz.OptimizeSDK.Conversions.Identity();
                    toRightHand[2][2] *= -1;
                    Geom.Native.Matrix4 rotation90 = new Geom.Native.Matrix4();
                    rotation90.tab = new Geom.Native.Array4[4];

                    rotation90.tab[0] = new Geom.Native.Array4(new double[] { 0.0, 0.0, 1.0, 0.0 });
                    rotation90.tab[1] = new Geom.Native.Array4(new double[] { 0.0, 1.0, 0.0, 0.0 });
                    rotation90.tab[2] = new Geom.Native.Array4(new double[] { -1.0, 0.0, 0.0, 0.0 });
                    rotation90.tab[3] = new Geom.Native.Array4(new double[] { 0.0, 0.0, 0.0, 1.0 });


                    Geom.Native.Matrix4 finalMat = Geom.Native.NativeInterface.MultiplyMatrices(toRightHand, rotation90);
                    Scene.Native.NativeInterface.SetLocalMatrix(scene, finalMat);

                    //Bake Impostor
                    octahedralIMP = Algo.Native.NativeInterface.BakeImpostor(occurrenceList[0], rule.impostorParameters.atlasSize, rule.impostorParameters.atlasSize, rule.impostorParameters.type == OptimizeSDK.Runtime.ImpostorType.HemiOctahedron ? true : false, rule.impostorParameters.resolution, 0, true, false, true);
                    Scene.Native.NativeInterface.SetLocalMatrix(scene, Pixyz.OptimizeSDK.Conversions.Identity());
                    Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);

                    if (rule.impostorParameters.renderOn == RenderImpostorOn.OrientedBoundingBox)
                    {
                        // Generate OBB mesh inside Pixyz (and assign the octahedral material later in post process)
                        var obbOccurrence = Scene.Native.NativeInterface.CreateOBBMesh(occurrenceList[0]);
                        Scene.Native.NativeInterface.MovePivotPointToOccurrenceCenter(new Scene.Native.OccurrenceList(new uint[] { obbOccurrence }), false);
                        Scene.Native.NativeInterface.ResetPartTransform(obbOccurrence);

                        // Retrieve output mesh
                        var outputMesh = Scene.Native.NativeInterface.GetPartMesh(Scene.Native.NativeInterface.GetComponent(obbOccurrence, Scene.Native.ComponentType.Part, true));
                        context.pixyzMeshes = new Polygonal.Native.MeshList(new uint[] { outputMesh });
                        context.pixyzMatrices = new Geom.Native.Matrix4List(new Geom.Native.Matrix4[] { Conversions.Identity() });
                    }
                    else
                    {
                        // Generate a plane so we have something as an output and post-process is called with a renderer
                        // In post-process, the plane will be replaced by either a quad or a custom mesh
                        var outputMesh = Scene.Native.NativeInterface.CreatePlane(1, 1, 1, 1, false);
                        context.pixyzMeshes = new Polygonal.Native.MeshList(new uint[] { outputMesh });
                        context.pixyzMatrices = new Geom.Native.Matrix4List(new Geom.Native.Matrix4[] { Conversions.Identity() });
                    }
                    
                    return true; // no other optimizations possible after this: return
                }

                if (rule.isRemeshFieldAlignedActivated || rule.isRemeshActivated)
                {
                    // Retopologize

                    Scene.Native.OccurrenceList newOccurrenceList = new Scene.Native.OccurrenceList(1);

                    if (rule.isRemeshFieldAlignedActivated)
                    {
                        // Re-mesh
                        if(rule.remeshFieldAlignedParameters.isTargetCount)
                        {
                            //RemeshFieldAligned
                            newOccurrenceList[0] = Algo.Native.NativeInterface.Retopologize(occurrenceList, rule.remeshFieldAlignedParameters.targetTriangleCount, rule.remeshFieldAlignedParameters.fullQuad, false, rule.remeshFieldAlignedParameters.featureSize);
                        }
                        else
                        {
                            //RemeshFieldAlignedToRatio
                            int polyCount = Scene.Native.NativeInterface.GetPolygonCount(occurrenceList, true, false, true);
                            int targetPolycount = (int)(rule.remeshFieldAlignedParameters.targetRatio * polyCount);
                            newOccurrenceList[0] = Algo.Native.NativeInterface.Retopologize(occurrenceList, targetPolycount, rule.remeshFieldAlignedParameters.fullQuad, false, rule.remeshFieldAlignedParameters.featureSize);
                        }

                        // Bake animations in new mesh
                        if (rule.remeshFieldAlignedParameters.transferAnimations)
                        {
                            Algo.Native.NativeInterface.BakeVertexAttributes(newOccurrenceList, occurrenceList, true, false, false);
                        }

                        // Bake maps from original meshes to new mesh
                        if (rule.remeshFieldAlignedParameters.bakeMaps)
                        {
                            Algo.Native.BakeMapList mapsToBake = new Algo.Native.BakeMapList(new Algo.Native.BakeMap[] {
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.Diffuse },
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.Normal },
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.Metallic },
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.Roughness },
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.Opacity },
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.AO },
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.Emissive },
                        });

                            OptimizeSDKUtils.BakeMaterials(occurrenceList, newOccurrenceList[0], rule.remeshFieldAlignedParameters.mapsResolution, 1, true, Algo.Native.BakingMethod.RayOnly, mapsToBake, context.pixyzMatrices, Conversions.Identity());
                        }
                    }

                    else if (rule.isRemeshActivated)
                    {
                        // Proxy mesh

                        var bakeOptions = new Process.Native.BakeOptionSelector();
                        bakeOptions._type = rule.remeshParameters.bakeMaps ? Process.Native.BakeOptionSelector.Type.YES : Process.Native.BakeOptionSelector.Type.NO;
                        bakeOptions.Yes = new Process.Native.BakeOptions()
                        {
                            padding = 1,
                            resolution = rule.remeshParameters.mapsResolution,
                            textures = new Algo.Native.BakeMaps()
                            {
                                diffuse = true,
                                ambientOcclusion = true,
                                metallic = true,
                                emissive = true,
                                normal = true,
                                opacity = true,
                                roughness = true
                            }
                        };
                        newOccurrenceList[0] = Process.Native.NativeInterface.ProxyFromMeshes(occurrenceList, rule.remeshParameters.qualityValue, bakeOptions, true, false);
                    }
                    // Reset matrices
                    Scene.Native.NativeInterface.ResetTransform(scene, true, true, false);

                    // Make sure new generated materials are on patches so we can retrieve them
                    Scene.Native.NativeInterface.TransferMaterialsOnPatches(newOccurrenceList[0]);

                    // Retrieve output mesh
                    var outputMesh = Scene.Native.NativeInterface.GetPartMesh(Scene.Native.NativeInterface.GetComponent(newOccurrenceList[0], Scene.Native.ComponentType.Part, true));
                    context.pixyzMeshes = new Polygonal.Native.MeshList(new uint[] { outputMesh });
                    context.pixyzMatrices = new Geom.Native.Matrix4List(new Geom.Native.Matrix4[] { Conversions.Identity() });
                }

                
                else if (rule.isOcclusionActivated)
                {
                    // Remove Hidden

                    if (rule.occlusionParameters.mode == OcclusionMode.Standard)
                    {
                        Algo.Native.NativeInterface.HiddenRemoval(occurrenceList, (Algo.Native.SelectionLevel)rule.occlusionParameters.selectionLevel,
                            rule.occlusionParameters.precision, 16, 90, rule.occlusionParameters.considerTransparentOpaque, rule.occlusionParameters.neighbourPreservation);
                        
                    }
                    else
                    {
                        // convert minimumCavityVolume to m3
                        var minimumCavityVolumeMeters = rule.occlusionParameters.minimumCavityVolume * UnityEngine.Mathf.Pow(10, -9);
                        Algo.Native.NativeInterface.SmartHiddenRemoval(occurrenceList, Algo.Native.SelectionLevel.Polygons, rule.occlusionParameters.voxelSize, minimumCavityVolumeMeters,
                                rule.occlusionParameters.precision, Algo.Native.SmartHiddenType.All, rule.occlusionParameters.considerTransparentOpaque, rule.occlusionParameters.neighbourPreservation);
                    }
                    Scene.Native.NativeInterface.ResetPartTransform(scene);
                }

                if (rule.isDecimateToQualityActivated)
                {
                    Algo.Native.NativeInterface.Decimate(
                        occurrenceList,
                        rule.decimateToQualityParam.surfacicTolerance,
                        rule.decimateToQualityParam.lineicTolerance,
                        rule.decimateToQualityParam.normalTolerance,
                        rule.decimateToQualityParam.uvTolerance,
                        false);
                    Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]); // MANDATORY
                }
                else if (rule.isDecimateToTargetActivated)
                {
                    // HACK: scale 1000 so error is correct on Pixyz side
                    Geom.Native.Point3 p3 = new Geom.Native.Point3() { x = 1000, y = 1000, z = 1000 };
                    Geom.Native.Matrix4 nMat = Geom.Native.NativeInterface.FromTRS(new Geom.Native.Point3(), new Geom.Native.Point3(), p3);
                    Scene.Native.NativeInterface.SetLocalMatrix(scene, nMat);

                    // CreateWeightsFromColors
                    if (rule.decimateToTarget.preservePaintedAreas)
                        Algo.Native.NativeInterface.CreateVertexWeightsFromVertexColors(occurrenceList, 1.0, 100);

                    Algo.Native.DecimateOptionsSelector options = new Algo.Native.DecimateOptionsSelector()
                    {
                        _type = rule.decimateToTarget.isTargetCount ? Algo.Native.DecimateOptionsSelector.Type.TRIANGLECOUNT : Algo.Native.DecimateOptionsSelector.Type.RATIO,
                        triangleCount = rule.decimateToTarget.polycount,
                        ratio = rule.decimateToTarget.ratio
                    };

                    Algo.Native.NativeInterface.DecimateTarget(occurrenceList, options, (Algo.Native.UVImportanceEnum)rule.decimateToTarget.uvImportance, rule.decimateToTarget.protectTopology, 5000000);

                    // HACK: restore default scale
                    p3 = new Geom.Native.Point3();
                    p3.x = 1; p3.y = 1; p3.z = 1;
                    nMat = Geom.Native.NativeInterface.FromTRS(new Geom.Native.Point3(), new Geom.Native.Point3(), p3);
                    Scene.Native.NativeInterface.SetLocalMatrix(scene, nMat);
                    
                    Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
                }

                if (rule.isCombineActivated) 
                {
                    // Merge & Combine

                    if (rule.combineMeshesParameters.mergeType == CombineMeshesParameters.MergeType.MergeAll && rule.isCombineMaterialsActivated)
                    {
                        // Use the Combine + Bake function

                        Algo.Native.BakeOption bakingOptions = new Algo.Native.BakeOption();
                        bakingOptions.resolution = rule.combineMeshesParameters.resolution;
                        bakingOptions.padding = rule.combineMeshesParameters.padding;
                        bakingOptions.bakingMethod = Algo.Native.BakingMethod.RayOnly;

                        bakingOptions.textures = new Algo.Native.BakeMaps();
                        bakingOptions.textures.ambientOcclusion = true;
                        bakingOptions.textures.diffuse = true;
                        bakingOptions.textures.metallic = true;
                        bakingOptions.textures.normal = true;
                        bakingOptions.textures.opacity = true;
                        bakingOptions.textures.roughness = true;
                        bakingOptions.textures.emissive = true;

                        uint occurrence = Algo.Native.NativeInterface.CombineMeshes(occurrenceList, bakingOptions, rule.combineMeshesParameters.forceUVGeneration);
                        uint outputMesh = Scene.Native.NativeInterface.GetPartMesh(Scene.Native.NativeInterface.GetComponent(occurrence, Scene.Native.ComponentType.Part, true));

                        // Make sure new generated materials are on patches so we can retrieve them
                        Scene.Native.NativeInterface.TransferMaterialsOnPatches(occurrence);

                        // Reset matrices
                        Scene.Native.NativeInterface.ResetTransform(occurrence, true, true, false);

                        // Retrive mesh and matrix
                        context.pixyzMeshes = new Polygonal.Native.MeshList(new uint[] { outputMesh });
                        context.pixyzMatrices = new Geom.Native.Matrix4List(new Geom.Native.Matrix4[] { Conversions.Identity() });
                    }
                    else if (rule.combineMeshesParameters.mergeType == CombineMeshesParameters.MergeType.MergeAll)
                    {
                        // Simple merge
                        var newOccurrenceList = Scene.Native.NativeInterface.MergeParts(occurrenceList, Scene.Native.MergeHiddenPartsMode.Destroy);

                        // Reset matrices
                        Scene.Native.NativeInterface.ResetTransform(newOccurrenceList[0], true, true, false);

                        // Retrive meshes and matrices
                        context.pixyzMeshes = Scene.Native.NativeInterface.GetPartsMeshes(new Scene.Native.PartList(Scene.Native.NativeInterface.GetComponentByOccurrence(Scene.Native.NativeInterface.GetPartOccurrences(newOccurrenceList[0]), Scene.Native.ComponentType.Part, true)));
                        
                        context.pixyzMatrices = new Geom.Native.Matrix4List(context.pixyzMeshes.length);
                        for (int i = 0; i < context.pixyzMatrices.length; i++)
                        {
                            context.pixyzMatrices[i] = Conversions.Identity();
                        }
                    } 
                    else if (rule.combineMeshesParameters.mergeType == CombineMeshesParameters.MergeType.MergeByMaterials)
                    {
                        // Merge parts by materials
                        var newOccurrenceList = Scene.Native.NativeInterface.MergePartsByMaterials(occurrenceList, true, Scene.Native.MergeHiddenPartsMode.Destroy, true);

                        // Reset matrices
                        Scene.Native.NativeInterface.ResetTransform(occurrenceList[0], true, true, false);

                        // Retrive meshes and matrices
                        context.pixyzMeshes = Scene.Native.NativeInterface.GetPartsMeshes(new Scene.Native.PartList(Scene.Native.NativeInterface.GetComponentByOccurrence(newOccurrenceList, Scene.Native.ComponentType.Part, true)));

                        context.pixyzMatrices = new Geom.Native.Matrix4List(context.pixyzMeshes.length);
                        for (int i = 0; i < context.pixyzMatrices.length; i++)
                        {
                            context.pixyzMatrices[i] = Conversions.Identity();
                        }

                    }
                }

                return true;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[LODGenerationError] {e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        public void StartPostProcess(LODRule rule, PixyzContext context, GameObject sourceGameObject, Renderer renderer, int LODLevel)
        {
            if (rule.isImpostorActivated)
            {
                //post
                UnityEngine.Texture2D diffuseMap = Conversions.ConvertImageDefinition(Material.Native.NativeInterface.GetImageDefinition(octahedralIMP.DiffuseMap));
                UnityEngine.Texture2D depthMap = Conversions.ConvertImageDefinition(Material.Native.NativeInterface.GetImageDefinition(octahedralIMP.DepthMap));
                UnityEngine.Texture2D normalMap = Conversions.ConvertImageDefinition(Material.Native.NativeInterface.GetImageDefinition(octahedralIMP.NormalMap));
                UnityEngine.Texture2D aoMap = Conversions.ConvertImageDefinition(Material.Native.NativeInterface.GetImageDefinition(octahedralIMP.AOMap));
                UnityEngine.Texture2D rghnssMap = Conversions.ConvertImageDefinition(Material.Native.NativeInterface.GetImageDefinition(octahedralIMP.RoughnessMap));

                if (rule.impostorParameters.saveMaps)
                {
                    OptimizeSDKUtils.SaveTextureAsPNG(diffuseMap, rule.impostorParameters.mapsPath + "/pxz_diffuseMap.png");
                    OptimizeSDKUtils.SaveTextureAsPNG(depthMap, rule.impostorParameters.mapsPath + "/pxz_depthMap.png");
                    OptimizeSDKUtils.SaveTextureAsPNG(normalMap, rule.impostorParameters.mapsPath + "/pxz_normalMap.png");
                    OptimizeSDKUtils.SaveTextureAsPNG(aoMap, rule.impostorParameters.mapsPath + "/pxz_aoMap.png");
                    OptimizeSDKUtils.SaveTextureAsPNG(rghnssMap, rule.impostorParameters.mapsPath + "/pxz_roughnessMap.png");
                }

                //Create Material
                Shader shader = Shader.Find("Pixyz/PxzImpostor");

                UnityEngine.Material material = new UnityEngine.Material(shader);
                {
                    //shader properties
                    material.SetTexture(Shader.PropertyToID("_PxzImposterAlbedoTexture"), diffuseMap);
                    material.SetTexture(Shader.PropertyToID("_PxzImposterNormalTexture"), normalMap);
                    material.SetTexture(Shader.PropertyToID("_PxzImposterDepthTexture"), depthMap);
                    material.SetTexture(Shader.PropertyToID("_PxzImposterRoughnessTexture"), rghnssMap);
                    material.SetTexture(Shader.PropertyToID("_PxzImposterAOTexture"), aoMap);

                    if ((int)rule.impostorParameters.type == 1)
                        material.SetFloat("_FullOcta", 0);
                    if ((int)rule.impostorParameters.renderOn == 1 || (int)rule.impostorParameters.renderOn == 2)
                        material.SetFloat("_PreciseMesh", 1);

                    material.SetInt("_PxzImposterFramesCount", rule.impostorParameters.atlasSize);
                    material.SetFloat("_OctahedronDiameter", (float)(octahedralIMP.Radius * 2.0));
                    material.enableInstancing = true;
                }

                //Create Material
                var impostorMaterial = OptimizeSDKUtils.CreateImpostorMaterial((int)rule.impostorParameters.type, (int)rule.impostorParameters.renderOn, octahedralIMP, rule.impostorParameters.atlasSize, diffuseMap, normalMap, depthMap, aoMap, rghnssMap);
                renderer.sharedMaterial = impostorMaterial;

                // Assign correct mesh to renderer
                switch (rule.impostorParameters.renderOn)
                {
                    case RenderImpostorOn.Quad:
                        // Mesh
                        Vector3[] vertices =
                        {
                            new Vector3(-0.5f,-0.5f),
                            new Vector3(0.5f,-0.5f),
                            new Vector3(0.5f,0.5f),
                            new Vector3(-0.5f,0.5f)
                        };

                        int[] triangles =
                        {
                            0,2,1,
                            0,3,2,
                        };

                        Vector2[] uvs =
                        {
                            new Vector2(0,0),
                            new Vector2(1,0),
                            new Vector2(1,1),
                            new Vector2(0,1)
                        };


                        Mesh mesh = new Mesh();
                        mesh.vertices = vertices;
                        mesh.triangles = triangles;
                        mesh.uv = uvs;

                        renderer.GetComponent<MeshFilter>().sharedMesh = mesh;
                        renderer.GetComponent<MeshRenderer>().sharedMaterial = impostorMaterial;

                        renderer.transform.position = sourceGameObject.GetBoundsWorldSpace(true).center;
                        renderer.transform.rotation = Quaternion.identity;
                        renderer.transform.localScale = Vector3.one;
                        break;

                    case RenderImpostorOn.CustomMesh:
                        renderer.GetComponent<MeshFilter>().sharedMesh = rule.impostorParameters.customMesh.GetComponent<MeshFilter>().sharedMesh;
                        break;

                    case RenderImpostorOn.OrientedBoundingBox: // already generated during process
                    default:
                        break;
                }

                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                    renderer.sharedMaterials[i] = impostorMaterial;
            }
        }

    }
}