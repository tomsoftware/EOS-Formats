#include "clSliceData.h"

//- Math macros -//
#define ABS(a) (((a)>=0)?(a):(-(a)))
#define ROUND(x) floor((x)+0.5)
#define MIN(a,b) (((a)<(b))?(a):(b))
#define MAX(a,b) (((a)>(b))?(a):(b))
#define SWAP(a, b)	{ int swaptmp = (b); (b) = (a); (a) = swaptmp; }


//---------------------------------------------------//
clSliceData::clSliceData()
{
	m_partLenght = 0;
	m_partCount = 0;
	m_parts = NULL;

	m_TransformedPoints = NULL;
	m_TransformedPointsLenght = 0;
}

//---------------------------------------------------//
clSliceData::~clSliceData()
{
	m_TransformedPointsLenght = 0;
	if (m_TransformedPoints != NULL) delete[]m_TransformedPoints;

	if (m_parts != NULL)
	{
		for (int p = m_partLenght - 1; p >= 0; p--)
		{
			if (m_parts[p].objects != NULL)
			{
				for (int o = m_parts[p].objectLenght - 1; o >= 0; o--)
				{
					delete [] m_parts[p].objects[o].points;

					m_parts[p].objects[o].points = NULL;
					m_parts[p].objects[o].pointLenght = 0;
				}

				delete [] m_parts[p].objects;
			}

			m_parts[p].objects = NULL;
			m_parts[p].objectLenght = 0;
		}

		delete [] m_parts;
	}

	m_partCount = 0;
	m_partLenght = 0;
	m_parts = NULL;
	
}

//---------------------------------------------------//
int clSliceData::getPartCount()
{
	return m_partCount;
}

//---------------------------------------------------//
int clSliceData::getObjectCount(int partIndex)
{
	if ((partIndex < 0) || (partIndex >= m_partCount)) return 0;
	return m_parts[partIndex].objectCount;
}

//---------------------------------------------------//
bool clSliceData::isPolygon(int partIndex, int objectIndex)
{
	if ((partIndex < 0) || (partIndex >= m_partCount)) return false;
	if ((objectIndex < 0) || (objectIndex >= m_parts[partIndex].objectCount)) return false;
	return !m_parts[partIndex].objects[objectIndex].isHatch;
}


//---------------------------------------------------//
bool clSliceData::isHatch(int partIndex, int objectIndex)
{
	if ((partIndex < 0) || (partIndex >= m_partCount)) return false;
	if ((objectIndex < 0) || (objectIndex >= m_parts[partIndex].objectCount)) return false;
	return m_parts[partIndex].objects[objectIndex].isHatch;
}


//---------------------------------------------------//
float * clSliceData::getObjectPoints(int partIndex, int objectIndex)
{
	if ((partIndex < 0) || (partIndex >= m_partCount)) return NULL;
	if ((objectIndex < 0) || (objectIndex >= m_parts[partIndex].objectCount)) return NULL;
	return m_parts[partIndex].objects[objectIndex].points;
}


//---------------------------------------------------//
float * clSliceData::getObjectPointsTransformed(int partIndex, int objectIndex, tyMatrix matrix)
{
	if ((partIndex < 0) || (partIndex >= m_partCount)) return NULL;
	if ((objectIndex < 0) || (objectIndex >= m_parts[partIndex].objectCount)) return NULL;

	float * p = m_parts[partIndex].objects[objectIndex].points;
	int count = m_parts[partIndex].objects[objectIndex].pointCount;
	
	//- create buffer
	if (m_TransformedPointsLenght < count)
	{
		m_TransformedPointsLenght = count + 32;
		if (m_TransformedPoints != NULL) delete []m_TransformedPoints;

		m_TransformedPoints = new float[m_TransformedPointsLenght*2];
	}

	//- get transformations matrix
	tyMatrix newMatrix;
	MatrixMult(&newMatrix, &matrix, &m_parts[partIndex].transformMatrix);

	float m11 = newMatrix.m11;
	float m12 = newMatrix.m12;
	float m13 = newMatrix.m13;
	float m21 = newMatrix.m21;
	float m22 = newMatrix.m22;
	float m23 = newMatrix.m23;


	float *pPointOut = m_TransformedPoints;
	float *pPointIn = p;

	for (int i = count; i > 0; i--)
	{
		float x = *pPointIn;
		pPointIn++;
		float y = *pPointIn;
		pPointIn++;

		*pPointOut = x * m11 + y * m12 + 1 * m13;
		pPointOut++;
		*pPointOut = x * m21 + y * m22 + 1 * m23;
		pPointOut++;
	}

	return m_TransformedPoints;
}


