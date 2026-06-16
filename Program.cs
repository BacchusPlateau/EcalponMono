using System;

namespace Ecalpon
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (TheGame game = new TheGame())
            {
                game.Run();
            }

        }
    }
}