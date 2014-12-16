#ifndef CLJOBSLICEFILE_H
#define CLJOBSLICEFILE_H

#include "clSliceData.h"
#include "abstractSliceFile.h"
#include "clSliFile.h"
#include "clJobFileInterpreter.h"
#include "clFile.h"
#include <math.h>       // cos, sin

#define PI 3.14159265

class clJobSliceFile : public abstractSliceFile
{

public:
	clJobSliceFile();
	~clJobSliceFile();

	/// <summary>Read and interpret job-data from File</summary>
	/// <param name="FileName">Filename of File to read</param>
	/// <returns>true on success; false on not</returns>
	bool readFromFile(const char * filename);

	/// <summary>Read the slice data and add it to the sliceData class</summary>
	/// <param name="sliceData">sliceData class</param>
	/// <param name="PartIndex">index of the Part</param>
	/// <param name="LayerIndex">index of the layer</param>
	/// <param name="storeAsPartIndex">[optional] Index in [sliceData] to store the data in, use -1 for same as PartIndex</param>
	/// <returns>true on success; false on not</returns>
	virtual bool readSliceData(clSliceData * sliceData, int PartIndex, int LayerIndex, int storeAsPartIndex=-1);

	/// <summary>returns the Layer Count of the file</summary>
	/// <param name="PartIndex">index of the Part</param>
	int getLayerCount(int PartIndex);

	/// <summary>returns the Layer position in [mm] of the sellected part</summary>
	/// <param name="PartIndex">index of the Part</param>
	/// <param name="layerIndex">index of the Layer</param>
	float getLayerPos(int PartIndex, int layerIndex);


	/// <summary>returns the name of the part</summary>
	/// <param name="PartIndex">index of the Part</param>
	char * getPartName(int PartIndex);

	/// <summary>returns a string with special propertys to use for this part</summary>
	/// <param name="PartIndex">index of the Part</param>
	char * getPartProperty(int PartIndex);

	/// <summary>returns the number of parts in this file</summary>
	int getPartCount();

private:

	struct tySliFile
	{
		char fileName[255];
		char partName[40];
		char exposureProfile[40];
		clSliceData::tyMatrix matrix;
		clSliFile sliFile;
	};

	tySliFile * m_SliFiles;
	int m_SliFilesCount;
	
	clError m_error;

	bool openPartFile(tySliFile * part, const char * jobFileName);
	char * strCopy(char * dest, const char * src, int maxCopyCount);
	int strIndexOfLast(const char * src, char findChar, int maxScanCount);
};



#endif