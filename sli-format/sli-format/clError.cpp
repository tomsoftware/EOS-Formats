#include "clError.h"
#include <stdarg.h> 

clError::clError(const char * className)
{
	m_ClassName = className;
}

//-------------------------------------//
void clError::AddError(const char * errorString, ...)
{
	va_list args;
	va_start(args, errorString);

	printf("!Error: [%s] ", m_ClassName);
	vprintf(errorString, args);
	printf("\n");

	va_end(args);
}


//-------------------------------------//
void clError::AddPlanText(const char * debugText)
{
	printf("%s", debugText);
	printf("\n");
}


//-------------------------------------//
void clError::AddDebug(const char * debugString, ...)
{
	va_list args;
	va_start(args, debugString);

	printf(" Debug: [%s] ", m_ClassName);
	vprintf(debugString, args);
	printf("\n");
	
	va_end(args);
}


//-------------------------------------//
clError::~clError()
{
}


