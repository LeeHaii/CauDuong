// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_VIEW_INTERFACE_VIEWINTERFACE_H_
#define _PXZ_VIEW_INTERFACE_VIEWINTERFACE_H_

#include "ViewTypes.h"

PXZ_MODULE_BEGIN(ViewI)

class PXZ_EXPORTED ViewInterface
{
public:
   //! Add a notification
   /*!
     \param text Notification text
     \param time Time to stay
   */
   static void addNotification(const CoreI::String & text, const CoreI::Ident & time = 3000);

   //! Add a root to the main viewer
   /*!
     \param occurrence Occurrence to add
   */
   static void addRootToMainViewer(const SceneI::Occurrence & occurrence);

   //! Fit the main camera to the given parts
   /*!
     \param occurrences Parts to fit
   */
   static void fit(const SceneI::OccurrenceList & occurrences);

   //! Set the camera position
   /*!
     \param newPosition New camera position
   */
   static void setCameraPosition(const GeomI::Point3 & newPosition);

   //! Place the camera to origin
   static void toOrigin();

   //! event emited once the frame buffer is created
   static CoreI::Ident addAfterFramebufferCreateCallback(void(*fp)(void *), void * userdata = nullptr);
   //! method to remove a callback event
   static void removeAfterFramebufferCreateCallback(CoreI::Ident id); 

   //! event emited before the frame buffer is deleted
   static CoreI::Ident addBeforeFramebufferDeleteCallback(void(*fp)(void *), void * userdata = nullptr);
   //! method to remove a callback event
   static void removeBeforeFramebufferDeleteCallback(CoreI::Ident id); 

   //! event emited before frame refreshes
   /*!
     \param globalTimeMillisecond Global time in millisecond
   */
   static CoreI::Ident addBeforeRefreshCallback(void(*fp)(void *, const CoreI::ULong &), void * userdata = nullptr);
   //! method to remove a callback event
   static void removeBeforeRefreshCallback(CoreI::Ident id); 


   /**
    * \defgroup Animation Player AP related functions
    * @{
    */
   //! pauses an animation
   static void pauseAnimation();

   //! plays an animation
   /*!
     \param animation Animation to play
     \param speed Speed
     \param times Number of loops
   */
   static void playAnimation(const SceneI::Animation & animation, const CoreI::Double & speed, const CoreI::Int & times = -1);

   //! stops an animation
   static void stopAnimation();

   /**@}*/

   /**
    * \defgroup Occurrence 
    * @{
    */
   //! Create an occurrence on the target position of the camera (mid-click)
   /*!
     \param name Name of the new occurrence
     \param parent If defined, the new occurrence will be added as a child of the parent. Else the new parent will be the root of the current variant
     \return occurrence The created occurrence
   */
   static SceneI::Occurrence createOccurrenceOnCameraTarget(const CoreI::String & name, const SceneI::Occurrence & parent = SceneI::Occurrence());

   /**@}*/

   /**
    * \defgroup OverrideMaterial 
    * @{
    */
   static void disableOverrideMaterial();

   /*!
     \param material The material to enable as override material
   */
   static void enableOverrideMaterial(const MaterialI::Material & material);

   /**@}*/

   /**
    * \defgroup Rendering Events 
    * @{
    */
   //! emits when a pick was performed and there is a result to share
   /*!
     \param result Result of the picking
     \param session View session
   */
   static CoreI::Ident addPickResultCallback(void(*fp)(void *, const PickResult &, const ViewSession &), void * userdata = nullptr);
   //! method to remove a callback event
   static void removePickResultCallback(CoreI::Ident id); 

   //! emits when a view session is done resizing
   /*!
     \param session View session
   */
   static CoreI::Ident addViewSessionResizedCallback(void(*fp)(void *, const ViewSession &), void * userdata = nullptr);
   //! method to remove a callback event
   static void removeViewSessionResizedCallback(CoreI::Ident id); 

   /**@}*/

   /**
    * \defgroup View Session 
    * @{
    */
   /*!
     \param session 
     \param root 
   */
   static void addViewSessionRoot(const ViewSession & session, const SceneI::Occurrence & root);

   /*!
     \param session 
     \return result 
   */
   static CoreI::Bool isViewSessionExist(const ViewSession & session);

   /*!
     \param session 
     \param x 
     \param y 
   */
   static void pickOccurrences(const ViewSession & session, const CoreI::Int & x, const CoreI::Int & y);

   /*!
     \param session 
     \param root 
   */
   static void removeViewSessionRoot(const ViewSession & session, const SceneI::Occurrence & root);

   //! call this function when the texture has to be resized. the resize will take effect during the next render
   /*!
     \param session 
     \param width Viewer width
     \param height Viewer height
     \param textures Textures that need to be resized
   */
   static void resizeViewSession(const ViewSession & session, const CoreI::Int & width, const CoreI::Int & height, const ViewSessionTextureList & textures);

