// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_GEOM_INTERFACE_GEOMINTERFACE_H_
#define _PXZ_GEOM_INTERFACE_GEOMINTERFACE_H_

#include "GeomTypes.h"

PXZ_MODULE_BEGIN(GeomI)

class PXZ_EXPORTED GeomInterface
{
public:
   //! Apply a transformation matrix to a geometrical entity
   /*!
     \param entity The geometric entity
     \param matrix The transformation matrix
   */
   static void applyTransform(const GeomEntity & entity, const Matrix4 & matrix);

   //! Retrieve the Axis-Aligned Bounded Box of a geometric entity
   /*!
     \param entity The geometric entity
     \return aabb The axis aligned bounded box
   */
   static AABB getEntityAABB(const GeomEntity & entity);


   /**
    * \defgroup Math 
    * @{
    */
   //! Construct a Change of Basis Matrix (e.g multiplying the point [0,0,0] will result to the point origin)
   /*!
     \param origin Origin of the new basis
     \param x X axis of the new basis
     \param y Y axis of the new basis
     \param z Z axis of the new basis
     \return changeOfBasis The change of basis matrix
   */
   static Matrix4 changeOfBasisMatrix(const Point3 & origin, const Vector3 & x, const Vector3 & y, const Vector3 & z);

   //! Create a Matrix from an origin and a normal vector
   /*!
     \param origin The origin point
     \param normal The normal vector
     \return matrix The created Matrix
   */
   static Matrix4 fromOriginNormal(const Point3 & origin, const Vector3 & normal);

   //! Create a Matrix from translation, rotation and scaling vectors
   /*!
     \param T The translation vector
     \param R The rotations vector
     \param S The scaling vector
     \return matrix The created Matrix
   */
   static Matrix4 fromTRS(const Vector3 & T, const Vector3 & R, const Vector3 & S);

   //! Invert a matrix
   /*!
     \param matrix The matrix to invert
     \return inverted The inverted matrix
   */
   static Matrix4 invertMatrix(const Matrix4 & matrix);

   //! Multiply two matrices, returns left*right
   /*!
     \param left Left side matrix
     \param right Right side matrix
     \return result Result of the matrices multiplication
   */
   static Matrix4 multiplyMatrices(const Matrix4 & left, const Matrix4 & right);

   //! Multiply a point by a matrix (i.e apply the matrix to a point)
   /*!
     \param matrix The matrix to apply
     \param point The point to multiply
     \return result The resulting point
   */
   static Point3 multiplyMatrixPoint(const Matrix4 & matrix, const Point3 & point);

   //! Multiply a vector by a matrix (i.e apply the matrix to a vector)
   /*!
     \param matrix The matrix to apply
     \param vector The vector to multiply
     \return result The resulting point
   */
   static Vector3 multiplyMatrixVector(const Matrix4 & matrix, const Vector3 & vector);

   //! Decompose a Matrix into translation, rotation and scaling vectors
   /*!
     \param matrix The Matrix to be decomposed
     \return TRS The TRS list
   */
   static Vector3List toTRS(const Matrix4 & matrix);

   /**@}*/

};

PXZ_MODULE_END



#endif
