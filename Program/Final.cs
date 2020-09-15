using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    class Final : Game
    {
        public Final(int width, int height, string title)
            : base(width, height, title)
        {
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.0f, 0.4f, 0.6f, 1.0f);
            base.OnLoad(e);
            
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }
    }
}
