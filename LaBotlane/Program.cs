using System;

namespace Labotlane
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            using (LaBotlane game = new LaBotlane())
            {
                game.Run();
            }
        }
    }
#endif
}

