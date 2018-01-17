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

	/*	Count the number of bytes in a file excluding \n and \r
	 */
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

	// Memorize current position so we can return file pointer to the start of the important part of the file
	fgetpos(fp, &pos);

	// Iterate until end of line
	for(;;){
		c = fgetc(fp);
		if (c == EOF || c == '\n')
			break;
		lineSize++;
	}

	// Return the file pointer to the memorized position
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

void printMatrix(int* matrix, int rows, int cols, char* inputGenome1, char* inputGenome2){
	for (int i = 0; i < rows + 1; i++){
		for (int j = 0; j < cols + 1; j++){
			if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == 0)){
				printf("   ");
			}

			// Print the first input genome
			else if (i == 0)
				printf("%3c", inputGenome1[j - 2]);

			// Print the second input genome
			else if (j == 0)
				printf("%3c", inputGenome2[i - 2]);

			// Print the matrix of similarity
			else
				printf("%3d", *((matrix + (i-1) * cols) + j-1));
		}
		printf("\n");
	}
}

int	calculateDirection(int diagonal, int left, int up, int wMismatch, int wDeletion, int wInsertion){
	int max;
	max = ((diagonal - wMismatch) >= (left - wDeletion)) ? (((diagonal - wMismatch) >= (up - wInsertion)) ? 0 : 2) : (((left - wDeletion) >= (up - wInsertion)) ? 1 : 2 );
	return max;
}

int roundToZero(int number, int weight){
	int result;
	result = number + weight;
	if (result < 0)
		return 0;
	else
		return result;
}

char checkForMax(int current[], int tempRow, int tempCol){
	int sum1 = current[1] + current[2];
	int sum2 = tempCol + tempRow;
	if (sum1 < sum2)
		return 0;
	if (sum1 == sum2)
		if (current[1] < tempRow)
			return 0;
	return 1;
}

int* fillMatrix(int* matrix, int rows, int cols, int wMatch, int wMismatch, int wInsertion, int wDeletion, char* inputGenome1, char* inputGenome2){

	int direction;
	static int maxNumber[] = {0, 0, 0};

	for(int i = 0; i < rows; i++){
		for (int j = 0; j < cols; j++){

			// Fill first line and column with zeros
			if((i == 0) || (j == 0)){
				matrix[i * cols + j] =  0;
				continue;
			}
			if (inputGenome1[j] == inputGenome2[i]){
				matrix[i * cols + j] = matrix[(i-1) * cols + (j-1)] + wMatch;
			} else {
				direction = calculateDirection(matrix[(i-1)*cols + (j-1)], matrix[i*cols + (j-1)], matrix[(i-1)*cols + j], wMismatch, wDeletion, wInsertion);
				if (direction == 0)
					matrix[i * cols + j] = roundToZero(matrix[(i-1) * cols + (j-1)], wMismatch);
				else if (direction == 1)
					matrix[i * cols + j] = roundToZero(matrix[i * cols + (j-1)], wDeletion);
				else
					matrix[i * cols + j] = roundToZero(matrix[(i-1) * cols + j], wInsertion);
			}
			if (matrix[i * cols + j] > maxNumber[0]){
				maxNumber[0] = matrix[i * cols + j];
				maxNumber[1] = i;
				maxNumber[2] = j;
			} else if (matrix[i * cols + j] == maxNumber[0]){
				char decision = checkForMax(maxNumber, i, j);
				if (decision){
					maxNumber[0] = matrix[i * cols + j];
					maxNumber[1] = i;
					maxNumber[2] = j;
				}
			}
		}
	}
	return maxNumber;
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

	int rows = sizeOfInputFile2 + 1;
	int columns = sizeOfInputFile1 + 1;
	int matrixOfSimilarity[rows][columns];
	int *maxAlign;

//	for (int i = 0; i < rows; i++){
//		for (int j = 0; j < columns; j++){
//			matrixOfSimilarity[i][j] = i + j;
//		}
//	}

	maxAlign = fillMatrix((int *) matrixOfSimilarity, rows, columns, weightMatch, weightMismatch, weightInsertion, weightDeletion, inputGenome1, inputGenome2);
	printMatrix((int *)matrixOfSimilarity, rows, columns, inputGenome1, inputGenome2);

	printf("Maximum number: %5d; Location: Row %5d, Column %5d", maxAlign[0], maxAlign[1], maxAlign[2]);

	 //Ispis matrice
//		for (int i = 0; i <= sizeOfInputFile2 + 1; i++){
//			for (int j = 0; j <= sizeOfInputFile1 + 1; j++){
//				if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == 0)){
//					printf("   ");
//					continue;
//				}
//				else if (i == 0)
//					printf("%3c", inputGenome1[j - 2]);
//				else if (j == 0)
//					printf("%3c", inputGenome2[i - 2]);
//				else
//					printf("%3d", matrixOfSimilarity[i - 1][j - 1]);
//			}
//			printf("\n");
//		}

	fclose(fileInput1);
	fclose(fileInput2);

	free(inputGenome1);
	free(inputGenome2);

}
