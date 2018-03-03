using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace InformationRetrieval.Classes
{
    public class MyStem
    {
        public string Stemer(string _original)
        {
            string directory = "/Users/User/Documents/itis/Basic_Of_Inf_retrieval/Inf_retrieval/InformationRetrieval/InformationRetrieval";
            string result = "";

            File.WriteAllText(directory + "/input.txt", _original, Encoding.GetEncoding("Utf-8"));

            Process process = new Process();
            process.StartInfo.FileName = directory + "/mystem";
            process.StartInfo.Arguments = "-e utf-8 -ld input.txt output.txt";
            process.StartInfo.WorkingDirectory = directory;
            process.Start();
            process.WaitForExit();
            process.Close();
            ////приходится стопить программу, иначе запрос не успевает отработать и в результате значение нынешнего нода присвоится следующему и так по цепочке
            //System.Threading.Thread.Sleep(1000);
            string[] lines = File.ReadAllLines(directory + "/output.txt");
            Console.WriteLine(lines.Length);
            for (int i = 0; i < lines.Length; i++)
            {

                result += lines[i].Replace("{", "").Replace("}", " ");

            }

            return result;
        }
        }
    }

