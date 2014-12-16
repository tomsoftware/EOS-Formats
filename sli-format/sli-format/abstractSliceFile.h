#ifndef ABSTRACT_SLICE_FILE_H
#define ABSTRACT_SLICE_FILE_H

#include "abstractSliceFile.h"

class abstractSliceFile
{
public:

	/// <summary>Read and interpret slice-data from File</summary>
	/// <param name="FileName">Filename to read from</param>
	/// <returns>true on success; false on not</returns>
	virtual bool readFromFile(const char * filename) = 0;

	/// <summary>Read the slice data and add it to the sliceData class to the PartIndex index</summary>
	/// <param name="sliceData">sliceData class</param>
	/// <param name="PartIndex">index of the Part to read</param>
	/// <param name="LayerIndex">index of the layer to read</param>
	/// <param name="storeAsPartIndex">[optional] Index in [sliceData] to store the data in, use -1 for same as PartIndex</param>
	/// <returns>true on success; false on not</returns>
	virtual bool readSliceData(clSliceData * sliceData, int PartIndex, int LayerIndex, int storeAsPartIndex = -1) = 0;

	/// <summary>returns the Layer Count of the file</summary>
	/// <param name="PartIndex">index of the Part</param>
	virtual int getLayerCount(int PartIndex) = 0;

	/// <summary>returns the Layer position in [mm] of the sellected part</summary>
	/// <param name="PartIndex">index of the Part</param>
	/// <param name="layerIndex">index of the Layer</param>
	virtual float getLayerPos(int PartIndex, int layerIndex) = 0;

	/// <summary>returns the name of the part</summary>
	/// <param name="PartIndex">index of the Part</param>
	virtual char * getPartName(int PartIndex) = 0;


	/// <summary>returns a string with special propertys to use for this part</summary>
	/// <param name="PartIndex">index of the Part</param>
	virtual char * getPartProperty(int PartIndex) = 0;
	


	/// <summary>returns the number of parts in this file</summary>
	virtual int getPartCount() = 0;
};


#endif