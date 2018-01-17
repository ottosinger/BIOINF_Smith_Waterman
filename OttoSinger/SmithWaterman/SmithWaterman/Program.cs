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
            int match = 10;
            int mismatch = -9;
            int insertion = -10;
            int deletion = -10;
            int[] weight = { match, mismatch, insertion, deletion };

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

            int[] maxKnowledge = CalculateMatrix(firstGenome, secondGenome, InitialMatrix, weight);
            int maxScore = maxKnowledge[0];
            int maxI = maxKnowledge[1];
            int maxJ = maxKnowledge[2];

            PrintMatrix(firstGenome.Length, secondGenome.Length, InitialMatrix, firstGenome, secondGenome);

            List<int[]> jea = Traceback(InitialMatrix, maxKnowledge, weight);
            //array has 4 values: {element score, element x value, element y value, direction (ACTION) of move from 
            //previous [actually later one in reverse list] (10-diagonal-match_PREPISI, 11-diagonal missmatch_SAMO DRUGI NAPISI,
            //2 -left-deletion_OBRISI, 3-up-insertion_crtica)}



            if (firstGenome[jea.Last()[2]] != secondGenome[jea.Last()[1]])
            {
                jea.Remove(jea.Last());
            }

            jea.Reverse();

            
            int startOfAlignmentFirstGenome = jea.First()[2];
            int startOfAlignmentSecondGenome = jea.First()[1];

            int endOfAlignmentFirstGenome = jea.Last()[2];
            int endOfAlignmentSecondGenome = jea.Last()[1];

            string alignmentString = getAlignmentString(jea, firstGenome, secondGenome);


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



        public static int[] CalculateMatrix(string firstGenome, string secondGenome, int[,] Matrix, int[] weight)
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
                    int score = CalculateScore(Matrix, i, j, firstGenome, secondGenome, weight);
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


        public static int CalculateScore(int[,] Matrix, int i, int j, string firstGenome, string secondGenome, int[] weight)
        {
            int similarity = 0;
            int match = weight[0];
            int mismatch = weight[1];
            int insertion = weight[2];
            int deletion = weight[3];

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







        public static List<int[]> Traceback(int[,] Matrix, int[] maxKnowledge, int[] weight)
        {
            int maxScore = maxKnowledge[0];
            int maxI = maxKnowledge[1];
            int maxJ = maxKnowledge[2];

            int[] maxKnowledgeUpgrade = { maxScore, maxI, maxJ, 10 };//najveci element uvijek je "dijete" match-a, znaci ide 10 unutra

            List<int[]> listOfElements = new List<int[]>();
            //array has 4 values: [element score, element x value, element y value, direction of move from previous(0-initial value, 10-diagonal-match, 11-diagonal missmatch, 2-left, 3-up)]
            listOfElements.Add(maxKnowledgeUpgrade);

            //diagonal: match / mismatch
            //up: gap in sequence 1
            //left: gap in sequence 2

            //start position of traceback
            int x = maxI;
            int y = maxJ;
            
            string previousElement = NextMove(Matrix , x+1, y+1, weight);// ove plus jedinice na x i y su zbog prvog reda i stupca nula pa je sve u banani sa indexima

            while (previousElement != "stop" && previousElement != "invalid move")
            {
                if (previousElement == "diagonal-match" || previousElement == "diagonal-mismatch")
                {
                    int[] helper = { Matrix[x + 1 - 1, y + 1 - 1], x - 1, y - 1, 0};//DODANO

                    if (previousElement == "diagonal-match")
                    {
                        helper[3] = 10;//MATCH
                    }
                    else
                    {
                        helper[3] = 11;//MISMATCH
                    };
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
                previousElement = NextMove(Matrix, x+1, y+1, weight);// ove plus jedinice na x i y su zbog prvog reda i stupca nula pa je sve u banani sa indexima
            }
            return listOfElements;
        }


        public static string NextMove(int[,] Matrix, int x, int y, int[] weight)
        {
            //OVDJE X I Y IMAJU PRAVU VRIJEDNOST, MOZES KORISTITI INDEXE U ISPISU
            int match = weight[0];
            int mismatch = weight[1];
            int insertion = weight[2];
            int deletion = weight[3];

            int diagonal = Matrix[x - 1, y - 1];
            int up = Matrix[x - 1, y];
            int left = Matrix[x, y - 1];

            int current = Matrix[x, y];

            if (diagonal + match == current || diagonal + mismatch == current)//diagonal >= up && diagonal >= left
            {
                if (diagonal != 0)
                {
                    if (diagonal + match == current)
                    {
                        return "diagonal-match";
                    }
                    else //(diagonal + mismatch == current)
                    {
                        return "diagonal-mismatch";
                    }
                    //OVI PODUVJETI GORE ZAMIJENILI OVAJ DOLJE
                    //return "diagonal";
                }
                else
                {
                    return "stop";
                }
            }
            else
            {
                if (up + insertion == current)//up >= diagonal && up >= left
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
                    if (left + deletion == current)//left >= diagonal && left >= up
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


        public static string getAlignmentString(List<int[]> jea, string firstGenome, string secondGenome)
        {
            string alignment = "";

            foreach (var item in jea)
            {
                if (item[3] == 10)//MATCH
                {
                    int yCoord = item[1];
                    alignment += secondGenome[yCoord];
                }
                else
                {
                    if (item[3] == 11)//MISMATCH
                    {
                        int yCoord = item[1] + 1;//+1 zbog prvog reda i stupca sa nulama
                        alignment += secondGenome[yCoord].ToString().ToLower();
                    }
                    else
                    {
                        if (item[3] == 2)//DELETION
                        {
                            alignment += ".";
                        }
                        else
                        {
                            if (item[3] == 3)//INSERTION
                            {
                                alignment += "-";
                            }
                            else
                            {
                                Console.WriteLine("error plz help");
                            }
                        }
                    }
                }
            }//foreach
            return alignment;
        }




    }//class program
}//namespace
