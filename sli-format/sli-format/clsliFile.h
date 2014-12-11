#ifndef CL_STLFILE_H
#define CL_STLFILE_H


#include "clError.h"
#include "clFile.h"
#include "clSliceData.h"

class clSliFile
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
		/// <param name="LayerIndex">index of the layer</param>
		/// <param name="PartIndex">index of the Part</param>
		/// <returns>true on success; false on not</returns>
		bool readSliceData(clSliceData * sliceData, int PartIndex, int LayerIndex);

		/// <summary>returns the Layer Count of the file</summary>
		/// <param name="PartIndex">index of the Part</param>
		int getLayerCount(int PartIndex);

		/// <summary>returns the Layer position in [mm] of the sellected part</summary>
		/// <param name="PartIndex">index of the Part</param>
		/// <param name="layerIndex">index of the Layer</param>
		float getLayerPos(int PartIndex, int layerIndex);

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

			float Dimension_x0;
			float Dimension_x1;
			float Dimension_y0;
			float Dimension_y1;
			float Dimension_z0;
			float Dimension_z1;
		};

		tyFileHead m_FileHead;

		clError m_error;


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
