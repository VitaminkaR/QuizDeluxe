using System;

namespace QuizDeluxe
{
    public static class Program
    {
        static void Main() { }

        public static void Main(string ip, string name)
        {
            using (var game = new Main(ip, name))
                game.Run();
        }
    }
}
