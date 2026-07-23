#ifndef PXZIMPOSTER_CGINC
#define PXZIMPOSTER_CGINC

#define ORTHO_OFFSET 10000

uniform sampler2D _PxzImpostorAlbedoOcclusion;
uniform sampler2D _PxzImpostorSpecularRoughness;
uniform sampler2D _PxzImpostorNormals;
uniform sampler2D _PxzImpostorEmission;
uniform sampler2D _PxzImpostorDepth;
uniform float _PxzImpostorFramesCount;
uniform float _OctahedronDiameter;
uniform float _Pxz_Clip;
uniform float _FullOcta;
uniform float _Linear;
uniform float _Binary;
uniform float _ShadowBias;
uniform float _PreciseMesh;
uniform float _PrecisionDepth;

struct PxzImpostorData
{
	float4 vertex;
	float3 localRayDir;
	float2 frameOcta2D;
	float3 viewPos;
};

float2 OctaDecode(float3 n)
{
	n /= dot(1.0f, abs(n));
	if (n.y <= 0.0f)
	{
		n.xz = (1 - abs(n.zx)) * (n.xz >= 0.0f ? 1.0f : -1.0f);
	}
	return n.xz;
}

float3 OctaEncode(float2 n)
{
	float3 forward = float3(n.x, 1.0f - abs(n.x) - abs(n.y), n.y);
	float t = clamp(-forward.y, 0.0f, 1.0f);
	forward.x = forward.x + (forward.x >= 0.0f ? -t : t);
	forward.z = forward.z + (forward.z >= 0.0f ? -t : t);
	return normalize(forward);
}

float3 HemiOctaEncode(float2 n)
{
	float tmpx = n.x;
	float tmpy = n.y;
	n.x = (tmpx + tmpy) * 0.5;
	n.y = (tmpx - tmpy) * 0.5;
	float3 forward = float3(n.x, 1.0 - abs(n.x) - abs(n.y), n.y);
	return normalize(forward);
}

float2 HemiOctaDecode(float3 n)
{
	n.xz /= dot(1.0, abs(n));
	return float2(n.x + n.z, n.x - n.z);
}


float3 OctaFrame3D(float2 frameUV)
{
	frameUV.x = frameUV.x * 2.0f - 1.0f;
	frameUV.y = frameUV.y * 2.0f - 1.0f;

	float3 octahedron;

	if (_FullOcta)
	{
		octahedron = OctaEncode(frameUV);

	}
	else
	{
		octahedron = HemiOctaEncode(frameUV);

	}

	return octahedron;
}


float3 toColor(float3 v)
{
	return (v + float3(1, 1, 1)) * 0.5;
}

float2 FrameToMosaique(float2 frameUV, float2 frameIdx)
{
	float newU = (frameUV.x + frameIdx.x) / _PxzImpostorFramesCount;
	float newV = (frameUV.y + frameIdx.y) / _PxzImpostorFramesCount;
	return float2(newU, newV);
}

float2 MosaiqueToFrame(float2 mos)
{
	float newU = frac(mos.x * _PxzImpostorFramesCount);
	float newV = frac(mos.y * _PxzImpostorFramesCount);
	return float2(newU, newV);
}

float2 getFrameIndex(float2 mosaiqueUv)
{
	//frames index
	float frameX = floor(mosaiqueUv.x * (_PxzImpostorFramesCount - 1.0));
	float frameY = floor(mosaiqueUv.y * (_PxzImpostorFramesCount - 1.0));
	return float2(frameX, frameY);
}


float DistanceToDepth(float3 objRayDir, float3 objCameraForward, float distance)
{
	float distanceToDepth = dot(-objRayDir, objCameraForward);
	float d = distance * distanceToDepth;
	if (UNITY_MATRIX_P[3][3] == 1) // Orthographic
	{
		d -= ORTHO_OFFSET;
	}
	return d;
}


float3 getObjCameraPos()
{
	float3 worldCameraPos;
	if (UNITY_MATRIX_P[3][3] == 1) // Orthographic
	{
		//if no perspective offset the cameraPos for rays to be parallel
		worldCameraPos = UNITY_MATRIX_I_V._m03_m13_m23 + mul(UNITY_MATRIX_I_V, float4(0, 0, ORTHO_OFFSET, 0));
	}
	else
	{
		worldCameraPos = UNITY_MATRIX_I_V._m03_m13_m23;
	}

	return mul(unity_WorldToObject, float4(worldCameraPos, 1.0)).xyz;
}

