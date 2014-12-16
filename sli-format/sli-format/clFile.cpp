#include "clFile.h"


clFile::clFile(const char * FilePath, const char * FileName)
{
	m_buffer = NULL;
	m_filesize = 0;
	m_bufferLen = 0;
	m_filePos = 0;
	m_eof = true;

	openFile(FilePath, FileName);
	
}


//---------------------------------------------------//
clFile::~clFile()
{
	//- close file
	closeFile();
}


//---------------------------------------------------//
void clFile::closeFile()
{
	//- delete buffer if already used
	if (m_buffer != NULL) delete m_buffer;

	m_buffer = NULL;
	m_filesize = 0;
	m_bufferLen = 0;
	m_filePos = 0;
	m_eof = true;

	m_File.close();
	m_File.clear();
}

//---------------------------------------------------//
bool clFile::openFile(const char * FileName)
{
	return openFile(NULL, FileName);
}


//---------------------------------------------------//
bool clFile::fileExist(const char * Filename)
{
	struct stat buffer;
	return (stat(Filename, &buffer) == 0);
}

//---------------------------------------------------//
bool clFile::openFile(const char * FilePath, const char * FileName)
{
	if (FileName == NULL) return false;

	if (FilePath != NULL) FileName = std::string(FilePath).append(FileName).c_str();

	m_buffer = NULL;
	m_bufferLen = 0;
	m_filesize = 0;
	m_filePos = 0;
	m_eof = true;
	m_fileReadPos = 0;

	m_File.open(FileName, ios::in | ios::binary);
	if (m_File)
	{
		m_error.AddDebug("Open file %s", FileName);

		//- get stream size
		m_File.seekg(0, m_File.end);
		m_filesize = (unsigned int) m_File.tellg();
		m_File.seekg(0, m_File.beg);

		m_eof = false;
	}
	else
	{
		m_error.AddError("Unable to open file: %s", FileName);
		return false;
	}

	return true;

	
}



//---------------------------------------------------//
bool clFile::readFilePart(unsigned int offset, int size)
{
	if (m_File)
	{
		//- move to file pos
		if (offset >= 0)
		{
			if (offset > m_filesize)
			{
				m_filePos = m_filesize;
				m_error.AddError("readFile(): file seek(EOF)!");
				m_eof = true;
				return false;
			}
			m_File.seekg(offset, m_File.beg);
			m_filePos = offset;
		}
		else
		{
			offset = m_fileReadPos; //- use the end-pos of the last read
		}


		if (size < 0)
		{
			//- read everything left
			size = m_filesize - offset;
		}


		//- create buffer
		if (m_bufferLen < size)
		{
			//- delete buffer if already used
			if (m_buffer != NULL) delete m_buffer;

			m_bufferLen = size;

			//- create buffer
			m_buffer = new char[m_bufferLen];

			if (m_buffer == NULL)
			{
				m_error.AddError("readFile(): Out of memory");
				return false;
			}
		}


		//- read over EOF?
		if (size + offset > m_filesize)
		{
			size = m_filesize - offset;
		}

		//- read data from file to buffer
		m_File.read(m_buffer, size);
		m_bufferLen = size;
		m_fileReadPos = offset + size;
		m_bufferPos = offset;
	}
	else
	{
		m_error.AddError("file is not open");
	}

	return true;
}


//---------------------------------------------------//
int clFile::readString(char * destBuffer, int size, int offset)
{
	int readCount = 0;
	const char * buffer;
	char * pOut = destBuffer;

	buffer = readString(&readCount, size, offset);

	for (int i = readCount; i > 0; i--)
	{
		*pOut = *buffer;
		pOut++;
		buffer++;
	}

	for (int i = size - readCount; i > 0; i--)
	{
		*pOut = '\0';
		pOut++;
	}

	return readCount;
}


//---------------------------------------------------//
const char * clFile::readString(int * outSize, int size, int offset)
{
	const char * out;

	if (offset>-1)
	{
		m_bufferPos = offset - m_filePos;
		m_eof = false; //- we do check this later...
	}

	//- if no size given then return to the end of buffer
	if (size == -1) size = m_bufferLen - m_bufferPos;
	if (size < 0) size = 0;


	int ofs = m_bufferPos - m_filePos;

	//- not EOF?
	if (ofs < m_bufferLen)
	{
		//- requestet size is available?
		if (ofs + size > m_bufferLen)
		{
			m_eof = true;

			if (outSize == NULL)
			{
				m_error.AddError("Out of filesize! Available size: %i / requestet size: %i", m_bufferLen, size);
				return NULL; //- the required Buffer size can't be returned
			}

			m_eof = true;
			size = m_bufferLen - m_bufferPos;
		}


		out = &m_buffer[ofs];
	}
	else
	{
		m_error.AddError("EOF!");
		m_eof = true;
		size = 0;
		out = NULL;
	}
	m_bufferPos += size;

	if (outSize != NULL) *outSize = size;

	return out;
}


//---------------------------------------------------//
unsigned int clFile::readIntBE(int size, int offset)
{
	int retSize = 0;
	const char * tmp = readString(&retSize, size, offset);

	if (tmp == NULL) return 0;
	if (retSize == 4) return *((int*) tmp);
	if (retSize == 2) return ((*((int*) tmp)) & 0x0000FFFF);
	if (retSize == 1) return ((*((int*) tmp)) & 0x000000FF);
	if (retSize == 3) return ((*((int*) tmp)) & 0x00FFFFFF);

	return 0;
}


//---------------------------------------------------//
int clFile::readSignedWordBE(int offset)
{
	int retSize = 0;
	const char * tmp = readString(&retSize, 2, offset);

	if (retSize != 2) return 0;

	return (*((short*) tmp));
}

//---------------------------------------------------//
float clFile::readFloat(int offset)
{
	UNION_FLOAT_2_CHAR destF;

	if (readString((char*) destF.ch, 4, offset) == 4) return destF.f;

	return 0.0f;
}

//---------------------------------------------------//
int clFile::getOffset()
{
	return m_bufferPos;
}

//---------------------------------------------------//
bool clFile::setOffset(int newOffset)
{
	int tmpOffset = newOffset - m_filePos;

	if ((tmpOffset >= 0) && (tmpOffset < m_bufferLen))
	{
		m_bufferPos = tmpOffset;
		return true;
	}
	else
	{
		m_error.AddError("setOffset(%i) out of buffered File [%i - %i]", newOffset, m_filePos, m_filePos + m_bufferLen);
		return false;
	}
}

//---------------------------------------------------//
bool clFile::eof()
{
	return m_eof;
}