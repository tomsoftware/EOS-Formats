#ifndef CL_JOBFILEINTERPRETER_H
#define CL_JOBFILEINTERPRETER_H

#include "clJobFile.h"

//- for atoi
#include <cstdlib>

class clJobFileInterpreter
{
	public:
		clJobFileInterpreter();
		~clJobFileInterpreter();

		/// <summary>Read and interpret job-data from File</summary>
		/// <param name="FileName">Filename of File to read</param>
		/// <returns>true on sucess</returns>
		bool readFromFile(const char * filename);

		/// <summary>Interpret job-data from char buffer</summary>
		/// <param name="buffer">char Buffer to interpre</param>
		/// <param name="bufferLength">lenght of [buffer]</param>
		/// <returns>true on sucess</returns>
		bool readFromBuffer(const char * buffer, int bufferLength);

		/// <summary>Print out the job-file in XML-Format</summary>
		bool printXML();


		//---------------------------------------------------//
		char * getKeyName(int keyIndex);
		int getFirstChild(int keyIndex=clJobFile::ROOT_ELEMENT);
		int getChild(int keyIndex, const char * KeyName);
		int getNextChild(int keyIndex);
		int getProperty(int id, const char * propertyName);
		int getFirstProperty(int keyIndex);
		int getNextProperty(int propertyIndex);
		char * getPropertyName(int propertyIndex);
		char * getPropertyValue(int propertyIndex);
		float getPropertyValue(int propertyIndex, float defaultValue);
		char * getPropertyComment(int propertyIndex);
		int getChildCount(int id);

	private:
		clJobFile m_JobFile;
		clError m_error;


		struct tyConfigValue
		{
			int index;
			char * name;
			char * value;
			char type;
			char * comment;
			tyConfigValue * next_property;
		};


		struct tyConfigKey
		{
			int index;
			tyConfigKey * parent; //- root of this item
			tyConfigKey * first_client; //- first leave on this item
			tyConfigKey * last_client; //- last leave on this item
			tyConfigKey * next_item; //- next item in this branch
			tyConfigValue * first_property; // => tyConfigValue
			tyConfigValue * last_property; // => tyConfigValue
			char * name;
		};


		tyConfigValue * m_values;
		int m_values_length;
		int m_values_count;

		tyConfigKey * m_keys;
		int m_keys_length;
		int m_keys_count;

		/// <summary>Interpret the job-data from [m_Buffer]</summary>
		bool interpret();

		/// <summary>creates the [m_values] and [m_keys]</summary>
		bool createBuffers(const char * pBuffer, int bufferLen);

		/// <summary>find the position of the first appearance of a byte</summary>
		int findByte(const char * pInBuffer, int lineLen, char Byte2Find);

		/// <summary>retunes first byte not ' ' and '\t'</summary>
		char * LTrim(char * buffer);

		/// <summary>set all ' ' and '\t' at the end to '\0'</summary>
		char * RTrim(char * buffer);

		/// <summary>set all ' ' and '\t' at the end to '\0' and returns the first char-pos</summary>
		char * Trim(char * buffer);

		/// <summary>helper for recursion output</summary>
		void printXMLout(int deep, tyConfigKey * key);
};

#endif