float2 RayPlaneIntersection(float planeOffset, float3 objNormal, float3 objRayDirection, float3 objRayOrigin, float4x4 objToFrame, out float distance)
{
	/**
		Rayon : P = rayOrigin + rayDirection * t
		Plan : P . N  = d
		https://math.stackexchange.com/questions/83990/line-and-plane-intersection-in-3d

	*/

	float rayDotNormal = dot(objRayDirection, objNormal);


	float normalDotOrigin = dot(objNormal, objRayOrigin);

	float t = (planeOffset - normalDotOrigin) / rayDotNormal;

	distance = t;

	float3 intersection = (objRayOrigin + objRayDirection * t); //Intersection in obj space


	float2 uvFrame = mul(objToFrame, float4(intersection, 1.0)).xy;

	return uvFrame;

}

float4x4 ObjToFrameMatrix(float2 currentFrameIndex, out float3 octaFrameNormal)
{
	float2 currentFrameMosaicUV = (currentFrameIndex / (_PxzImpostorFramesCount - 1.0));
	float3 centerOfFrame = OctaFrame3D(currentFrameMosaicUV);


	float3 up = float3(0, 1, 0);
	float3 Ox = normalize(cross(centerOfFrame, up) + float3(0.0001, 0.0, 0.0)); //offset a bit or less impostor disapears when directly seen from under 
	float3 Oy = normalize(cross(Ox, centerOfFrame));
	float3 Oz = normalize(centerOfFrame);

	float4x4 objToFrame = float4x4(float4(Ox, 0), float4(Oy, 0), float4(Oz, 0), float4(0, 0, 0, 1));

	float4x4 translate = float4x4(float4(1, 0, 0, 0.5 * _OctahedronDiameter), float4(0, 1, 0, 0.5 * _OctahedronDiameter), float4(0, 0, 1, 0), float4(0, 0, 0, 1));
	float4x4 scale = float4x4(float4(1 / _OctahedronDiameter, 0, 0, 0), float4(0, 1 / _OctahedronDiameter, 0, 0), float4(0, 0, 1 / 1, 0), float4(0, 0, 0, 1));
	float4x4 objToFrameFinal = mul(scale, mul(translate, objToFrame));

	octaFrameNormal = Oz;
	return objToFrameFinal;
}


float2 computeMosaicUVInFrame(float2 frameIndex, float3 objRayDir, float3 objCameraPos, out float distance)
{
	float fractionFrames = 1.0 / _PxzImpostorFramesCount;
	float3 octaFrameNormal;
	float4x4 objToFrame = ObjToFrameMatrix(frameIndex, octaFrameNormal);

	float2 mosaicUV = float2(-1, -1);
	float linear_it = _Linear;
	float binary_it = _Binary;
	float layerDepth = 1.0 / linear_it;

	float currentObjDepth = 0.0;
	float bestDepth = 0.0;

	//linear
	for (int iter = 0; iter < linear_it; ++iter)
	{
		currentObjDepth += layerDepth * _OctahedronDiameter;

		float planeOffset = _OctahedronDiameter * 0.5 - currentObjDepth;

		float2 frameUV = RayPlaneIntersection(planeOffset, octaFrameNormal, objRayDir, objCameraPos, objToFrame, distance);
		mosaicUV = FrameToMosaique(frameUV, frameIndex);

      float sampledDepth = tex2D(_PxzImpostorDepth, mosaicUV).r;

		if (sampledDepth * _OctahedronDiameter > currentObjDepth) //we are under the surface
			bestDepth = sampledDepth;

	}
	currentObjDepth = (bestDepth - layerDepth) * _OctahedronDiameter;
	//binary
	for (int i = 0; i < binary_it; ++i)
	{

		layerDepth *= 0.5;
		float planeOffset = _OctahedronDiameter * 0.5 - currentObjDepth;

		float2 frameUV = RayPlaneIntersection(planeOffset, octaFrameNormal, objRayDir, objCameraPos, objToFrame, distance);
		mosaicUV = FrameToMosaique(frameUV, frameIndex);

		float sampledDepth = tex2D(_PxzImpostorDepth, mosaicUV).r;

		if (sampledDepth * _OctahedronDiameter > currentObjDepth)
		{
			bestDepth = sampledDepth;
			currentObjDepth -= 2 * layerDepth * _OctahedronDiameter;
		}
		currentObjDepth += layerDepth * _OctahedronDiameter;
	}


	float planeOffset = _OctahedronDiameter * 0.5 - bestDepth * _OctahedronDiameter;
	float2 frameUV = RayPlaneIntersection(planeOffset, octaFrameNormal, objRayDir, objCameraPos, objToFrame, distance);
	mosaicUV = FrameToMosaique(frameUV, frameIndex);
	return mosaicUV;

}

