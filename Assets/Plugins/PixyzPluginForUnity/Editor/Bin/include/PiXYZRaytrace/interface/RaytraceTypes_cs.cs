#pragma warning disable CA2101

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace Pixyz.Raytrace.Native {

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Camera
	{
		public Camera() {}
		public Camera(Camera o) {
			this.position = o.position;
			this.direction = o.direction;
			this.up = o.up;
			this.fov = o.fov;
		}
		public Geom.Native.Point3 position;
		public Geom.Native.Point3 direction;
		public Geom.Native.Point3 up;
		public System.Double fov;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Camera_c
	{
		public Geom.Native.Point3_c position;
		public Geom.Native.Point3_c direction;
		public Geom.Native.Point3_c up;
		public System.Double fov;
	}

}
