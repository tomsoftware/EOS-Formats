#include "clJobFileInterpreter.h"


clJobFileInterpreter::clJobFileInterpreter()
	: m_error("clJobFileInterpreter")
{
	m_values = NULL;
	m_values_length = 0;
	m_values_count = 0;

	m_keys = NULL;
	m_keys_length = 0;
	m_keys_count = 0;
}

//---------------------------------------------------//
clJobFileInterpreter::~clJobFileInterpreter()
{
	if (m_keys != NULL)
	{
		delete[] m_keys;
		m_keys = NULL;
	}
	m_keys_length = 0;


	if (m_values != NULL)
	{
		delete[] m_values;
		m_values = NULL;
	}
	m_values_length = 0;
}


//---------------------------------------------------//
bool clJobFileInterpreter::readFromFile(const char * filename)
{
	if (m_JobFile.readFromFile(filename) == NULL)
	{
		m_error.AddError("readFromFile() failed");
		return false;
	}

	return interpret();
}


//---------------------------------------------------//
bool clJobFileInterpreter::readFromBuffer(const char * buffer, int bufferLength)
{

	if (m_JobFile.readFromBuffer(buffer, bufferLength) == NULL)
	{
		m_error.AddError("readFromBuffer() failed");
		return false;
	}

	return interpret();
}


//---------------------------------------------------//
bool clJobFileInterpreter::interpret()
{
	char * pBuffer = m_JobFile.getBuffer();
	int bufferLen = m_JobFile.getBufferLenght();
	
	if (pBuffer == NULL)
	{
		m_error.AddError("interpret() no data");
		return false;
	}


	//- first count keys and propertys and create buffers
	createBuffers(pBuffer, bufferLen);

	
	//- add root item
	m_keys_count = 1;
	m_keys[0].name = (char *)"_root_";
	m_keys[0].first_client = NULL;
	m_keys[0].last_client = NULL;
	m_keys[0].first_property = NULL;
	m_keys[0].last_property = NULL;
	m_keys[0].next_item = NULL;
	m_keys[0].parent = NULL;
	m_keys[0].index = 0;

	//- add dummy item for index 0
	m_values_count = 1;
	m_values[0].comment = NULL;
	m_values[0].name = NULL;
	m_values[0].type = '\0';
	m_values[0].value = NULL;
	m_values[0].next_property = NULL;
	m_values[0].index = 0;


	char * pLineStart = pBuffer;
	bool isFirstChar = true;
	int lineLen = 0;

	int parents[100]; //- lookup tabel for parrents
	//- set lookup-table to root element
	for (int i = 0; i<100; i++) parents[i] = 0;

	int currentLeafe = 0;


	for (int i = bufferLen; i > 0; i--)
	{
		char value = *pBuffer;
		if ((value == '\n') || (value == '\r')) //- is this a "\n" or "\r" or "\r\n"
		{
			if (lineLen > 1)
			{
				if (*pLineStart == '[')
				{
					//-------------------------
					//-- this is a KEY-Element

					int pos = findByte(pLineStart, lineLen, ']');
					if (pos < 1)
					{
						m_error.AddError("Syntax-Error. Missing ']'");
					}
					else
					{
						int newDeep = std::atoi(pLineStart + 1);
						if (newDeep > 99) newDeep = 99;

						if (newDeep > 0) //- every element has to be in the root-element
						{
							//- close the "m_keys[newKey].name" string
							*pBuffer = '\0';

							int newKey = m_keys_count;
							m_keys_count++;

							//- set values for this key
							m_keys[newKey].index = newKey;
							m_keys[newKey].parent = &m_keys[parents[newDeep - 1]];
							m_keys[newKey].name = Trim(pLineStart + pos + 1);
							m_keys[newKey].first_client = NULL;
							m_keys[newKey].last_client = NULL;
							m_keys[newKey].next_item = NULL;
							m_keys[newKey].first_property = NULL;
							m_keys[newKey].last_property = NULL;


							//- update parrent
							if (m_keys[newKey].parent->first_client == NULL) m_keys[newKey].parent->first_client = &m_keys[newKey];

							if (m_keys[newKey].parent->last_client == NULL)
							{
								m_keys[newKey].parent->last_client = &m_keys[newKey];
							}
							else
							{
								//- update the next-item property of the last item on this level 
								m_keys[newKey].parent->last_client->next_item = &m_keys[newKey];
								m_keys[newKey].parent->last_client = &m_keys[newKey];
							}

							//- save state
							parents[newDeep] = newKey;
							currentLeafe = newKey;
						}
						else
						{
							currentLeafe = 0;
						}
					}
				}
				else
				{
					//-------------------------
					//-- this is a Property

					int pos = findByte(pLineStart, lineLen, '=');
					if (pos < 1)
					{
						m_error.AddError("Syntax-Error. Missing '='");
					}
					else
					{
						//- close the property-name string: "m_value[newValue].name"
						*(pLineStart + pos) = '\0';
						//- close the property-value string: "m_value[newValue].value"
						*pBuffer = '\0';


						int newValue = m_values_count;
						m_values_count++;

						char * value = LTrim(pLineStart + pos + 1);
						char * comment = NULL;

						int c_pos = findByte(value, lineLen, '#');
						if (c_pos > 0)
						{
							*(value + c_pos) = '\0';
							comment = value + c_pos + 1;

							RTrim(value); //- remove all spaces at the end of value
						}

						m_values[newValue].index = newValue;
						m_values[newValue].name = Trim(pLineStart);
						m_values[newValue].value = value + 1;
						m_values[newValue].type = *value; //- the type is in the first char
						m_values[newValue].comment = LTrim(comment);
						m_values[newValue].next_property = NULL;

						//- update parrent
						if (m_keys[currentLeafe].first_property == NULL)
						{
							m_keys[currentLeafe].first_property = &m_values[newValue];
						}
						else
						{
							//- update last property in this key
							m_keys[currentLeafe].last_property->next_property = &m_values[newValue];
						}

						m_keys[currentLeafe].last_property = &m_values[newValue];

					}
				}
				//- this is a valid line
			}


			//- is this a "\r\n" ?
			if ((value == '\r') && (*pBuffer == '\n')) pBuffer++;

			//- next start pos
			isFirstChar = true;
			pLineStart = pBuffer + 1;
			lineLen = 0;
		}
		else
		{
			if (isFirstChar)
			{
				//- jump over spaces on the right
				if ((value != ' ') && (value != '\t'))
				{
					isFirstChar = false;
					pLineStart = pBuffer;
					lineLen = 0;
				}
			}
			lineLen++;
		}


		pBuffer++;
	}


	return true;
}


