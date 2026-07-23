// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_VIEW_INTERFACE_VIEWINTERFACE_C_H_
#define _PXZ_VIEW_INTERFACE_VIEWINTERFACE_C_H_

#include "ViewTypes_c.h"

PXZ_EXPORTED char * View_getLastError();

// Add a notification
PXZ_EXPORTED void View_addNotification(Core_String text, Core_Ident time);
// Add a root to the main viewer
PXZ_EXPORTED void View_addRootToMainViewer(Scene_Occurrence occurrence);
// Fit the main camera to the given parts
PXZ_EXPORTED void View_fit(Scene_OccurrenceList occurrences);
// Set the camera position
PXZ_EXPORTED void View_setCameraPosition(Geom_Point3 newPosition);
// Place the camera to origin
PXZ_EXPORTED void View_toOrigin();
// event emited once the frame buffer is created
PXZ_EXPORTED Core_Ident View_addAfterFramebufferCreateCallback(void(*fp)(void *), void * userdata = nullptr);
   //! method to remove a callback event
PXZ_EXPORTED void View_removeAfterFramebufferCreateCallback(Core_Ident id);

// event emited before the frame buffer is deleted
PXZ_EXPORTED Core_Ident View_addBeforeFramebufferDeleteCallback(void(*fp)(void *), void * userdata = nullptr);
   //! method to remove a callback event
PXZ_EXPORTED void View_removeBeforeFramebufferDeleteCallback(Core_Ident id);

// event emited before frame refreshes
   /*!
     \param globalTimeMillisecond Global time in millisecond
   */
PXZ_EXPORTED Core_Ident View_addBeforeRefreshCallback(void(*fp)(void *, Core_ULong), void * userdata = nullptr);
   //! method to remove a callback event
PXZ_EXPORTED void View_removeBeforeRefreshCallback(Core_Ident id);


// ----------------------------------------------------
// Animation Player
// AP related functions
// ----------------------------------------------------

// pauses an animation
PXZ_EXPORTED void View_pauseAnimation();
// plays an animation
PXZ_EXPORTED void View_playAnimation(Scene_Animation animation, Core_Double speed, Core_Int times);
// stops an animation
PXZ_EXPORTED void View_stopAnimation();

// ----------------------------------------------------
// Occurrence
// ----------------------------------------------------

// Create an occurrence on the target position of the camera (mid-click)
PXZ_EXPORTED Scene_Occurrence View_createOccurrenceOnCameraTarget(Core_String name, Scene_Occurrence parent);

// ----------------------------------------------------
// OverrideMaterial
// ----------------------------------------------------

PXZ_EXPORTED void View_disableOverrideMaterial();
PXZ_EXPORTED void View_enableOverrideMaterial(Material_Material material);

// ----------------------------------------------------
// Rendering Events
// ----------------------------------------------------

// emits when a pick was performed and there is a result to share
   /*!
     \param result Result of the picking
     \param session View session
   */
PXZ_EXPORTED Core_Ident View_addPickResultCallback(void(*fp)(void *, View_PickResult, View_ViewSession), void * userdata = nullptr);
   //! method to remove a callback event
PXZ_EXPORTED void View_removePickResultCallback(Core_Ident id);

// emits when a view session is done resizing
   /*!
     \param session View session
   */
PXZ_EXPORTED Core_Ident View_addViewSessionResizedCallback(void(*fp)(void *, View_ViewSession), void * userdata = nullptr);
   //! method to remove a callback event
PXZ_EXPORTED void View_removeViewSessionResizedCallback(Core_Ident id);


// ----------------------------------------------------
// View Session
// ----------------------------------------------------

PXZ_EXPORTED void View_addViewSessionRoot(View_ViewSession session, Scene_Occurrence root);
PXZ_EXPORTED Core_Bool View_isViewSessionExist(View_ViewSession session);
PXZ_EXPORTED void View_pickOccurrences(View_ViewSession session, Core_Int x, Core_Int y);
PXZ_EXPORTED void View_removeViewSessionRoot(View_ViewSession session, Scene_Occurrence root);
// call this function when the texture has to be resized. the resize will take effect during the next render
PXZ_EXPORTED void View_resizeViewSession(View_ViewSession session, Core_Int width, Core_Int height, View_ViewSessionTextureList textures);
PXZ_EXPORTED void View_setViewSessionViewerProperty(View_ViewSession session, Core_String name, Core_String value);
// emits when a session is initialized (via getInitfunction)
   /*!
     \param session View session
   */
PXZ_EXPORTED Core_Ident View_addViewSessionInitializedCallback(void(*fp)(void *, View_ViewSession), void * userdata = nullptr);
   //! method to remove a callback event
