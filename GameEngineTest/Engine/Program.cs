using System;

namespace GameEngineTest.Engine
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameLoop())
            {
                game.Run();
            }
        }
    }
}
