#ifndef _PXZCORE_INTERFACE_INTERFACEBASE_C_H_
#define _PXZCORE_INTERFACE_INTERFACEBASE_C_H_

# define PXZ_STRINGIFY_HELPER(a) #a
# define PXZ_STRINGIFY(a) PXZ_STRINGIFY_HELPER(a)

/*******************************
* Interface linkage macro     *
*******************************/
#if defined(_MSC_VER)
# if defined(PXZ_BUILDING_SHARED)
#   define PXZ_EXPORTED  __declspec(dllexport)
#  else
#   define PXZ_EXPORTED __declspec(dllimport)
# endif
#else
# if __GNUC__ >= 4
#  define PXZ_EXPORTED __attribute__ ((visibility ("default")))
# else
#  define PXZ_EXPORTED
# endif
#endif

#include <stdlib.h>
#include <stdint.h>

#if defined(BUILDING_CORE_INTERFACE)
# ifdef OS_WIN32
    __declspec(dllexport) void * pxzAllocDll(size_t size);
    __declspec(dllexport) void pxzFreeDll(void * buf);
# else
    __attribute__ ((visibility ("default"))) void * pxzAllocDll(size_t size);
    __attribute__ ((visibility ("default"))) void pxzFreeDll(void * buf);
# endif
#else
    void * pxzAllocDll(size_t size);
    void pxzFreeDll(void * buf);
#endif

void * pxzAlloc(size_t size);
void pxzFree(void * buf);

typedef int Core_None;
typedef int Core_Int;
typedef unsigned int Core_UInt;
typedef unsigned int Core_Ident;
typedef short Core_Short;
typedef unsigned short Core_UShort;
typedef double Core_Double;
typedef long long Core_Long;
typedef unsigned long long Core_ULong;
typedef int Core_Bool;
typedef char * Core_String;
typedef uint8_t Core_Byte;
typedef void* Core_Ptr;

#ifdef PXZ_BUILD_GUI
typedef void * CoreGUI_QWidget;
typedef void * CoreGUI_QAction;
typedef void * CoreGUI_QDialog;
#endif

#endif
