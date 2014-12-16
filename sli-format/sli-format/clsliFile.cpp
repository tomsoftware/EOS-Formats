#include "clSliFile.h"


//---------------------------------------------------//
clSliFile::clSliFile()
:m_error("clSliFile")
{
	m_IndexTable = NULL;
	m_IndexTable_lenght = 0;

	reset();
}



//---------------------------------------------------//
clSliFile::~clSliFile()
{
	m_file.closeFile();

	reset();
}


//---------------------------------------------------//
void clSliFile::reset()
{
	if (m_IndexTable != NULL) delete[] m_IndexTable;
	m_IndexTable = NULL;

	m_IndexTable_lenght = 0;

	memset(&m_FileHead, 0, sizeof(m_FileHead));

}


//---------------------------------------------------//
bool clSliFile::readFromFile(const char * filename)
{

	if (!m_file.openFile(filename))
	{
		m_error.AddError("readFromFile(): Unable to open file: %s", filename);
		return false;
	}

	m_file.readFilePart(0, 160);
	

	m_file.readString(m_FileHead.magic, 40);

	if (strncmp("EOS 1993 SLI FILE                       ", m_FileHead.magic, 40) != 0)
	{
		m_error.AddError("Wrong File-magic. Not a SLI file?");
		m_file.closeFile();
		return false;
	}

	m_FileHead.version = m_file.readIntBE(2);
	m_FileHead.int02 = m_file.readIntBE(2);
	m_FileHead.HeaderSize = m_file.readIntBE(4);
	m_FileHead.int05 = m_file.readIntBE(4);
	m_FileHead.int07 = m_file.readIntBE(4);
	m_FileHead.FileSliceDataOffset = m_file.readIntBE(4);
	m_FileHead.FileIndexPos = m_file.readIntBE(4);
	m_file.readString(m_FileHead.creator, 40);
	m_FileHead.LayerCount = m_file.readIntBE(4);
	m_FileHead.PolylineCount = m_file.readIntBE(4);
	m_FileHead.int14 = m_file.readIntBE(4);

	//- jump over 8 unknown DWords
	m_file.setOffset(m_file.getOffset() + 4 * 8);

	m_FileHead.scaleFactor = m_file.readFloat();
	
	m_FileHead.Dimension_x0 = m_file.readFloat(4);
	m_FileHead.Dimension_x1 = m_file.readFloat(4);
	m_FileHead.Dimension_y0 = m_file.readFloat(4);
	m_FileHead.Dimension_y1 = m_file.readFloat(4);
	m_FileHead.Dimension_z0 = m_file.readFloat(4);
	m_FileHead.Dimension_z1 = m_file.readFloat(4);



	//- read Index table
	return readIndexTable(m_FileHead.FileIndexPos, m_FileHead.HeaderSize, m_FileHead.LayerCount);
}



//---------------------------------------------------//
bool clSliFile::readIndexTable(int FilePos, int FileOffset, int LayerCount)
{
	m_file.readFilePart(FilePos + FileOffset, LayerCount * 6);

	m_IndexTable_lenght = 0;
	if (m_IndexTable != NULL) delete []m_IndexTable;

	m_IndexTable = new tyIndexTable[LayerCount];

	while ((!m_file.eof()) && (m_IndexTable_lenght < LayerCount))
	{
		//- Start of a layer with upper surface at height z (z*units [mm]). All layers must be sorted in ascending order with respect to z. The thickness of the layer is given by the difference between the z values of the current and previous layers. A thickness for the first (lowest) layer can be specified by including a "zero-layer" with a given z value but with no polyline. 
		m_IndexTable[m_IndexTable_lenght].layerPos = m_file.readIntBE(2) * m_FileHead.scaleFactor;

		m_IndexTable[m_IndexTable_lenght].FileOffset = m_file.readIntBE(4) + FileOffset;

		//m_error.AddDebug("[%i]  %f @ %i", m_IndexTable_lenght, m_IndexTable[m_IndexTable_lenght].layerPos, m_IndexTable[m_IndexTable_lenght].FileOffset);

		m_IndexTable_lenght++;
	}

	m_error.AddDebug("Layer count: %i", m_IndexTable_lenght);
	return true;
}


//---------------------------------------------------//
int clSliFile::getLayerCount(int PartIndex)
{
	if (PartIndex != 0) return 0;
	return m_FileHead.LayerCount;
}

//---------------------------------------------------//
int clSliFile::getPartCount()
{
	return 1;
}

//---------------------------------------------------//
float clSliFile::getLayerPos(int PartIndex, int LayerIndex)
{
	if (PartIndex != 0) return -1;
	if ((LayerIndex < 0) || (LayerIndex >= m_FileHead.LayerCount)) return - 1;

	return m_IndexTable[LayerIndex].layerPos;
}

