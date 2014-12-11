#ifndef MAIN_H
#define MAIN_H

#include "clSliFile.h"
#include "clSliceData.h"

#define MAGIC ('S' | ('l'<<8) | ('i'<<16) | (254<24))

struct tyLibraryInterface
{
	int magic;
	clSliFile * sliFile;
	clSliceData * sliceData;
};


//---- DLL EXPORTS ------//
#define DllExport extern "C" __declspec(dllexport)

DllExport int sf_initLib();
DllExport void sf_freeLib(int sliI);
DllExport int sf_readFromFile(int sliI, char * fileName);
DllExport int sf_getLayerCount(int sliI, int partIndex);
DllExport int sf_readSliceData(int sliI, int partIndex, int layerIndex);
DllExport int sf_addRasterObject(int sliI, int * outFilledPicture, int * outLinePicture, int partIndex, clSliceData::tyMatrix matrix, int color, int weight, int height);
DllExport int sf_getPartCount(int sliI);
DllExport float sf_getLayerPos(int sliI, int partIndex, int layerIndex);

#endif
