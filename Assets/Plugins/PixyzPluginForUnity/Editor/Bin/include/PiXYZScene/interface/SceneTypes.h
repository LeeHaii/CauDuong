// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_SCENE_INTERFACE_SCENETYPES_H_
#define _PXZ_SCENE_INTERFACE_SCENETYPES_H_

#include <PiXYZCore/interface/InterfaceBase.h>
#include <PiXYZCAD/interface/CADTypes.h>
#include <PiXYZCore/interface/CoreTypes.h>
#include <PiXYZGeom/interface/GeomTypes.h>
#include <PiXYZMaterial/interface/MaterialTypes.h>
#include <PiXYZPolygonal/interface/PolygonalTypes.h>
#include <functional> 

PXZ_MODULE_BEGIN(SceneI)

typedef CoreI::Entity AlternativeTree;

typedef CoreI::List<AlternativeTree> AlternativeTreeList;

typedef CoreI::Entity AnimChannel;

typedef CoreI::List<AnimChannel> AnimChannelList;

typedef CoreI::Entity Animation;

typedef CoreI::List<Animation> AnimationList;

typedef CoreI::Long AnimationTime;

typedef CoreI::Entity Annotation;

typedef CoreI::Entity AnnotationGroup;

typedef CoreI::List<AnnotationGroup> AnnotationGroupList;

typedef CoreI::List<Annotation> AnnotationList;

typedef GeomI::GeomEntity Shape;

typedef Shape BRepShape;

struct Camera {
   GeomI::Point3 position;
   GeomI::Vector3 direction;
   GeomI::Vector3 up;
   CoreI::Double fov;
   Camera(GeomI::Point3 _position = GeomI::Point3(), GeomI::Vector3 _direction = GeomI::Vector3(), GeomI::Vector3 _up = GeomI::Vector3(), CoreI::Double _fov = 60) : position(_position), direction(_direction), up(_up), fov(_fov) {}
};

typedef CoreI::Entity Component;

typedef CoreI::List<Component> ComponentList;

class ComponentType {
public:
   typedef enum {
   Part = 0,
   PMI = 1,
   Light = 2,
   VisualBehavior = 3,
   InteractionBehavior = 4,
   Metadata = 5,
   Variant = 6,
   Animation = 7,
   Joint = 8,
   Widget = 9,
   OoCComponent = 10,
   LODComponent = 11,
   ExternalDataComponent = 12
   } Type;
private:
   Type _value;
public:
   ComponentType(Type val = Part) : _value(val) {}
   ComponentType & operator=(Type val) { _value = val; return *this; }
   ComponentType(const CoreI::Int& val) : _value((Type)val) {}
   ComponentType & operator=(const CoreI::Int& val) { _value = (Type)val; return *this; }
   Type value() const { return _value; }
   operator Type() const { return _value; }
};
typedef CoreI::Entity DirectionalLight;

typedef Component ExternalDataComponent;

typedef CoreI::String FilterExpression;

struct Filter {
   CoreI::Ident id;
   CoreI::String name;
   FilterExpression expr;
   Filter(CoreI::Ident _id = 0, CoreI::String _name = "", FilterExpression _expr = "") : id(_id), name(_name), expr(_expr) {}
};

typedef CoreI::List<Filter> FilterList;

typedef Component JointComponent;

typedef CoreI::Entity Keyframe;

typedef CoreI::List<Keyframe> KeyframeList;

typedef CoreI::Entity LOD;

typedef Component LODComponent;

typedef CoreI::List<LODComponent> LODComponentList;

typedef CoreI::List<LOD> LODList;

typedef CoreI::Entity Light;

typedef Component LightComponent;

struct MergeByRegionsStrategy {
   typedef enum {
      UNKNOWN=0,
      NUMBEROFREGIONS,
      SIZEOFREGIONS
   } Type;
   Type _type; 
   CoreI::Int NumberOfRegions;
   GeomI::Distance SizeOfRegions;
   MergeByRegionsStrategy() : _type(UNKNOWN) {}
   MergeByRegionsStrategy(const CoreI::Int & v) : _type(NUMBEROFREGIONS), NumberOfRegions(v) {}
   MergeByRegionsStrategy(const GeomI::Distance & v) : _type(SIZEOFREGIONS), SizeOfRegions(v) {}
};
class MergeHiddenPartsMode {
public:
   typedef enum {
   Destroy = 0,
   MakeVisible = 1,
   MergeSeparately = 2
   } Type;
private:
   Type _value;
public:
   MergeHiddenPartsMode(Type val = Destroy) : _value(val) {}
   MergeHiddenPartsMode & operator=(Type val) { _value = val; return *this; }
   MergeHiddenPartsMode(const CoreI::Int& val) : _value((Type)val) {}
   MergeHiddenPartsMode & operator=(const CoreI::Int& val) { _value = (Type)val; return *this; }
   Type value() const { return _value; }
   operator Type() const { return _value; }
};
class MergeStrategy {
public:
   typedef enum {
   MergeParts = 0,
   MergeByMaterials = 1
   } Type;
private:
   Type _value;
public:
   MergeStrategy(Type val = MergeParts) : _value(val) {}
   MergeStrategy & operator=(Type val) { _value = val; return *this; }
   MergeStrategy(const CoreI::Int& val) : _value((Type)val) {}
   MergeStrategy & operator=(const CoreI::Int& val) { _value = (Type)val; return *this; }
   Type value() const { return _value; }
   operator Type() const { return _value; }
};
typedef Component Metadata;

