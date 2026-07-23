#pragma warning disable CA2101

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace Pixyz.Core.Native {

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct AliasTypeDesc
	{
		public AliasTypeDesc(AliasTypeDesc o) {
			this.name = o.name;
			this.module = o.module;
			this.description = o.description;
			this.alias = o.alias;
		}
		public System.String name;
		public System.String module;
		public System.String description;
		public System.String alias;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AliasTypeDesc_c
	{
		public IntPtr name;
		public IntPtr module;
		public IntPtr description;
		public IntPtr alias;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct ArrayTypeDesc
	{
		public ArrayTypeDesc(ArrayTypeDesc o) {
			this.name = o.name;
			this.module = o.module;
			this.description = o.description;
			this.arrayType = o.arrayType;
		}
		public System.String name;
		public System.String module;
		public System.String description;
		public System.String arrayType;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ArrayTypeDesc_c
	{
		public IntPtr name;
		public IntPtr module;
		public IntPtr description;
		public IntPtr arrayType;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct StringPair
	{
		public StringPair(StringPair o) {
			this.key = o.key;
			this.value = o.value;
		}
		public System.String key;
		public System.String value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct StringPair_c
	{
		public IntPtr key;
		public IntPtr value;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class StringPairList {
		public Core.Native.StringPair[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public StringPairList(Core.Native.StringPair[] tab) { list = tab; }
		public static implicit operator Core.Native.StringPair[](StringPairList o) { return o.list; }
		public Core.Native.StringPair this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public StringPairList(int size) { list = new Core.Native.StringPair[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct StringPairList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class AutoValueReturn
	{
		public AutoValueReturn() {}
		public AutoValueReturn(AutoValueReturn o) {
			this.values = o.values;
			this.message = o.message;
		}
		public StringPairList values;
		public System.String message;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AutoValueReturn_c
	{
		public StringPairList_c values;
		public IntPtr message;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct BaseTypeDesc
	{
		public BaseTypeDesc(BaseTypeDesc o) {
			this.name = o.name;
			this.module = o.module;
			this.description = o.description;
		}
		public System.String name;
		public System.String module;
		public System.String description;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BaseTypeDesc_c
	{
		public IntPtr name;
		public IntPtr module;
		public IntPtr description;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class BoolList {
		public System.Boolean[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public BoolList() {}
		public BoolList(System.Boolean[] tab) { list = tab; }
		public static implicit operator System.Boolean[](BoolList o) { return o.list; }
		public System.Boolean this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public BoolList(int size) { list = new System.Boolean[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BoolList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ByteList {
		public System.Byte[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public ByteList() {}
		public ByteList(System.Byte[] tab) { list = tab; }
		public static implicit operator System.Byte[](ByteList o) { return o.list; }
		public System.Byte this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public ByteList(int size) { list = new System.Byte[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ByteList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Color
	{
		public Color(Color o) {
			this.r = o.r;
			this.g = o.g;
			this.b = o.b;
		}
		public System.Double r;
		public System.Double g;
		public System.Double b;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Color_c
	{
		public System.Double r;
		public System.Double g;
		public System.Double b;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct ColorAlpha
	{
		public ColorAlpha(ColorAlpha o) {
			this.r = o.r;
			this.g = o.g;
			this.b = o.b;
			this.a = o.a;
		}
		public System.Double r;
		public System.Double g;
		public System.Double b;
		public System.Double a;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ColorAlpha_c
	{
		public System.Double r;
		public System.Double g;
		public System.Double b;
		public System.Double a;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ColorAlphaList {
		public Core.Native.ColorAlpha[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public ColorAlphaList(Core.Native.ColorAlpha[] tab) { list = tab; }
		public static implicit operator Core.Native.ColorAlpha[](ColorAlphaList o) { return o.list; }
		public Core.Native.ColorAlpha this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public ColorAlphaList(int size) { list = new Core.Native.ColorAlpha[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ColorAlphaList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ColorList {
		public Core.Native.Color[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public ColorList(Core.Native.Color[] tab) { list = tab; }
		public static implicit operator Core.Native.Color[](ColorList o) { return o.list; }
		public Core.Native.Color this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public ColorList(int size) { list = new Core.Native.Color[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ColorList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Date
	{
		public Date(Date o) {
			this.year = o.year;
			this.month = o.month;
			this.day = o.day;
		}
		public System.Int32 year;
		public System.Int32 month;
		public System.Int32 day;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Date_c
	{
		public Int32 year;
		public Int32 month;
		public Int32 day;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class DoubleList {
		public System.Double[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public DoubleList() {}
		public DoubleList(System.Double[] tab) { list = tab; }
		public static implicit operator System.Double[](DoubleList o) { return o.list; }
		public System.Double this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public DoubleList(int size) { list = new System.Double[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DoubleList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class DoubleListList {
		public Core.Native.DoubleList[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public DoubleListList(Core.Native.DoubleList[] tab) { list = tab; }
		public static implicit operator Core.Native.DoubleList[](DoubleListList o) { return o.list; }
		public Core.Native.DoubleList this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public DoubleListList(int size) { list = new Core.Native.DoubleList[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DoubleListList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class EntityList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public EntityList() {}
		public EntityList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](EntityList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public EntityList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct EntityList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class EntityListList {
		public Core.Native.EntityList[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public EntityListList(Core.Native.EntityList[] tab) { list = tab; }
		public static implicit operator Core.Native.EntityList[](EntityListList o) { return o.list; }
		public Core.Native.EntityList this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public EntityListList(int size) { list = new Core.Native.EntityList[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct EntityListList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class IntList {
		public System.Int32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public IntList() {}
		public IntList(System.Int32[] tab) { list = tab; }
		public static implicit operator System.Int32[](IntList o) { return o.list; }
		public System.Int32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public IntList(int size) { list = new System.Int32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct IntList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class StringList {
		public System.String[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public StringList() {}
		public StringList(System.String[] tab) { list = tab; }
		public static implicit operator System.String[](StringList o) { return o.list; }
		public System.String this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public StringList(int size) { list = new System.String[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct StringList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class EnumTypeDesc
	{
		public EnumTypeDesc() {}
		public EnumTypeDesc(EnumTypeDesc o) {
			this.name = o.name;
			this.module = o.module;
			this.description = o.description;
			this.values = o.values;
			this.labels = o.labels;
			this.valueType = o.valueType;
		}
		public System.String name;
		public System.String module;
		public System.String description;
		public IntList values;
		public StringList labels;
		public System.String valueType;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct EnumTypeDesc_c
	{
		public IntPtr name;
		public IntPtr module;
		public IntPtr description;
		public IntList_c values;
		public StringList_c labels;
		public IntPtr valueType;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct ListTypeDesc
	{
		public ListTypeDesc(ListTypeDesc o) {
			this.name = o.name;
			this.module = o.module;
			this.description = o.description;
			this.listType = o.listType;
		}
		public System.String name;
		public System.String module;
		public System.String description;
		public System.String listType;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ListTypeDesc_c
	{
		public IntPtr name;
		public IntPtr module;
		public IntPtr description;
		public IntPtr listType;
	}

	public enum TypeDescType
	{
		UNKNOWN = 0,
		BASE = 1,
		ALIAS = 2,
		ENUM = 3,
		LIST = 4,
		ARRAY = 5,
		STRUCT = 6,
		SELECT = 7,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Field
	{
		public Field() {}
		public Field(Field o) {
			this.name = o.name;
			this.type = o.type;
			this.defaultValue = o.defaultValue;
			this.description = o.description;
			this.predefinedValues = o.predefinedValues;
		}
		public System.String name;
		public System.String type;
		public System.String defaultValue;
		public System.String description;
		public StringPairList predefinedValues;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Field_c
	{
		public IntPtr name;
		public IntPtr type;
		public IntPtr defaultValue;
		public IntPtr description;
		public StringPairList_c predefinedValues;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class FieldList {
		public Core.Native.Field[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public FieldList(Core.Native.Field[] tab) { list = tab; }
		public static implicit operator Core.Native.Field[](FieldList o) { return o.list; }
		public Core.Native.Field this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public FieldList(int size) { list = new Core.Native.Field[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FieldList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class StructTypeDesc
	{
		public StructTypeDesc() {}
		public StructTypeDesc(StructTypeDesc o) {
			this.name = o.name;
			this.module = o.module;
			this.description = o.description;
			this.type = o.type;
			this.fields = o.fields;
		}
		public System.String name;
		public System.String module;
		public System.String description;
		public TypeDescType type;
		public FieldList fields;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct StructTypeDesc_c
	{
		public IntPtr name;
		public IntPtr module;
		public IntPtr description;
		public Int32 type;
		public FieldList_c fields;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class SelectTypeDesc
	{
		public SelectTypeDesc() {}
		public SelectTypeDesc(SelectTypeDesc o) {
			this.name = o.name;
			this.module = o.module;
			this.description = o.description;
			this.type = o.type;
		}
		public System.String name;
		public System.String module;
		public System.String description;
		public TypeDescType type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SelectTypeDesc_c
	{
		public IntPtr name;
		public IntPtr module;
		public IntPtr description;
		public Int32 type;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct TypeDesc
	{
		public enum Type
		{
			UNKNOWN = 0,
			BASETYPE = 1,
			ALIASTYPE = 2,
			ENUMTYPE = 3,
			LISTTYPE = 4,
			ARRAYTYPE = 5,
			STRUCTTYPE = 6,
			SELECTTYPE = 7,
		}
		public BaseTypeDesc baseType;
		public AliasTypeDesc aliasType;
		public EnumTypeDesc enumType;
		public ListTypeDesc listType;
		public ArrayTypeDesc arrayType;
		public StructTypeDesc structType;
		public SelectTypeDesc selectType;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct TypeDesc_c
	{
		public BaseTypeDesc_c baseType;
		public AliasTypeDesc_c aliasType;
		public EnumTypeDesc_c enumType;
		public ListTypeDesc_c listType;
		public ArrayTypeDesc_c arrayType;
		public StructTypeDesc_c structType;
		public SelectTypeDesc_c selectType;
		public int _type;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ParameterDesc
	{
		public ParameterDesc() {}
		public ParameterDesc(ParameterDesc o) {
			this.name = o.name;
			this.type = o.type;
			this.description = o.description;
			this.optional = o.optional;
			this.defaultValue = o.defaultValue;
		}
		public System.String name;
		public TypeDesc type;
		public System.String description;
		public System.Boolean optional;
		public System.String defaultValue;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ParameterDesc_c
	{
		public IntPtr name;
		public TypeDesc_c type;
		public IntPtr description;
		public Int32 optional;
		public IntPtr defaultValue;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ParameterDescList {
		public Core.Native.ParameterDesc[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public ParameterDescList(Core.Native.ParameterDesc[] tab) { list = tab; }
		public static implicit operator Core.Native.ParameterDesc[](ParameterDescList o) { return o.list; }
		public Core.Native.ParameterDesc this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public ParameterDescList(int size) { list = new Core.Native.ParameterDesc[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ParameterDescList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class EventDesc
	{
		public EventDesc() {}
		public EventDesc(EventDesc o) {
			this.module = o.module;
			this.name = o.name;
			this.description = o.description;
			this.parameters = o.parameters;
		}
		public System.String module;
		public System.String name;
		public System.String description;
		public ParameterDescList parameters;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct EventDesc_c
	{
		public IntPtr module;
		public IntPtr name;
		public IntPtr description;
		public ParameterDescList_c parameters;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class FilePathList {
		public System.String[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public FilePathList() {}
		public FilePathList(System.String[] tab) { list = tab; }
		public static implicit operator System.String[](FilePathList o) { return o.list; }
		public System.String this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public FilePathList(int size) { list = new System.String[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FilePathList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class FunctionDesc
	{
		public FunctionDesc() {}
		public FunctionDesc(FunctionDesc o) {
			this.name = o.name;
			this.module = o.module;
			this.parameters = o.parameters;
			this.returns = o.returns;
		}
		public System.String name;
		public System.String module;
		public ParameterDescList parameters;
		public ParameterDescList returns;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FunctionDesc_c
	{
		public IntPtr name;
		public IntPtr module;
		public ParameterDescList_c parameters;
		public ParameterDescList_c returns;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class FunctionDescList {
		public Core.Native.FunctionDesc[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public FunctionDescList(Core.Native.FunctionDesc[] tab) { list = tab; }
		public static implicit operator Core.Native.FunctionDesc[](FunctionDescList o) { return o.list; }
		public Core.Native.FunctionDesc this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public FunctionDescList(int size) { list = new Core.Native.FunctionDesc[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FunctionDescList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class GroupDesc
	{
		public GroupDesc() {}
		public GroupDesc(GroupDesc o) {
			this.name = o.name;
			this.description = o.description;
			this.functions = o.functions;
		}
		public System.String name;
		public System.String description;
		public FunctionDescList functions;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct GroupDesc_c
	{
		public IntPtr name;
		public IntPtr description;
		public FunctionDescList_c functions;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class GroupDescList {
		public Core.Native.GroupDesc[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public GroupDescList(Core.Native.GroupDesc[] tab) { list = tab; }
		public static implicit operator Core.Native.GroupDesc[](GroupDescList o) { return o.list; }
		public Core.Native.GroupDesc this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public GroupDescList(int size) { list = new Core.Native.GroupDesc[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct GroupDescList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct IdentPair
	{
		public IdentPair(IdentPair o) {
			this.key = o.key;
			this.value = o.value;
		}
		public System.UInt32 key;
		public System.UInt32 value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct IdentPair_c
	{
		public System.UInt32 key;
		public System.UInt32 value;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class IdentPairList {
		public Core.Native.IdentPair[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public IdentPairList(Core.Native.IdentPair[] tab) { list = tab; }
		public static implicit operator Core.Native.IdentPair[](IdentPairList o) { return o.list; }
		public Core.Native.IdentPair this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public IdentPairList(int size) { list = new Core.Native.IdentPair[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct IdentPairList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	public enum InheritableBool
	{
		False = 0,
		True = 1,
		Inherited = 2,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class InheritableBoolList {
		public Core.Native.InheritableBool[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public InheritableBoolList(Core.Native.InheritableBool[] tab) { list = tab; }
		public static implicit operator Core.Native.InheritableBool[](InheritableBoolList o) { return o.list; }
		public Core.Native.InheritableBool this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public InheritableBoolList(int size) { list = new Core.Native.InheritableBool[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct InheritableBoolList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class IntListList {
		public Core.Native.IntList[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public IntListList(Core.Native.IntList[] tab) { list = tab; }
		public static implicit operator Core.Native.IntList[](IntListList o) { return o.list; }
		public Core.Native.IntList this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public IntListList(int size) { list = new Core.Native.IntList[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct IntListList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class LicenseInfos
	{
		public LicenseInfos() {}
		public LicenseInfos(LicenseInfos o) {
			this.version = o.version;
			this.customerName = o.customerName;
			this.customerCompany = o.customerCompany;
			this.customerEmail = o.customerEmail;
			this.startDate = o.startDate;
			this.endDate = o.endDate;
		}
		public System.String version;
		public System.String customerName;
		public System.String customerCompany;
		public System.String customerEmail;
		public Date startDate;
		public Date endDate;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LicenseInfos_c
	{
		public IntPtr version;
		public IntPtr customerName;
		public IntPtr customerCompany;
		public IntPtr customerEmail;
		public Date_c startDate;
		public Date_c endDate;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ModuleDesc
	{
		public ModuleDesc() {}
		public ModuleDesc(ModuleDesc o) {
			this.name = o.name;
			this.description = o.description;
			this.author = o.author;
			this.groups = o.groups;
		}
		public System.String name;
		public System.String description;
		public System.String author;
		public GroupDescList groups;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ModuleDesc_c
	{
		public IntPtr name;
		public IntPtr description;
		public IntPtr author;
		public GroupDescList_c groups;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ModuleDescList {
		public Core.Native.ModuleDesc[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public ModuleDescList(Core.Native.ModuleDesc[] tab) { list = tab; }
		public static implicit operator Core.Native.ModuleDesc[](ModuleDescList o) { return o.list; }
		public Core.Native.ModuleDesc this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public ModuleDescList(int size) { list = new Core.Native.ModuleDesc[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ModuleDescList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class StringPairListList {
		public Core.Native.StringPairList[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public StringPairListList(Core.Native.StringPairList[] tab) { list = tab; }
		public static implicit operator Core.Native.StringPairList[](StringPairListList o) { return o.list; }
		public Core.Native.StringPairList this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public StringPairListList(int size) { list = new Core.Native.StringPairList[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct StringPairListList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ULongList {
		public System.UInt64[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public ULongList() {}
		public ULongList(System.UInt64[] tab) { list = tab; }
		public static implicit operator System.UInt64[](ULongList o) { return o.list; }
		public System.UInt64 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public ULongList(int size) { list = new System.UInt64[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ULongList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	public enum Verbose
	{
		ERR = 0,
		WARNING = 1,
		INFO = 2,
		SCRIPT = 5,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class WebLicenseInfo
	{
		public WebLicenseInfo() {}
		public WebLicenseInfo(WebLicenseInfo o) {
			this.id = o.id;
			this.product = o.product;
			this.validity = o.validity;
			this.count = o.count;
			this.inUse = o.inUse;
			this.onMachine = o.onMachine;
			this.current = o.current;
		}
		public System.UInt32 id;
		public System.String product;
		public Date validity;
		public System.Int32 count;
		public System.Int32 inUse;
		public System.Boolean onMachine;
		public System.Boolean current;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WebLicenseInfo_c
	{
		public System.UInt32 id;
		public IntPtr product;
		public Date_c validity;
		public Int32 count;
		public Int32 inUse;
		public Int32 onMachine;
		public Int32 current;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class WebLicenseInfoList {
		public Core.Native.WebLicenseInfo[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public WebLicenseInfoList(Core.Native.WebLicenseInfo[] tab) { list = tab; }
		public static implicit operator Core.Native.WebLicenseInfo[](WebLicenseInfoList o) { return o.list; }
		public Core.Native.WebLicenseInfo this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public WebLicenseInfoList(int size) { list = new Core.Native.WebLicenseInfo[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WebLicenseInfoList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getLicenseServerReturn
	{
		public getLicenseServerReturn(getLicenseServerReturn o) {
			this.serverHost = o.serverHost;
			this.serverPort = o.serverPort;
			this.useFlexLM = o.useFlexLM;
		}
		public System.String serverHost;
		public System.UInt16 serverPort;
		public System.Boolean useFlexLM;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getLicenseServerReturn_c
	{
		public IntPtr serverHost;
		public System.UInt16 serverPort;
		public Int32 useFlexLM;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct availableMemoryReturn
	{
		public availableMemoryReturn(availableMemoryReturn o) {
			this.availVirt = o.availVirt;
			this.totalVirt = o.totalVirt;
			this.availPhys = o.availPhys;
			this.totalPhys = o.totalPhys;
		}
		public System.Int64 availVirt;
		public System.Int64 totalVirt;
		public System.Int64 availPhys;
		public System.Int64 totalPhys;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct availableMemoryReturn_c
	{
		public System.Int64 availVirt;
		public System.Int64 totalVirt;
		public System.Int64 availPhys;
		public System.Int64 totalPhys;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct checkForUpdatesReturn
	{
		public checkForUpdatesReturn(checkForUpdatesReturn o) {
			this.newVersionAvailable = o.newVersionAvailable;
			this.newVersion = o.newVersion;
			this.newVersionLink = o.newVersionLink;
		}
		public System.Boolean newVersionAvailable;
		public System.String newVersion;
		public System.String newVersionLink;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct checkForUpdatesReturn_c
	{
		public Int32 newVersionAvailable;
		public IntPtr newVersion;
		public IntPtr newVersionLink;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getInterfaceLoggerConfigurationReturn
	{
		public getInterfaceLoggerConfigurationReturn(getInterfaceLoggerConfigurationReturn o) {
			this.functionEnabled = o.functionEnabled;
			this.parametersEnabled = o.parametersEnabled;
			this.executionTimeEnabled = o.executionTimeEnabled;
		}
		public System.Boolean functionEnabled;
		public System.Boolean parametersEnabled;
		public System.Boolean executionTimeEnabled;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getInterfaceLoggerConfigurationReturn_c
	{
		public Int32 functionEnabled;
		public Int32 parametersEnabled;
		public Int32 executionTimeEnabled;
	}

}
