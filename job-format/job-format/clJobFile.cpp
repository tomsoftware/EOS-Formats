#include "clJobFile.h"


//---------------------------------------------------//
clJobFile::clJobFile()
:m_error("clJobFile")
{
	m_filebuffer = NULL;
	m_filebuffer_lenght = 0;
}



//---------------------------------------------------//
clJobFile::~clJobFile()
{
	if (m_filebuffer != NULL)
	{
		delete[] m_filebuffer;
		m_filebuffer = NULL;
	}
	m_filebuffer_lenght = 0;
	m_filebuffer_used_len = 0;

}


//---------------------------------------------------//
int clJobFile::getBufferLenght()
{
	if (m_filebuffer == NULL) return 0;
	
	return m_filebuffer_used_len;

}

//---------------------------------------------------//
char * clJobFile::getBuffer()
{
	if (m_filebuffer == NULL) return NULL;

	return m_filebuffer;

}



//---------------------------------------------------//
char * clJobFile::readFromFile(const char * filename)
{
	int filesize;
	char * buffer=NULL;

	std::ifstream myFile(filename, std::ios::in | std::ios::binary);
	if (!myFile)
	{
		m_error.AddError("readFromFile(): Unable to open file: %s", filename);
		return NULL;
	}


	m_error.AddDebug("Open file %s", filename);


	//- get stream size
	myFile.seekg(0, myFile.end);
	filesize = (unsigned int) myFile.tellg();
	myFile.seekg(0, myFile.beg);

	//- read file to buffer
	buffer = new char[filesize];

	myFile.read(buffer, filesize);

	//- interpret the job-file content
	return readFromBuffer(buffer, filesize);
}


//---------------------------------------------------//
char * clJobFile::readFromBuffer(const char * buffer, int bufferLen)
{
	if ((buffer == NULL) || (bufferLen<0))
	{
		m_error.AddError("readFromBuffer(): Unable to read buffer");
		return NULL;
	}

	//- free file buffer
	if (m_filebuffer != NULL) delete[] m_filebuffer;
	m_filebuffer_lenght = bufferLen;

	//- create file to buffer
	m_filebuffer = new char[bufferLen+10]; //- +3 for end: "\r\n\0" and checksum "\r\n[]\r\n"
	m_filebuffer[bufferLen+9] = '\0'; //- safety end-of-string


	//- uncode file
	const char * pBuffer = buffer;
	const char * pLineStart = pBuffer;
	char * pOutBuffer = m_filebuffer;

	int lineCount = 0;
	int lineLen = 0;

	for (int i = bufferLen; i > 0; i--)
	{
		char value = *pBuffer;

		if ((value == '\n') || (value == '\r')) //- is this a "\n" or "\r" or "\r\n"
		{
			if (lineLen > 0)
			{
				pOutBuffer = uncryptLine(pOutBuffer, pLineStart, lineLen);
				lineCount++;
			}
			lineLen = 0;

			//- copy line feet to the buffer
			*pOutBuffer = value;
			pOutBuffer++;

			if ((value == '\r') && (*pBuffer == '\n'))
			{
				//- copy line feet to out buffer
				*pOutBuffer = *pBuffer;
				pOutBuffer++;

				//- is this a "\r\n" ?
				pBuffer++;
				bufferLen++;
			}
			pLineStart = pBuffer+1;
		}
		else
		{
			lineLen++;
		}

		pBuffer++;
	}

	//- are there data at the end of the file without a line feed?
	if (lineLen != 0) 
	{
		pOutBuffer = uncryptLine(pOutBuffer, pLineStart, lineLen);
		lineCount++;
	}


	*pOutBuffer++ = '\r';  //- close string
	*pOutBuffer++ = '\n';  //- close string

	m_filebuffer_used_len = (pOutBuffer - m_filebuffer);

	*pOutBuffer = '\0';  //- close string

	return m_filebuffer;
	
}


