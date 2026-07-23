// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_CORE_INTERFACE_CORETYPES_H_
#define _PXZ_CORE_INTERFACE_CORETYPES_H_

#include <PiXYZCore/interface/InterfaceBase.h>
#include <functional> 

PXZ_MODULE_BEGIN(CoreI)

struct AliasTypeDesc {
   String name;
   String module;
   String description;
   String alias;
   AliasTypeDesc(String _name = "", String _module = "", String _description = "", String _alias = "") : name(_name), module(_module), description(_description), alias(_alias) {}
};

struct ArrayTypeDesc {
   String name;
   String module;
   String description;
   String arrayType;
   ArrayTypeDesc(String _name = "", String _module = "", String _description = "", String _arrayType = "") : name(_name), module(_module), description(_description), arrayType(_arrayType) {}
};

struct StringPair {
   String key;
   String value;
   StringPair(String _key = "", String _value = "") : key(_key), value(_value) {}
};

typedef CoreI::List<StringPair> StringPairList;

struct AutoValueReturn {
   StringPairList values;
   String message;
   AutoValueReturn(StringPairList _values = StringPairList(), String _message = "") : values(_values), message(_message) {}
};

struct BaseTypeDesc {
   String name;
   String module;
   String description;
   BaseTypeDesc(String _name = "", String _module = "", String _description = "") : name(_name), module(_module), description(_description) {}
};

typedef CoreI::List<Bool> BoolList;

typedef Bool Boolean;

typedef CoreI::List<Byte> ByteList;

typedef Double Coeff;

struct Color {
   Double r;
   Double g;
   Double b;
   Color(Double _r = 1, Double _g = 1, Double _b = 1) : r(_r), g(_g), b(_b) {}
};

struct ColorAlpha {
   Double r;
   Double g;
   Double b;
   Double a;
   ColorAlpha(Double _r = 1, Double _g = 1, Double _b = 1, Double _a = 1) : r(_r), g(_g), b(_b), a(_a) {}
};

typedef CoreI::List<ColorAlpha> ColorAlphaList;

typedef CoreI::List<Color> ColorList;

struct Date {
   Int year;
   Int month;
   Int day;
   Date(Int _year = 2015, Int _month = 1, Int _day = 1) : year(_year), month(_month), day(_day) {}
};

typedef String DirectoryPath;

typedef CoreI::List<Double> DoubleList;

typedef CoreI::List<DoubleList> DoubleListList;

typedef Ident Entity;

typedef CoreI::List<Entity> EntityList;

typedef CoreI::List<EntityList> EntityListList;

typedef CoreI::List<Int> IntList;

typedef CoreI::List<String> StringList;

struct EnumTypeDesc {
   String name;
   String module;
   String description;
   IntList values;
   StringList labels;
   String valueType;
   EnumTypeDesc(String _name = "", String _module = "", String _description = "", IntList _values = IntList(), StringList _labels = StringList(), String _valueType = "") : name(_name), module(_module), description(_description), values(_values), labels(_labels), valueType(_valueType) {}
};

struct ListTypeDesc {
   String name;
   String module;
   String description;
   String listType;
   ListTypeDesc(String _name = "", String _module = "", String _description = "", String _listType = "") : name(_name), module(_module), description(_description), listType(_listType) {}
};

class TypeDescType {
public:
   typedef enum {
   UNKNOWN = 0,
   BASE = 1,
   ALIAS = 2,
   ENUM = 3,
   LIST = 4,
   ARRAY = 5,
   STRUCT = 6,
   SELECT = 7
   } Type;
private:
   Type _value;
public:
   TypeDescType(Type val = UNKNOWN) : _value(val) {}
   TypeDescType & operator=(Type val) { _value = val; return *this; }
   TypeDescType(const CoreI::Int& val) : _value((Type)val) {}
   TypeDescType & operator=(const CoreI::Int& val) { _value = (Type)val; return *this; }
   Type value() const { return _value; }
   operator Type() const { return _value; }
};
struct Field {
   String name;
   String type;
   String defaultValue;
   String description;
   StringPairList predefinedValues;
   Field(String _name = "", String _type = "", String _defaultValue = "", String _description = "", StringPairList _predefinedValues = StringPairList()) : name(_name), type(_type), defaultValue(_defaultValue), description(_description), predefinedValues(_predefinedValues) {}
};

