using System.IO;
using System.Collections.Generic;

namespace QuizDeluxe.Utils
{
    public class FileWork
    {
        static public void Write(string path, string message)
        {
            StreamWriter sw = new StreamWriter(path);
            sw.Write(message + '\n');
            sw.Close();
        }

        static public string[] Read(string path)
        {
            if (!File.Exists(path))
                return null;

            StreamReader sr = new StreamReader(path);
            List<string> lines = new List<string>();

            string line = "";

            while (true)
            {
                line = sr.ReadLine();

                if (line == null)
                    break;

                lines.Add(line);
            }

            sr.Close();
            return lines.ToArray();
        }
    }
}