   /*!
     \param session 
     \param name 
     \param value 
   */
   static void setViewSessionViewerProperty(const ViewSession & session, const CoreI::String & name, const CoreI::String & value);

   //! emits when a session is initialized (via getInitfunction)
   /*!
     \param session View session
   */
   static CoreI::Ident addViewSessionInitializedCallback(void(*fp)(void *, const ViewSession &), void * userdata = nullptr);
   //! method to remove a callback event
   static void removeViewSessionInitializedCallback(CoreI::Ident id); 

   /**@}*/

   /**
    * \defgroup View Session Settings 
    * @{
    */
   //! Returns ViewSession viewer's draw properties
   /*!
     \param session 
     \return primitives 
   */
   static DrawPrimitives getViewSessionDrawPrimitives(const ViewSession & session);

   //! Set ViewSession viewer's draw properties
   /*!
     \param session 
     \param primitives 
   */
   static void setViewSessionDrawPrimitives(const ViewSession & session, const DrawPrimitives & primitives);

   //! Update explode settings for the viewer associated to the ViewSession
   /*!
     \param session 
     \param enabled 
     \param factor 
     \param X 
     \param Y 
     \param Z 
   */
   static void updateExplodeView(const ViewSession & session, const CoreI::Bool & enabled, const CoreI::Double & factor, const CoreI::Bool & X, const CoreI::Bool & Y, const CoreI::Bool & Z);

   //! Update cut plane for the viewer associated to the ViewSession
   /*!
     \param session 
     \param enabled 
     \param matrix 
   */
   static void updateViewSessionCuttingPlane(const ViewSession & session, const CoreI::Bool & enabled, const GeomI::Matrix4 & matrix);

   /**@}*/

   /**
    * \defgroup elements 
    * @{
    */
   /*!
     \param show True to enable, False to disable
     \param viewer 
   */
   static void showBReps(const CoreI::Boolean & show, const Viewer & viewer = -1);

   /*!
     \param show True to enable, False to disable
     \param viewer 
   */
   static void showEdges(const CoreI::Boolean & show, const Viewer & viewer = -1);

   //! Switch between show hidden and show visible mode
   /*!
     \param enable True to enable, False to disable
     \param viewer 
   */
   static void showHidden(const CoreI::Boolean & enable, const Viewer & viewer = -1);

   /*!
     \param show True to enable, False to disable
     \param viewer 
   */
   static void showLines(const CoreI::Boolean & show, const Viewer & viewer = -1);

   /*!
     \param show True to enable, False to disable
     \param viewer 
   */
   static void showPatchesBorders(const CoreI::Boolean & show, const Viewer & viewer = -1);

   /*!
     \param show True to enable, False to disable
     \param viewer 
   */
   static void showPoints(const CoreI::Boolean & show, const Viewer & viewer = -1);

   /*!
     \param show True to enable, False to disable
     \param viewer 
   */
   static void showPolygons(const CoreI::Boolean & show, const Viewer & viewer = -1);

   /*!
     \param show True to enable, False to disable
     \param viewer 
   */
   static void showSkeleton(const CoreI::Boolean & show, const Viewer & viewer = -1);

   /**@}*/

   /**
    * \defgroup interop 
    * @{
    */
   /*!
     \param interop 
     \param pxzTexture 
     \param dxTexture 
   */
   static void addSharedD3D11Texture(const D3D11Interop & interop, const TextureHandle & pxzTexture, const ID3D11Resource & dxTexture);

   /*!
     \param viewer 
     \param device 
     \return interop 
   */
   static D3D11Interop createD3D11Interop(const Viewer & viewer, const ID3D11Device & device);

   /*!
     \param interop 
   */
   static void deleteD3D11Interop(const D3D11Interop & interop);

   /*!
     \param interop 
     \return locked 
   */
   static CoreI::Bool isD3D11InteropLocked(const D3D11Interop & interop);

   /*!
     \param interop 
   */
   static void lockD3D11Interop(const D3D11Interop & interop);

   /*!
     \param interop 
     \param pxzTexture 
   */
   static void removeSharedD3D11Texture(const D3D11Interop & interop, const TextureHandle & pxzTexture);

   /*!
     \param interop 
   */
   static void unlockD3D11Interop(const D3D11Interop & interop);

   /**@}*/

   /**
    * \defgroup offscreen 
    * @{
    */
   //! Add a viewer root
   /*!
     \param root Occurrence to add
     \param viewer Viewer to modify
   */
   static void addRoot(const SceneI::Occurrence & root, const Viewer & viewer = -1);

