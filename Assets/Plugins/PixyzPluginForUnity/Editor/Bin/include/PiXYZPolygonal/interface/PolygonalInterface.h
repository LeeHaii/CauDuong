// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_POLYGONAL_INTERFACE_POLYGONALINTERFACE_H_
#define _PXZ_POLYGONAL_INTERFACE_POLYGONALINTERFACE_H_

#include "PolygonalTypes.h"

PXZ_MODULE_BEGIN(PolygonalI)

class PXZ_EXPORTED PolygonalInterface
{
public:
   /**
    * \defgroup checksum functions to compute mesh checksum to identify identical representations
    * @{
    */
   //! Compute a checksum of the mesh topology, connectivity
   /*!
     \param mesh The mesh
     \return checksum The mesh
   */
   static CoreI::String computeMeshTopoChecksum(const Mesh & mesh);

   //! Compute a checksum of the mesh vertices positions
   /*!
     \param mesh The mesh
     \param precisionFloat Floating point precision [1..24], number of significant numbers kept. -1 means no rounded will be applied
     \return checksum The mesh
   */
   static CoreI::String computeMeshVertexPositionsChecksum(const Mesh & mesh, const CoreI::Int & precisionFloat = -1);

   //! Compute a checksum of the uvs topology, connectivity
   /*!
     \param mesh The mesh
     \param uvChannel The uv channel
     \return checksum The mesh
   */
   static CoreI::String computeUVTopoChecksum(const Mesh & mesh, const CoreI::Int & uvChannel);

   //! Compute a checksum of the vertices positions in uv space
   /*!
     \param mesh The mesh
     \param uvChannel The uv channel
     \param precisionFloat Floating point precision [1..24], number of significant numbers kept. -1 means no rounded will be applied
     \return checksum The mesh
   */
   static CoreI::String computeUVVertexPositionsChecksum(const Mesh & mesh, const CoreI::Int & uvChannel, const CoreI::Int & precisionFloat = -1);

   /**@}*/

   /**
    * \defgroup draco Draco compression related functions
    * @{
    */
   //! decode a mesh using draco
   /*!
     \param buffer 
     \param jointIndicesId Unique ID of Generic attribute encoding joint indices
     \param jointWeightsId Unique ID of Generic attribute encoding joint weights
     \return mesh 
   */
   static StaticMesh dracoDecode(const CoreI::ByteList & buffer, const CoreI::Int & jointIndicesId = -1, const CoreI::Int & jointWeightsId = -1);

   //! encode a mesh using draco
   /*!
     \param mesh 
     \param compressionLevel 0=faster but the worst compression, 10=slower but the best compression
     \param quantizationPosition Number of quantization bits used for position attributes
     \param quantizationNormal Number of quantization bits used for normal attributes
     \param quantizationTexCoord Number of quantization bits used for texture coordinates attributes
     \return buffer 
     \return jointIndicesId Unique ID of Generic attribute encoding joint indices (-1 if not applicable or if enocdeSkeleton is false)
     \return jointWeightsId Unique ID of Generic attribute encoding joint weights (-1 if not applicable or if enocdeSkeleton is false)
   */
   static dracoEncodeReturn dracoEncode(const StaticMesh & mesh, const CoreI::Int & compressionLevel = 7, const CoreI::Int & quantizationPosition = -1, const CoreI::Int & quantizationNormal = -1, const CoreI::Int & quantizationTexCoord = -1);

   /**@}*/

   /**
    * \defgroup element attributes access functions to access element attributes
    * @{
    */
   //! Return the normal attribute of a polygon at a specified vertex
   /*!
     \param Polygon The polygon
     \param Vertex The vertex
     \return normal The normal
   */
   static GeomI::Vector3 getNormal(const Polygon & Polygon, const Vertex & Vertex);

   //! Return the texture coordinates attribute of all the polygons from the tessellation
   /*!
     \param tessellation The tessellation of the wanted polygons
     \return textureCoordinates The textures coordinates
   */
   static UVCoordList getTextureCoordinates(const Tessellation & tessellation);

   //! return the visible polygons from the Visibility attributes (see algo.createVisibilityAttributes)
   /*!
     \param tessellation The tessellation of the wanted polygons
     \return polygons The polygons seen at least once
     \return pixelCounts The number of pixels seen during the whole visibility session
   */
   static getVisiblePolygonsReturn getVisiblePolygons(const Tessellation & tessellation);

   /**@}*/

   /**
    * \defgroup geometry access functions to access geometry structure of tessellations
    * @{
    */
   //! Create fake joint to store in mesh definitions. Thus we can retrieve stored data from getJointPlaceholders
   /*!
     \param data Create as much joints as there are data, each joint store one data
     \param worldMatrices World matrix for each joints
     \return joints Returns one placeholder joint per given data
   */
   static PlaceholderJointList createJointPlaceholders(const CoreI::ULongList & data, const GeomI::Matrix4List & worldMatrices);

   //! Create a new mesh with the given MeshDefinition
   /*!
     \param meshDefinition Mesh definition
     \return mesh The new mesh
   */
   static Mesh createMeshFromDefinition(const MeshDefinition & meshDefinition);

