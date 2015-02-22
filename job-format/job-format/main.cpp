#include "main.h"


int main(int argc, char* argv [])
{
	
	clJobFileInterpreter jobFile;

	if (argc > 1)
	{
		jobFile.readFromFile(argv[1]);
		jobFile.printXML();
	}

}



