/*** Source code of RocheeeLimit.exe ***/
/*** To create binary for Linux, compile with .NET Core SDK or .NET 5 SDK ***/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RocheeeLimit
{
    class Program
    {
        const int X = 0, Y = 1, Z = 2;
        static bool reError = true;
        static void Main()
        {
            string[] pars = { "TargetLine=2", "ModifyPattern=", "InitialValue=100", "Step=10", "MaxIteration=100", "RedirectStdError=true", "Standard=0.00004"};
            if (File.Exists("setup.par")) pars = File.ReadAllLines("setup.par");
            if (File.Exists("./output/csv/out.csv")) File.Delete("./output/csv/out.csv");
            if (File.Exists("temp.ss")) File.Delete("temp.ss");
            else
            {
                Console.WriteLine("First run，generating default setup file...");
                File.WriteAllLines("setup.par", pars);
            }
            int target_line = Convert.ToInt32(pars[0].Split('=')[1])-1;
            string template = pars[1].Split('=')[1];
            double initial_value = Convert.ToDouble(pars[2].Split('=')[1]);
            double step = Convert.ToDouble(pars[3].Split('=')[1]);
            int number = Convert.ToInt32(pars[4].Split('=')[1]);
            reError = Convert.ToBoolean(pars[5].Split('=')[1]);
            double std = Convert.ToDouble(pars[6].Split('=')[1]);
            Console.WriteLine($"Target modifying line number：{target_line}");
            Console.WriteLine($"Modifying pattern：{template}");
            Console.WriteLine($"Initial value：{initial_value}");
            Console.WriteLine($"Step：{step}");
            Console.WriteLine($"Repeat times：{number}");
            Console.WriteLine($"Destruction standard: {std} km");
            string[,] table = new string[401, number + 1];
            for (int n = 0; n < number; n++)
            {
                Console.WriteLine($"\n【Simulation#{n+1}/{number}】：");
                double vari = initial_value + n * step;
                string[] rpg_par = File.ReadAllLines("rpg.par");
                rpg_par[target_line] = template.Replace("$", vari.ToString());
                File.WriteAllLines("rpg.par", rpg_par);
                Console.WriteLine($"Writing {rpg_par[target_line]} to line {target_line+1}.");
                table[0, n] = "x="+vari;
                double real_density = ExtractDensity(RunCmd("./rpg"));
                ChangeDelta(real_density);
                Console.Write("Running rpx.sh...");
                RunBash("rpx.sh");
                Console.WriteLine("Done.");
                Console.Write("Running calc.sh...");
                RunBash("calc.sh");
                Console.WriteLine("Done.");
                Console.Write("Running bt.sh...");
                RunBash("bt.sh");
                Console.WriteLine("Done.");
                Console.Write("Calculating median...");
                string re = "Frame,Distance Median\n";
                bool found = false;
                for (int i = 1; i <= 400; i++)
                {
                    string frame_num = i.ToString().PadLeft(5, '0');
                    var distances = DumpDistances($"./output/bt/out_file.{frame_num}.bt");
                    double md = distances[distances.Length / 2];//MedianDistance($"./output/bt/out_file.{frame_num}.bt");
                    table[i, n] = md.ToString();
                    
                    if (!found && md > std)
                    {
                        Console.Write($"\tSuccesfully located the destruction frame[{i}].\n\tExtracting COM position data...");
                        double[] pos = ExtractPos(i);
                        double d = Math.Sqrt(pos[X] * pos[X] + pos[Y] * pos[Y] + pos[Z] * pos[Z]);
                        Console.WriteLine($"\n\tCOM position is ({pos[X]}, {pos[Y]}, {pos[Z]})；Distance is {d}");
                        Console.WriteLine($"\tAppending to csv file...");
                        File.AppendAllText("./output/csv/out.csv", $"\n{vari},{d}");
                        found = true;
                    } 
                    re += $"{i},{md}\n";
                }
                Console.WriteLine("Calculation all done.");
                Console.WriteLine("Write to csv file...");
                if (!Directory.Exists("output/csv/")) Directory.CreateDirectory("output/csv/");
                File.WriteAllText($"./output/csv/distance_median@{vari}.csv", re);
                Console.WriteLine($"Frame[{n}] Done.");
            }
            Console.WriteLine($"Producing overall data table...");
            string str = "";
            for (int frame = 0; frame < 401; frame++)
            {
                for (int col = 0; col < number; col++)
                {
                    str += table[frame, col] + ",";
                }
                str += "\n";
            }
            File.WriteAllText($"all{DateTime.Now:HH_mm_ss}.csv", str);
            Console.WriteLine("All done.");
        }

        static void ChangeDelta(double density)
        {
            double G = 6.674e-11;
            double dyt = 1 / Math.Sqrt(2 * G * Convert.ToInt32(density));
            Console.WriteLine($"The dynamic time is {dyt}.");
            double dDelta = Math.PI * dyt / 1.577e8;
            Console.WriteLine($"dDelta = {dDelta}.");
            string[] ss_par = File.ReadAllLines("ss.par");
            ss_par[9] = $"dDelta\t= {dDelta}\t\t# maximum step in units of yr / 2 pi";
            File.WriteAllLines("ss.par", ss_par);
            Console.WriteLine($"Successfully changed dDelta to {dDelta}");
        }
        static double ExtractDensity(string rpg_out)
        {
            string line = rpg_out.Split('\n')[6];
            Console.WriteLine(line);
            string density = line.Split(' ')[3].Split('=')[1];
            Console.WriteLine($"Actual bulk density：{density}");
            return Convert.ToDouble(density);
        }
        static double[] ExtractPos(int frame)
        {
            string file_name = $"out_file.{frame.ToString().PadLeft(5, '0')}";
            File.Copy($"output/ss/{file_name}", "temp.ss");
            //4.#Centre-of-mass#position#=#-329582.22#132590.37#-953442.61#km
            string[] re = RunBash($"rpx-helper.sh").Split(' ');
            return new double[] 
            { 
                Convert.ToDouble(re[4]),
                Convert.ToDouble(re[5]),
                Convert.ToDouble(re[6])
            };
        }
        static double[] DumpDistances(string bt_file)
        {
            string[] lines = File.ReadAllLines(bt_file);
            double[,] r = new double[lines.Length, 3];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(' ');
                r[i, X] = 1000 * Convert.ToDouble(data[4]);
                r[i, Y] = 1000 * Convert.ToDouble(data[5]);
                r[i, Z] = 1000 * Convert.ToDouble(data[6]);
            }
            int num = lines.Length;
            List<double> ds = new List<double>((1 + num) * num / 2);
            for (int n = 0; n < lines.Length; n++)
            {
                for (int m = n + 1; m < lines.Length; m++)
                {
                    ds.Add(Distance(r[m, X], r[m, Y], r[m, Z], r[n, X], r[n, Y], r[n, Z]));
                }
            }
            ds.Sort();
            return ds.ToArray();
        }
        static double Distance(double ax, double ay, double az, double bx, double by, double bz)
        {
            double dX = (ax - bx);
            double dY =  (ay - by);
            double dZ = (az - bz);
            double distance = Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
            return distance;
        }
        static string RunBash(string name)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = name,
                    RedirectStandardOutput = true,
                    RedirectStandardError = reError,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
        static string RunCmd(string cmd) => RunBash($"-c {cmd}");
    }
}
