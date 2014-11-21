#ifndef MAIN_H
#define MAIN_H

#include "clJobFileInterpreter.h"

#define MAGIC ('J' | ('o'<<8) | ('B'<<16) | (255<24))

struct tyLibraryInterface
{
	int magic;
	clJobFileInterpreter * jobFile;
};


//---- DLL EXPORTS ------//
#define DllExport extern "C" __declspec(dllexport)

DllExport int jf_initLib();
DllExport void jf_freeLib(int jobI);
DllExport int jf_readFromFile(int jobI, char * fileName);
DllExport int jf_printXML(int jobI);
DllExport char * jf_getKeyName(int jobI, int keyIndex);
DllExport int jf_getFirstKeyChild(int jobI, int keyIndex);
DllExport int jf_getNextKeyChild(int jobI, int keyIndex);
DllExport int jf_getFirstProperty(int jobI, int propertyIndex);
DllExport int jf_getNextProperty(int jobI, int propertyIndex);
DllExport char * jf_getPropertyName(int jobI, int propertyIndex);
DllExport char * jf_getPropertyValue(int jobI, int propertyIndex);
DllExport char * jf_getPropertyComment(int jobI, int propertyIndex);


#endif
