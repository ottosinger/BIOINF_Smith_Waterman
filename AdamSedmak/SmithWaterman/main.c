/*  Watterman - Eggert algorithm in C
 *  @author: Adam Sedmak
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>


// Double linked list which holds data for each trace - value inside the cell, row, column and nucleobase

struct trace{
	int score, row, col;
	char sign;
	trace *next;
	trace *prev;

};


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

	lineSize = 0;

	// Skip the first line
	for(;;){
		c = fgetc(fp);
		if (c == '\n')
			break;
	}

	// Iterate until end of line
	for(;;){
		c = fgetc(fp);
		if (c == EOF || c == '\n')
			break;
		lineSize++;
	}

	// Return the file pointer to the memorized position
	rewind(fp);
	return lineSize;
}

/*  Function that removes all appearances of a character in a string
 *  @param *str -> string to remove all characters from
 *  @param toRemove -> character to remove
 */
void removeAll(char* str, const char toRemove)
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

            // If a character is removed then make sure i doesn't increment
            i--;
        }
    }
}

/*	Function that prints given matrix and nucleobases in the first row and column
 *  @param matrix -> matrix to print
 *  @param rows -> number of rows in a matrix
 *  @param cols -> number of colums in a matrix
 *  @param inputGenome1 -> referent genome, writen above columns
 *  @param inputGenome2 -> second genome, writen left of rows
 */
