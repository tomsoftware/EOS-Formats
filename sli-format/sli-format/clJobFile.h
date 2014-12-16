#ifndef CL_JOBFILE_H
#define CL_JOBFILE_H

#include <stdio.h>
#include "clError.h"
#include <iostream>
#include <fstream>

class clJobFile
{
	public:
		clJobFile();
		~clJobFile();

		static const int ROOT_ELEMENT = 0;

		/// <summary>Read and interpret job-data from File</summary>
		/// <param name="FileName">Filename of File to read</param>
		/// <returns>uncoded buffer on success; NULL on error</returns>
		char * readFromFile(const char * filename);

		/// <summary>Interpret job-data from char buffer</summary>
		/// <param name="buffer">char Buffer to interpre</param>
		/// <param name="bufferLength">lenght of [buffer]</param>
		/// <returns>uncoded buffer on success; NULL on error</returns>
		char * readFromBuffer(const char * buffer, int bufferLength);


		/// <summary>returns the lenght of the uncrypted buffer/data</summary>
		/// <returns>the lenght of the uncrypted data</returns>
		int getBufferLenght();

		/// <summary>returns the uncrypted buffer/data</summary>
		/// <returns>the uncrypted data</returns>
		char * getBuffer();


	private:


		clError m_error;

		/// <summary>Counting not empty lines in file</summary>
		int countBufferLines(const char * buffer, int bufferLen);

		/// <summary>uncodes a cryptet text-line (with crypting header) of [lineLen] characters from [pInBuffer] to [pOutBuffer]. Returns the new end of [pOutBuffer]</summary>
		char * uncryptLine(char * pOutBuffer, const char * pInBuffer, int lineLen);

		/// <summary>uncrypt [lineLen] characters from [pInBuffer] to [pOutBuffer]. Returns the new end of [pOutBuffer]</summary>
		char * uncryptString(char * pOutBuffer, const char * pInBuffer, int lineLen);

		
		/// <summary>find the position of the first appearance of a byte</summary>
		int findByte(const char * pInBuffer, int lineLen, char Byte2Find);


		char * m_filebuffer;
		int m_filebuffer_lenght;
		int m_filebuffer_used_len;
};


#endif