//---------------------------------------------------//
char * clSliFile::getPartName(int PartIndex)
{
	if (PartIndex != 0) return NULL;
	return m_partName;
}

//---------------------------------------------------//
char * clSliFile::getPartProperty(int PartIndex)
{
	if (PartIndex != 0) return NULL;
	return "";
}




//---------------------------------------------------//
bool clSliFile::readSliceData(clSliceData * sliceData, int PartIndex, int LayerIndex, int storeAsPartIndex)
{
	int n = 0;
	float scaleFactor = m_FileHead.scaleFactor;

	if (PartIndex != 0) return false;
	if (storeAsPartIndex == -1) storeAsPartIndex = PartIndex;

	//- clear old data or define new empty part
	int newObject = sliceData->createPart(storeAsPartIndex, scaleFactor, 0, 0, 0, scaleFactor, 0);

	if ((LayerIndex < 0) || (LayerIndex >= m_FileHead.LayerCount)) return false;

	m_file.readFilePart(m_IndexTable[LayerIndex].FileOffset, 32);


	int OType = 0;

	while (OType != 2)
	{
		//- command
		OType = m_file.readIntBE(1);

		switch (OType)
		{
			case 1:
				{
					//- header of layer
					int unknownLayerPos = m_file.readIntBE(2); //- LayerPos as int

					//- unknownLayerThickness1 == unknownLayerThickness2 !
					float unknownLayerThickness1 = m_file.readFloat(); 
					float unknownLayerThickness2 = m_file.readFloat();

					//m_error.AddDebug("pos %i, float1 %f, float2 %f", unknownLayerPos, unknownLayerThickness1, unknownLayerThickness2);
				}

				//- padding
				if (m_file.readIntBE(1) != 0)
				{
					m_error.AddError("unknown Byte @ %i", m_file.getOffset() - 1);
				}

				break;

			case 2:
				//- end of layer
				break;

			case 3:
				//Command : start polyline
				//Syntax : $$POLYLINE/id,dir,n,p1x,p1y,...pnx,pny
				//Parameters:
				//
				//	id		: INTEGER
				//	dir,n		: INTEGER
				//	p1x..pny	: REAL
				//
				//
				//id : identifier to allow more than one model information in one file.
				//id refers to the parameter id of command $$LABEL (HEADER-section).
				//dir : Orientation of the line when viewing in the negative z-direction
				//0 : clockwise (internal)
				//1 : counter-clockwise (external)
				//2 : open line (no solid)
				//n : number of points
				//p1x..pny : coordinates of the points 1..n 

				if (m_file.readIntBE(1) != 0) //- dir or padding?
				{
					m_error.AddError("unknown Byte @ %i " + m_file.getOffset() - 1);
					return false;
				}


				n = m_file.readIntBE(2);
				if (n > 0)
				{
					float * points = sliceData->createPolygon(storeAsPartIndex, n);

					//- read file
					m_file.readFilePart(m_file.getOffset(), n * 4 * 2 + 32);

					//- read points - every Hatch has 4 points
					for (int i = n * 2; i > 0; i--)
					{
						*points = (float)m_file.readIntBE(2);
						points++;
					}

				}

				break;
			case 4:
				//- support
				//m_error.addWarning("Support is not supported!");

				//Command : start hatches
				//Syntax : $$HATCHES/id,n,p1sx,p1sy,p1ex,p1ey,...pnex,pney
				//Parameters:
				//
				//	id		: INTEGER
				//	n		: INTEGER
				//	p1sx..pney	: REAL
				//
				//id : identifier to allow more than one model information in one file.
				//id refers to the parameter id of command $$LABEL (HEADER-section).
				//n : number of hatches (n*4 =number of coordinates)
				//p1sx..pney : coordinates of the hatches 1..n
				//4 parameters for every hatch (startx,starty,endx,endy) 

				//- padding
				if (m_file.readIntBE(1) != 0)
				{
					m_error.AddError("unknown Byte @ %i " + m_file.getOffset() - 1);
					return false;
				}

				n = m_file.readIntBE(2);

				if (n > 0)
				{
					float * points = sliceData->createHatch(storeAsPartIndex, n);

					//- read file
					m_file.readFilePart(m_file.getOffset(), n * 4 * 4 + 32);

					//- read points - every Hatch has 4 points
					for (int i = n * 4; i > 0; i--)
					{
						*points = (float)m_file.readIntBE(2);
						points++;
					}

				}

				break;

			default:
				m_error.AddError("Unknow opcode %i at %i ", OType, m_file.getOffset() - 1);
				return false;

		}
	}

	return true;
}