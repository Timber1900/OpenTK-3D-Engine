using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Program;


namespace Tests
{

    internal static class Program
    {
        private static void Main()
        {
            var game = new Game(1000, 1000, "Test", 60.0);
            game.Run();
        }
    }

    public class Game : MainRenderWindow
    {
        public Game(int width, int height, string title, double FPS) : base(width, height, title, FPS)
        {
        }

        protected override void OnLoad()
        {
            //CenterWindow();
            SetClearColor(new Color4(1.0f, 1.0f, 1.0f, 1.0f)); //Sets Background Color
            UseDepthTest = false; //Enables Depth Testing for 3D
            UseAlpha = true; //Enables alpha use
            KeyboardAndMouseInput = false; //Enables keyboard and mouse input for 3D movement
            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Clear();
            fill(0f, 0.3f, 0.7f, 0.7f);
            stroke(0f, 0f, 0f, 1f);
            strokeWeight(1);
            ellipse(Width / 2f, Height / 2f, 100, 100);
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Title = $"Test app, FPS: {1/ e.Time}";
            base.OnUpdateFrame(e);
        }
    }
}