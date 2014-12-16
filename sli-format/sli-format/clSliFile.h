#ifndef CL_STLFILE_H
#define CL_STLFILE_H


#include "clError.h"
#include "clFile.h"
#include "clSliceData.h"
#include "abstractSliceFile.h"

class clSliFile : public abstractSliceFile
{
	public:
		clSliFile();
		~clSliFile();

		/// <summary>Read and interpret sli-data from File</summary>
		/// <param name="FileName">Filename of File to read</param>
		/// <returns>true on success; false on not</returns>
		bool readFromFile(const char * filename);

		/// <summary>Read the slice data and add it to the sliceData class</summary>
		/// <param name="sliceData">sliceData class</param>
		/// <param name="PartIndex">index of the Part</param>
		/// <param name="LayerIndex">index of the layer</param>
		/// <param name="storeAsPartIndex">[optional] Index in [sliceData] to store the data in, use -1 for same as PartIndex</param>
		/// <returns>true on success; false on not</returns>
		virtual bool readSliceData(clSliceData * sliceData, int PartIndex, int LayerIndex, int storeAsPartIndex = -1);

		/// <summary>returns the Layer Count of the file</summary>
		/// <param name="PartIndex">index of the Part</param>
		int getLayerCount(int PartIndex);

		/// <summary>returns the Layer position in [mm] of the sellected part</summary>
		/// <param name="PartIndex">index of the Part</param>
		/// <param name="layerIndex">index of the Layer</param>
		float getLayerPos(int PartIndex, int layerIndex);

		/// <summary>returns the number of parts in this file</summary>
		int getPartCount();

		/// <summary>returns the name of the part</summary>
		/// <param name="PartIndex">index of the Part</param>
		char * getPartName(int PartIndex);


		/// <summary>returns a string with special propertys to use for this part</summary>
		/// <param name="PartIndex">index of the Part</param>
		char * getPartProperty(int PartIndex);


		void reset();


	private:
		struct tyFileHead
		{
			char magic[40];
			int version; //- version? "v divided by 100 gives the version number. "
			int int02; //- Head count ?
			int HeaderSize; //- Header offset
			int int05; //- unknown
			int int07; //- unknown
			int FileSliceDataOffset;
			int FileIndexPos;
			char creator[40];
			int LayerCount;
			int PolylineCount;
			int int14; //- unknown
			float scaleFactor; 

			float Dimension_x0; //- Bounding Box
			float Dimension_x1;
			float Dimension_y0;
			float Dimension_y1;
			float Dimension_z0;
			float Dimension_z1;
		};

		tyFileHead m_FileHead;

		clError m_error;

		char m_partName[255];

		clFile m_file;

		struct tyIndexTable
		{
			int FileOffset;
			float layerPos; //- Syntax : $$LAYER/z  - Start of a layer with upper surface at height z (z*units [mm]).
		};

		tyIndexTable * m_IndexTable;
		int m_IndexTable_lenght;


		bool readIndexTable(int FilePos, int FileOffset, int LayerCount);
};


#endif
