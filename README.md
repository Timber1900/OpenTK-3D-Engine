# OpenTK3DEngine

### Todo:

- [x] Add simple Cube and Sphere functions,

- [ ] Add drawPolygon Function (p5.js Triangle strip style),

- [x] Add 2D rendering

### Using the library:

1. Reference *"Program.dll"* to your application,
2. Add the OpenTK library from nuget,
    1. Make sure to also add System.Drawing.Common and Microsoft.Win32.SystemEvents,
3. Add the Shaders, Objs and Resources folder to the same path of your .exe file

### Example code:

1. Add a new .cs line to your console app, that will be your main place to code <br>(Example bellow no. 1)
2. On your program.cs file add the example code no. 2,
3. If you dont want the console opening change your project properties to Windows Application

```c#
using System;
using OpenTK;
using OpenTK.Graphics;
using Program;

namespace yourNameSpace
{
    public class yourClass : MainRenderWindow
    {
        public yourClass(int width, int height, string title)
            : base(width, height, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            setClearColor(new Color4(0.0f, 0.0f, 0.0f, 1.0f)); //Sets Background Color
            UseDepthTest = false; //Enables Depth Testing for 3D
            RenderLight = false; //Makes the 3D light visible
            UseAlpha = true; //Enables alpha use
            KeyboardAndMouseInput = false; //Enables keyboard and mouse input for 3D movement
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Clear();

            drawEllipse(500, 500, 10f, 10f, new Color4(1.0f, 1.0f, 1.0f, 1.0f)) //Draws a circle

            base.OnRenderFrame(e);

        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }
    }
}
```
```C#
namespace yourNamespace
{
    static class Program
    {
        static void Main(string[] args)
        {
            using youClass game = new youClass(1000, 1000, "Test App");
            //Run takes a double, which is how many frames per second it should strive to reach.
            //You can leave that out and it'll just update as fast as the hardware will allow it.
            game.Run(60.0);
        }
    }
}
```

### Current Available Functions

##### On **OnLoad**
 
1. Make sure to add these **after** base.OnLoad()

1. createMainLight(Vector3 pos, Vector3 color) => Creates your main Light (static), make sure to run this function __before__ any other 3D function but after Base.OnLoad(),

1. createCube(Vector3 Color) => Creates a 3D cube of the color you specify, returns a handle for making modifications to the cube,

1. createSphere(Vector3 Color) => Creates a 3D sphere of the color you specify, returns a handle for making modifications to the sphere,

1. createTorus(Vector3 Color) => Creates a 3D torus of the color you specify, returns a handle for making modifications to the torus,

1. createCylinder(Vector3 Color) => Creates a 3D cylinder of the color you specify, returns a handle for making modifications to the cylinder,

1. public int createPlane(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4, float z4, Vector3 color) => Creates a 3D plane of the color you specify, returns a handle for making modifications to the plane,

1. openTexturedObj(string obj, string texture) => Opens .obj file with an attached .png texture, returns a handle for making modifications to the object,

1. openObj(string obj, Vector3 color) => Opens .obj file with no attached .png texture, returns a handle for making modifications to the object,

##### On **OnRenderFrame**

1. 3D Functions:
    1. rotateObject(float x, float y, float z, int handle),
    1. rotateTexturedObject(float x, float y, float z, int handle),
    1. scaleObject(float scale, int handle),
    1. translateObject(float x, float y, float z, int handle),
    1. translateTexturedObject(float x, float y, float z, int handle),

1. 2D Functions:
    1. drawRectangle(float x1, float y1, float x2, float y2, Color4 color),
    1. drawLine(float x1, float y1, float x2, float y2, Color4 color),
    1. drawEllipse(float x, float y, float radiusX, float radiusY, Color4 color),

 