// bounds: minX, maxX, minY, maxY
float4 getFrameBounds(float2 mosaiqueUV)
{
	float2 lowerIndex = getFrameIndex(mosaiqueUV);
	float2 upperIndex = lowerIndex + float2(1.0, 1.0);

	float2 minUV = lowerIndex / _PxzImpostorFramesCount;
	float2 maxUV = upperIndex / _PxzImpostorFramesCount;

	return float4 (minUV.x, maxUV.x, minUV.y, maxUV.y);
}

bool inFrame(float2 uv, float4 bounds)
{
	if (uv.x < bounds.x)
		return false;
	if (uv.x > bounds.y)
		return false;
	if (uv.y < bounds.z)
		return false;
	if (uv.y > bounds.w)
		return false;
	return true;
}


void PxzImpostorVertex(inout PxzImpostorData imp)
{
	float3 objCameraPos = getObjCameraPos();
	float3 objCameraDir = normalize(objCameraPos);

	float scale = _OctahedronDiameter;

	float3 upVector = float3(0.0, 1.0, 0.0);
	float3 horizontalVector = normalize(cross(objCameraDir, upVector));
	float3 verticalVector = cross(horizontalVector, objCameraDir);

	float3 billboard = imp.vertex.xyz;

	if (!_PreciseMesh)
	{
		float2 expand = imp.vertex.xy * scale; //scale the billboard
		billboard = horizontalVector * expand.x + verticalVector * expand.y; //orient the billboard
		imp.vertex.xyz = billboard;
	}

	float2 frameOcta2D;
	//Octa frame
	if (_FullOcta) {

		frameOcta2D = OctaDecode(objCameraDir.xyz) * 0.5 + 0.5;

	}
	else {
		objCameraDir.y = max(0.000001, objCameraDir.y);
		frameOcta2D = HemiOctaDecode(objCameraDir.xyz) * 0.5 + 0.5; //UV coords in mosaique
	}


	imp.localRayDir = billboard - objCameraPos;
	imp.frameOcta2D = frameOcta2D;
	imp.viewPos = UnityObjectToViewPos(billboard); //interpolated vertex position in view space

}


void PxzImpostorFragment(in PxzImpostorData imp,
						       out fixed4 blendedAlbedo,
                         out float4 blendedNormal,
                         out float blendedSmoothness,
                         out float blendedOcclusion,
                         out float3 blendedEmission,
                         out float outDepth,
                         out float3 worldPos)
                       
