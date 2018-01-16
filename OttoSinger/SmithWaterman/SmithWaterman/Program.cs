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
            PrintFiles(bothFiles);
            ParseFile(bothFiles[0]);
            ParseFile(bothFiles[1]);





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

        public static void ParseFile(string MyFile)
        {
            //Console.WriteLine("Myfile u parse metodi tj genom je : " + MyFile);

            string genome = RemoveFirstLinesFromFile(MyFile, 1);
            
            int genomeLength = CalculateGenomeLength(genome);

            genome = Regex.Replace(genome, @"\s+", string.Empty);

            PrintGenome(genome);
            PrintStatisticsOfGenome(genome, genomeLength);
            //PrintNucleotidesOfGenome(genome);

        }

        public static string RemoveFirstLinesFromFile(string text, int linesCount)
        {
            var lines = Regex.Split(text, "\r\n|\r|\n").Skip(linesCount);
            return string.Join(Environment.NewLine, lines.ToArray());
        }

        public static int CalculateGenomeLength(string genome)
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
            Console.WriteLine("Genom in its final form: \n" + genome);
        }

        public static string InitiateMatrix(string firstGenome, string secondGenome)
        {



            //TU SAM STAO #######################################################


            return "";
        }








        

    }//class program
}//namespace
