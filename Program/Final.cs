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
        int cubeHandle, sphereHandle, torusHandle, cilinderHandle;
        float angle1, angle2;
        public Final(int width, int height, string title)
            : base(width, height, title)
        {
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            base.OnLoad(e);
            //openTexturedObj("Objs/spiro.obj", "Resources/high.png");
            createMainLight(new Vector3(2.5f, 0f, 10f), new Vector3(1f, 1f, 1f));
            Vector3 mainColor = new Vector3(1f, 1f, 1f);
            cubeHandle = createCube(mainColor);
            sphereHandle = createSphere(mainColor);
            torusHandle = createTorus(mainColor);
            cilinderHandle = createCilinder(mainColor);
            translateObject(5.0f, 0f, 0f, sphereHandle);
            translateObject(-5.0f, 0f, 0f, torusHandle);
            translateObject(10.0f, 0f, 0f, cilinderHandle);
            RenderLight = false;
            angle1 = 0.0f; angle2 = 0.0f;
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            rotateObject(angle1, 0, angle2, cubeHandle);
            rotateObject(angle1, 0, angle2, sphereHandle);
            rotateObject(angle1, 0, angle2, torusHandle);
            rotateObject(angle1, 0, angle2, cilinderHandle);
            base.OnRenderFrame(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            angle1 += 0.01f; angle2 += 0.02f;
            base.OnUpdateFrame(e);
        }
    }
}