//---------------------------------------------------//
bool clJobFileInterpreter::printXML()
{
	if (m_JobFile.getBuffer() == NULL)
	{
		m_error.AddError("job-file is not open");
		return false;
	}

	if (m_keys == NULL)
	{
		m_error.AddError("no data in file");
		return false;	
	}
	printXMLout(0, m_keys);

	return true;
}


//---------------------------------------------------//
void clJobFileInterpreter::printXMLout(int deep, tyConfigKey * key)
{
	char padding[100];

	for (int i = 0; i < deep; i++) padding[i] = '\t';
	padding[deep] = '\0';

	printf("%s<%s>\n", padding, key->name);

	//- print propertys
	tyConfigValue * value = key->first_property;
	while (value != NULL)
	{
		printf("%s\t<value name=\"%s\" type=\"%c\" value=\"%s\" ", padding, value->name, value->type, value->value);
		
		if (value->comment != NULL) printf("comment=\"%s\" ", value->comment);

		printf("/>\n");

		value = value->next_property;
	}

	//- print childs
	tyConfigKey * child = key->first_client;
	while (child != NULL)
	{
		printXMLout(deep+1, child);
		child = child->next_item;
	}

	printf("%s</%s>\n", padding, key->name);
}


//---------------------------------------------------//
bool clJobFileInterpreter::createBuffers(const char * pBuffer, int bufferLen)
{
	//-------------------
	//- count keys and propertys
	int keys_c = 0;
	int prop_c = 0;
	int lineLen = 0;
	bool doneFirstChar = false;

	//- free old buffers
	if (m_values != NULL) delete[] m_values;
	m_values = NULL;

	if (m_keys != NULL) delete[] m_keys;
	m_keys = NULL;

	//- count
	for (int i = bufferLen; i > 0; i--)
	{
		char value = *pBuffer;
		if ((value == '\n') || (value == '\r')) //- is this a "\n" or "\r" or "\r\n"
		{
			doneFirstChar = false;

			//- is this a "\r\n" ?
			if ((value == '\r') && (*pBuffer == '\n')) pBuffer++;
		}
		else if (!doneFirstChar)
		{
			//- ignore left free-spaces
			if ((value != ' ') && (value != '\t'))
			{
				doneFirstChar = true;

				if (value == '[')
				{
					keys_c++;
				}
				else
				{
					prop_c++;
				}

			}
		}

		pBuffer++;
	}

	//- create buffers
	m_values_length = prop_c + 2; //- +1 for 0 item
	m_values = new tyConfigValue[m_values_length];
	m_values_count = 0;

	m_keys_length = keys_c + 2; //- +1 for root item
	m_keys = new  tyConfigKey[m_keys_length];
	m_keys_count = 0;
	


	return true;
}



//---------------------------------------------------//
int clJobFileInterpreter::findByte(const char * pInBuffer, int lineLen, char Byte2Find)
{
	const char * pIn = pInBuffer;

	for (int i = 0; i < lineLen; i++)
	{
		char v = *pIn;

		if (v == Byte2Find) return i;
		if (v == '\0') return -1;

		pIn++;
	}
	return -1;
}


//---------------------------------------------------//
char * clJobFileInterpreter::LTrim(char * buffer)
{
	char * pIn = buffer;
	if (pIn == NULL) return NULL;

	while (*pIn != '\0')
	{
		char v = *pIn;
		if ((v != ' ') && (v != '\t')) return pIn;
		pIn++;
	}
	return pIn;
}