typedef CoreI::List<Field> FieldList;

struct StructTypeDesc {
   String name;
   String module;
   String description;
   TypeDescType type;
   FieldList fields;
   StructTypeDesc(String _name = "", String _module = "", String _description = "", TypeDescType _type = TypeDescType::BASE, FieldList _fields = FieldList()) : name(_name), module(_module), description(_description), type(_type), fields(_fields) {}
};

struct SelectTypeDesc {
   String name;
   String module;
   String description;
   TypeDescType type;
   SelectTypeDesc(String _name = "", String _module = "", String _description = "", TypeDescType _type = TypeDescType::BASE) : name(_name), module(_module), description(_description), type(_type) {}
};

struct TypeDesc {
   typedef enum {
      UNKNOWN=0,
      BASETYPE,
      ALIASTYPE,
      ENUMTYPE,
      LISTTYPE,
      ARRAYTYPE,
      STRUCTTYPE,
      SELECTTYPE
   } Type;
   Type _type; 
   BaseTypeDesc baseType;
   AliasTypeDesc aliasType;
   EnumTypeDesc enumType;
   ListTypeDesc listType;
   ArrayTypeDesc arrayType;
   StructTypeDesc structType;
   SelectTypeDesc selectType;
   TypeDesc() : _type(UNKNOWN) {}
   TypeDesc(const BaseTypeDesc & v) : _type(BASETYPE), baseType(v) {}
   TypeDesc(const AliasTypeDesc & v) : _type(ALIASTYPE), aliasType(v) {}
   TypeDesc(const EnumTypeDesc & v) : _type(ENUMTYPE), enumType(v) {}
   TypeDesc(const ListTypeDesc & v) : _type(LISTTYPE), listType(v) {}
   TypeDesc(const ArrayTypeDesc & v) : _type(ARRAYTYPE), arrayType(v) {}
   TypeDesc(const StructTypeDesc & v) : _type(STRUCTTYPE), structType(v) {}
   TypeDesc(const SelectTypeDesc & v) : _type(SELECTTYPE), selectType(v) {}
};
struct ParameterDesc {
   String name;
   TypeDesc type;
   String description;
   Bool optional;
   String defaultValue;
   ParameterDesc(String _name = "", TypeDesc _type = TypeDesc(), String _description = "", Bool _optional = false, String _defaultValue = "") : name(_name), type(_type), description(_description), optional(_optional), defaultValue(_defaultValue) {}
};

typedef CoreI::List<ParameterDesc> ParameterDescList;

struct EventDesc {
   String module;
   String name;
   String description;
   ParameterDescList parameters;
   EventDesc(String _module = "", String _name = "", String _description = "", ParameterDescList _parameters = ParameterDescList()) : module(_module), name(_name), description(_description), parameters(_parameters) {}
};

typedef String FilePath;

typedef CoreI::List<FilePath> FilePathList;

struct FunctionDesc {
   String name;
   String module;
   ParameterDescList parameters;
   ParameterDescList returns;
   FunctionDesc(String _name = "", String _module = "", ParameterDescList _parameters = ParameterDescList(), ParameterDescList _returns = ParameterDescList()) : name(_name), module(_module), parameters(_parameters), returns(_returns) {}
};

typedef CoreI::List<FunctionDesc> FunctionDescList;

struct GroupDesc {
   String name;
   String description;
   FunctionDescList functions;
   GroupDesc(String _name = "", String _description = "", FunctionDescList _functions = FunctionDescList()) : name(_name), description(_description), functions(_functions) {}
};

typedef CoreI::List<GroupDesc> GroupDescList;

struct IdentPair {
   Ident key;
   Ident value;
   IdentPair(Ident _key = 0, Ident _value = 0) : key(_key), value(_value) {}
};

typedef CoreI::List<IdentPair> IdentPairList;