//---------------------------------------------------//
bool clSliceData::drwaRasteredObject(int *outFilledPicture, int * outLinePicture, int partIndex, tyMatrix matrix, int color, int weight, int height)
{
	if ((partIndex < 0) || (partIndex >= m_partCount)) return false;
	if ((outFilledPicture == NULL) && (outLinePicture == NULL)) return false;

	int objectCount = m_parts[partIndex].objectCount;


	//---------------------------------
	//- First draw all poly-lines

	//- do this for all Objects of this part
	for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
	{
		int PointCount = m_parts[partIndex].objects[objectIndex].pointCount;
		if (PointCount < 1) continue;

		//- disable output for Hatches
		if (!m_parts[partIndex].objects[objectIndex].isHatch)
		{
			//- transform data
			float * points = getObjectPointsTransformed(partIndex, objectIndex, matrix);
			float * pPoint = points;

			float x1;
			float x2;
			float y1;
			float y2;

			x1 = x2 = *pPoint++;
			y1 = y2 = *pPoint++;

			for (int i = PointCount - 1; i > 0; i--)
			{
				x2 = *pPoint++;
				y2 = *pPoint++;

				//- draw polygon line
				if (outLinePicture != NULL) drawLine(outLinePicture, weight, height, ROUND(x1), ROUND(y1), ROUND(x2), ROUND(y2), color);

				//- add points for add Edge flag algorithm
				if (outFilledPicture != NULL) addEdgeflag(outFilledPicture, weight, height, ROUND(x1), ROUND(y1), ROUND(x2), ROUND(y2), color);

				x1 = x2;
				y1 = y2;
			}
		}
	}

	//- fill Object
	if (outFilledPicture != NULL) fillEdgePoly(outFilledPicture, weight, height, color);
	


	//---------------------------------
	//- draw all hatches


	//- do this for all Objects of this part
	for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
	{
		int PointCount = m_parts[partIndex].objects[objectIndex].pointCount;
		if (PointCount < 1) continue;

		//- only Hatches
		if (m_parts[partIndex].objects[objectIndex].isHatch)
		{
			//- transform data
			float * points = getObjectPointsTransformed(partIndex, objectIndex, matrix);
			float * pPoint = points;

			for (int i = (PointCount>>2); i > 0; i--)
			{
				float x1 = *pPoint++;
				float y1 = *pPoint++;
				float x2 = *pPoint++;
				float y2 = *pPoint++;

				//- draw hatch lines
				if (outLinePicture != NULL) drawLine(outLinePicture, weight, height, x1, y1, x2, y2, color);
			}
		}
	}


	return true;
}



//---------------------------------------------------//
void clSliceData::IdentityMatrix(tyMatrix * dest)
{
	dest->m11 = 1; dest->m12 = 0; dest->m13 = 0;
	dest->m21 = 0; dest->m22 = 1; dest->m23 = 0;
}


//---------------------------------------------------//
void clSliceData::MatrixMult(tyMatrix * dest, tyMatrix* A, tyMatrix *B)
{
	dest->m11 = A->m11 * B->m11 + A->m12 * B->m21 + A->m13 * 0;
	dest->m12 = A->m11 * B->m12 + A->m12 * B->m22 + A->m13 * 0;
	dest->m13 = A->m11 * B->m13 + A->m12 * B->m23 + A->m13 * 1;

	dest->m21 = A->m21 * B->m11 + A->m22 * B->m21 + A->m23 * 0;
	dest->m22 = A->m21 * B->m12 + A->m22 * B->m22 + A->m23 * 0;
	dest->m23 = A->m21 * B->m13 + A->m22 * B->m23 + A->m23 * 1;

	return;
}

