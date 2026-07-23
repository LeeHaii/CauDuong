// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_UNITY_INTERFACE_UNITYINTERFACE_H_
#define _PXZ_UNITY_INTERFACE_UNITYINTERFACE_H_

#include "UnityTypes.h"

PXZ_MODULE_BEGIN(UnityI)

class PXZ_EXPORTED UnityInterface
{
public:
   /**
    * \defgroup Rendering Functions 
    * @{
    */
   /*!
     \return destroyFunction 
   */
   static UnityRenderingEvent getDestroyFunction();

   /*!
     \return drawFunction 
   */
   static UnityRenderingEventAndData getDrawFunction();

   /*!
     \return initFunction 
   */
   static UnityRenderingEvent getInitFunction();

   /**@}*/

   /**
    * \defgroup Rendering functions 
    * @{
    */
   /*!
     \param width Viewer width
     \param height Viewer height
     \param textures Textures that will be written on
     \return session 
   */
   static ViewI::ViewSession createViewSession(const CoreI::Int & width, const CoreI::Int & height, const ViewI::ViewSessionTextureList & textures);

   /**@}*/

   /**
    * \defgroup directx 
    * @{
    */
   /*!
     \return device 
   */
   static ID3D11Device getD3D11Device();

   /*!
     \param surface 
     \return rtv 
   */
   static ID3D11RenderTargetView getD3D11RenderTargetViewFromRenderBuffer(const UnityRenderBuffer & surface);

   /*!
     \param texture 
     \return srv 
   */
   static ID3D11ShaderResourceView getD3D11ShaderResourceViewFromNativeTexture(const UnityTextureID & texture);

   /*!
     \param texture 
     \return dxTexture 
   */
   static ID3D11Resource getD3D11TextureFromNativeTexture(const UnityTextureID & texture);

   /*!
     \param buffer 
     \return dxTexture 
   */
   static ID3D11Resource getD3D11TextureFromRenderBuffer(const UnityRenderBuffer & buffer);

   //! Lock the mutex that ensure no rendering is done between lock/unlock
   static void lockRenderUpdate();

   //! Try to lock the mutex that ensure no rendering is done between lock/unlock, returns true if the mutex has been locked, false if it was already locked
   /*!
     \return locked True if the mutex has been successfully locked
   */
   static CoreI::Bool tryLockRenderUpdate();

   //! Unlock the mutex that ensure no rendering is done between lock/unlock
   static void unlockRenderUpdate();

   /**@}*/

};

PXZ_MODULE_END



#endif
