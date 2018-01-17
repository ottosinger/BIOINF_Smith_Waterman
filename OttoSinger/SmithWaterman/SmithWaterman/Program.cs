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

            int[] maxKnowledge = CalculateMatrix(firstGenome, secondGenome, InitialMatrix);
            int maxScore = maxKnowledge[0];
            int maxI = maxKnowledge[1];
            int maxJ = maxKnowledge[2];

            PrintMatrix(firstGenome.Length, secondGenome.Length, InitialMatrix, firstGenome, secondGenome);

            List<int[]> jea = Traceback(InitialMatrix, maxKnowledge);

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
            int[,] InitialMatrix = new int[lengthOfSecondGenome+1, lengthOfFirstGenome+1];  //dodano +1 na size zbog 1 reda i 1 stupca sa nulama

            for (int i = 0; i < lengthOfSecondGenome+1; i++)
            {
                for (int j = 0; j < lengthOfFirstGenome+1; j++) 
                {
                    InitialMatrix[i, j] = 0;
                }
            }
            return InitialMatrix;
        }

        public static void PrintMatrix(int lengthOfFirstGenome, int lengthOfSecondGenome, int[,] Matrix, string firstGenome, string SecondGenome)
        {
            Console.WriteLine("\n \nGenome Matrix: \n");
            
            Console.Write("               ");
            for (int i = 0; i < lengthOfFirstGenome; i++)
            {
                Console.Write(i.ToString().PadRight(5));
            }
            Console.WriteLine();
            
            Console.Write("               ");
            for (int i = 0; i < lengthOfFirstGenome; i++)
            {
                Console.Write(firstGenome[i].ToString().PadRight(5));
            }
            Console.WriteLine();
            
            for (int i = 0; i < lengthOfSecondGenome+1; i++) 
            {
                for (int j = 0; j < lengthOfFirstGenome+1; j++)
                {
                    if (j == 0 && i == 0)
                    {
                        Console.Write("          ");
                    }
                    if (j == 0 && i > 0)
                    {
                        Console.Write((i-1).ToString().PadRight(5) + SecondGenome[i-1].ToString().PadRight(5));
                    }
                    Console.Write(Matrix[i, j].ToString().PadRight(5));
                }
                Console.WriteLine();
            }
        }



        public static int[] CalculateMatrix(string firstGenome, string secondGenome, int[,] Matrix)
        {
            int lengthOfFirstGenome = firstGenome.Length;
            int lengthOfSecondGenome = secondGenome.Length;
            
            int maxScore = 0;
            int maxI = 0;
            int maxJ = 0;

            for (int i = 1; i < lengthOfSecondGenome+1; i++)
            {
                for (int j = 1; j < lengthOfFirstGenome+1; j++)
                {
                    int score = CalculateScore(Matrix, i, j, firstGenome, secondGenome);
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxI = i-1;
                        maxJ = j-1;
                    }
                    Matrix[i,j] = score;
                }
            }

            int[] maxKnowledge = { maxScore, maxI, maxJ };
            return maxKnowledge;
        }


        public static int CalculateScore(int[,] Matrix, int i, int j, string firstGenome, string secondGenome)
        {
            int similarity = 0;
            int match = 10;
            int mismatch = -9;
            int insertion = -10;
            int deletion = -10;
            //int weight = 0;

            if (firstGenome[j-1] == secondGenome[i-1])
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

            int score = CalculateMaximum(0, diagonalValue, upValue, leftValue);//IMPLEMENTIRAJ MAKSIMALNU FUNKCIJU ZA 4 PARAMETRA

            return score;
        }



        public static int CalculateMaximum(int zero, int diagonalValue, int upValue, int leftValue)
        {
            return Math.Max(zero, Math.Max(diagonalValue, Math.Max(upValue, leftValue)));
        }







        public static List<int[]> Traceback(int[,] Matrix, int[] maxKnowledge)
        {
            int maxScore = maxKnowledge[0];
            int maxI = maxKnowledge[1];
            int maxJ = maxKnowledge[2];

            List<int[]> listOfElements = new List<int[]>();
            //array has 4 values: [element score, element x value, element y value, direction of move from previous(0-initial value, 1-diagonal, 2-left, 3-up)]
            listOfElements.Add(maxKnowledge);

            //diagonal: match / mismatch
            //up: gap in sequence 1
            //left: gap in sequence 2

            //start position of traceback
            int x = maxI;
            int y = maxJ;
            
            string previousElement = NextMove(Matrix , x+1, y+1);// ove plus jedinice na x i y su zbog prvog reda i stupca nula pa je sve u banani sa indexima

            while (previousElement != "stop" || previousElement != "invalid move")
            {
                if (previousElement == "diagonal")
                {
                    int[] helper = { Matrix[x+1 - 1, y+1 - 1], x - 1, y - 1, 1 };//DODANO
                    listOfElements.Add(helper);
                    x--;
                    y--;
                }
                else
                {
                    if (previousElement == "up")
                    {
                        int[] helper = { Matrix[x+1 - 1, y+1], x - 1, y, 3 }; //DODANO
                        listOfElements.Add(helper);
                        x--;
                    }
                    else
                    {
                        if (previousElement == "left")
                        {
                            int[] helper = { Matrix[x+1, y+1 - 1], x, y - 1, 2 };//DODANO
                            listOfElements.Add(helper);
                            y--;
                        }
                        else
                        {
                            //Console.WriteLine("end of matrix (genome) reached.");
                            break;
                        }
                    }
                }
                previousElement = NextMove(Matrix, x+1, y+1);// ove plus jedinice na x i y su zbog prvog reda i stupca nula pa je sve u banani sa indexima
            }
            return listOfElements;
        }


        public static string NextMove(int[,] Matrix, int x, int y)
        {
            int diagonal = Matrix[x - 1, y - 1];
            int up = Matrix[x - 1, y];
            int left = Matrix[x, y - 1];

            if (diagonal >= up && diagonal >= left)
            {
                if (diagonal != 0)
                {
                    return "diagonal";
                }
                else
                {
                    return "stop";
                }
            }
            else
            {
                if (up >= diagonal && up >= left)
                {
                    if (up != 0)
                    {
                        return "up";
                    }
                    else
                    {
                        return "stop";
                    }
                }
                else
                {
                    if (left >= diagonal && left >= up)
                    {
                        if (up != 0)
                        {
                            return "left";
                        }
                        else
                        {
                            return "stop";
                        }
                    }
                    else
                    {
                        return "invalid move";
                    }
                }
            }
        }





    }//class program
}//namespace