//---------------------------------------------------//
bool clSliceData::clearParts(int partCount)
{
	for (int i = 0; i < m_partLenght; i++)
	{
		m_parts[i].objectCount = 0;
		m_parts[i].transformMatrix = { 0, 0, 0, 0, 0, 0 };

		for (int j = 0; j < m_parts[i].objectLenght; j++)
		{
			m_parts[i].objects[j].pointCount = 0;
		}
	}

	createObject(partCount, 0, 0, 0, 0, 0, 0);
	m_partCount = 0;

	return true;
}


//---------------------------------------------------//
bool clSliceData::createObject(int partIndex, float m11, float m12, float  m13, float  m21, float  m22, float  m23)
{
	//- increase buffer size
	if (m_partLenght <= partIndex)
	{
		int newLenght = partIndex + 10;
		tyPart * newObj = new tyPart[newLenght];

		//- copy old objects to new object
		for (int i = 0; i < m_partLenght; i++)
		{
			newObj[i] = m_parts[i];
		}

		//- define new objects
		for (int i = m_partLenght; i < newLenght; i++)
		{
			newObj[i].objectCount = 0;
			newObj[i].objectLenght = 0;
			newObj[i].objects = NULL;
			newObj[i].transformMatrix = { 0, 0, 0, 0, 0, 0 };
		}

		if (m_parts != NULL) delete [] m_parts;
		m_parts = newObj;

		m_partLenght = newLenght;
	}

	m_parts[partIndex].objectCount = 0;
	m_parts[partIndex].transformMatrix = { m11, m12, m13, m21, m22, m23 };

	m_partCount = MAX(m_partCount, partIndex + 1);

	return true;
}


//---------------------------------------------------//
float * clSliceData::createPolygon(int partIndex, int pointCount)
{
	tyObject * newPoly = createNewObject(partIndex, pointCount);

	newPoly->isHatch = false;

	return newPoly->points;

}


//---------------------------------------------------//
float * clSliceData::createHatch(int partIndex, int lineCount)
{
	tyObject * newPoly = createNewObject(partIndex, lineCount * 2);

	newPoly->isHatch = true;

	return newPoly->points;
}


//---------------------------------------------------//
clSliceData::tyObject * clSliceData::createNewObject(int partIndex, int coordinatesCount)
{
	if (partIndex >= m_partCount) return NULL;
	
	tyPart * ob = &m_parts[partIndex];

	int chordCount = coordinatesCount;
	int polyIndex = ob->objectCount;

	if (ob->objectLenght <= polyIndex)
	{
		ob->objectLenght = polyIndex + 10;
		tyObject * newPoly = new tyObject[ob->objectLenght];

		//- copy old objects to new object
		for (int i = 0; i < ob->objectCount; i++)
		{
			newPoly[i] = ob->objects[i];
		}

		//- define new polylines
		for (int i = ob->objectCount; i < ob->objectLenght; i++)
		{
			newPoly[i].isHatch = false;
			newPoly[i].points = NULL;
			newPoly[i].pointCount = 0;
			newPoly[i].pointLenght = 0;
		}

		if (ob->objects != NULL) delete[] ob->objects;
		ob->objects = newPoly;
	}
	
	m_parts[partIndex].objectCount = polyIndex+1;

	//- prepare new polyline
	tyObject * newPoly = &ob->objects[polyIndex];
	newPoly->isHatch = false;
	
	if (newPoly->pointLenght < chordCount)
	{
		newPoly->pointLenght = chordCount + 32;
		if (newPoly->points != NULL) delete [] newPoly->points;
		newPoly->points = new float[newPoly->pointLenght*2];
	}

	newPoly->pointCount = chordCount;

	return newPoly;
}



