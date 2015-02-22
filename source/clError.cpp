#include "clError.h"
#include <stdarg.h> 

#pragma warning (push)
#pragma warning(disable : 4996)


bool clError::s_hasLastError = false;
char clError::s_lastError[2048];
bool clError::s_hasLastDebug = false;
char clError::s_lastDebug[2048];


clError::clError(const char * className)
{
	m_ClassName = className;
}

//-------------------------------------//
void clError::AddError(const char * errorString, ...)
{
	va_list args;
	va_start(args, errorString);

	char * infoStr = formatError(s_lastError, sizeof(s_lastError), "!Error", m_ClassName, errorString, args);

	printf("%s\n", infoStr);

	s_hasLastError = true;

	va_end(args);
}



//-------------------------------------//
void clError::AddDebug(const char * debugString, ...)
{
	va_list args;
	va_start(args, debugString);

	char * infoStr = formatError(s_lastDebug, sizeof(s_lastDebug), " Debug", m_ClassName, debugString, args);

	printf("%s\n", infoStr);

	s_hasLastDebug = true;

	va_end(args);
}


//-------------------------------------//
char * clError::formatError(char * dest, int destSize, const char* ErrorType, const char* className, const char* outputString, va_list args)
{
	int startsize = strlen(dest);
	int sizeofprefix = strlen(ErrorType) + strlen(className) + 20;

	if ((startsize + sizeofprefix) >= destSize)
	{
		//- on overflow restart!
		dest[0] = '\0';
		startsize = 0;
	}

	if (clError::s_hasLastError)
	{
		dest[startsize] = '\n';
		startsize++;
	}
	else
	{
		startsize = 0;
	}

	sprintf(&dest[startsize], "%s: [%s] ", ErrorType, className);
	int size = strlen(&dest[startsize]) + startsize;

	vsnprintf(&dest[size], destSize - size, outputString, args);

	clError::s_hasLastError = true;

	return &dest[startsize];
}


//-------------------------------------//
clError::~clError()
{
}


//-------------------------------------//
char * clError::getLastError()
{
	if (!clError::s_hasLastError) return NULL;
	clError::s_hasLastError = false;
	return clError::s_lastError;
}

//-------------------------------------//
char * clError::getLastDebug()
{
	if (!clError::s_hasLastDebug) return NULL;
	clError::s_hasLastDebug = false;
	return clError::s_lastDebug;
}



#pragma warning (pop)