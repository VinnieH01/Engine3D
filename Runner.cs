using System;

namespace Engine3D
{
    public static class Runner
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Application())
                game.Run();
        }
    }
}
