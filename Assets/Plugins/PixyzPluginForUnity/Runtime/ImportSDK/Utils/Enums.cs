namespace Pixyz.ImportSDK {
    public enum TreeProcessType
    {
        FULL,
        CLEANUP_INTERMEDIARY_NODES,
        TRANSFER_ALL_UNDER_ROOT,
        MERGE_ALL,
        MERGE_BY_MATERIAL,
        MERGE_FINAL_LEVEL,
        MERGE_BY_HIERARCHY_LEVEL,
        MERGE_BY_NAME,
        MERGE_BY_REGIONS
    }

    public enum MergeType
    {
        MERGE_GAMEOBJECTS,
        MERGE_BY_MATERIAL
    }

    public enum MergeByRegions
    {
        NUMBER_OF_REGIONS,
        SIZE_OF_REGIONS
    }


    public enum MeshQuality
    {
        MAXIMUM,
        HIGH,
        MEDIUM,
        LOW,
        POOR,
        CUSTOM
    }

    public enum Orientation
    {
        AUTOMATIC,
        MANUAL
    }
}