   //! Create a new viewer
   /*!
     \param width Width of the viewer framebuffer
     \param height Height of the viewer framebuffer
     \param handleSelection Does the viewer handle selection of occurrences
     \return viewer 
   */
   static Viewer createViewer(const CoreI::Int & width, const CoreI::Int & height, const CoreI::Bool & handleSelection = true);

   /*!
     \param viewer Viewer to destroy
   */
   static void destroyViewer(const Viewer & viewer);

   //! Fit scene to viewer
   /*!
     \param viewer Viewer to modify
   */
   static void fitView(const Viewer & viewer = -1);

   /*!
     \param viewer 
     \param index 
     \return handle 
   */
   static TextureHandle getColorTextureHandle(const Viewer & viewer = -1, const CoreI::Int & index = 0);

   //! Get a viewer property value
   /*!
     \param propertyName 
     \param viewer 
     \return propertyValue 
   */
   static CoreI::String getCuttingPlaneProperty(const CoreI::String & propertyName, const Viewer & viewer = -1);

   //! Get depth texture handle
   /*!
     \param viewer Targeted viewer
     \return handle Get texture handle
   */
   static TextureHandle getDepthTextureHandle(const Viewer & viewer = -1);

   //! Get a viewer property value
   /*!
     \param propertyName 
     \param viewer 
     \return propertyValue 
   */
   static CoreI::String getExplodeViewProperty(const CoreI::String & propertyName, const Viewer & viewer = -1);

   /*!
     \param viewer 
     \return handle 
   */
   static TextureHandle getFXAATextureHandle(const Viewer & viewer = -1);

   /*!
     \param viewer 
     \return handle 
   */
   static TextureHandle getFinalTextureHandle(const Viewer & viewer = -1);

   /*!
     \param viewer 
     \return primitivies 
   */
   static DrawPrimitives getViewerDrawPrimitives(const Viewer & viewer = -1);

   /*!
     \param viewer 
     \return views 
     \return projs 
     \return clipping 
   */
   static getViewerMatricesReturn getViewerMatrices(const Viewer & viewer = -1);

   //! Get a viewer property value
   /*!
     \param propertyName 
     \param viewer 
     \return propertyValue 
   */
   static CoreI::String getViewerProperty(const CoreI::String & propertyName, const Viewer & viewer = -1);

   //! Retrieve the viewport size of a viewer
   /*!
     \param viewer 
     \return width 
     \return height 
   */
   static getViewerSizeReturn getViewerSize(const Viewer & viewer = -1);

   //! Get the list of viewer properties
   /*!
     \param viewer 
     \return propertyValues 
   */
   static CoreI::StringList listViewerProperties(const Viewer & viewer = -1);

   /*!
     \param viewer Targeted viewer
   */
   static void makeCurrent(const Viewer & viewer = -1);

   /*!
     \param x 
     \param y 
     \param viewer 
     \return occurrence Picked occurrence, 0 if not occurrence picked
     \return position World space position of the picking point
   */
   static pickReturn pick(const CoreI::Int & x, const CoreI::Int & y, const Viewer & viewer = -1);

   //! Refresh the viewer
   /*!
     \param viewer Viewer to refresh
     \param frameCount Number of frames to render
     \param forceUpdate Force the viewer to update pending modification on the geometry. By default this is disabled while running process
   */
   static void refreshViewer(const Viewer & viewer = -1, const CoreI::Int & frameCount = 1, const CoreI::Boolean & forceUpdate = false);

   //! Remove a viewer root
   /*!
     \param root Occurrence to remove
     \param viewer Viewer to modify
   */
   static void removeRoot(const SceneI::Occurrence & root, const Viewer & viewer = -1);

   /*!
     \param width Width of the viewer framebuffer
     \param height Height of the viewer framebuffer
     \param viewer 
   */
   static void resizeViewer(const CoreI::Int & width, const CoreI::Int & height, const Viewer & viewer = -1);

   //! Set a viewer property value
   /*!
     \param propertyName 
     \param propertyValue 
     \param viewer 
   */
   static void setCuttingPlaneProperty(const CoreI::String & propertyName, const CoreI::String & propertyValue, const Viewer & viewer = -1);

   //! Set a viewer property value
   /*!
     \param propertyName 
     \param propertyValue 
     \param viewer 
   */
   static void setExplodeViewProperty(const CoreI::String & propertyName, const CoreI::String & propertyValue, const Viewer & viewer = -1);

   /*!
     \param primitivies 
     \param viewer 
   */
   static void setViewerDrawPrimitives(const DrawPrimitives & primitivies, const Viewer & viewer = -1);

   /*!
     \param views 
     \param projs 
     \param clipping 
     \param viewer 
   */
   static void setViewerMatrices(const GeomI::Matrix4List & views, const GeomI::Matrix4List & projs, const GeomI::Vector2 & clipping, const Viewer & viewer = -1);

