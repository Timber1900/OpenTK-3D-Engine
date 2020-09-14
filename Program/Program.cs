using System;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game(1000, 1000, "Test App"))
            {
                //Run takes a double, which is how many frames per second it should strive to reach.
                //You can leave that out and it'll just update as fast as the hardware will allow it.
                game.Run(60.0);
                
            }
        }
    }
}
