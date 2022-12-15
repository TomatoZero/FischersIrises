using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;

namespace CreateRandomData {
    internal class Program {
        public static void Main(string[] args) {
            CreateCarrData("InputData.txt", "OutputData.txt", 20);
        }

        public static List<double[]> CreateCarrData(string fileNameInput, string fileNameOutput, int numbPerClass) {
            var randomData = new List<double[]>();
            var AllClassType = ReadData(fileNameInput);
            Random rnd = new Random();
            
            using(var writer = new StreamWriter(fileNameOutput, false)) {
                for (var i = 0; i < AllClassType.Count; i++) {
                    for (var j = 0; j < numbPerClass; j++) {
                        
                        if(AllClassType[i].ClassType == "SUV")
                            continue;
                        
                        writer.Write(RandomDoubleBetween(AllClassType[i].Length, rnd).ToString("0.") + " ");
                        writer.Write(RandomDoubleBetween(AllClassType[i].Width, rnd).ToString("0.") + " ");
                        writer.Write(RandomDoubleBetween(AllClassType[i].Height, rnd).ToString("0.") + " ");
                        writer.Write(RandomDoubleBetween(AllClassType[i].DistanceBetweenWheels, rnd).ToString("0.") + " ");
                        writer.Write(RandomDoubleBetween(AllClassType[i].GroundClearance, rnd).ToString("0.00") + " ");
                        // writer.Write(RandomDoubleBetween(AllClassType[i].EngineVolume, rnd).ToString("0.00") + " ");
                        writer.Write(RandomDoubleBetween(AllClassType[i].Weight, rnd).ToString("0.00") + " ");
                        writer.Write(RandomDoubleBetween(AllClassType[i].Speed, rnd).ToString("0.") + " ");
                        writer.Write(RandomDoubleBetween(AllClassType[i].HundredIn3Sec, rnd).ToString("0.00") + " ");
                        // writer.Write(RandomDoubleBetween(AllClassType[i].LiterPerHundredMil, rnd).ToString("0.00") + " ");
                        writer.Write(AllClassType[i].ClassType + "\n");
                    }
                }
            }
            
            return randomData;
        }

        public static double RandomDoubleBetween(double[] data, Random rnd) =>
            rnd.NextDouble() * (data[1] - data[0]) + data[0];

        public static List<CarClass> ReadData(string fileName) {
            List<CarClass> carClass = new List<CarClass>();
            try {
                using (var reader = new StreamReader(fileName, Encoding.UTF8)) {
                    while (!reader.EndOfStream) {
                        var list = reader.ReadLine().Trim().Split();

                        string classType = list[0];
                        var length = new double[2] { Double.Parse(list[1]), Double.Parse(list[2]) };
                        var width = new double[2] { Double.Parse(list[3]), Double.Parse(list[4]) };
                        var height = new double[2] { Double.Parse(list[5]), Double.Parse(list[6]) };
                        var distanceBetweenWheels = new double[2] { Double.Parse(list[7]), Double.Parse(list[8]) };
                        var groundClearance = new double[2] { Double.Parse(list[9]), Double.Parse(list[10]) };
                        var engineVolume = new double[2] { Double.Parse(list[11]), Double.Parse(list[12]) };
                        var weight = new double[2] { Double.Parse(list[13]), Double.Parse(list[14]) };
                        var speed = new double[2] { Double.Parse(list[15]), Double.Parse(list[16]) };
                        var hundredIn3Sec = new double[2] { Double.Parse(list[17]), Double.Parse(list[18]) };
                        var literPerHundredMil = new double[2] { Double.Parse(list[19]), Double.Parse(list[20]) };
                        
                        carClass.Add(new CarClass(classType, length, width, height, distanceBetweenWheels, 
                            groundClearance, engineVolume, weight, speed, hundredIn3Sec, literPerHundredMil));
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }

            return carClass;
        }
    }


    class CarClass {
        public string ClassType { get; set; }
        public double[] Length { get; set; }
        public double[] Width { get; set; }
        public double[] Height { get; set; }
        public double[] DistanceBetweenWheels { get; set; }
        public double[] GroundClearance { get; set; }
        public double[] EngineVolume { get; set; }
        public double[] Weight { get; set; }
        public double[] Speed { get; set; }
        public double[] HundredIn3Sec { get; set; }
        public double[] LiterPerHundredMil { get; set; }
        
        public CarClass(string classType, double[] length, double[] width, double[] height, double[] distanceBetweenWheels, double[] groundClearance, double[] engineVolume, double[] weight, double[] speed, double[] hundredIn3Sec, double[] literPerHundredMil) {
            ClassType = classType;
            Length = length;
            Width = width;
            Height = height;
            DistanceBetweenWheels = distanceBetweenWheels;
            GroundClearance = groundClearance;
            EngineVolume = engineVolume;
            Weight = weight;
            Speed = speed;
            HundredIn3Sec = hundredIn3Sec;
            LiterPerHundredMil = literPerHundredMil;
        }
    }
}