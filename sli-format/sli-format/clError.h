#ifndef CLERROR_H
#define CLERROR_H

#include <stdio.h>
#include <string.h>

class clError
{
private:
	const char * m_ClassName;
	static char s_lastError[2048];
	static bool s_hasLastError;
	static char s_lastDebug[2048];
	static bool s_hasLastDebug;

	char * formatError(char * dest, int destSize, const char* ErrorType, const char* className, const char* outputString, va_list ap);
public:

	clError(const char * className);
	~clError();
	void AddError(const char * errorString, ...);
	void AddDebug(const char * debugString, ...);
	static char * getLastError();
	static char * getLastDebug();
};

#endif