   //! Set a viewer property value
   /*!
     \param propertyName 
     \param propertyValue 
     \param viewer 
   */
   static void setViewerProperty(const CoreI::String & propertyName, const CoreI::String & propertyValue, const Viewer & viewer = -1);

   //! Take a screenshot
   /*!
     \param filename File path to save at
     \param viewer Targeted viewer
   */
   static void takeScreenshot(const CoreI::FilePath & filename, const Viewer & viewer = -1);

   /**@}*/

   /**
    * \defgroup viewer 
    * @{
    */
   //! Returns the information of the current camera (used in the main viewer)
   /*!
     \return camera Current camera
   */
   static SceneI::Camera getCurrentCamera();

   //! Get a viewer property value
   /*!
     \param propertyName Property name
     \return propertyValue 
   */
   static CoreI::String getMainViewerCuttingPlaneProperty(const CoreI::String & propertyName);

   //! Get a viewer property value
   /*!
     \param propertyName Property name
     \return propertyValue 
   */
   static CoreI::String getMainViewerExplodeViewProperty(const CoreI::String & propertyName);

   //! Get a viewer property value
   /*!
     \param propertyName Property name
     \return propertyValue 
   */
   static CoreI::String getMainViewerProperty(const CoreI::String & propertyName);

   //! Returns the main viewer current view matrix
   /*!
     \return projection Projection matrix of the current camera in the main viewer
   */
   static GeomI::Matrix4 getProjectionMatrix();

   //! Returns the main viewer current view matrix
   /*!
     \return view View matrix of the current camera in the main viewer
   */
   static GeomI::Matrix4 getViewMatrix();

   //! Get the list of viewer properties
   /*!
     \return propertyValues 
   */
   static CoreI::StringList listMainViewerCuttingPlaneProperties();

   //! Get the list of viewer properties
   /*!
     \return propertyValues 
   */
   static CoreI::StringList listMainViewerExplodeViewProperty();

   //! Get the list of viewer properties
   /*!
     \return propertyValues 
   */
   static CoreI::StringList listMainViewerProperties();

   //! Pause the viewer
   static void pauseViewer();

   //! Pause the viewer
   static void resumeViewer();

   //! Set a viewer property value
   /*!
     \param propertyName Property name
     \param propertyValue Property value
   */
   static void setMainViewerCuttingPlaneProperty(const CoreI::String & propertyName, const CoreI::String & propertyValue);

   //! Set a viewer property value
   /*!
     \param propertyName Property name
     \param propertyValue Property value
   */
   static void setMainViewerExplodeViewProperty(const CoreI::String & propertyName, const CoreI::String & propertyValue);

   //! Set a viewer property value
   /*!
     \param propertyName Property name
     \param propertyValue Property value
     \return propertyValue 
   */
   static CoreI::String setMainViewerProperty(const CoreI::String & propertyName, const CoreI::String & propertyValue);

   /**@}*/

   /**
    * \defgroup visibility Visibility functions
    * @{
    */
   //! Start a new visibility session
   /*!
     \param width Width of the renderer used for the visibility session
     \param height Width of the renderer used for the visibility session
     \return visibilitySession Identifier of the visibility session
   */
   static CoreI::Ident beginVisibilitySession(const CoreI::Int & width, const CoreI::Int & height);

   //! Create a mesh capping the cutting plane and display it
   static void drawCappingPlane();

   //! Terminate a visibility session
   /*!
     \param visibilitySession Identifier of the visibility session
   */
   static void endVisibilitySession(const CoreI::Ident & visibilitySession);

   //! Place the camera of a given visibility session
   /*!
     \param visibilitySession Identifier of the visibility session
     \param position Position of the camera
     \param view View direction of the camera
     \param up Up direction of the camera
     \param fovx Horizontal field of view (in degree)
   */
   static void setVisibilitySessionCamera(const CoreI::Ident & visibilitySession, const GeomI::Point3 & position, const GeomI::Vector3 & view, const GeomI::Vector3 & up, const GeomI::Angle & fovx);

   //! Render one frame of the visibility session
   /*!
     \param visibilitySession Identifier of the visibility session
     \param parts If false, optimize when parts seen are not wanted
     \param patches If false, optimize when patches seen are not wanted
     \param polygons If false, optimize when polygons seen are not wanted
     \param countOnce Optimize when it is not needed to count the numbers of pixels seen during the session
     \return sceneOccurrences The list of scene paths seen from this shoot
   */
   static SceneI::OccurrenceList visibilityShoot(const CoreI::Ident & visibilitySession, const CoreI::Bool & parts, const CoreI::Bool & patches, const CoreI::Bool & polygons, const CoreI::Bool & countOnce);

   /**@}*/

};

PXZ_MODULE_END



#endif