struct PropertyValue {
   //! name of the property
   CoreI::String name;
   //! value of the property
   CoreI::String value;
   PropertyValue(CoreI::String _name = "", CoreI::String _value = "") : name(_name), value(_value) {}
};

typedef CoreI::List<PropertyValue> PropertyValueList;

typedef PropertyValueList MetadataDefinition;

typedef CoreI::List<MetadataDefinition> MetadataDefinitionList;

typedef CoreI::List<Metadata> MetadataList;

typedef GeomI::GeomEntity Occurrence;

typedef CoreI::List<Occurrence> OccurrenceList;

typedef CoreI::List<OccurrenceList> OccurrenceListList;

typedef Component OoCComponent;

struct PackedTree {
   //! list of all occurrences of the tree. (Note: all parent occurrences appears before their children, thus the first occurrence is the root)
   OccurrenceList occurrences;
   //! specifies the parent for each occurrence (same index than occurrences). root's parent is -1
   CoreI::IntList parents;
   //! name of each occurrence (same index than occurrences)
   CoreI::StringList names;
   //! visibility of each occurrence (same index than occurrences)
   CoreI::InheritableBoolList visibles;
   //! material identifier of each occurrence (same index than occurrences)
   MaterialI::MaterialList materials;
   //! indices of transform matrix of each occurrence (same index than occurrences). (Note: Identity matrix is always 0)
   CoreI::IntList transformIndices;
   //! Matrices referenced by transformIndices. (Note: The first matrix is always Identity)
   GeomI::Matrix4List transformMatrices;
   //! custom properties of each occurrence (same index than occurrences), pair is name -> value
   CoreI::StringPairListList customProperties;
   PackedTree(OccurrenceList _occurrences = OccurrenceList(), CoreI::IntList _parents = CoreI::IntList(), CoreI::StringList _names = CoreI::StringList(), CoreI::InheritableBoolList _visibles = CoreI::InheritableBoolList(), MaterialI::MaterialList _materials = MaterialI::MaterialList(), CoreI::IntList _transformIndices = CoreI::IntList(), GeomI::Matrix4List _transformMatrices = GeomI::Matrix4List(), CoreI::StringPairListList _customProperties = CoreI::StringPairListList()) : occurrences(_occurrences), parents(_parents), names(_names), visibles(_visibles), materials(_materials), transformIndices(_transformIndices), transformMatrices(_transformMatrices), customProperties(_customProperties) {}
};

typedef Component Part;

typedef CoreI::List<Part> PartList;

typedef CoreI::Entity PositionalLight;

class Primitive_Type {
public:
   typedef enum {
   CUBE = 0,
   SPHERE = 1,
   PLAN = 2,
   CONE = 3,
   ARROW = 4,
   CYLINDER = 5
   } Type;
private:
   Type _value;
public:
   Primitive_Type(Type val = CUBE) : _value(val) {}
   Primitive_Type & operator=(Type val) { _value = val; return *this; }
   Primitive_Type(const CoreI::Int& val) : _value((Type)val) {}
   Primitive_Type & operator=(const CoreI::Int& val) { _value = (Type)val; return *this; }
   Type value() const { return _value; }
   operator Type() const { return _value; }
};
struct ProberInfo {
   Occurrence occurrence;
   GeomI::Point3 position;
   ProberInfo(Occurrence _occurrence = 0, GeomI::Point3 _position = GeomI::Point3()) : occurrence(_occurrence), position(_position) {}
};

struct RayHit {
   CoreI::Double rayParam;
   Occurrence occurrence;
   CoreI::Int triangleIndex;
   GeomI::Point2 triangleParam;
   RayHit(CoreI::Double _rayParam = -1, Occurrence _occurrence = 0, CoreI::Int _triangleIndex = -1, GeomI::Point2 _triangleParam = GeomI::Point2()) : rayParam(_rayParam), occurrence(_occurrence), triangleIndex(_triangleIndex), triangleParam(_triangleParam) {}
};

typedef CoreI::List<RayHit> RayHitList;

struct ResizeByMaximumSizeOptions {
   CoreI::Int TextureSize;
   CoreI::Boolean KeepTextureRatio;
   ResizeByMaximumSizeOptions(CoreI::Int _TextureSize = 4096, CoreI::Boolean _KeepTextureRatio = true) : TextureSize(_TextureSize), KeepTextureRatio(_KeepTextureRatio) {}
};

