// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_PROCESS_INTERFACE_PROCESSINTERFACE_H_
#define _PXZ_PROCESS_INTERFACE_PROCESSINTERFACE_H_

#include "ProcessTypes.h"

PXZ_MODULE_BEGIN(ProcessI)

class PXZ_EXPORTED ProcessInterface
{
public:
   //! Automatically decimates a selection of meshes, using as a target a triangle count or a ratio (reduction percentage), and bakes Normals information into a texture (plus other textures).
   /*!
     \param occurrences Occurrences to process
     \param decimationTargetType 
     \param bakingOptions Option maps baking
     \param overrideExistingUV Override the original UV or not
   */
   static void decimateTargetBake(const SceneI::OccurrenceList & occurrences, const AlgoI::DecimateOptionsSelector & decimationTargetType, const BakeOptions & bakingOptions, const CoreI::Boolean & overrideExistingUV = true);

   //! Automatically generates 3 LODs for the current selection.
   /*!
     \param occurrences Scene paths of components to process
     \param decimateParametersList The list of all LOD decimate parameters
   */
   static void generateLODChain(const SceneI::OccurrenceList & occurrences, const DecimateParametersList & decimateParametersList = DecimateParametersList(2).set(0, DecimateParameters(1., 0.1, 5.)).set(1, DecimateParameters(2., 1., 10.)));

   //! Automatically generates one unique optimized mesh out of the models in the scene, with material(s).
   /*!
     \param occurrences Occurrences to process
     \param generateDiffuseMap 
   */
   static void generatePhantomMesh(const SceneI::OccurrenceList & occurrences, const GenerateDiffuseMap & generateDiffuseMap = GenerateDiffuseMap(BakeDiffuseOptions(1024, 1)));

   //! The Guided import automatically converts and readies your 3D model(s) with guided parameters.
   /*!
     \param fileNames List of files to import
     \param coordinateSystem "Position and Scale" parameters are meant to properly position, orient and scale the imported model, to match Pixyz Studio's own coordinate sytem (right-handed, Y is the Up-axis) and units (mm).<br>
     \param tessellation Tessellation is the process of converting CAD models into meshes, usable by any 3D app (check out "About Tessellation" in the documentation for more information).<br>With 'Use Preset', Pixyz Studio automatically sets tessellation values to reach a predefined mesh quality.<br>With 'Use custom values', tessellation values can be set by the user.<br>Note that the Tessellation process is ignored when importing files containing only meshes (polygonal or 3D printing models for example), or Point clouds: this Tessellation process does not affect or change them.
     \param otherOptions Use the following parameters to fine-tune the import process
     \param importLines Use to import the lines (which can be CAD curves or polylines) included in the imported model, that are important to import and preserve.
     \param importPoints Use to import the points (which can be CAD point or free points, used as references for example) included in the imported model, that are important to import and preserve.<br>No need to enable this parameter when importing Point clouds.
     \param importHidden Use to import the parts that are originally hidden in the imported model.<br>They are imported but not made visible though.
     \param importPMI Use to import the PMI attached to the imported model (applicable for CAD models only)
     \param importVariants Use to import the variants attached to the imported model (applicable for CAD models only)
     \param useAlternativeImporter Some 3D/CAD formats handled by Pixyz Studio can be imported through 2 different importing technologies (or libraries).<br>When one file/model is not properly imported, or not imported at all, using this parameter allows to test importing through an alternative importer and having a better luck (providing the format offers an alternative importer).
     \return dest The root occurrences of each imported file
   */
   static SceneI::OccurrenceList guidedImport(const IOI::ImportFilePathList & fileNames, const CoordinateSystemOptions & coordinateSystem, const TessellationSettings & tessellation, const ImportOptions & otherOptions, const CoreI::Boolean & importLines = false, const CoreI::Boolean & importPoints = false, const CoreI::Boolean & importHidden = false, const CoreI::Boolean & importPMI = false, const CoreI::Boolean & importVariants = false, const CoreI::Boolean & useAlternativeImporter = false);

   //! Automatically generates a Proxy Mesh out of a selection of meshes, with optional automatic textures generation.
   /*!
     \param occurrences Occurrences to process
     \param gridResolution Resolution of the voxel grid used to generated the proxy
     \param generateTextures Option maps baking
     \param transferAnimations If false, skinned animations will be lost
     \param keepOriginalMesh If true, does not delete original mesh at the end of the process
     \return proxyMesh The generated proxy mesh
   */
   static SceneI::Occurrence proxyFromMeshes(const SceneI::OccurrenceList & occurrences, const CoreI::Int & gridResolution, const BakeOptionSelector & generateTextures, const CoreI::Bool & transferAnimations, const CoreI::Bool & keepOriginalMesh);

   //! Automatically generates a Proxy Mesh out of a selection of Point Cloud, with optional automatic diffuse texture generation.
   /*!
     \param occurrences Occurrences to process
     \param gridResolution Resolution of the voxel grid used to generated the proxy
     \param generateDiffuseTexture Baking options
     \param keepOriginalPointCloud If true, does not delete original point cloud at the end of the process
     \return proxyMesh The generated proxy mesh
   */
   static SceneI::Occurrence proxyFromPointCloud(const SceneI::OccurrenceList & occurrences, const CoreI::Int & gridResolution, const GenerateDiffuseMap & generateDiffuseTexture, const CoreI::Bool & keepOriginalPointCloud);

   //! Automatically creates optimized meshes out of any 3D model (mesh or CAD).
   /*!
     \param overrideExistingUVs If True, UVs for channel 0 will be recreated (algo GenerateUVonAABB , size=100)
     \param repackUVs If True, UVs for Channel 1 will be automatically repacked. Do not use if the original UV layout is fine (meshes already properly UVed).
   */
   static void runGenericProcess(const CoreI::Boolean & overrideExistingUVs, const CoreI::Boolean & repackUVs);


};

PXZ_MODULE_END



#endif