   //! Create new meshes with the given MeshDefinitions
   /*!
     \param meshDefinitions The MeshDefinitions
     \return meshes The new Meshes
   */
   static MeshList createMeshesFromDefinitions(const MeshDefinitionList & meshDefinitions);

   //! Returns the polygons connected to an edge
   /*!
     \param edge The edge
     \return polygons The edge polygons
   */
   static PolygonList getEdgePolygons(const Edge & edge);

   //! Returns the vertices of an edge
   /*!
     \param edge The edge
     \return vertices The edge vertices
   */
   static VertexList getEdgeVertices(const Edge & edge);

   //! Returns the edges corresponding to the given connectivity
   /*!
     \param tessellation The tessellation
     \param category Category mask of the wanted edges
     \return edges The edges
   */
   static EdgeList getEdges(const Tessellation & tessellation, const TopologyCategoryMask & category);

   //! Returns the free edges of a tessellation
   /*!
     \param tessellation The tessellation
     \return freeEdges All free edges of the tessellation
   */
   static EdgeList getFreeEdges(const Tessellation & tessellation);

   //! Returns the free vertices of a tessellation
   /*!
     \param tessellation The tessellation
     \return freeVertices All free vertices of the tessellation
   */
   static VertexList getFreeVertices(const Tessellation & tessellation);

   //! Get data stored in joint placeholders
   /*!
     \param joints Placeholder joints to get data from
     \return data Data stored in each placehold joint (for invalid joint, returned data is undefined)
   */
   static CoreI::ULongList getJointPlaceholders(const PlaceholderJointList & joints);

   //! Returns the definition
   /*!
     \param mesh The mesh to get definition
     \return meshDefinition Mesh definition
   */
   static MeshDefinition getMeshDefinition(const Mesh & mesh);

   //! Returns the definition
   /*!
     \param meshes The meshes to get definitions
     \return meshDefinitions The MeshDefinitions
   */
   static MeshDefinitionList getMeshDefinitions(const MeshList & meshes);

   //! Returns the joints/IBMs list of a given mesh (those referenced by jointIndices)
   /*!
     \param mesh 
     \return joints 
     \return IBMs Inverse Bind Matrices
   */
   static getMeshSkinningReturn getMeshSkinning(const StaticMesh & mesh);

   //! Returns the patches of a tessellation
   /*!
     \param tessellation The tessellation
     \return patches All patches of the tessellation
   */
   static PatchList getPatches(const Tessellation & tessellation);

   //! Returns the edges of a a polygon
   /*!
     \param polygon The polygon
     \return edges The polygon edges
   */
   static EdgeList getPolygonEdges(const Polygon & polygon);

   //! Returns the vertices of a a polygon
   /*!
     \param polygon The polygon
     \return vertices The polygon vertices
   */
   static VertexList getPolygonVertices(const Polygon & polygon);

   //! Returns the polygons of a tessellation
   /*!
     \param tessellation The tessellation
     \return polygons All polygons of the tessellation
   */
   static PolygonList getPolygons(const Tessellation & tessellation);

   //! Get boundary edges of a tessellation grouped by cycles
   /*!
     \param tessellation The Tessellation
     \return boundaries List of boundary edges grouped by cycles in the given tessellation
   */
   static EdgeListList getTessellationBoundaries(const Tessellation & tessellation);

   //! Returns the vertex coordinates in the tessellation local space
   /*!
     \param vertex The vertex
     \return coordinates Vertex coordinates
   */
   static GeomI::Point3 getVertexCoordinates(const Vertex & vertex);

   //! Returns the edges connected to a vertex
   /*!
     \param vertex The vertex
     \return edges The vertex edges
   */
   static EdgeList getVertexEdges(const Vertex & vertex);

   //! Returns the polygons connected to a vertex
   /*!
     \param vertex The vertex
     \return polygons The vertex polygons
   */
   static PolygonList getVertexPolygons(const Vertex & vertex);

   //! Returns the vertices of a tessellation
   /*!
     \param tessellation The tessellation
     \param category Category mask of the wanted edges
     \return vertices All vertices of the tessellation
   */
   static VertexList getVertices(const Tessellation & tessellation, const TopologyCategoryMask & category);

   //! Set/Replace the list of joints/IBMs of a given mesh (those referenced by jointIndices)
   /*!
     \param mesh 
     \param joints 
     \param IBMs Inverse Bind Matrices
   */
   static void setMeshSkinning(const StaticMesh & mesh, const JointList & joints, const GeomI::Matrix4List & IBMs);

   /**@}*/

   /**
    * \defgroup modification function to modify the tessellation
    * @{
    */
   //! Destroy all the given elements
   /*!
     \param elements List of elements to destroy
   */
   static void destroyElements(const ElementList & elements);

   //! Invert the orientation of all the given elements
   /*!
     \param elements List of elements to invert
   */
   static void invertElements(const ElementList & elements);

   /**@}*/

};

PXZ_MODULE_END



#endif