{
	float fractionFrames = 1.0 / _PxzImpostorFramesCount;
	float frameCountMinusOne = _PxzImpostorFramesCount - 1.0;
	float prevFractionFrames = 1.0 / (_PxzImpostorFramesCount - 1.0);


	float3 objCameraPos = getObjCameraPos();

	//per pixel rayDir
	float3 objRayDir = normalize(imp.localRayDir);

	float4 frameBounds = getFrameBounds(imp.frameOcta2D);


	//Frame1
	float2 previousFrame = imp.frameOcta2D * frameCountMinusOne;
	float2 frame1Index = floor(previousFrame);
	float distance1;
	float2 frame1UV = computeMosaicUVInFrame(frame1Index, objRayDir, objCameraPos, distance1);


	//Frame2
	float2 fracPart = frac(previousFrame);
	float2 cornerOffset = lerp(float2(0, 1), float2(1, 0), step(0, fracPart.x - fracPart.y));
	float2 frame2Index = frame1Index + cornerOffset;
	float distance2;
	float2 frame2UV = computeMosaicUVInFrame(frame2Index, objRayDir, objCameraPos, distance2);

	//Frame3
	float2 frame3Index = frame1Index + float2(1, 1);
	float distance3;
	float2 frame3UV = computeMosaicUVInFrame(frame3Index, objRayDir, objCameraPos, distance3);


	// Weights
	float2 invFracPart = 1 - fracPart;
	float3 weights;
	weights.x = min(invFracPart.x, invFracPart.y);
	weights.y = abs(fracPart.x - fracPart.y);
	weights.z = min(fracPart.x, fracPart.y);

	fixed4 albedo1 = tex2D(_PxzImpostorAlbedoOcclusion, frame1UV);
	fixed4 albedo2 = tex2D(_PxzImpostorAlbedoOcclusion, frame2UV);
	fixed4 albedo3 = tex2D(_PxzImpostorAlbedoOcclusion, frame3UV);

	float4 normal1 = tex2D(_PxzImpostorNormals, frame1UV);
	float4 normal2 = tex2D(_PxzImpostorNormals, frame2UV);
	float4 normal3 = tex2D(_PxzImpostorNormals, frame3UV);

	float smoothness1 = tex2D(_PxzImpostorSpecularRoughness, frame1UV).a;
	float smoothness2 = tex2D(_PxzImpostorSpecularRoughness, frame2UV).a;
	float smoothness3 = tex2D(_PxzImpostorSpecularRoughness, frame3UV).a;

	float3 emission1 = tex2D(_PxzImpostorEmission, frame1UV).rgb;
	float3 emission2 = tex2D(_PxzImpostorEmission, frame2UV).rgb;
	float3 emission3 = tex2D(_PxzImpostorEmission, frame3UV).rgb;

	float occlusion1 = tex2D(_PxzImpostorAlbedoOcclusion, frame1UV).a;
	float occlusion2 = tex2D(_PxzImpostorAlbedoOcclusion, frame2UV).a;
	float occlusion3 = tex2D(_PxzImpostorAlbedoOcclusion, frame3UV).a;


	blendedAlbedo = albedo1 * weights.x + albedo2 * weights.y + albedo3 * weights.z;

	blendedNormal = normal1 * weights.x + normal2 * weights.y + normal3 * weights.z;
	float3 localNormal = blendedNormal.rgb * 2.0 - 1.0;

	blendedNormal.rgb = mul(localNormal.rgb, (float3x3) unity_WorldToObject);

	if (!inFrame(frame1UV, frameBounds))
		blendedNormal.a = 0;


	blendedSmoothness = smoothness1 * weights.x + smoothness2 * weights.y + smoothness3 * weights.z;
	blendedOcclusion = occlusion1 * weights.x + occlusion2 * weights.y + occlusion3 * weights.z;
	blendedEmission = emission1 * weights.x + emission2 * weights.y + emission3 * weights.z;

	float3 objCameraForward = normalize(UNITY_MATRIX_IT_MV[2].xyz);
	float scale = length(unity_ObjectToWorld[2].xyz);

	float depth1 = DistanceToDepth(objRayDir, objCameraForward, distance1);
	float depth2 = DistanceToDepth(objRayDir, objCameraForward, distance2);
	float depth3 = DistanceToDepth(objRayDir, objCameraForward, distance3);

	float depth = (depth1 * weights.x + depth2 * weights.y + depth3 * weights.z) * scale;

#if defined(SHADOWS_DEPTH)
	if (unity_LightShadowBias.y > 0.0)
	{
		imp.viewPos.z += -_ShadowBias;
	}
	else
	{
		imp.viewPos.z = -depth;
	}
#else
	imp.viewPos.z = -depth;
#endif

	float4 clipPos = mul(UNITY_MATRIX_P, float4(imp.viewPos, 1.0));
    worldPos = mul(UNITY_MATRIX_I_V, float4(imp.viewPos, 1.0));

#if defined(SHADOWS_DEPTH)
	clipPos = UnityApplyLinearShadowBias(clipPos);
#endif


	float NDCPosZ = (clipPos.z / clipPos.w); //Perspective division

	if (UNITY_NEAR_CLIP_VALUE < 0)
		NDCPosZ = NDCPosZ * 0.5 + 0.5;

	outDepth = NDCPosZ;
    imp.viewPos.xy = clipPos.xy; // re use viewPos because it is not needed further and max interpolator = 10 for ForwardBase Pass
}



#endif