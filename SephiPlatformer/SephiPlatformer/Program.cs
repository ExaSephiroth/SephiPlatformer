using System;

namespace SephiPlatformer
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SimplePlatformerGame game = new SimplePlatformerGame())
            {
                game.Run();
            }
        }
    }
#endif
}