//-------------------------------------------------------//
int clSliceData::drawLine(int * DataDest, int width, int height, int x1, int y1, int x2, int y2, int color)
{
	if (DataDest == 0) return -1;

	//- Ignore lines out of the [DataDest]
	if (((x1 < 0) && (x2 < 0)) || ((y1 < 0) && (y2 < 0))) return 0;
	if (((x1 >= width) && (x2 >= width)) || ((y1 >= height) && (y2 >= height))) return 0;

	//- ignore short lines 
	int abs_w = ABS(x2 - x1);
	int abs_h = ABS(y2 - y1);

	if ((abs_w == 0) && (abs_h == 0)) return -6;
	
	int pixelCount = width*height;

	if (abs_w > abs_h) //- this is a horizontal line
	{
		if (x2 < x1)
		{
			SWAP(x1, x2);
			SWAP(y1, y2);
		}

		float d = (double) (y2 - y1) / (x2 - x1); //- Slope

		int tmpX = x1;
		float currentY = y1;

		for (int i = abs_w; i >= 0 ; i--)
		{
			int index = ROUND(currentY) * width + tmpX;

			if ((index >= 0) && (index < pixelCount))
			{			
				//-- DataDest[tmpX,tmpY]
				if ((tmpX > 0) && (tmpX < width)) 
					*(DataDest + index) = color;
			}

			tmpX++;
			currentY += d;
		}
		

	}
	else  //- this is a vertical line
	{
		if (y2 < y1)
		{
			SWAP(x1, x2);
			SWAP(y1, y2);
		}

		float d = (double) (x2 - x1) / (y2 - y1); //- Slope

		float currentX = x1;
		int tmpY = y1 * width;

		for (int i = abs_h; i >= 0; i--)
		{
			int tmpX = ROUND(currentX);
			int index = tmpY + tmpX;

			if ((index >= 0) && (index < pixelCount))
			{
				//-- DataDest[tmpX,tmpY]
				if ((tmpX > 0) && (tmpX < width)) 
					*(DataDest + index) = color;
			}

			currentX += d;
			tmpY += width;
		}

	}


	return 1;
}



//-------------------------------------------------------//
int clSliceData::addEdgeflag(int * DataDest, int width, int height, int x1, int y1, int x2, int y2, int color)
{
	if (DataDest == 0) return -1;

	//- Quelle: http://hullooo.blogspot.de/2011/02/solid-area-scan-conversion.html
	int plotx, ploty;
	double slope_inv, xintersection, yintersection;

	int img_w = width;
	int img_h = height;

	if (y1 == y2) return -5;  // leave horizontal lines


	if (y1 > y2)
	{
		SWAP(x1, x2);
		SWAP(y1, y2);
	}

	//- contour outline
	int max_y2 = MIN(y2, img_h);

	for (ploty = MAX(y1, 0); ploty < max_y2; ploty++)
	{
		slope_inv = (double) (x1 - x2) / (y1 - y2); //- slop

		yintersection = ploty + 0.5;
		xintersection = x1 + slope_inv * (yintersection - y1);

		plotx = (int) floor(xintersection);
		if (plotx + 0.5 <= xintersection) plotx++;

		// marking the border pixel for each scan line intersecting an edge
		if (plotx < 0) plotx = 0;
		if (plotx >= img_w) plotx = img_w - 1;

		//-- DataDest[ploty,plotx]
		int * piont = DataDest + ploty*width + plotx;

		*piont = (*piont != color) ? color : 0;

	}

	return 0;

}


//-------------------------------------------------------//
int clSliceData::fillEdgePoly(int * DataDest, int width, int height, int color)
{
	if (DataDest == 0) return -1;


	bool state = false;

	int * outP = DataDest;

	//- m_sliceMask füllem
	for (int y = height; y > 0; y--)
	{
		state = false;

		for (int x = width; x > 0; x--)
		{
			if (*outP == color) state = !state;
			if (state) *outP = color;
			outP++;
		}
	}

	return 1;
}


//-------------------------------------------------------//
int clSliceData::fillEdgePolyROI(int * DataDest, int width, int height, int min_x, int min_y, int max_x, int max_y, int color)
{
	if (DataDest == 0) return -1;

	//- Fläche füllen
	int x_pos = min_x;
	bool state = false;

	int * outP = DataDest;

	//- m_sliceMask füllem
	for (int y = min_y; y < max_y; y++)
	{
		state = false;
		outP = DataDest + width * y + min_x;

		for (int x = min_x; x < max_x; x++)
		{
			if (*outP == color) state = !state;
			if (state) *outP = color;
			outP++;
		}
	}

	return 1;
}
