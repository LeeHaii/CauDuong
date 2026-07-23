using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixyz.ImportSDK
{
	public static partial class Conversions
	{
		#region Point <> Vector

		public static Geom.Native.Point4 ToPoint4(this Vector4 point)
		{
			return new Geom.Native.Point4() { x = (double)point.x, y = (double)point.y, z = (double)point.z, w = (double)point.w };
		}

		public static Geom.Native.Point3 ToPoint3(this Vector3 point)
		{
			return new Geom.Native.Point3() { x = (double)point.x, y = (double)point.y, z = (double)point.z };
		}

		public static Geom.Native.Point3 ToPoint2(this Vector2 point)
		{
			return new Geom.Native.Point3() { x = (double)point.x, y = (double)point.y };
		}

		public static Vector4 ToVector3(this Geom.Native.Point4 point)
		{
			return new Vector4((float)point.x, (float)point.y, (float)point.z, (float)point.w);
		}

		public static Vector3 ToVector3(this Geom.Native.Point3 point)
		{
			return new Vector3((float)point.x, (float)point.y, (float)point.z);
		}

		public static Vector2 ToVector2(this Geom.Native.Point2 point)
		{
			return new Vector2((float)point.x, (float)point.y);
		}

		#endregion

		#region Matrix4 <> Matrix4x4
		public static Geom.Native.Matrix4 ConvertMatrix(Matrix4x4 matrix4x4)
		{
			var matrix4 = new Geom.Native.Matrix4();
			matrix4.tab = new Geom.Native.Array4[4];
			matrix4[0] = new Geom.Native.Array4();
			matrix4[0][0] = matrix4x4.m00;
			matrix4[0][1] = matrix4x4.m01;
			matrix4[0][2] = matrix4x4.m02;
			matrix4[0][3] = matrix4x4.m03;
			matrix4[1] = new Geom.Native.Array4();
			matrix4[1][0] = matrix4x4.m10;
			matrix4[1][1] = matrix4x4.m11;
			matrix4[1][2] = matrix4x4.m12;
			matrix4[1][3] = matrix4x4.m13;
			matrix4[2] = new Geom.Native.Array4();
			matrix4[2][0] = matrix4x4.m20;
			matrix4[2][1] = matrix4x4.m21;
			matrix4[2][2] = matrix4x4.m22;
			matrix4[2][3] = matrix4x4.m23;
			matrix4[3] = new Geom.Native.Array4();
			matrix4[3][0] = matrix4x4.m30;
			matrix4[3][1] = matrix4x4.m31;
			matrix4[3][2] = matrix4x4.m32;
			matrix4[3][3] = matrix4x4.m33;
			return matrix4;
		}

		public static Matrix4x4 ConvertMatrix(Geom.Native.Matrix4 matrix4)
		{
			var matrix4x4 = new Matrix4x4();
			matrix4x4.m00 = (float)matrix4[0][0];
			matrix4x4.m01 = (float)matrix4[0][1];
			matrix4x4.m02 = (float)matrix4[0][2];
			matrix4x4.m03 = (float)matrix4[0][3];
			matrix4x4.m10 = (float)matrix4[1][0];
			matrix4x4.m11 = (float)matrix4[1][1];
			matrix4x4.m12 = (float)matrix4[1][2];
			matrix4x4.m13 = (float)matrix4[1][3];
			matrix4x4.m20 = (float)matrix4[2][0];
			matrix4x4.m21 = (float)matrix4[2][1];
			matrix4x4.m22 = (float)matrix4[2][2];
			matrix4x4.m23 = (float)matrix4[2][3];
			matrix4x4.m30 = (float)matrix4[3][0];
			matrix4x4.m31 = (float)matrix4[3][1];
			matrix4x4.m32 = (float)matrix4[3][2];
			matrix4x4.m33 = (float)matrix4[3][3];

			return matrix4x4;
		}

		public static Geom.Native.Matrix4List ConvertMatrices(IList<Matrix4x4> matrices)
		{
			var matrix4List = new Geom.Native.Matrix4List(matrices.Count);
			for (int i = 0; i < matrices.Count; i++)
			{
				matrix4List[i] = ConvertMatrix(matrices[i]);
			}
			return matrix4List;
		}

		public static IList<Matrix4x4> ConvertMatrices(Geom.Native.Matrix4List matrices)
		{
			var matrix4x4List = new List<Matrix4x4>();
			for (int i = 0; i < matrices.length; i++)
			{
				matrix4x4List.Add(ConvertMatrix(matrices[i]));
			}
			return matrix4x4List;
		}

		public static Geom.Native.Matrix4 Identity()
		{
			Geom.Native.Matrix4 mat = new Geom.Native.Matrix4();

			mat.tab = new Geom.Native.Array4[4];
			mat.tab[0] = new Geom.Native.Array4(new double[] { 1.0, 0.0, 0.0, 0.0 });
			mat.tab[1] = new Geom.Native.Array4(new double[] { 0.0, 1.0, 0.0, 0.0 });
			mat.tab[2] = new Geom.Native.Array4(new double[] { 0.0, 0.0, 1.0, 0.0 });
			mat.tab[3] = new Geom.Native.Array4(new double[] { 0.0, 0.0, 0.0, 1.0 });

			return mat;
		}
		#endregion

		#region ColorPixyz <> ColorUnity

		public static Core.Native.Color ToPiXYZColor(this Color colorU)
		{
			return new Core.Native.Color() { r = colorU.r, g = colorU.g, b = colorU.b };
		}

		public static Color ToUnityColor(this Core.Native.Color color)
		{
			return new Color() { r = (float)color.r, g = (float)color.g, b = (float)color.b };
		}
		public static Core.Native.ColorAlpha ToPiXYZColorAlpha(this Color colorU)
		{
			return new Core.Native.ColorAlpha() { r = colorU.r, g = colorU.g, b = colorU.b, a = colorU.a };
		}
		public static Core.Native.ColorAlpha ColorToColorAlpha(this Core.Native.Color colorAlpha)
		{
			return new Core.Native.ColorAlpha() { r = colorAlpha.r, g = colorAlpha.g, b = colorAlpha.b, a = 1 };
		}
		public static Core.Native.Color ColorAlphaToColor(this Core.Native.ColorAlpha color)
		{
			return new Core.Native.Color() { r = color.r, g = color.g, b = color.b };
		}

		public static Color ToUnityColor(this Core.Native.ColorAlpha color)
		{
			return new Color() { r = (float)color.r, g = (float)color.g, b = (float)color.b, a = (float)color.a };
		}

		public static Color IntToColor(uint intColor)
		{
			uint r = (intColor >> 24) & 0xff;
			uint g = (intColor >> 16) & 0xff;
			uint b = (intColor >> 8) & 0xff;
			uint a = (intColor) & 0xff;
			return new Color((float)(r / 255.0f), (float)(g / 255.0f), (float)(b / 255.0f), (float)(a / 255.0f));
		}

		public static uint ColorToInt(Color color)
		{
			return ColorToInt(color.r, color.g, color.b, color.a);
		}

		public static uint ColorToInt(Core.Native.ColorAlpha color)
		{
			return ColorToInt((float)color.r, (float)color.g, (float)color.b, (float)color.a);
		}

		public static uint ColorToInt(Core.Native.Color color)
		{
			return ColorToInt((float)color.r, (float)color.g, (float)color.b, 0.0f);
		}

		public static uint ColorToInt(float r, float g, float b, float a)
		{
			uint rShort = (uint)(r * 255.0f);
			uint gShort = (uint)(g * 255.0f);
			uint bShort = (uint)(b * 255.0f);
			uint aShort = (uint)(a * 255.0f);

			return (rShort & 0xff) << 24 | (gShort & 0xff) << 16 | (bShort & 0xff) << 8 | (aShort & 0xff);
		}
		#endregion

		#region TimePixyz <> TimeUnity

		public static DateTime ToUnityTime(this Core.Native.Date date)
		{
			return new DateTime(date.year, date.month, date.day);
		}

		public static Core.Native.Date ToPixyzTime(this DateTime date)
		{
			return new Core.Native.Date() { year = date.Year, month = date.Month, day = date.Day };
		}

		#endregion
	}
}