//---------------------------------------------------//
char * clJobFile::uncryptLine(char * pOutBuffer, const char * pInBuffer, int lineLen)
{
	int pos = 0;
	char * pOut = (char *) pOutBuffer;
	char * pIn = (char *) pInBuffer;


	if (*pIn == '$') //- crypted?
	{
		//- Format:
		//-  $tt xxxxxxxxxxxxxxxxxxxxxxx?
		//- or:
		//-  $tt [n] xxxxxxxxxxxxxxxx?
		//-
		//- $: line is crypted
		//- t: type of line
		//- n: tree-depth in config file
		//- x: crypted data
		//- ?: checksum of line


		//- first char is a '$' marker, so jump
		pIn++;
		lineLen--;

		//- last char is the check sum of this line
		int checkSum = *(pIn + lineLen - 1);
		lineLen--;
		
		//- line-Type (eg "8"=normal, "1c"=file checksum, ... ??? )... we do ignore this!
		pos = findByte(pIn, lineLen, ' ');
		if ((pos < 0) || (pos>3))
		{
			m_error.AddError("uncryptString(): Error in line");
			return pOut;
		}
		
		if (*pIn != '8') //- default type
		{
			if ((*pIn == '1') && (*(pIn + 1) == 'c'))
			{
				//- this is the file Checksum! ... to be compatible to the rest of the file
				*pOut++ = '\n';
				*pOut++ = '[';
				*pOut++ = '0';
				*pOut++ = ']';
				*pOut++ = '\n';
			}
			else
			{
				m_error.AddError("uncryptString(): Error line type unknown: %i ", (int)(*pIn));
			}
		}


		pIn += pos + 1; //- jump over line-Type + space
		lineLen -= pos + 1;


		if (*pIn == '[')
		{
			//- the "tree-depth" e.g. "[5] " is not crypted ... so copy
			for (; lineLen > 0; lineLen--)
			{
				char v = *pIn;
				*pOut = v;
				pOut++;
				pIn++;

				if (v == ' ') break;
			}
			lineLen--;
		}

		//- uncrypt data
		return uncryptString(pOut, pIn, lineLen);
	}
	else
	{
		//- not crypted... so copy
		for (int i = lineLen; i > 0; i--)
		{
			*pOut = *pIn;
			pOut++;
			pIn++;
		}

		return pOut;
	}
}


//---------------------------------------------------//
char * clJobFile::uncryptString(char * pOutBuffer, const char * pInBuffer, int lineLen)
{
	if (pOutBuffer == NULL) return NULL;
	if (pInBuffer == NULL) return pOutBuffer;

	int outPos = 0;
	unsigned char * pOut = (unsigned char *) pOutBuffer;
	unsigned char * pIn = (unsigned char *) pInBuffer;

	//- init key
	int int_Key = 0xAD;
	int int_lastKey = 0x77;
	int byte_lastChar = 0;


	//- alle Bytes entschlüsseln
	for (int i = lineLen; i > 0 ; i--)
	{
		unsigned char ch_ori = *pIn;
		pIn++;

		int ch = ch_ori;

		//- special character 
		switch (ch)
		{
			case 0x1F: ch = 0x3B; break;
			case 0x1E: ch = 0x24; break;
			case 0x1D: ch = 0x5B; break;
			case 0x1C: ch = 0x23; break;
			case 0x1B: ch = 0x5C; break;
			case 0x11: ch = 0x20; break;
		}

		//- calc next/new Key
		int	key = ((int_Key * int_lastKey) % 0x3E8) + byte_lastChar;
		int_lastKey = int_Key;
		int_Key = key;


		byte_lastChar = ch_ori;

		ch = ch - (key % 0xDF);
		if (ch < 0) ch = ch + 0xDF;

		*pOut = (unsigned char) ch;
		pOut++;
	}


	return (char *) pOut;
}


//---------------------------------------------------//
int clJobFile::findByte(const char * pInBuffer, int lineLen, char Byte2Find)
{
	const char * pIn = pInBuffer;

	for (int i = 0; i < lineLen; i++)
	{
		if (*pIn == Byte2Find) return i;
		pIn++;
	}
	return -1;
}
