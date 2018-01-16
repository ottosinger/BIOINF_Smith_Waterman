using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmithWaterman
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] bothFiles = ReadFiles();
            //PrintFiles(bothFiles);

            string firstGenome = ParseFile(bothFiles[0]);
            string secondGenome = ParseFile(bothFiles[1]);

            PrintGenome(firstGenome);
            Console.WriteLine("length of 1st genome main programme: " + firstGenome.Length);
            //PrintNucleotidesOfGenome(firstGenome);

            PrintGenome(secondGenome);
            Console.WriteLine("length of 2nd genome main programme: " + secondGenome.Length);
            //PrintNucleotidesOfGenome(secondGenome);


            int[,] InitialMatrix = InitiateMatrix(firstGenome, secondGenome);

            PrintMatrix(firstGenome.Length, secondGenome.Length, InitialMatrix, firstGenome, secondGenome);

            CalculateMatrix(firstGenome, secondGenome, InitialMatrix);

            PrintMatrix(firstGenome.Length, secondGenome.Length, InitialMatrix, firstGenome, secondGenome);

            Console.ReadKey();
        }





        public static string[] ReadFiles()
        {
            string firstFile = System.IO.File.ReadAllText(@"C:\Users\ottos\Downloads\Chrome\testni1.fa");
            string secondFile = System.IO.File.ReadAllText(@"C:\Users\ottos\Downloads\Chrome\testni2.fa");
            string[] bothFiles = { firstFile, secondFile };
            return bothFiles;
        }

        public static void PrintFiles(string[] bothFiles)
        {
            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine("File number " + (i+1) + ": \n" + bothFiles[i]);
                Console.WriteLine();
            }
        }

        public static string ParseFile(string MyFile)
        {
            //Console.WriteLine("Myfile in ParseFile method is : \n" + MyFile);

            string genome = RemoveFirstLinesFromFile(MyFile, 1);

            //int numberOfLinesInGenomeFile = NumberOfLinesInGenome(genome);
            //PrintStatisticsOfGenome(genome, numberOfLinesInGenomeFile);
            
            genome = RemoveSpacesFromGenome(genome);
            
            return genome;
        }

        public static string RemoveFirstLinesFromFile(string text, int linesCount)
        {
            var lines = Regex.Split(text, "\r\n|\r|\n").Skip(linesCount);
            return string.Join(Environment.NewLine, lines.ToArray());
        }

        public static string RemoveSpacesFromGenome(string genome)
        {
            genome = Regex.Replace(genome, @"\s+", string.Empty);
            return genome;
        }

        public static int NumberOfLinesInGenome(string genome)
        {
            var lines = Regex.Split(genome, "\r\n|\r|\n");
            return lines.Length;
        }

        public static void PrintNucleotidesOfGenome(string genome)
        {
            for (int i = 0; i < genome.Length; i++)
            {
                Console.WriteLine(i + 1 + "th nucleotide of genome: " + genome[i]);
            }
        }

        public static void PrintStatisticsOfGenome(string genome, int genomeLength)
        {
            Console.WriteLine("Number of lines in file (genome) is: " + genomeLength);
            Console.WriteLine("Number of nucleotides in genome: " + genome.Length);
        }

        public static void PrintGenome(string genome)
        {
            Console.WriteLine("Genome in its final form: \n" + genome);
        }

        public static int[,] InitiateMatrix(string firstGenome, string secondGenome)
        {
            int lengthOfFirstGenome = firstGenome.Length;
            int lengthOfSecondGenome = secondGenome.Length;
            int[,] InitialMatrix = new int[lengthOfSecondGenome, lengthOfFirstGenome]; 

            for (int i = 0; i < lengthOfSecondGenome; i++)
            {
                for (int j = 0; j < lengthOfFirstGenome; j++) 
                {
                    InitialMatrix[i, j] = 0;
                }
            }
            return InitialMatrix;
        }


        public static void PrintMatrix(int lengthOfFirstGenome, int lengthOfSecondGenome, int[,] Matrix, string firstGenome, string SecondGenome)
        {
            Console.WriteLine("\n \nGenome Matrix: \n");

            Console.Write("    " + "    ");
            for (int i = 0; i < lengthOfFirstGenome; i++)
            {
                Console.Write(i + "   ");
            }
            Console.WriteLine();
            Console.Write("    " + "    ");
            for (int i = 0; i < lengthOfFirstGenome; i++)
            {
                if (i < 10)
                {
                    Console.Write(firstGenome[i] + "   ");
                }
                else
                {
                    Console.Write(firstGenome[i] + "    ");
                }

            }
            Console.WriteLine();
            
            for (int i = 0; i < lengthOfSecondGenome; i++) 
            {
                if (i < 10)
                    Console.Write(i + "   " + SecondGenome[i] + "   ");
                else
                    Console.Write(i + "  " + SecondGenome[i] + "   ");
                for (int j = 0; j < lengthOfFirstGenome; j++)
                {
                    if (j < 10)
                    {
                        if (Matrix[i, j] < 10)
                        {
                            Console.Write(Matrix[i, j] + "   ");
                        }
                        else
                        {
                            Console.Write(Matrix[i, j] + "  ");
                        } 
                    }
                    else
                    {
                        if (Matrix[i, j] < 10)
                        {
                            Console.Write(Matrix[i, j] + "    ");
                        }
                        else
                        {
                            Console.Write(Matrix[i, j] + "   ");
                        }
                    }
                }
                Console.WriteLine();
            }
        }



        public static int[,] CalculateMatrix(string firstGenome, string secondGenome, int[,] Matrix)
        {
            int lengthOfFirstGenome = firstGenome.Length;
            int lengthOfSecondGenome = secondGenome.Length;
            
            int maxScore = 0;
            int maxI = 0;
            int maxJ = 0;
            
            for (int i = 0; i < lengthOfSecondGenome; i++)
            {
                for (int j = 0; j < lengthOfFirstGenome; j++)
                {
                    int score = CalculateScore(Matrix, i, j, firstGenome, secondGenome);
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxI = i;
                        maxJ = j;
                    }
                    Matrix[i,j] = score;
                }
            }

            return Matrix;
        }


        public static int CalculateScore(int[,] Matrix, int i, int j, string firstGenome, string secondGenome)
        {
            int similarity = 0;
            int match = 10;
            int mismatch = 0;
            int insertion = 7;
            int deletion = 6;
            int weight = 0;

            if (firstGenome[i] == secondGenome[j])
            {
                similarity = match;
            }
            else
            {
                similarity = mismatch;
            }

            int diagonalValue = Matrix[i - 1, j - 1] + similarity;
            int upValue = Matrix[i - 1, j] + insertion;//NISAM SIGURAN KOJI JE INSERTION A KOJI DELETION
            int leftValue = Matrix[i, j - 1] + deletion;//NISAM SIGURAN KOJI JE INSERTION A KOJI DELETION

            //return max(0, diagonalValue, upValue, leftValue);//IMPLEMENTIRAJ MAKSIMALNU FUNKCIJU ZA 4 PARAMETRA
            return 0;
        }





        }//class program
}//namespace
