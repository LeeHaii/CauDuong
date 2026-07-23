// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_GEOM_INTERFACE_GEOMINTERFACE_C_H_
#define _PXZ_GEOM_INTERFACE_GEOMINTERFACE_C_H_

#include "GeomTypes_c.h"

PXZ_EXPORTED char * Geom_getLastError();

// Apply a transformation matrix to a geometrical entity
PXZ_EXPORTED void Geom_applyTransform(Geom_GeomEntity entity, Geom_Matrix4 matrix);
// Retrieve the Axis-Aligned Bounded Box of a geometric entity
PXZ_EXPORTED Geom_AABB Geom_getEntityAABB(Geom_GeomEntity entity);

// ----------------------------------------------------
// Math
// ----------------------------------------------------

// Construct a Change of Basis Matrix (e.g multiplying the point [0,0,0] will result to the point origin)
PXZ_EXPORTED Geom_Matrix4 Geom_changeOfBasisMatrix(Geom_Point3 origin, Geom_Vector3 x, Geom_Vector3 y, Geom_Vector3 z);
// Create a Matrix from an origin and a normal vector
PXZ_EXPORTED Geom_Matrix4 Geom_fromOriginNormal(Geom_Point3 origin, Geom_Vector3 normal);
// Create a Matrix from translation, rotation and scaling vectors
PXZ_EXPORTED Geom_Matrix4 Geom_fromTRS(Geom_Vector3 T, Geom_Vector3 R, Geom_Vector3 S);
// Invert a matrix
PXZ_EXPORTED Geom_Matrix4 Geom_invertMatrix(Geom_Matrix4 matrix);
// Multiply two matrices, returns left*right
PXZ_EXPORTED Geom_Matrix4 Geom_multiplyMatrices(Geom_Matrix4 left, Geom_Matrix4 right);
// Multiply a point by a matrix (i.e apply the matrix to a point)
PXZ_EXPORTED Geom_Point3 Geom_multiplyMatrixPoint(Geom_Matrix4 matrix, Geom_Point3 point);
// Multiply a vector by a matrix (i.e apply the matrix to a vector)
PXZ_EXPORTED Geom_Vector3 Geom_multiplyMatrixVector(Geom_Matrix4 matrix, Geom_Vector3 vector);
// Decompose a Matrix into translation, rotation and scaling vectors
PXZ_EXPORTED Geom_Vector3List Geom_toTRS(Geom_Matrix4 matrix);



#endif
