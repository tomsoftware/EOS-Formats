#ifndef CLFILE_H
#define CLFILE_H

#include <iostream>
#include <fstream>
#include <sys/stat.h>


#include "clError.h"

using namespace std;

class clFile
{
	private:
		union BinaryConvertIntToFloat
		{
			float toFloat;
			int toInt;
		};


		char * m_buffer;
		int m_bufferLen;
		//- offset in buffer
		int m_bufferPos;

		int m_filePos;
		int m_fileReadPos;


		bool m_eof = false;


		clError m_error = clError("clFile");
		unsigned int m_filesize;

		std::ifstream m_File;

		union UNION_FLOAT_2_CHAR
		{
			float f;
			char ch[4];
		};


	public:
		//clFile();
		clFile::clFile(const char * FilePath = NULL, const char * FileName = NULL);
		~clFile();





		/// <summary>open a File</summary>
		/// <param name="FilePath">Path of File to read from</param>
		/// <param name="FileName">Filename of File to read from</param>
		/// <returns>true on success</returns>
		bool openFile(const char * FilePath, const char * FileName);
		bool openFile(const char * FileName);


		/// <summary>close the file</summary>
		void closeFile();



		/// <summary>reads from opened File</summary>
		/// <param name="offset">offset in file; values smaler 0 read from current position</param>
		/// <param name="size">count of bytes to to read</param>
		/// <returns>true on success</returns>
		bool readFilePart(unsigned int offset, int size);
		bool readFilePart(int size);

		/// <summary>return the next [size] byte.</summary>
		/// <param name="outSize">count of bytes read.</param>
		/// <param name="size">count of bytes to read.</param>
		/// <param name="offset">offset to start reading.</param>
		/// <returns>pointer to binary char buffer</returns>
		const char * readString(int * outSize, int size, int offset= -1);

		/// <summary>copy [size] byte from file buffer to dest buffer.</summary>
		/// <param name="outSize"></param>
		/// <param name="size">count of bytes to read.</param>
		/// <param name="offset">offset to start reading.</param>
		/// <returns>count of bytes read</returns>
		int readString(char * destBuffer, int size, int offset = -1);

		/// <summary>return the next [size] byte and convert them to a BigEnding Integer.</summary>
		/// <param name="size">count of bytes to read.</param>
		/// <param name="offset">offset to start reading.</param>
		unsigned int readIntBE(int size = 4, int offset = -1);

		int readSignedWordBE(int offset);

		/// <summary>return the next 4 byte and convert them to a float.</summary>
		/// <param name="offset">offset to start reading.</param>
		float readFloat(int offset = -1);

		/// <summary>return the current offset in the file</summary>
		/// <returns>file offset</returns>
		int getOffset();

		/// <summary>set the current offset in the file</summary>
		/// <returns>true on success</returns>
		bool setOffset(int newOffset);


		/// <summary>Check if File [Filename] exists</summary>
		/// <param name="Filename">Filename to test.</param>
		/// <returns>true on success</returns>
		static bool fileExist(const char * Filename);

		bool eof();
	
};

#endif