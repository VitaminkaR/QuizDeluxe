using System;
using System.Collections.Generic;
using Server.Utils;

namespace Server
{
    public class Table
    {
        // вопросы и ответы к нему
        public List<string> Questions { get; private set; }



        public Table() => Questions = new List<string>();



        // загружает вопросы
        public void Load(string path)
        {
            string[] data = FileWork.Read(path);
            for (int i = 0; i < data.Length; i++)
                if(data[i] == "[q]")
                    Questions.Add($"{data[i + 1]}|{data[i + 2]}|{data[i + 3]}|{data[i + 4]}|{data[i + 5]}");
        }

        // возвращает массив с вопросом и ответами
        public string Get()
        {
            Random rand = new Random();
            int i = rand.Next(0, Questions.Count);
            string @out = Questions[i];
            Questions.Remove(@out);
            return @out;
        }
    }
}
