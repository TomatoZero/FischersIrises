using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TheMethodOfLeastSquares
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Console.WriteLine($"Write n:");
            // int n = int.Parse(Console.ReadLine().Trim());
            // double[,] input = new double[n,2]; 
            // for (var i = 0; i < n; i++) {
            //     var arr = Console.ReadLine()?.Trim().Split().Select(s => Double.Parse(s)).ToArray();
            //     if (arr != null) {
            //         input[i, 0] = arr[0];
            //         input[i, 1] = arr[1];
            //     }
            // }
            //
            // Console.WriteLine(input.GetLength(0));
            //
            // Console.WriteLine($"{n+1} {FindNext(input)}");

            var input = GetData("input.txt");
            var briefInput = new double[input.Count - 1, input[0].Length];

            for (var i = 0; i < briefInput.GetLongLength(0); i++) {
                for (var j = 0; j < briefInput.GetLongLength(1); j++) {
                    briefInput[i, j] = input[i][j];
                }
            }

            var output = FindCoefficients(briefInput);
            var forecast = output.a * input.Count - 1 + output.b;
            var real = input[input.Count - 1];

            Console.WriteLine($"Next year {real[1]} Calculated value {forecast}. Real value {real[0]}. Error {(Math.Abs(real[0] - forecast) / real[0])* 100}");
            
        }

        public static (double a, double b) FindCoefficients(double[,] input)
        {
            var numb = 0d;
            var sumY = 0d;
            var sumX = 0d;
            var sumSquaresX = 0d;
            var n = input.GetLength(0);
            for (int i = 0; i < n; i++) {
                numb += input[i, 0] * (i + 1);
                sumY += input[i, 0];
                sumX += (i + 1);
                sumSquaresX += Math.Pow((i + 1), 2);
            }

            var a = (numb - ((sumX * sumY) / n)) / (sumSquaresX - ((sumX * sumX) / n));
            var b = (sumY / n) - (a * sumX) / n;

            // var meanAbsoluteError = new double[n];
            // var sumMeanAbsoluteError = 0d;
            // for (var i = 0; i < n; i++)
            // {
            //     var theoreticalY = (a * (i + 1)) + b;
            //
            //     meanAbsoluteError[i] = (Math.Abs(input[i, 0] - theoreticalY) / input[i, 0]) * 100;
            //     sumMeanAbsoluteError += meanAbsoluteError[i];
            // }
            
            return (a, b);
        }
        
        
        private static List<double[]> GetData(string path)
        {
            var data = new List<double[]>();

            using (var streamReader = new StreamReader(path, Encoding.UTF8))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine()?.Trim().Split();

                    if (line == null) continue;

                    Console.WriteLine(String.Join(" ", line));
                    
                    data.Add(new double[2] { Double.Parse(line[0]), Double.Parse(line[1])});
                }
            }

            // var output = new double[data.Count, data[0].Length];
            //
            // for (var i = 0; i < data.Count; i++) {
            //     for (var j = 0; j < data[1].Length; j++) {
            //         output[i, j] = data[i][j];
            //     }
            // }
            return data;
        }
    }
}