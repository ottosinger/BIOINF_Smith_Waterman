#include "main.h"


/*	This function counts the number of bytes in a given file without the first line
 *	@param *fp -> file to read
 *	return -> filesize in bytes counting out the first line
 */
int countFileSize(FILE *fp){
	int size;
	char c;

	size = 0;
	// Skip the first line
	for(;;){
		c = fgetc(fp);
		if (c == '\n'){
			break;
		}
	}
	for(;;){
		c = fgetc(fp);
		if (c == EOF)
			break;
		if ((c != '\n') && (c != '\r'))
			size++;
	}
	rewind(fp);
	return size;
}

/*	This function counts the number of bytes in line of a genome in a given file
 *	@param *fp -> file to read
 *	return -> lineSize in bytes
 */
int lineSize(FILE *fp){
	char c;
	int lineSize;
	fpos_t pos;

	lineSize = 0;
	// Skip the first line
	for(;;){
		c = fgetc(fp);
		if (c == '\n')
			break;
	}
	fgetpos(fp, &pos);

	for(;;){
		c = fgetc(fp);
		if (c == EOF || c == '\n')
			break;
		lineSize++;
	}
	fsetpos(fp, &pos);
	return lineSize;
}

void removeAll(char * str, const char toRemove)
{
    int i, j;
    int len = strlen(str);

    for(i=0; i<len; i++)
    {
        /*
         * If the character to remove is found then shift all characters to one
         * place left and decrement the length of string by 1.
         */
        if(str[i] == toRemove)
        {
            for(j=i; j<len; j++)
            {
                str[j] = str[j+1];
            }

            len--;

            // If a character is removed then make sure i doesn't increments
            i--;
        }
    }
}

void printMatrix(int* matrix,int rows, int cols, char* inputGenome1, char* inputGenome2){
	for (int i = 0; i <= rows + 1; i++){
		for (int j = 0; j <= cols + 1; j++){
			if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == 0)){
				printf("   ");
				continue;
			}
			else if (i == 0)
				printf("%3c", inputGenome1[j - 2]);
			else if (j == 0)
				printf("%3c", inputGenome2[i - 2]);
			else
				printf("%3d", *(matrix + (i-1)*cols + (j-1)));
		}
		printf("\n");
	}
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
	fileOutput = fopen(argv[7], "w+");

	printf("---------------------------------------------\n");
	printf("Reading data from files...\n");

	sizeOfInputFile1 = countFileSize(fileInput1);
	sizeOfInputFile2 = countFileSize(fileInput2);

	printf("Size of input file1: %d\n", sizeOfInputFile1);
	printf("Size of input file2: %d\n", sizeOfInputFile2);

	lineSize1 = lineSize(fileInput1);
	lineSize2 = lineSize(fileInput2);

	printf("Size of a line in file1: %d\n", lineSize1);
	printf("Size of a line in file2: %d\n", lineSize2);

	inputGenome1 = (char *) malloc(sizeOfInputFile1 + 1);
	inputGenome2 = (char *) malloc(sizeOfInputFile2 + 1);

	char line1[lineSize1+1], line2[lineSize2+1];

	while(fgets(line1, sizeof(line1), fileInput1) != NULL){
		strcat(inputGenome1, line1);
	}
	removeAll(inputGenome1, '\r');
	removeAll(inputGenome1, '\n');

	while(fgets(line2, sizeof(line2), fileInput2) != NULL){
		strcat(inputGenome2, line2);
	}
	removeAll(inputGenome2, '\r');
	removeAll(inputGenome2, '\n');

	printf("---------------------------------------------\n");
	
	printf("File 1:\n\n");
	printf("%s\n", inputGenome1);
	printf("---------------------------------------------\n");

	printf("File 2:\n\n");
	printf("%s\n", inputGenome2);
	printf("---------------------------------------------\n");
	printf("Filling the matrix...\n\n");

	int matrixOfSimilarity[sizeOfInputFile2][sizeOfInputFile1];
	int i,j;

	for (i = 0; i <= sizeOfInputFile2; i++){
		for (j = 0; j <= sizeOfInputFile1; j++){
			matrixOfSimilarity[i][j] = i + j;
		}
	}


	// TODO: neki kurac ne ispisuje dobro
	printMatrix((int *)matrixOfSimilarity, sizeOfInputFile2, sizeOfInputFile1, inputGenome1, inputGenome2);

	// Ispis matrice
//	for (i = 0; i <= sizeOfInputFile2 + 1; i++){
//		for (j = 0; j <= sizeOfInputFile1 + 1; j++){
//			if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == 0)){
//				printf("   ");
//				continue;
//			}
//			else if (i == 0)
//				printf("%3c", inputGenome1[j - 2]);
//			else if (j == 0)
//				printf("%3c", inputGenome2[i - 2]);
//			else
//				printf("%3d", matrixOfSimilarity[i - 1][j - 1]);
//		}
//		printf("\n");
//	}



	fclose(fileInput1);
	fclose(fileInput2);

	free(inputGenome1);
	free(inputGenome2);

}