struct ResizeByTexturesOptions {
   typedef enum {
      UNKNOWN=0,
      ALLTEXTURES,
      SELECTION
   } Type;
   Type _type; 
   CoreI::None AllTextures;
   MaterialI::ImageList Selection;
   ResizeByTexturesOptions() : _type(UNKNOWN) {}
   ResizeByTexturesOptions(const CoreI::None & v) : _type(ALLTEXTURES), AllTextures(v) {}
   ResizeByTexturesOptions(const MaterialI::ImageList & v) : _type(SELECTION), Selection(v) {}
};
struct ResizeTexturesInputMode {
   typedef enum {
      UNKNOWN=0,
      OCCURRENCES,
      TEXTURES
   } Type;
   Type _type; 
   OccurrenceList Occurrences;
   ResizeByTexturesOptions Textures;
   ResizeTexturesInputMode() : _type(UNKNOWN) {}
   ResizeTexturesInputMode(const OccurrenceList & v) : _type(OCCURRENCES), Occurrences(v) {}
   ResizeTexturesInputMode(const ResizeByTexturesOptions & v) : _type(TEXTURES), Textures(v) {}
};
struct ResizeTexturesResizeMode {
   typedef enum {
      UNKNOWN=0,
      RATIO,
      MAXIMUMSIZE
   } Type;
   Type _type; 
   CoreI::Percent Ratio;
   ResizeByMaximumSizeOptions MaximumSize;
   ResizeTexturesResizeMode() : _type(UNKNOWN) {}
   ResizeTexturesResizeMode(const CoreI::Percent & v) : _type(RATIO), Ratio(v) {}
   ResizeTexturesResizeMode(const ResizeByMaximumSizeOptions & v) : _type(MAXIMUMSIZE), MaximumSize(v) {}
};
typedef CoreI::Entity SpotLight;

typedef Shape TessellatedShape;

typedef CoreI::Entity Variant;

typedef Component VariantComponent;

typedef CoreI::List<VariantComponent> VariantComponentList;

struct VariantDefinition {
   Variant variant;
   PropertyValueList overridedProperties;
   VariantDefinition(Variant _variant = 0, PropertyValueList _overridedProperties = PropertyValueList()) : variant(_variant), overridedProperties(_overridedProperties) {}
};

typedef CoreI::List<VariantDefinition> VariantDefinitionList;

typedef CoreI::List<VariantDefinitionList> VariantDefinitionListList;

typedef CoreI::List<Variant> VariantList;

class VisibilityMode {
public:
   typedef enum {
   Inherited = 0,
   Hide = 1
   } Type;
private:
   Type _value;
public:
   VisibilityMode(Type val = Inherited) : _value(val) {}
   VisibilityMode & operator=(Type val) { _value = val; return *this; }
   VisibilityMode(const CoreI::Int& val) : _value((Type)val) {}
   VisibilityMode & operator=(const CoreI::Int& val) { _value = (Type)val; return *this; }
   Type value() const { return _value; }
   operator Type() const { return _value; }
};
typedef struct {
   CoreI::Int partCount;
   CoreI::Int partOccurrenceCount;
   CoreI::Int triangleCount;
   CoreI::Int triangleOccurrenceCount;
   CoreI::Int vertexCount;
   CoreI::Int vertexOccurrenceCount;
} getSubTreeStatsReturn;

typedef struct {
   GeomI::Point3List positions;
   GeomI::Point3List directions;
} getViewpointsFromCavitiesReturn;

typedef struct {
   CoreI::String implementationType;
   CoreI::String implementationParameters;
} getOoCConfigurationReturn;

typedef struct {
   CoreI::Int partCount;
   CoreI::Int totalPartCount;
   CoreI::Int vertexCount;
   CoreI::Int totalVertexCount;
   CoreI::Int edgeCount;
   CoreI::Int totalEdgeCount;
   CoreI::Int domainCount;
   CoreI::Int totalDomainCount;
   CoreI::Int bodyCount;
   CoreI::Int totalBodyCount;
   CoreI::Double area2Dsum;
   CoreI::Int boundaryCount;
   CoreI::Int boundaryEdgeCount;
} getBRepInfosReturn;

typedef struct {
   CoreI::Int partCount;
   CoreI::Int totalPartCount;
   CoreI::Int vertexCount;
   CoreI::Int totalVertexCount;
   CoreI::Int edgeCount;
   CoreI::Int totalEdgeCount;
   CoreI::Int polygonCount;
   CoreI::Int totalPolygonCount;
   CoreI::Int patchCount;
   CoreI::Int totalPatchCount;
   CoreI::Int boundaryCount;
   CoreI::Int boundaryEdgeCount;
} getTessellationInfosReturn;

typedef struct {
   OccurrenceList occurrences;
   CoreI::StringList evaluations;
} evaluateExpressionOnSubTreeReturn;

typedef struct {
   CoreI::IntList indices;
   GeomI::Matrix4List transforms;
} getPartsTransformsIndexedReturn;


PXZ_MODULE_END

#endif
