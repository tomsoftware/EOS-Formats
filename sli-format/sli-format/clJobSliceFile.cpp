#include "clJobSliceFile.h"


clJobSliceFile::clJobSliceFile()
	:m_error("clJobSliceFile")
{

	m_SliFiles = NULL;
	m_SliFilesCount = 0;
}

//---------------------------------------------------//
clJobSliceFile::~clJobSliceFile()
{
	if (m_SliFiles != NULL) delete[] m_SliFiles;
	m_SliFilesCount = 0;
}


//------------------------------------------------------------//
bool clJobSliceFile::readFromFile(const char * filename)
{
	clJobFileInterpreter job;

	if (!job.readFromFile(filename)) return false;


	int GeneralKey = job.getChild(clJobFile::ROOT_ELEMENT, "General");
	if (GeneralKey > 0)
	{
		int prop_LayerThickness = job.getProperty(GeneralKey, "LayerThickness");
		m_LayerThickness = job.getPropertyValue(prop_LayerThickness, 0.00f);
	}
	else
	{
		m_LayerThickness = 0.0f;
		m_error.AddError("No [General] Element found in .job file : Layer-Thickness may wrong!");
	}
	

	int PartsKey = job.getChild(clJobFile::ROOT_ELEMENT, "Parts");
	if (PartsKey <= 0)
	{
		m_error.AddError("No [Parts] Element found in .job file");
		return false;
	}

	//- chreate buffer for sli-files
	m_SliFilesCount = job.getChildCount(PartsKey);

	if (m_SliFiles != NULL) delete [] m_SliFiles;
	m_SliFiles = new tySliFile[m_SliFilesCount];

	 
	int clild = job.getFirstChild(PartsKey);
	int partID = 0;

	while (clild>0)
	{
		//- get keys to propertys
		int prop_ExpParName = job.getProperty(clild, "ExpParName");
		int prop_FileName = job.getProperty(clild, "FileName");
		int prop_x = job.getProperty(clild, "x");
		int prop_y = job.getProperty(clild, "y");
		int prop_rotation = job.getProperty(clild, "Rotation");
		
		//- check if propertys available
		if ((prop_ExpParName > 0) && (prop_FileName > 0))
		{
			tySliFile * part = &m_SliFiles[partID];

			//- reset all
			part->exposureProfile[0] = '\0';
			part->matrix = { 0, 0, 0, 0, 0, 0 };
			strCopy(part->fileName, job.getPropertyValue(prop_FileName), sizeof(part->fileName));
			part->sliFile.reset();

			//- read .sli file
			if (openPartFile(part, filename))
			{
				//- copy propertys
				strCopy(part->partName, job.getKeyName(clild), sizeof(part->partName));


				strCopy(part->exposureProfile, job.getPropertyValue(prop_ExpParName), sizeof(part->exposureProfile));
				
				
				float a = (float) (job.getPropertyValue(prop_rotation, 0.f) * 2 * PI / 360.0);


				part->matrix.m11 = cos(a);;
				part->matrix.m12 = -sin(a);
				part->matrix.m13 = job.getPropertyValue(prop_x, 0.f);
				part->matrix.m21 = sin(a);;
				part->matrix.m22 = cos(a);
				part->matrix.m23 = job.getPropertyValue(prop_y, 0.f);


				//- move to next part
				partID++;
			}
			else
			{
				m_error.AddError("File for Part [%s] not found [%s]", job.getKeyName(clild), part->fileName);
			}
		}
		else
		{
			m_error.AddError("Missing information for Part [%s]", job.getKeyName(clild));
		}

		m_error.AddDebug("%s", job.getKeyName(clild));

		//- read next key in .job-file
		clild = job.getNextChild(clild);


	}
	
	m_SliFilesCount = partID;
	
	return true;
}

//---------------------------------------------------//
int clJobSliceFile::getPartCount()
{
	return m_SliFilesCount;
}


