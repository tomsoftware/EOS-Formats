#include "export_lib_sli.h"


//------------------------------------------------------------//
int strIndexOfLast(const char * src, char findChar, int maxScanCount)
{
	if (src == NULL) return -1;

	const char * srcP = src;

	int pos = -1;

	for (int i = 0; i<maxScanCount; i++)
	{
		char s = *srcP++;
		if (s == 0) return pos;
		if (s == findChar) pos = i;
	}
	return pos;
}



//---------------------------------------------------//
//-- library interface--//
//---------------------------------------------------//
int sf_initLib()
{

	tyLibraryInterface *libInt = new tyLibraryInterface;

	libInt->magic = MAGIC_SLI;
	libInt->sliFile = NULL;
	libInt->sliceData = new clSliceData();

	return (int) libInt;
}

//---------------------------------------------------//
void sf_freeLib(int sliI)
{
	if (sliI != NULL)
	{
		tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
		if (libInt->magic == MAGIC_SLI)
		{
			if (libInt->sliFile != NULL) delete libInt->sliFile;
			if (libInt->sliceData != NULL) delete libInt->sliceData;

			libInt->magic = 0;
			libInt->sliFile = NULL;
			libInt->sliceData = NULL;
		}
	}
}

//---------------------------------------------------//
int sf_readFromFile(int sliI, char * fileName)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC_SLI) return -2;

	if (libInt->sliFile != NULL) delete libInt->sliFile;
	libInt->sliFile = NULL;

	int pos = strIndexOfLast(fileName, '.', 1024);
	if (pos < 1) return -3;

	//- select the right format by file extention
	if (_stricmp(&fileName[pos + 1], "sli") == 0)
	{
		libInt->sliFile = new clSliFile();
	}
	else if (_stricmp(&fileName[pos + 1], "job") == 0)
	{
		libInt->sliFile = new clJobSliceFile();
	}
	else
	{
		return -4;
	}

	if (libInt->sliFile == NULL) return -3;

	if (libInt->sliFile->readFromFile(fileName)) return 1;

	return -5;
}

//---------------------------------------------------//
int sf_getLayerCount(int sliI, int partIndex)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC_SLI) return -2;
	if (libInt->sliFile == NULL) return -3;

	return libInt->sliFile->getLayerCount(partIndex);
}


//---------------------------------------------------//
float sf_getMaxLayerPos(int sliI, int partIndex)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC_SLI) return -2;
	if (libInt->sliFile == NULL) return -3;

	return libInt->sliFile->getMaxLayerPos(partIndex);
}


//---------------------------------------------------//
float sf_getLayerThickness(int sliI)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC_SLI) return -2;
	if (libInt->sliFile == NULL) return -3;

	return libInt->sliFile->getLayerThickness();
}


//---------------------------------------------------//
float sf_getLayerPos(int sliI, int partIndex, int layerIndex)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC_SLI) return -2;
	if (libInt->sliFile == NULL) return -3;

	return libInt->sliFile->getLayerPos(partIndex, layerIndex);
}


//---------------------------------------------------//
int sf_getLayerIndexByPos(int sliI, int partIndex, float layerPos)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC_SLI) return -2;
	if (libInt->sliFile == NULL) return -3;

	return libInt->sliFile->getLayerIndexByPos(partIndex, layerPos);
}


//---------------------------------------------------//
char * sf_getPartName(int sliI, int partIndex)
{
	if (sliI == NULL) return NULL;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC_SLI) return NULL;
	if (libInt->sliFile == NULL) return NULL;

	return libInt->sliFile->getPartName(partIndex);
}

//---------------------------------------------------//
char * sf_getPartProperty(int sliI, int partIndex)
{
	if (sliI == NULL) return NULL;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC_SLI) return NULL;
	if (libInt->sliFile == NULL) return NULL;

	return libInt->sliFile->getPartProperty(partIndex);
}


//---------------------------------------------------//
int sf_readSliceData(int sliI, int partIndex, int layerIndex)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC_SLI) return -2;
	if (libInt->sliFile == NULL) return -3;
	if (libInt->sliceData == NULL) return -4;

	return libInt->sliFile->readSliceData(libInt->sliceData, partIndex, layerIndex);
}


//---------------------------------------------------//
int sf_addRasterObject(int sliI, int * outFilledPicture, int * outLinePicture, int partIndex, clSliceData::tyMatrix matrix, int color, int width, int height)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC_SLI) return -2;
	if (libInt->sliceData == NULL) return -4;

	return libInt->sliceData->drawRasteredObject(outFilledPicture, outLinePicture, partIndex, matrix, color, width, height);

}

//---------------------------------------------------//
int sf_getPartCount(int sliI)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC_SLI) return -2;
	if (libInt->sliFile == NULL) return -3;

	return libInt->sliFile->getPartCount();
}


//---------------------------------------------------//
char * sf_getLastError()
{
	return clError::getLastError();
}

//---------------------------------------------------//
char * sf_getLastDebug()
{
	return clError::getLastDebug();
}
