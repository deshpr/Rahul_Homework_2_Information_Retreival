using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Homework_2
{
    class Program
    {

        public static bool IsDone(Matrix<double> difference, int nodeCount)
        {
            int total = 0;
            bool hasNan = false;
            for (int i = 0; i < nodeCount; i++)
            {
                if (Double.IsNaN(difference[i, 0]))
                {
                    return true;                                        
                }
                if(difference[i,0] < 0.00001)
                {
                    total += 1;
                }
            }
            return total == nodeCount;
        }


        public static bool CheckAnswer(Matrix<double> rankMatrix, int nodeCount)
        {
            double sum = 0;
            for (int i = 0; i < nodeCount; i++)
            {
                sum += rankMatrix[i, 0];
            }
            Console.WriteLine("Sum: " + sum);
            return sum == 1;
        }

        static void Main(string[] args)
        {
            
            string inputPath = @"C:\UW Tacoma\Information Retrieval\turn_in\\Input_Matrix.txt";
            string outputPath = @"C:\Users\Rahul Deshpande\Desktop\Output_final_1.txt";

            var transitionMatrix = Matrix<double>.Build.Dense(6,6, (i, j) => 0);
            var matrixContent = System.IO.File.ReadAllText(inputPath);
            var rows = matrixContent.Split('\n');
            foreach(var row in rows)
            {
                var info = row.Split(',');
                int rowId = Convert.ToInt32(info[0]);
                int columnId = Convert.ToInt32(info[1]);
                int value = Convert.ToInt32(info[2]);
                transitionMatrix[rowId, columnId] = value;
            }

            double Beta = 0.85;
            double e = 1.0;
            // Starting all values are equal
            int nodeCount = 6;

            // Set the ranks
            for (int i = 0; i < nodeCount; i++)
            {
                double sum = 0;
                for(int j = 0; j < nodeCount; j++)
                {
                    sum += transitionMatrix[j, i]; 
                }
                double value = 1 / sum;
                for (int j = 0; j < nodeCount; j++)
                {
                    if(transitionMatrix[j,i] != 0)
                    {
                        transitionMatrix[j, i] = value;
                    }
                }

            }

            Console.WriteLine("Output");
            File.AppendAllText(outputPath, "Transition Matrix:");
            File.AppendAllText(outputPath, Environment.NewLine);
            File.AppendAllText(outputPath, transitionMatrix.ToMatrixString());
            File.AppendAllText(outputPath, "\n");
            Console.WriteLine(transitionMatrix.ToString());

            var other = Matrix<double>.Build.Dense(nodeCount, 1, (i, j) => ((1.0 - Beta) * e)/(Convert.ToDouble(4)));

            var rankedMatrix = Matrix<double>.Build.Dense(nodeCount, 1, (i, j) => 1.0 / (Convert.ToDouble(nodeCount)));
            Console.WriteLine("Initial rank matrix");
            Console.WriteLine(rankedMatrix);
            File.AppendAllText(outputPath, "Original rank matrix:");
            File.AppendAllText(outputPath, Environment.NewLine);
            File.AppendAllText(outputPath, rankedMatrix.ToMatrixString());
            File.AppendAllText(outputPath, Environment.NewLine);

            int iterationCount = 0;
            while (true)
            {
                iterationCount += 1;
                Console.WriteLine($"At iteration = {iterationCount}");
                var newRankedMatrix = Beta * transitionMatrix * rankedMatrix + other;
                Console.WriteLine("new ranked matrix");
                Console.WriteLine(newRankedMatrix);
                var difference = newRankedMatrix - rankedMatrix;
                Console.WriteLine("difference matrix");
                Console.WriteLine(difference);

                if (IsDone(difference, nodeCount))
                {
                    rankedMatrix = newRankedMatrix;
                    break;
                }
                rankedMatrix = newRankedMatrix;
                Console.WriteLine("Now ranked matrix");
                    Console.WriteLine(rankedMatrix);
            }

            var res =CheckAnswer(rankedMatrix, nodeCount);
            if (res)
            {
                Console.WriteLine("Correct Answer!");
            }
            Console.WriteLine($"Iteration Count: {iterationCount}");
            Console.WriteLine("Converged ranked matrix");
            Console.WriteLine(rankedMatrix);
            File.AppendAllText(outputPath, "Converged rank matrix:");
            File.AppendAllText(outputPath, Environment.NewLine);
            File.AppendAllText(outputPath, rankedMatrix.ToMatrixString());
            File.AppendAllText(outputPath, Environment.NewLine);
            File.AppendAllText(outputPath, $"Iteration Count: {iterationCount}");
            Console.WriteLine("Program complete.");
            Console.ReadLine();

        }
    }
}
