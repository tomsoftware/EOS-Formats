#include "main.h"


int main(int argc, char* argv [])
{
	
	clSliFile sliFile;
	clSliceData sliceData;


	if (argc > 1)
	{
		sliFile.readFromFile(argv[1]);
	}
	else
	{
		sliFile.readFromFile("D:\\Entwicklung\\VC\\ThermoBoxEmgu\\_test_files_\\140627_PowerCheck\\Pelton_1_c.sli");
	}



	sliFile.readSliceData(&sliceData, 0, 0);

	clSliceData::tyMatrix TransMatrix;
	clSliceData::IdentityMatrix(&TransMatrix);

	int partCount = sliceData.getPartCount();
	for (int part = 0; part < partCount; part++)
	{
		int objectCount = sliceData.getObjectCount(part);
		for (int object = 0; object < objectCount; object++)
		{
			//float * points = sliceData.getObjectPointsTransformed(part, object, TransMatrix);


			//sliceData.drwaRasteredObject(&imgFilled, &imgPolyLine, part, object, TransMatrix, object, w, h);
		}
	}

	system("pause");
}



//---------------------------------------------------//
//-- library interface--//
//---------------------------------------------------//
int sf_initLib()
{

	tyLibraryInterface *libInt = new tyLibraryInterface;
	
	libInt->magic = MAGIC;
	libInt->sliFile = new clSliFile();
	libInt->sliceData = new clSliceData();

	return (int) libInt;
}

//---------------------------------------------------//
void sf_freeLib(int sliI)
{
	if (sliI != NULL)
	{
		tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
		if (libInt->magic == MAGIC)
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
	if (libInt->magic != MAGIC) return -2;
	if (libInt->sliFile == NULL) return -3;

	if (libInt->sliFile->readFromFile(fileName)) return 1;
	return -4;
}

//---------------------------------------------------//
int sf_getLayerCount(int sliI, int partIndex)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC) return -2;
	if (libInt->sliFile == NULL) return -3;

	return libInt->sliFile->getLayerCount(partIndex);
}


//---------------------------------------------------//
float sf_getLayerPos(int sliI, int partIndex, int layerIndex)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC) return -2;
	if (libInt->sliFile == NULL) return -3;

	return libInt->sliFile->getLayerPos(partIndex, layerIndex);
}



//---------------------------------------------------//
int sf_readSliceData(int sliI, int partIndex, int layerIndex)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC) return -2;
	if (libInt->sliFile == NULL) return -3;
	if (libInt->sliceData == NULL) return -4;

	return libInt->sliFile->readSliceData(libInt->sliceData, partIndex, layerIndex);
}


//---------------------------------------------------//
int sf_addRasterObject(int sliI, int * outFilledPicture, int * outLinePicture, int partIndex, clSliceData::tyMatrix matrix, int color, int width, int height)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC) return -2;
	if (libInt->sliceData == NULL) return -4;

	return libInt->sliceData->drwaRasteredObject(outFilledPicture, outLinePicture, partIndex, matrix, color, width, height);

}

//---------------------------------------------------//
int sf_getPartCount(int sliI)
{
	if (sliI == NULL) return -1;
	tyLibraryInterface * libInt = (tyLibraryInterface *) sliI;
	if (libInt->magic != MAGIC) return -2;
	if (libInt->sliceData == NULL) return -3;

	return libInt->sliceData->getPartCount();
}