//------------------------------------------------------------//
bool clJobSliceFile::openPartFile(tySliFile * part, const char * jobFileName)
{
	if ((part == NULL)) return false;
	if (part->fileName == NULL) return false;


	if (clFile::fileExist(part->fileName))
	{
		return part->sliFile.readFromFile(part->fileName);
	}
	
	int lastBack = strIndexOfLast(jobFileName, '\\', 1024);
	if (lastBack > 0) 
	{
		char buffer[255];
		lastBack+=2; //- copy also the '\\'

		lastBack = (sizeof(buffer) < lastBack) ? sizeof(buffer) : lastBack;
		strCopy(buffer, &jobFileName[0], lastBack);
	
		//- Path + Filename
		std::string newFileName = std::string(buffer).append(part->fileName);

		if (clFile::fileExist(newFileName.c_str()))
		{
			return part->sliFile.readFromFile(newFileName.c_str());
		}
	}


	lastBack = strIndexOfLast(jobFileName, '/', 1024);
	if (lastBack > 0)
	{
		char buffer[255];
		lastBack += 2; //- copy also the '/'

		lastBack = (sizeof(buffer) < lastBack) ? sizeof(buffer) : lastBack;
		strCopy(buffer, &jobFileName[0], lastBack);

		//- Path + Filename
		std::string newFileName = std::string(buffer).append(part->fileName);

		if (clFile::fileExist(newFileName.c_str()))
		{
			return part->sliFile.readFromFile(newFileName.c_str());
		}
	}

	m_error.AddError("File [%s] not found.", part->fileName);
}


//------------------------------------------------------------//
int clJobSliceFile::getLayerIndexByPos(int PartIndex, float LayerPos)
{
	if (m_SliFiles == NULL) return false;
	if ((PartIndex < 0) && (PartIndex >= m_SliFilesCount)) return false;

	tySliFile * part = &m_SliFiles[PartIndex];

	return part->sliFile.getLayerIndexByPos(0, LayerPos);
}


//------------------------------------------------------------//
bool clJobSliceFile::readSliceData(clSliceData * sliceData, int PartIndex, int LayerIndex, int storeAsPartIndex)
{
	if (m_SliFiles == NULL) return false;
	if ((PartIndex < 0) && (PartIndex >= m_SliFilesCount)) return false;
	if (storeAsPartIndex == -1) storeAsPartIndex = PartIndex;

	tySliFile * part = &m_SliFiles[PartIndex];

	bool ret = part->sliFile.readSliceData(sliceData, 0, LayerIndex, storeAsPartIndex);

	sliceData->PartMatrixMult(storeAsPartIndex, part->matrix);

	return ret;
}


//------------------------------------------------------------//
char * clJobSliceFile::strCopy(char *dest, const char * src, int maxCount)
{
	if (dest == NULL) return "";
	*dest = 0;
	if (src == NULL) return dest;

	char * destP = dest;
	const char * srcP = src;

	for (int i = maxCount - 1; i > 0; i--)
	{
		char s = *srcP++;
		if (s == 0) break;

		*destP = s;
		destP++;
	}
	*destP = 0;
	return dest;

}

//------------------------------------------------------------//
int clJobSliceFile::strIndexOfLast(const char * src, char findChar, int maxScanCount)
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

//------------------------------------------------------------//
int clJobSliceFile::getLayerCount(int PartIndex)
{
	if (m_SliFiles == NULL) return 0;
	if ((PartIndex < 0) && (PartIndex >= m_SliFilesCount)) return 0;

	tySliFile * part = &m_SliFiles[PartIndex];

	return part->sliFile.getLayerCount(0);
}


//------------------------------------------------------------//
float clJobSliceFile::getMaxLayerPos(int PartIndex)
{
	if (m_SliFiles == NULL) return 0;
	if ((PartIndex < 0) && (PartIndex >= m_SliFilesCount)) return 0;

	tySliFile * part = &m_SliFiles[PartIndex];

	return part->sliFile.getMaxLayerPos(0);
}


//------------------------------------------------------------//
float clJobSliceFile::getLayerThickness()
{
	return m_LayerThickness;
}


//------------------------------------------------------------//
float clJobSliceFile::getLayerPos(int PartIndex, int layerIndex)
{
	if (m_SliFiles == NULL) return 0.f;
	if ((PartIndex < 0) && (PartIndex >= m_SliFilesCount)) return 0.f;

	tySliFile * part = &m_SliFiles[PartIndex];

	return part->sliFile.getLayerPos(0, layerIndex);
}

//------------------------------------------------------------//
char * clJobSliceFile::getPartName(int PartIndex)
{
	if (m_SliFiles == NULL) return NULL;
	if ((PartIndex < 0) && (PartIndex >= m_SliFilesCount)) return NULL;

	tySliFile * part = &m_SliFiles[PartIndex];

	return part->partName;
}

//---------------------------------------------------//
char * clJobSliceFile::getPartProperty(int PartIndex)
{
	if (m_SliFiles == NULL) return NULL;
	if ((PartIndex < 0) && (PartIndex >= m_SliFilesCount)) return NULL;

	tySliFile * part = &m_SliFiles[PartIndex];

	return part->exposureProfile;
}