//---------------------------------------------------//
char * clJobFileInterpreter::RTrim(char * buffer)
{
	char * pIn = buffer;
	if (pIn == NULL) return NULL;

	char * lastChar = pIn;

	while (*pIn != '\0')
	{
		char v = *pIn;
		if ((v != ' ') && (v != '\t')) lastChar = pIn;
		pIn++;
	}

	*(lastChar + 1) = '\0';

	return pIn;
}



//---------------------------------------------------//
char * clJobFileInterpreter::Trim(char * buffer)
{
	char * pIn = buffer;
	if (pIn == NULL) return NULL;

	//- find first non space
	while (*pIn != '\0')
	{
		char v = *pIn;

		if ((v != ' ') && (v != '\t'))
		{
			buffer = pIn;
			break;
		}
		pIn++;
	}


	//- stripe last emptys
	char * lastChar = pIn;

	while (*pIn != '\0')
	{
		char v = *pIn;
		if ((v != ' ') && (v != '\t')) lastChar = pIn;
		pIn++;
	}

	*(lastChar + 1) = '\0';

	return buffer;
}

//---------------------------------------------------//
char * clJobFileInterpreter::getKeyName(int id)
{
	if ((id < 0) || (id >= m_keys_count)) return NULL;
	return m_keys[id].name;
}

//---------------------------------------------------//
int clJobFileInterpreter::getFirstChild(int id)
{
	if ((id < 0) || (id >= m_keys_count)) return -1;
	if (m_keys[id].first_client == NULL) return -1;

	return m_keys[id].first_client->index;
}

//---------------------------------------------------//
int clJobFileInterpreter::getNextChild(int id)
{
	if ((id < 0) || (id >= m_keys_count)) return -1;
	if (m_keys[id].next_item == NULL) return -1;

	return m_keys[id].next_item->index;
}

//---------------------------------------------------//
int clJobFileInterpreter::getChild(int id, const char * keyName)
{
	if ((id < 0) || (id >= m_keys_count)) return -1;
	if (m_keys[id].first_client == NULL) return -1;


	int key = m_keys[id].first_client->index;

	while (key > 0)
	{
		if (_strcmpi(keyName, m_keys[key].name) == 0) return key;

		if (m_keys[key].next_item <= 0) return -1;
		key = m_keys[key].next_item->index;
	}

	return -1;
}


//---------------------------------------------------//
int clJobFileInterpreter::getChildCount(int id)
{
	if ((id < 0) || (id >= m_keys_count)) return 0;
	if (m_keys[id].first_client == NULL) return 0;

	int count = 0;

	int key = m_keys[id].first_client->index;

	while (key > 0)
	{
		count++;
		if (m_keys[key].next_item <= 0) return count;
		key = m_keys[key].next_item->index;
	}

	return count;
}

//---------------------------------------------------//
int clJobFileInterpreter::getProperty(int id, const char * propertyName)
{
	if ((id < 0) || (id >= m_keys_count)) return -1;
	if (m_keys[id].first_property == NULL) return -1;

	int prop = m_keys[id].first_property->index;

	while (prop > 0)
	{
		if (_strcmpi(propertyName, m_values[prop].name) == 0) return prop;
		
		if (m_values[prop].next_property == NULL) return -1;
		prop = m_values[prop].next_property->index;
	}


	return -1;
}


//---------------------------------------------------//
int clJobFileInterpreter::getFirstProperty(int id)
{
	if ((id < 0) || (id >= m_keys_count)) return -1;
	if (m_keys[id].first_property == NULL) return -1;

	return m_keys[id].first_property->index;
}


//---------------------------------------------------//
int clJobFileInterpreter::getNextProperty(int id)
{
	if ((id < 0) || (id >= m_values_count)) return -1;
	if (m_values[id].next_property == NULL) return -1;

	return m_values[id].next_property->index;
}

//---------------------------------------------------//
char * clJobFileInterpreter::getPropertyName(int id)
{
	if ((id < 0) || (id >= m_values_count)) return NULL;
	return m_values[id].name;
}

//---------------------------------------------------//
char * clJobFileInterpreter::getPropertyValue(int id)
{
	if ((id < 0) || (id >= m_values_count)) return NULL;
	return m_values[id].value;
}

//---------------------------------------------------//
float clJobFileInterpreter::getPropertyValue(int id, float defaultValue)
{
	char * val = getPropertyValue(id);
	if (val == NULL) return defaultValue;


	char * e;
	errno = 0;
	float x = (float)std::strtod(val, &e);

	if (*e != '\0' ||  // error, we didn't consume the entire string
		errno != 0)   // error, overflow or underflow
	{
		return defaultValue;
	}


	return x;
}


//---------------------------------------------------//
char * clJobFileInterpreter::getPropertyComment(int id)
{
	if ((id < 0) || (id >= m_values_count)) return NULL;
	return m_values[id].comment;
}