PXZ_EXPORTED void View_removeViewSessionInitializedCallback(Core_Ident id);


// ----------------------------------------------------
// View Session Settings
// ----------------------------------------------------

// Returns ViewSession viewer's draw properties
PXZ_EXPORTED View_DrawPrimitives View_getViewSessionDrawPrimitives(View_ViewSession session);
// Set ViewSession viewer's draw properties
PXZ_EXPORTED void View_setViewSessionDrawPrimitives(View_ViewSession session, View_DrawPrimitives primitives);
// Update explode settings for the viewer associated to the ViewSession
PXZ_EXPORTED void View_updateExplodeView(View_ViewSession session, Core_Bool enabled, Core_Double factor, Core_Bool X, Core_Bool Y, Core_Bool Z);
// Update cut plane for the viewer associated to the ViewSession
PXZ_EXPORTED void View_updateViewSessionCuttingPlane(View_ViewSession session, Core_Bool enabled, Geom_Matrix4 matrix);

// ----------------------------------------------------
// elements
// ----------------------------------------------------

PXZ_EXPORTED void View_showBReps(Core_Boolean show, View_Viewer viewer);
PXZ_EXPORTED void View_showEdges(Core_Boolean show, View_Viewer viewer);
// Switch between show hidden and show visible mode
PXZ_EXPORTED void View_showHidden(Core_Boolean enable, View_Viewer viewer);
PXZ_EXPORTED void View_showLines(Core_Boolean show, View_Viewer viewer);
PXZ_EXPORTED void View_showPatchesBorders(Core_Boolean show, View_Viewer viewer);
PXZ_EXPORTED void View_showPoints(Core_Boolean show, View_Viewer viewer);
PXZ_EXPORTED void View_showPolygons(Core_Boolean show, View_Viewer viewer);
PXZ_EXPORTED void View_showSkeleton(Core_Boolean show, View_Viewer viewer);

// ----------------------------------------------------
// interop
// ----------------------------------------------------

PXZ_EXPORTED void View_addSharedD3D11Texture(View_D3D11Interop interop, View_TextureHandle pxzTexture, View_ID3D11Resource dxTexture);
PXZ_EXPORTED View_D3D11Interop View_createD3D11Interop(View_Viewer viewer, View_ID3D11Device device);
PXZ_EXPORTED void View_deleteD3D11Interop(View_D3D11Interop interop);
PXZ_EXPORTED Core_Bool View_isD3D11InteropLocked(View_D3D11Interop interop);
PXZ_EXPORTED void View_lockD3D11Interop(View_D3D11Interop interop);
PXZ_EXPORTED void View_removeSharedD3D11Texture(View_D3D11Interop interop, View_TextureHandle pxzTexture);
PXZ_EXPORTED void View_unlockD3D11Interop(View_D3D11Interop interop);

// ----------------------------------------------------
// offscreen
// ----------------------------------------------------

