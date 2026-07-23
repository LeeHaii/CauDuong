// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_UNITY_INTERFACE_UNITYINTERFACE_C_H_
#define _PXZ_UNITY_INTERFACE_UNITYINTERFACE_C_H_

#include "UnityTypes_c.h"

PXZ_EXPORTED char * Unity_getLastError();

// ----------------------------------------------------
// Rendering Functions
// ----------------------------------------------------

PXZ_EXPORTED Unity_UnityRenderingEvent Unity_getDestroyFunction();
PXZ_EXPORTED Unity_UnityRenderingEventAndData Unity_getDrawFunction();
PXZ_EXPORTED Unity_UnityRenderingEvent Unity_getInitFunction();

// ----------------------------------------------------
// Rendering functions
// ----------------------------------------------------

PXZ_EXPORTED View_ViewSession Unity_createViewSession(Core_Int width, Core_Int height, View_ViewSessionTextureList textures);

// ----------------------------------------------------
// directx
// ----------------------------------------------------

PXZ_EXPORTED Unity_ID3D11Device Unity_getD3D11Device();
PXZ_EXPORTED Unity_ID3D11RenderTargetView Unity_getD3D11RenderTargetViewFromRenderBuffer(Unity_UnityRenderBuffer surface);
PXZ_EXPORTED Unity_ID3D11ShaderResourceView Unity_getD3D11ShaderResourceViewFromNativeTexture(Unity_UnityTextureID texture);
PXZ_EXPORTED Unity_ID3D11Resource Unity_getD3D11TextureFromNativeTexture(Unity_UnityTextureID texture);
PXZ_EXPORTED Unity_ID3D11Resource Unity_getD3D11TextureFromRenderBuffer(Unity_UnityRenderBuffer buffer);
// Lock the mutex that ensure no rendering is done between lock/unlock
PXZ_EXPORTED void Unity_lockRenderUpdate();
// Try to lock the mutex that ensure no rendering is done between lock/unlock, returns true if the mutex has been locked, false if it was already locked
PXZ_EXPORTED Core_Bool Unity_tryLockRenderUpdate();
// Unlock the mutex that ensure no rendering is done between lock/unlock
PXZ_EXPORTED void Unity_unlockRenderUpdate();



#endif
