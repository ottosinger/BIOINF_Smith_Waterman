#include "main.h"


/*	This function counts the number of bytes in a given file without the first line
 *	@param *fp -> file to read
 *	return -> filesize in bytes counting out the first line
 */
int countFileSize(FILE *fp){
	int size, firstLineSize;
	char c;

	firstLineSize = 0;

	// Skip the first line
	do{
		c = fgetc(fp);
		firstLineSize++;
	} while( c != '\n');

	fseek(fp, 0L, SEEK_END);
	size = ftell(fp);
	rewind(fp);

	return (size - firstLineSize);
}

/*	This function counts the number of bytes in line of a genome in a given file
 *	@param *fp -> file to read
 *	return -> lineSize in bytes
 */
int lineSize(FILE *fp){
	char c;
	int lineSize;

	// Skip the first line
	do{
		c = fgetc(fp);
	} while( c != '\n');

	for(;;){
		c = fgetc(fp);
		if (c == EOF || c == '\n')
			break;
		lineSize++;
	}
	return lineSize;
}

int main(int argc, char *argv[]) {

	FILE *fileInput1, *fileInput2, *fileOutput;
	int weightMatch, weightMismatch, weightInsertion, weightDeletion;
	int sizeOfInputFile1, sizeOfInputFile2;
	int lineSize1, lineSize2;
	char* inputGenome1;
	char* inputGenome2;

	if (argc != 8) {
		printf("ERROR: Invalid number of arguments!\n");
		return 0;
	}

	// Putting input arguments into variables
	fileInput1 = fopen(argv[1], "r");
	fileInput2 = fopen(argv[2], "r");
	weightMatch = atoi(argv[3]);
	weightMismatch = atoi(argv[4]);
	weightInsertion = atoi(argv[5]);
	weightDeletion = atoi(argv[6]);
	fileOutput = fopen(argv[7], "w");

	printf("---------------------------------------------\n");
	printf("Reading data from files...\n");

	sizeOfInputFile1 = countFileSize(fileInput1);
	sizeOfInputFile2 = countFileSize(fileInput2);

	lineSize1 = lineSize(fileInput1);
	lineSize2 = lineSize(fileInput2);

	inputGenome1 = (char *) malloc(sizeOfInputFile1);
	inputGenome2 = (char *) malloc(sizeOfInputFile2);

	char line1[lineSize1 + 1], line2[lineSize2 + 1];

	while(fgets(line1, sizeof(line1), fileInput1) != NULL){


	}

	printf("---------------------------------------------\n");
	

}
