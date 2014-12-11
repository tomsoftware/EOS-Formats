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

		bool clearParts(int partCount);

		bool createObject(int partIndex, float m11, float m12, float  m13, float  m21, float  m22, float  m23);

		float * createPolygon(int partIndex, int PointCount);
		float * createHatch(int partIndex, int LineCount);

		bool isPolygon(int partIndex, int objectIndex);
		bool isHatch(int partIndex, int objectIndex);


		int getPartCount();
		int getObjectCount(int partIndex);

		float * getObjectPoints(int partIndex, int objectIndex);

		float * getObjectPointsTransformed(int partIndex, int objectIndex, tyMatrix matrix);

		bool drwaRasteredObject(int * outFilledPicture, int * outLinePicture, int partIndex, tyMatrix matrix, int color, int weight, int height);

		static void MatrixMult(tyMatrix * dest, tyMatrix* A, tyMatrix *B);
		static void IdentityMatrix(tyMatrix * dest);


		static int fillEdgePoly(int  * DataDest, int width, int height, int color);
		static int fillEdgePolyROI(int * DataDest, int width, int height, int min_x, int min_y, int max_x, int max_y, int color);
		static int addEdgeflag(int  * DataDest, int width, int height, int x1, int y1, int x2, int y2, int color);
		static int drawLine(int  * DataDest, int width, int height, int x1, int y1, int x2, int y2, int color);
};


#endif