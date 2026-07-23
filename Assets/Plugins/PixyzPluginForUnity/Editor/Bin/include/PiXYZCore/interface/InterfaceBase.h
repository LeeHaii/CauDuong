#ifndef _PXZCORE_INTERFACE_INTERFACEBASE_H_
#define _PXZCORE_INTERFACE_INTERFACEBASE_H_

# define PXZ_STRINGIFY_HELPER(a) #a
# define PXZ_STRINGIFY(a) PXZ_STRINGIFY_HELPER(a)

/****************************************
* Module management & namespaces stuff *
****************************************/
#define PXZ_NS PiXYZ

#ifdef PXZ_DISABLE_NAMESPACE
# define PXZ_NS_BEGIN
# define PXZ_NS_END
# define USE_PXZ
# define PXZ_USE_MODULE(module) using namespace module;
#else
# define PXZ_NS_BEGIN namespace PXZ_NS {
# define PXZ_NS_END }
# define USE_PXZ using namespace PXZ_NS;
# define PXZ_USE_MODULE(module) using namespace PXZ_NS::module;
# define PXZ_USE_GUI_MODULE(module) using namespace PXZ_NS::module::GUI;
#endif

#define PXZ_MODULE_BEGIN(module) \
   PXZ_NS_BEGIN \
namespace module {

#define PXZ_MODULE_END \
} \
   PXZ_NS_END

/*******************************
* Interface linkage macro     *
*******************************/
#ifndef PXZ_EXPORTED
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
#endif

#include <cstdlib>

extern "C" {
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
}

inline void * pxzAlloc(size_t size)
{
#ifdef BUILDING_PXZ
   return malloc(size);
#else
   return pxzAllocDll(size);
#endif
}

inline void pxzFree(void * buf)
{
#ifdef BUILDING_PXZ
   free(buf);
#else
   pxzFreeDll(buf);
#endif
}

#include "List.h"
#include "Array.h"
#include "String.h"
PXZ_MODULE_BEGIN(CoreI)

typedef int None;
typedef int Int;
typedef unsigned int UInt;
typedef unsigned int Ident;
typedef short Short;
typedef unsigned short UShort;
typedef double Double;
typedef int64_t Long;
typedef bool Bool;
typedef uint8_t Byte;
typedef uint64_t ULong;
typedef void* Ptr;

class Exception
{
private:
   String _desc;
public:
   Exception(const String & desc) : _desc(desc) {}
   const String & getDescription() const { return _desc; }
};

class InterruptException : public Exception {
public:
   InterruptException(const String & desc) : Exception(desc) {}
};


PXZ_MODULE_END

#ifdef PXZ_BUILD_GUI
PXZ_MODULE_BEGIN(CoreGUII)

class PtrClass {
public:
   PtrClass(CoreI::Ptr ptr) : _ptr(ptr) {}
   virtual ~PtrClass() {}
   CoreI::Ptr getPtr() const { return _ptr; }
   void setPtr(CoreI::Ptr ptr) { _ptr = ptr; }
private:
   CoreI::Ptr _ptr;  
};

#define QTWIDGET_TYPEDEF(NAME) typedef                \
class PtrClass##NAME : public PtrClass {              \
public:                                               \
   PtrClass##NAME() : PtrClass##NAME(nullptr) {}      \
   PtrClass##NAME(CoreI::Ptr ptr) : PtrClass(ptr) {}  \
   virtual ~PtrClass##NAME() {}                       \
} NAME;

QTWIDGET_TYPEDEF(QWidget);
QTWIDGET_TYPEDEF(QAction);
QTWIDGET_TYPEDEF(QDialog);

PXZ_MODULE_END

PXZ_MODULE_BEGIN(Core)
namespace Utils {
   inline std::string toString(const CoreGUII::PtrClass & c) {
      if(c.getPtr())
         return "<Pointer>";
      return "<Empty>";
   }
}
PXZ_MODULE_END
#endif

#endif