void printMatrix(int* matrix, int rows, int cols, char* inputGenome1, char* inputGenome2){
	for (int i = 0; i < rows + 1; i++){
		for (int j = 0; j < cols + 1; j++){

			// For upper left corner, print only spaces
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


/*  Function that calculates number which will be written in the matrix. If the number is less than zero, print zero, else print that number
 *  @param number -> number of current cell in matrix
 *  @param weight -> weight of the action we are about to perform (match, mismatch, insertion, deletion)
 *  return -> sum of number and weight, or zero if the sum is smaller than zero
 */
int roundToZero(int number, int weight){
	int result;
	result = number + weight;
	if (result < 0)
		return 0;
	else
		return result;
}

/*  Function that checks whether the current cell value(that is equal than the one calculated before) is considered optimal for the local alignment.
 * 	It implements equations (5) and (6) from the Watterman-Eggert algorithm paper.
 * 	@param current -> value, row and column of the current max value in the cell
 * 	@param tempRow -> row of the cell we are in now, that has the same value as current max value
 * 	@param tempCol -> column of the cell we are in now, that has the same value as current max value
 * 	return -> 1 if current cell is optimal, 0 if the cell we are in now is optimal
 */
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

/*  Function that fills the given matrix based on given genome inputs and weights
 * 	@param matrix -> matrix to fill
 * 	@param rows -> number of rows in matrix
 * 	@param cols -> number of columns in matrix
 * 	@param wMatch -> weight of matching genome nucleobases
 * 	@param wMismatch -> weight of mismatch in genome nucleobases
 * 	@param wInsertion -> weight of insertion in genome nucleobases
 * 	@param wDeletion -> weight of deletion in genome nucleobases
 * 	@param inputGenome1 -> referent genome sequence
 * 	@param inputGenome2 -> other genome sequence
 * 	return -> optimal and maximum value for a local alignment in a matrix
 */
int* fillMatrix(int* matrix, int rows, int cols, int wMatch, int wMismatch, int wInsertion, int wDeletion, char* inputGenome1, char* inputGenome2){

	char direction;
	// Maximum and optimal number in matrix, {value, row, column}
	static int maxNumber[] = {0, 0, 0};

	for(int i = 0; i < rows; i++){
		for (int j = 0; j < cols; j++){

			// Fill first line and column with zeros
			if((i == 0) || (j == 0)){
				matrix[i * cols + j] =  0;
				continue;
			}
			// If the nucleobase of referent and other genome are the same, calculate the value of the cell
			if (inputGenome1[j-1] == inputGenome2[i-1]){
				matrix[i * cols + j] = matrix[(i-1) * cols + (j-1)] + wMatch;
			} else {
				// Ternary operator that decides will we get the biggest value from mismatch, deletion or insertion, and returns, accordingly, 0, 1, 2
				direction = ((diagonal + wMismatch) >= (up + wInsertion)) ? (((diagonal + wMismatch) >= (left + wDeletion)) ? 0 : 2) : (((up + wInsertion) >= (left + wDeletion)) ? 1 : 2 );
				// If the biggest value is from mismatch, calculate it's value and put it to matrix
				if (direction == 0)
					matrix[i * cols + j] = roundToZero(matrix[(i-1) * cols + (j-1)], wMismatch);
				// Biggest value is from deletion, calculate value and put into matrix
				else if (direction == 1)
					matrix[i * cols + j] = roundToZero(matrix[(i-1) * cols + j], wInsertion);
				// Biggest value is from insertion, calculate value and put into matrix
				else
					matrix[i * cols + j] = roundToZero(matrix[i * cols + (j-1)], wDeletion);
			}
			// If the current cell value is bigger than before calculated maximum, current value becomes max value
			if (matrix[i * cols + j] > maxNumber[0]){
				maxNumber[0] = matrix[i * cols + j];
				maxNumber[1] = i;
				maxNumber[2] = j;
			// If the current cell value is equal than before calculated maximum, call checkForMax and do according to it's output
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

/*	Function that calculates reversed alignment traceback and stores it into a double linked list
 * 	@param lastTrace -> maximum and optimal value from which we start the traceback
 * 	@param matrix -> matrix in which we will calculate traceback
 * 	@param initCell -> value, row and column of the maximum and optimal value
 * 	@param rows -> number of rows in matrix
 * 	@param cols -> number of columns in matrix
 * 	@param wMatch -> weight of a match
 * 	@param wMismatch -> weight of a mismatch
 * 	@param wInsertion -> weight of an insertion
 * 	@param wDeletion -> weight of a deletion
 * 	@param inputGenome1 -> referent genome sequence
 * 	@param inputGenome2 -> other genome sequence
 * 	return -> trace where we end the traceback
 */
trace traceback(trace *lastTrace, int* matrix, int* initCell, int rows, int cols, int wMatch, int wMismatch, int wInsertion, int wDeletion, char* inputGenome1, char* inputGenome2){
	int i = initCell[1];
	int j = initCell[2];
	int diagonal, left, up;

	trace* prevTrace = lastTrace;

	// Do the traceback until we reach the cell value of zero
	while(matrix[i*cols + j] != 0){
		// Allocate space for the newTrace
		trace* newTrace = malloc(sizeof(trace));
		diagonal = 0;
		left = 0;
		up = 0;

		// This section calculates the direction from where we could have come using previous values and weights for each action
		// If we could have come from one direction, store the previous value in local variable
		if ((matrix[i*cols + j] == matrix[(i-1)*cols + (j-1)] + wMatch) && (inputGenome1[j-1] == inputGenome2[i-1]))
			diagonal = matrix[(i-1)*cols + (j-1)];
		if ((matrix[i*cols + j] == matrix[(i-1)*cols + (j-1)] + wMismatch) && (inputGenome1[j-1] != inputGenome2[i-1]))
			diagonal = matrix[(i-1)*cols + (j-1)];
		if (matrix[i*cols + j] == matrix[(i-1)*cols + j] + wInsertion)
			up = matrix[(i-1)*cols + j];
		if (matrix[i*cols + j] == matrix[i*cols + (j-1)] + wDeletion)
			left = matrix[i*cols + (j-1)];

		// In this section we are calculating the previous step. Previous cell is the one which is the biggest. If they are equal, priority is diagonal, up, left
		if((diagonal >= up) && (diagonal >= left)){
			newTrace->score = matrix[i*cols + j];
			newTrace->row = i;
			newTrace->col = j;
			newTrace->sign = inputGenome2[i - 1];	// Write nucleobase value to structure
			i -= 1;
			j -= 1;
		} else if (up >= left){
			newTrace->score = matrix[i*cols + j];
			newTrace->row = i;
			newTrace->col = j;
			newTrace->sign = inputGenome2[i - 1];
			i -= 1;
		} else {

			newTrace->score = matrix[i*cols + j];
			newTrace->row = i;
			newTrace->col = j;
			newTrace->sign = '-';					// If there was deletion, write dash
			j -= 1;
		}
		// Adjust pointers for double linked list
		prevTrace->prev = newTrace;
		newTrace->next = prevTrace;
		newTrace->prev = NULL;

		prevTrace = newTrace;
	}
	return *prevTrace;
}


int main(int argc, char *argv[]) {

	FILE *fileInput1, *fileInput2, *fileOutput;
	int weightMatch, weightMismatch, weightInsertion, weightDeletion;
	int sizeOfInputFile1, sizeOfInputFile2;
	int lineSize1, lineSize2;
	char* inputGenome1;
	char* inputGenome2;
	char firstLine1[100];
	char firstLine2[100];
	char *parseLine1;
	char *parseLine2;

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

	// Read first line
	fgets(firstLine1, sizeof(firstLine1), fileInput1);

	// Extract third part of the first line which is the name of genome
	parseLine1 = strtok(firstLine1, ":");
	parseLine1 = strtok(NULL, ":");
	parseLine1 = strtok(NULL, ":");

	fgets(firstLine2, sizeof(firstLine2), fileInput2);

	parseLine2 = strtok(firstLine2, ":");
	parseLine2 = strtok(NULL, ":");
	parseLine2 = strtok(NULL, ":");

	// Allocate memory for the string which is input genome
	inputGenome1 = (char *) malloc(sizeOfInputFile1 + 1);
	inputGenome2 = (char *) malloc(sizeOfInputFile2 + 1);

	// String for reading line by line
	char line1[lineSize1+1], line2[lineSize2+1];

	// Read all lines line by line and add it to one string
	while(fgets(line1, sizeof(line1), fileInput1) != NULL){
		removeAll(line1, '\r');
		removeAll(line1, '\n');
		if(line1[0] == '>')
			break;
		strcat(inputGenome1, line1);
	}

	while(fgets(line2, sizeof(line2), fileInput2) != NULL){
		removeAll(line2, '\r');
		removeAll(line2, '\n');
		if(line1[0] == '>')
			break;
		strcat(inputGenome2, line2);
	}

	int rows = sizeOfInputFile2 + 1;
	int columns = sizeOfInputFile1 + 1;
	int matrixOfSimilarity[rows][columns];
	int *maxAlign;

	trace lastTrace, curTrace, firstTrace;

	maxAlign = fillMatrix((int *) matrixOfSimilarity, rows, columns, weightMatch, weightMismatch, weightInsertion, weightDeletion, inputGenome1, inputGenome2);

	lastTrace.score = maxAlign[0];
	lastTrace.row = maxAlign[1];
	lastTrace.col = maxAlign[2];
	lastTrace.sign = inputGenome2[lastTrace.row - 1];
	lastTrace.next = NULL;
	lastTrace.prev = NULL;

	// Calculate traceback
	firstTrace = traceback(&lastTrace, (int *) matrixOfSimilarity, maxAlign, rows, columns, weightMatch, weightMismatch, weightInsertion, weightDeletion, inputGenome1, inputGenome2);

	curTrace = firstTrace;

	int n = 1;

	// Calculate number of alignments so we can create string of that length
	while(curTrace.next != NULL){
		curTrace = *curTrace.next;
		n++;
	}


	// Fill the string alignment with all nucleobases that are aligned
	char alignment[n-1];
	curTrace = firstTrace;
	n = 1;
	while(curTrace.next != NULL){
		alignment[n-1] = curTrace.sign;
		curTrace = *curTrace.next;
		n++;
	}


	// Calculate size of alignment without dashes
	n=0;
	int count = 0;
	while(alignment[n] != '\0'){
		if(alignment[n] != '-')
			count++;
		n++;
	}

	// Write data to the output file
	fprintf(fileOutput, "track name=%s\n", parseLine1);
	fprintf(fileOutput, "##maf version=1\n\n");
	fprintf(fileOutput, "a score=%d.0\n", lastTrace.score);
	fprintf(fileOutput, "s %s %d %d + %d %s\n", parseLine2, firstTrace.row, count, sizeOfInputFile2, alignment);

	fclose(fileInput1);
	fclose(fileInput2);
	fclose(fileOutput);

	free(inputGenome1);
	free(inputGenome2);
}
