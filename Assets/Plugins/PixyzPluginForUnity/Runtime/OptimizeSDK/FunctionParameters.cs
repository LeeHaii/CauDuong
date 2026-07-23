namespace Pixyz.OptimizeSDK.Runtime
{
    public enum QualityLevel
    {
        High = 0,
        Medium,
        Low,
        Poor,
        Custom
    }

    public enum Weight
    {
        Low = 1,
        Normal = 10,
        Important = 100,
        VeryImportant = 1000
    }

    public enum OcclusionMode
    {
        Standard = 0,
        Advanced
    }

    //Occlusion Density
    public enum Density
    {
        Low = 3,
        Medium = 20,
        High = 50
    }
    public enum PrecisonPreset : int
    {
        Custom = 0,
        Low = 256,
        Medium = 1024,
        High = 2048
    }

    public enum SelectionLevel
    {
        GameObject = 0,
        SubMesh = 1,
        Triangles = 2,
    }

    public enum RemeshQualityPreset
    {
        Custom = 0,
        Poor = 10,
        Low = 25,
        Medium = 50,
        High = 100,
        VeryHigh = 200
    }

    public enum MapDimensionPreset
    {
        Custom = 0,
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096,
        _8192 = 8192
    }

    public enum ImpostorType
    {
        Octahedron=0,
        HemiOctahedron=1
    }

    public enum RenderImpostorOn
    {
        Quad=0,
        OrientedBoundingBox=1,
        CustomMesh=2
    }

    public enum UvImportancePreset
    {
        PreserveSeamsAndReduceDeformation = 0,
        PreserveSeams = 1,
        DoNotCare =2
    }

    [System.Serializable]
    public class RepairMeshesParameters
    {
        public double tolerance;
        public bool orientFaces;

        public RepairMeshesParameters(double tolerance = 0.0001, bool orientFaces=false)
        {
            this.tolerance = tolerance;
            this.orientFaces = orientFaces;
        }

        public long ComputeHash()
        {
            return 0x4E1D2BB603113A3 ^ tolerance.GetHashCode() ^ 3;
        }
    }

    [System.Serializable]
    public class CombineMeshesParameters
    {
        public MergeType mergeType;
        public bool forceUVGeneration;
        public int resolution;
        public int padding;
        public enum MergeType
        {
            MergeAll, 
            MergeByMaterials
        }

        public CombineMeshesParameters(bool forceUVGeneration = false,int resolution = 1024, int padding = 1)
        {
            this.mergeType = MergeType.MergeAll;
            this.forceUVGeneration = forceUVGeneration;
            this.resolution = resolution;
            this.padding = padding;
        }

        public long ComputeHash()
        {
            return 0x4E1D2BC1031F347 ^ resolution.GetHashCode() ^ padding.GetHashCode() ^ 43;
        }
    }

    [System.Serializable]
    public class RemeshParameters
    {
        public double featureSize;
        public bool bakeMaps;
        public RemeshQualityPreset qualityPreset;
        public int qualityValue;
        public int mapsResolution;
        public bool isPointCloud;

        public RemeshParameters(double featureSize = 0.01, RemeshQualityPreset qualityPreset = RemeshQualityPreset.Medium, int qualityValue = 50, bool bakeMaps = true, int mapsResolution = 1024, bool isPointCloud = false)
        {
            this.featureSize = featureSize;
            this.bakeMaps = bakeMaps;
            this.qualityValue = qualityValue;
            this.mapsResolution = mapsResolution;
            this.isPointCloud = isPointCloud;
            this.qualityPreset = qualityPreset;
        }

        public long ComputeHash()
        {
            return 0x1B0D2DF1031A1000 ^ featureSize.GetHashCode() ^ qualityValue.GetHashCode() ^ 23 ^ bakeMaps.GetHashCode() ^ mapsResolution.GetHashCode();
        }
    }

    [System.Serializable]
    public class RemeshFieldAlignedParameters
    {
        public bool isTargetCount;

        public bool fullQuad;
        public bool transferAnimations;
        public bool bakeMaps;

        public bool useFeatureSize;
        public double featureSize;
        public int mapsResolution;

        /// <summary>
        /// From 0.0f(0%) to 1.0f(100%)
        /// </summary>
        public float targetRatio;
        public int targetTriangleCount;

        public RemeshFieldAlignedParameters(int targetTriangleCount, bool useFeatureSize, double featureSize = 0.1, bool transferAnimations = true, bool fullQuad = true, bool bakeMaps = true, int mapsResolution = 1024)
        {
            isTargetCount = true;
            this.targetTriangleCount = targetTriangleCount;
            this.useFeatureSize = useFeatureSize;
            this.featureSize = featureSize;
            this.fullQuad = fullQuad;
            this.transferAnimations = transferAnimations;
            this.bakeMaps = bakeMaps;
            this.mapsResolution = mapsResolution;
        }

        public RemeshFieldAlignedParameters(float targetRatio = 0.25f,bool useFeatureSize=false, double featureSize = 0.1, bool transferAnimations = true, bool fullQuad = true, bool bakeMaps = true, int mapsResolution = 1024)
        {
            isTargetCount = false;
            this.targetRatio = targetRatio;
            this.useFeatureSize = useFeatureSize;
            this.featureSize = featureSize;

            this.fullQuad = fullQuad;
            this.transferAnimations = transferAnimations;
            this.bakeMaps = bakeMaps;
            this.mapsResolution = mapsResolution;
        }

        public long ComputeHash()
        {
            return 0x2E0D2AF5031C7020 ^ targetTriangleCount.GetHashCode() ^ featureSize.GetHashCode() ^ 7 ^ fullQuad.GetHashCode() ^ transferAnimations.GetHashCode() ^ bakeMaps.GetHashCode() ^ mapsResolution.GetHashCode();
        }
    }

    [System.Serializable]
    public class DecimateToQualityVertexRemovalParameters
    {
        public QualityLevel quality;
        public double surfacicTolerance;
        public double lineicTolerance;
        public double normalTolerance;
        public double uvTolerance;

        public DecimateToQualityVertexRemovalParameters(double surfacicTolerance = 0.001, double lineicTolerance = -1, double normalTolerance = 8, double texCoordTolerance = -1, QualityLevel quality = QualityLevel.Medium)
        {
            this.surfacicTolerance = surfacicTolerance;
            this.lineicTolerance = lineicTolerance;
            this.normalTolerance = normalTolerance;
            this.uvTolerance = texCoordTolerance;
            this.quality = quality;
        }

        public static DecimateToQualityVertexRemovalParameters Preset(QualityLevel preset)
        {
            switch (preset)
            {
                
                case QualityLevel.High:
                    return High();
                case QualityLevel.Medium:
                    return Medium();
                case QualityLevel.Low:
                    return Low();
                case QualityLevel.Poor:
                    return Poor();
                default:
                    return new DecimateToQualityVertexRemovalParameters();
            } 
        }

        public static DecimateToQualityVertexRemovalParameters High()
        {
            return new DecimateToQualityVertexRemovalParameters(0.0005, 0.0001, 1, -1, QualityLevel.High);
        }

        public static DecimateToQualityVertexRemovalParameters Medium()
        {
            return new DecimateToQualityVertexRemovalParameters(0.001, -1, 8, -1, QualityLevel.Medium);
        }

        public static DecimateToQualityVertexRemovalParameters Low()
        {
            return new DecimateToQualityVertexRemovalParameters(0.003, -1, 15, -1, QualityLevel.Low);
        }

        public static DecimateToQualityVertexRemovalParameters Poor()
        {
            return new DecimateToQualityVertexRemovalParameters(0.01, -1, 20, -1, QualityLevel.Poor);
        }

        public long ComputeHash()
        {
            return 0x7E027AF1051D3023 ^ quality.GetHashCode() ^ surfacicTolerance.GetHashCode() ^ lineicTolerance.GetHashCode() ^ 19 ^ normalTolerance.GetHashCode() ^ uvTolerance.GetHashCode();
        }
    }

    [System.Serializable]
    public class DecimateToQualityParameters
    {
        public QualityLevel quality;

        public double normalTolerance;
        public double uvTolerance;
        public double surfacicTolerance;
        public double lineicTolerance;

        public DecimateToQualityParameters(double normalTolerance = -1, double uvTolerance = -1, double surfacicTolerance = 0.001,double lineicTolerance = -1, QualityLevel quality = QualityLevel.Medium)
        {
            this.normalTolerance = normalTolerance;
            this.uvTolerance = uvTolerance;
            this.surfacicTolerance = surfacicTolerance;
            this.lineicTolerance = lineicTolerance;
            this.quality = quality;
        }

        public static DecimateToQualityParameters Preset(QualityLevel preset)
        {
            switch (preset)
            {
                case QualityLevel.High:
                    return High();
                case QualityLevel.Medium:
                    return Medium();
                case QualityLevel.Low:
                    return Low();
                case QualityLevel.Poor:
                    return Poor();
                default:
                    return new DecimateToQualityParameters();
            }
        }

        public static DecimateToQualityParameters High()
        {
            return new DecimateToQualityParameters(lineicTolerance: 0.0001, normalTolerance: 1, uvTolerance: -1, surfacicTolerance: 0.0005, quality: QualityLevel.High);
        }

        public static DecimateToQualityParameters Medium()
        {
            return new DecimateToQualityParameters(lineicTolerance: -1, normalTolerance: 8, uvTolerance: -1,surfacicTolerance: 0.001, quality: QualityLevel.Medium);
        }

        public static DecimateToQualityParameters Low()
        {
            return new DecimateToQualityParameters(lineicTolerance: -1, normalTolerance: 15, uvTolerance: -1, surfacicTolerance: 0.003, quality: QualityLevel.Low);
        }

        public static DecimateToQualityParameters Poor()
        {
            return new DecimateToQualityParameters(lineicTolerance: -1, normalTolerance: 20, uvTolerance: -1,surfacicTolerance: 0.01, quality: QualityLevel.Poor);
        }

        public long ComputeHash()
        {
            return 0x2B627AF1051A3C23 ^ normalTolerance.GetHashCode() ^ 5 ^ uvTolerance.GetHashCode() ^ surfacicTolerance.GetHashCode() ^ lineicTolerance.GetHashCode();
        }
    }

    [System.Serializable]
    public class DecimateToTargetParameters
    {
        public bool isTargetCount;
        /// <summary>
        /// From 0.0f(0%) to 1.0f(100%)
        /// </summary>
        public double ratio;
        public int polycount;
        public bool preservePaintedAreas;
        public UvImportancePreset uvImportance;
        public bool protectTopology;

        public DecimateToTargetParameters(double ratio = 50,int polycount = 5000, bool preservePaintedAreas = false, UvImportancePreset uvImportance = UvImportancePreset.PreserveSeamsAndReduceDeformation, bool protectTopology = false)
        {
            isTargetCount = false;
            this.ratio = ratio;
            this.polycount = polycount;

            this.preservePaintedAreas = preservePaintedAreas;
            this.uvImportance = uvImportance;
            this.protectTopology = protectTopology;

        }

        public long ComputeHash()
        {
            return 0x2B354AB1051A3E ^ ratio.GetHashCode() ^ 5 ^ polycount.GetHashCode() ^ isTargetCount.GetHashCode() ^ 51 ^ preservePaintedAreas.GetHashCode() ^ 41 ^ uvImportance.GetHashCode() ^ protectTopology.GetHashCode();
        }
    }

    [System.Serializable]
    public class OcclusionCullingParameters
    {
        public OcclusionMode mode = OcclusionMode.Standard;
        public int precision = (int)PrecisonPreset.Medium;
        public int density = (int)Density.Medium;
        public SelectionLevel selectionLevel = SelectionLevel.GameObject;
        public bool considerTransparentOpaque;
        public int neighbourPreservation;
        public bool preserveCavities;
        public double minimumCavityVolume;
        public double voxelSize;

        public OcclusionCullingParameters(bool considerTransparentOpaque = true, double minimumCavityVolume = -1,double voxelSize=0.1, OcclusionMode mode = OcclusionMode.Standard, int neighbourPreservation = 1, int precision = 1024,int density = 20, SelectionLevel selectionLevel = SelectionLevel.GameObject, bool preserveCavities=true)
        {
            this.considerTransparentOpaque = considerTransparentOpaque;
            this.minimumCavityVolume = minimumCavityVolume;
            this.mode = mode;
            this.neighbourPreservation = neighbourPreservation;
            this.precision = precision;
            this.density = density;
            this.selectionLevel = selectionLevel;
            this.preserveCavities = preserveCavities;
            this.voxelSize = voxelSize;
        }

        public long ComputeHash()
        {
            return 0x2A0D435F19EFC4CF ^ considerTransparentOpaque.GetHashCode() ^ 1223 ^ mode.GetHashCode() ^ precision.GetHashCode() ^ minimumCavityVolume.GetHashCode() ^ voxelSize.GetHashCode() ^ neighbourPreservation.GetHashCode();
        }
    }

    [System.Serializable]
    public class BilloardParamters
    {
        public bool XPositiveEnable;
        public bool XNegativeEnable;
        public bool YPositiveEnable;
        public bool YNegativeEnable;
        public bool ZPositiveEnable;
        public bool ZNegativeEnable;

        public int resolution;


        public BilloardParamters(int resolution = 1024, bool XPositiveEnable = true, bool XNegativeEnable = true, bool YPositiveEnable = true, bool YNegativeEnable = true, bool ZPositiveEnable = true, bool ZNegativeEnable = true)
        {
            this.resolution = resolution;
            this.XPositiveEnable = XPositiveEnable;
            this.XNegativeEnable = XNegativeEnable;
            this.YPositiveEnable = YPositiveEnable;
            this.YNegativeEnable = YNegativeEnable;
            this.ZPositiveEnable = ZPositiveEnable;
            this.ZNegativeEnable = ZNegativeEnable;
        }

        public long ComputeHash()
        {
            return 0x336AB6FDD003EBC2 ^ resolution.GetHashCode() ^ 1543 ^ XPositiveEnable.GetHashCode() ^ XNegativeEnable.GetHashCode() ^ YPositiveEnable.GetHashCode() ^ 521 ^ YNegativeEnable.GetHashCode() ^ ZPositiveEnable.GetHashCode() ^ ZNegativeEnable.GetHashCode();
        }
    }

    [System.Serializable]
    public class ImpostorParameters
    {
        public int resolution;
        public int atlasSize;
        public ImpostorType type;
        public RenderImpostorOn renderOn;
        public bool saveMaps;
        public string mapsPath;
        public UnityEngine.GameObject customMesh;

        public ImpostorParameters(int resolution = 2048, int atlasSize = 32, ImpostorType type = ImpostorType.Octahedron, RenderImpostorOn renderOn = RenderImpostorOn.Quad, string mapsPath = "", bool saveMaps=false, UnityEngine.GameObject customMesh = null)
        {
            this.resolution = resolution;
            this.atlasSize = atlasSize;
            this.type = type;
            this.renderOn = renderOn;
            this.saveMaps = saveMaps;
            this.customMesh = customMesh;
            this.mapsPath = mapsPath;
        }

        public long ComputeHash()
        {
            return 0x336AB6FDD003EBC2 ^ resolution.GetHashCode() ^ 1543 ^ atlasSize.GetHashCode() ^ type.GetHashCode() ^ renderOn.GetHashCode() ^ 521 ^ saveMaps.GetHashCode();
        }
    }

     
}
