using System;

namespace Server
{
    static class Logger
    {
        static public void Log(this string str) => Console.WriteLine(str);
        static public void Log(this string str, ConsoleColor color)
        {
            ConsoleColor temp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ForegroundColor = temp;
        }
    }
}
