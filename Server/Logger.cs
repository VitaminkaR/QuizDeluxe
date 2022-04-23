using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    static class Logger
    {
        static public void Log(this string str) => Console.WriteLine(str);
    }
}