// Add a viewer root
PXZ_EXPORTED void View_addRoot(Scene_Occurrence root, View_Viewer viewer);
// Create a new viewer
PXZ_EXPORTED View_Viewer View_createViewer(Core_Int width, Core_Int height, Core_Bool handleSelection);
PXZ_EXPORTED void View_destroyViewer(View_Viewer viewer);
// Fit scene to viewer
PXZ_EXPORTED void View_fitView(View_Viewer viewer);
PXZ_EXPORTED View_TextureHandle View_getColorTextureHandle(View_Viewer viewer, Core_Int index);
// Get a viewer property value
PXZ_EXPORTED Core_String View_getCuttingPlaneProperty(Core_String propertyName, View_Viewer viewer);
// Get depth texture handle
PXZ_EXPORTED View_TextureHandle View_getDepthTextureHandle(View_Viewer viewer);
// Get a viewer property value
PXZ_EXPORTED Core_String View_getExplodeViewProperty(Core_String propertyName, View_Viewer viewer);
PXZ_EXPORTED View_TextureHandle View_getFXAATextureHandle(View_Viewer viewer);
PXZ_EXPORTED View_TextureHandle View_getFinalTextureHandle(View_Viewer viewer);
PXZ_EXPORTED View_DrawPrimitives View_getViewerDrawPrimitives(View_Viewer viewer);
typedef struct {
   Geom_Matrix4List views;
   Geom_Matrix4List projs;
   Geom_Vector2 clipping;
} View_getViewerMatricesReturn;
PXZ_EXPORTED View_getViewerMatricesReturn View_getViewerMatrices(View_Viewer viewer);
// Get a viewer property value
PXZ_EXPORTED Core_String View_getViewerProperty(Core_String propertyName, View_Viewer viewer);
// Retrieve the viewport size of a viewer
typedef struct {
   Core_Int width;
   Core_Int height;
} View_getViewerSizeReturn;
PXZ_EXPORTED View_getViewerSizeReturn View_getViewerSize(View_Viewer viewer);
// Get the list of viewer properties
PXZ_EXPORTED Core_StringList View_listViewerProperties(View_Viewer viewer);
PXZ_EXPORTED void View_makeCurrent(View_Viewer viewer);
typedef struct {
   Scene_Occurrence occurrence;
   Geom_Point3 position;
} View_pickReturn;
PXZ_EXPORTED View_pickReturn View_pick(Core_Int x, Core_Int y, View_Viewer viewer);
// Refresh the viewer
PXZ_EXPORTED void View_refreshViewer(View_Viewer viewer, Core_Int frameCount, Core_Boolean forceUpdate);
// Remove a viewer root
PXZ_EXPORTED void View_removeRoot(Scene_Occurrence root, View_Viewer viewer);
PXZ_EXPORTED void View_resizeViewer(Core_Int width, Core_Int height, View_Viewer viewer);
// Set a viewer property value
PXZ_EXPORTED void View_setCuttingPlaneProperty(Core_String propertyName, Core_String propertyValue, View_Viewer viewer);
// Set a viewer property value
PXZ_EXPORTED void View_setExplodeViewProperty(Core_String propertyName, Core_String propertyValue, View_Viewer viewer);
PXZ_EXPORTED void View_setViewerDrawPrimitives(View_DrawPrimitives primitivies, View_Viewer viewer);
PXZ_EXPORTED void View_setViewerMatrices(Geom_Matrix4List views, Geom_Matrix4List projs, Geom_Vector2 clipping, View_Viewer viewer);
// Set a viewer property value
PXZ_EXPORTED void View_setViewerProperty(Core_String propertyName, Core_String propertyValue, View_Viewer viewer);
// Take a screenshot
PXZ_EXPORTED void View_takeScreenshot(Core_FilePath filename, View_Viewer viewer);

// ----------------------------------------------------
// viewer
// ----------------------------------------------------

// Returns the information of the current camera (used in the main viewer)
PXZ_EXPORTED Scene_Camera View_getCurrentCamera();
// Get a viewer property value
PXZ_EXPORTED Core_String View_getMainViewerCuttingPlaneProperty(Core_String propertyName);
// Get a viewer property value
PXZ_EXPORTED Core_String View_getMainViewerExplodeViewProperty(Core_String propertyName);
// Get a viewer property value
PXZ_EXPORTED Core_String View_getMainViewerProperty(Core_String propertyName);
// Returns the main viewer current view matrix
PXZ_EXPORTED Geom_Matrix4 View_getProjectionMatrix();
// Returns the main viewer current view matrix
PXZ_EXPORTED Geom_Matrix4 View_getViewMatrix();
// Get the list of viewer properties
PXZ_EXPORTED Core_StringList View_listMainViewerCuttingPlaneProperties();
// Get the list of viewer properties
PXZ_EXPORTED Core_StringList View_listMainViewerExplodeViewProperty();
// Get the list of viewer properties
PXZ_EXPORTED Core_StringList View_listMainViewerProperties();
// Pause the viewer
PXZ_EXPORTED void View_pauseViewer();
// Pause the viewer
PXZ_EXPORTED void View_resumeViewer();
// Set a viewer property value
PXZ_EXPORTED void View_setMainViewerCuttingPlaneProperty(Core_String propertyName, Core_String propertyValue);
// Set a viewer property value
PXZ_EXPORTED void View_setMainViewerExplodeViewProperty(Core_String propertyName, Core_String propertyValue);
// Set a viewer property value
PXZ_EXPORTED Core_String View_setMainViewerProperty(Core_String propertyName, Core_String propertyValue);

// ----------------------------------------------------
// visibility
// Visibility functions
// ----------------------------------------------------

// Start a new visibility session
PXZ_EXPORTED Core_Ident View_beginVisibilitySession(Core_Int width, Core_Int height);
// Create a mesh capping the cutting plane and display it
PXZ_EXPORTED void View_drawCappingPlane();
// Terminate a visibility session
PXZ_EXPORTED void View_endVisibilitySession(Core_Ident visibilitySession);
// Place the camera of a given visibility session
PXZ_EXPORTED void View_setVisibilitySessionCamera(Core_Ident visibilitySession, Geom_Point3 position, Geom_Vector3 view, Geom_Vector3 up, Geom_Angle fovx);
// Render one frame of the visibility session
PXZ_EXPORTED Scene_OccurrenceList View_visibilityShoot(Core_Ident visibilitySession, Core_Bool parts, Core_Bool patches, Core_Bool polygons, Core_Bool countOnce);



#endif
