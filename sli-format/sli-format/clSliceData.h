#ifndef CL_SLICEDATA_H
#define CL_SLICEDATA_H

#include "clError.h"
#include <math.h>





/// Container Object for polygons and hatches
/// [Part]->[Object]->[Polygon or Hatch]
class clSliceData
{
	public:
		struct tyMatrix
		{
			float m11, m12, m13;
			float m21, m22, m23;
		};


	private:

		struct tyObject
		{
			float * points;
			int pointCount;
			int pointLenght;
			bool isHatch;
		};

		struct tyPart
		{
			tyObject * objects;
			int objectCount;
			int objectLenght;
			tyMatrix transformMatrix;
		};


		tyPart * m_parts;
		int m_partCount;
		int m_partLenght;

		tyObject * createNewObject(int partCount, int coordinatesCount);

		float * m_TransformedPoints;
		int m_TransformedPointsLenght;

	public:
		clSliceData();
		~clSliceData();

		/// <summary>reset the internal structure for part [partIndex]</summary>
		/// <param name="AllocatePartCount">[optional] Number of Parts to define</param>
		bool clearParts(int AllocatePartCount=0);

		/// <summary>create a new part with the index [partIndex]</summary>
		/// <param name="partIndex">index of the Part</param>
		/// <param name="m11">transformations matrix [m11] [m12] [m13]</param>
		/// <param name="m21">transformations matrix [m21] [m22] [m23]</param>
		bool createPart(int partIndex, float m11, float m12, float  m13, float  m21, float  m22, float  m23);

		/// <summary>define a new polygon/polyline with for the part [partIndex]</summary>
		/// <param name="partIndex">index of the Part</param>
		/// <param name="PointCount">number of points (2 floats per point)</param>
		/// <return>pointer to buffer to store data in (size: 2*[PointCount]*sizeof(float))</return>
		float * createPolygon(int partIndex, int PointCount);

		/// <summary>define a new hatch with for the part [partIndex]</summary>
		/// <param name="partIndex">index of the Part</param>
		/// <param name="LineCount">number of Lines (4 floats per line)</param>
		/// <return>pointer to buffer to store data in (size: 4*[LineCount]*sizeof(float))</return>
		float * createHatch(int partIndex, int LineCount);

		/// <summary>is this object of a Part a Polygon/Polyline (2 Floats per Point)</summary>
		/// <param name="partIndex">index of the Part</param>
		/// <param name="objectIndex">index of the polyline or Hatch of the part</param>
		bool isPolygon(int partIndex, int objectIndex);


		/// <summary>is this object of a Part a Hatch (4 Floats per Line)</summary>
		/// <param name="partIndex">index of the Part</param>
		/// <param name="objectIndex">index of the polyline or Hatch of the part</param>
		bool isHatch(int partIndex, int objectIndex);

		/// <summary>returnes the number of parts</summary>
		int getPartCount();

		/// <summary>returnes the number of objects of a part</summary>
		/// <param name="partIndex">index of the Part</param>
		int getObjectCount(int partIndex);

		/// <summary>returnes the coordinates of the part object [objectIndex]</summary>
		/// <param name="partIndex">index of the Part</param>
		/// <param name="objectIndex">index of the polyline or Hatch of the part</param>
		float * getObjectPoints(int partIndex, int objectIndex);


		/// <summary>returnes the transformed coordinates for the part object [objectIndex]</summary>
		/// <param name="partIndex">index of the Part</param>
		/// <param name="objectIndex">index of the polyline or Hatch of the part</param>
		/// <param name="matrix">transfrom matrix to apply to the part</param>
		float * getObjectPointsTransformed(int partIndex, int objectIndex, tyMatrix matrix);

		/// <summary>rasters/renders a part layer to a INT Array</summary>
		/// <param name="outFilledPicture">INT Array to render the part filled with [color] - can be NULL</param>
		/// <param name="outLinePicture">INT Array to render the part conture with [color] - can be NULL</param>
		/// <param name="partIndex">index of the Part</param>
		/// <param name="matrix">transfrom matrix to apply to the part</param>
		/// <param name="color">Value to use for rendering</param>
		/// <param name="width">width of [outFilledPicture] and [outLinePicture]</param>
		/// <param name="height">height of [outFilledPicture] and [outLinePicture]</param>
		bool drawRasteredObject(int * outFilledPicture, int * outLinePicture, int partIndex, tyMatrix matrix, int color, int widht, int height);


		/// <summary>applys a matrix to the current transfrom matrix of the part</summary>
		/// <param name="partIndex">index of the Part</param>
		/// <param name="matrix">matrix to apply</param>
		void PartMatrixMult(int partIndex, tyMatrix matrix);


		static void IdentityMatrix(tyMatrix * dest);
		static void MatrixMult(tyMatrix * dest, tyMatrix A, tyMatrix B);


		static int fillEdgePoly(int  * DataDest, int width, int height, int color);
		static int fillEdgePolyROI(int * DataDest, int width, int height, int min_x, int min_y, int max_x, int max_y, int color);
		static int addEdgeflag(int  * DataDest, int width, int height, int x1, int y1, int x2, int y2, int color);
		static int drawLine(int  * DataDest, int width, int height, int x1, int y1, int x2, int y2, int color);
};



#endif