class InheritableBool {
public:
   typedef enum {
   False = 0,
   True = 1,
   Inherited = 2
   } Type;
private:
   Type _value;
public:
   InheritableBool(Type val = False) : _value(val) {}
   InheritableBool & operator=(Type val) { _value = val; return *this; }
   InheritableBool(const CoreI::Int& val) : _value((Type)val) {}
   InheritableBool & operator=(const CoreI::Int& val) { _value = (Type)val; return *this; }
   Type value() const { return _value; }
   operator Type() const { return _value; }
};
typedef CoreI::List<InheritableBool> InheritableBoolList;

typedef CoreI::List<IntList> IntListList;

struct LicenseInfos {
   String version;
   String customerName;
   String customerCompany;
   String customerEmail;
   Date startDate;
   Date endDate;
   LicenseInfos(String _version = "", String _customerName = "", String _customerCompany = "", String _customerEmail = "", Date _startDate = Date(), Date _endDate = Date()) : version(_version), customerName(_customerName), customerCompany(_customerCompany), customerEmail(_customerEmail), startDate(_startDate), endDate(_endDate) {}
};

struct ModuleDesc {
   String name;
   String description;
   String author;
   GroupDescList groups;
   ModuleDesc(String _name = "", String _description = "", String _author = "", GroupDescList _groups = GroupDescList()) : name(_name), description(_description), author(_author), groups(_groups) {}
};

typedef CoreI::List<ModuleDesc> ModuleDescList;

typedef DirectoryPath OutputDirectoryPath;

typedef FilePath OutputFilePath;

typedef String Password;

typedef Double Percent;

typedef Coeff PreferredRangeCoef;

typedef Double Real;

typedef String Regex;

typedef CoreI::List<StringPairList> StringPairListList;

typedef CoreI::List<ULong> ULongList;

class Verbose {
public:
   typedef enum {
   ERR = 0,
   WARNING = 1,
   INFO = 2,
   SCRIPT = 5
   } Type;
private:
   Type _value;
public:
   Verbose(Type val = ERR) : _value(val) {}
   Verbose & operator=(Type val) { _value = val; return *this; }
   Verbose(const CoreI::Int& val) : _value((Type)val) {}
   Verbose & operator=(const CoreI::Int& val) { _value = (Type)val; return *this; }
   Type value() const { return _value; }
   operator Type() const { return _value; }
};
struct WebLicenseInfo {
   Ident id;
   String product;
   Date validity;
   Int count;
   Int inUse;
   Bool onMachine;
   Bool current;
   WebLicenseInfo(Ident _id = 0, String _product = "", Date _validity = Date(), Int _count = 0, Int _inUse = 0, Bool _onMachine = false, Bool _current = false) : id(_id), product(_product), validity(_validity), count(_count), inUse(_inUse), onMachine(_onMachine), current(_current) {}
};

typedef CoreI::List<WebLicenseInfo> WebLicenseInfoList;

typedef struct {
   String serverHost;
   UShort serverPort;
   Bool useFlexLM;
} getLicenseServerReturn;

typedef struct {
   Long availVirt;
   Long totalVirt;
   Long availPhys;
   Long totalPhys;
} availableMemoryReturn;

typedef struct {
   Bool newVersionAvailable;
   String newVersion;
   String newVersionLink;
} checkForUpdatesReturn;

typedef struct {
   Boolean functionEnabled;
   Boolean parametersEnabled;
   Boolean executionTimeEnabled;
} getInterfaceLoggerConfigurationReturn;

// ----------------------------------------------------
// Constants
// ----------------------------------------------------

inline Double Epsilon() { return 1e-6; }
inline Double EpsilonSquare() { return 1e-12; }
inline Double HalfPi() { return 1.5707963267948966192313215; }
inline Double Infinity() { return 1e12; }
inline Double OneDegree() { return 0.017453292519943295769236907684886; }
inline Double Pi() { return 3.141592653589793238462643; }
inline Double TwoPi() { return 6.283185307179586476925286; }

PXZ_MODULE_END

#endif
