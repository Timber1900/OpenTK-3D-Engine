# OpenTK3DEngine

### Todo:

- [ ] Add drawPolygon Function (p5.js Triangle strip style),

- [x] Add internal support for fonts using the microsoft Font class to create a bitmap and font data, removing the need for external programs,

- [ ] Complete settings class

- [ ] Make other 3D objects(cylinder, torus) not need .obj files to work


# Table of Contents

- [Overview](#overview)
- [Basic Setup Instructions](#using-the-library)
  - [Example code](#example-code)
- [Current Available Functions](#current-available-functions)
  - [On **OnLoad**](#on-onload)
    - [Before Base.OnLoad](#make-sure-to-add-these-before-baseonload)
    - [After Base.OnLoad](#make-sure-to-add-these-after-baseonload)
  - [On **OnRenderFrame**](#on-onrenderframe)
    - [3D Functions](#3d-functions)
    - [2D Functions](#2d-functions)
    - [Fonts Class](#font-class)
    - [Settings Class](#settings-class)

### Overview

This library simplifies the use of the openTK library by wrapping it into simple to use functions, perferct for C# and VB simple game development. 

### Using the library:

1. Add the library to your solution from Nuget.

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
        public Game(int width, int height, string title, double FPS) : base(width, height, title, FPS)
        {
        }

        protected override void OnLoad()
        {
            setClearColor(new Color4(0.0f, 0.0f, 0.0f, 1.0f)); //Sets Background Color
            UseDepthTest = false; //Enables Depth Testing for 3D
            RenderLight = false; //Makes the 3D light visible
            UseAlpha = true; //Enables alpha use
            KeyboardAndMouseInput = false; //Enables keyboard and mouse input for 3D movement
            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Clear();

            drawEllipse(500, 500, 10f, 10f, new Color4(1.0f, 1.0f, 1.0f, 1.0f)); //Draws a circle

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }
    }
}
```

```c#
namespace yourNamespace
{
    static class Program
    {
        static void Main(string[] args)
        {
            using youClass game = new youClass(1000, 1000, "Test App", 60.0);
            //Run takes a double, which is how many frames per second it should strive to reach.
            //You can leave that out and it'll just update as fast as the hardware will allow it.
            game.Run();
        }
    }
}
```

# Current Available Functions

### On OnLoad

#### Make sure to add these **before** base.OnLoad()

 - `Boolean UseDepthTest (*Default = false*)` => Specifies if rendering engine will use depth test *(When depth test is true alpha will **not** work, if false rendering will work based on the which functions are called first)*,
 - `Boolean UseAlpha (*Default = true*)` => Specifies if rendering engine will use alpha,
 - `Boolean KeyboardAndMouseInput (*Default = true*)` => Specifies if rendering engine will use default keyboard and mouse input,
 - `Boolean showSet (*Default = false*)` => Specifies if rendering engine open settings on esc,
 
#### Make sure to add these **after** base.OnLoad()

 - `createMainLight(Vector3 pos, Color4 color)` => Creates your main Light (static), make sure to run this function __before__ any other 3D function but after Base.OnLoad(),
 - `createCube(Color4 Color)` => Creates a 3D cube of the color you specify, returns a handle for making modifications to the cube,
 - `createSphere(Color4 Color)` => Creates a 3D sphere of the color you specify, returns a handle for making modifications to the sphere,
 - `createTorus(Color4 Color)` => Creates a 3D torus of the color you specify, returns a handle for making modifications to the torus,
 - `createCylinder(Color4 Color)` => Creates a 3D cylinder of the color you specify, returns a handle for making modifications to the cylinder,
 - `createPlane(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4, float z4, Color4 color)` => Creates a 3D plane of the color you specify, returns a handle for making modifications to the plane,
 - `openTexturedObj(string obj, string texture)` => Opens .obj file with an attached .png texture, returns a handle for making modifications to the object,
 - `openObj(string obj, Color4 color)` => Opens .obj file with no attached .png texture, returns a handle for making modifications to the object,

### On **OnRenderFrame**

#### 3D Functions:
1. `rotateObject(float x, float y, float z, int handle)`
1. `rotateTexturedObject(float x, float y, float z, int handle)`
1. `scaleObject(float scale, int handle)`
1. `translateObject(float x, float y, float z, int handle)`
1. `translateTexturedObject(float x, float y, float z, int handle)`

#### 2D Functions:
1. `drawRectangle(float x1, float y1, float x2, float y2, Color4 color)`
1. `drawLine(float x1, float y1, float x2, float y2, Color4 color)`
1. `drawEllipse(float x, float y, float radiusX, float radiusY, Color4 color)`
1. `drawTexturedLine(float x1, float y1, float u1, float v1, float x2, float y2, float u2, float v2, Texture texture, Color4 color)`
1. `drawQuad(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4, float z4, Color4 color)`
1. `drawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Color4 color)`
1. `drawText(string text, float x, float y, Font f, Color4 col, int TextAlign)`
1. ***drawTexturedRectangle() overloads***
    1. `drawTexturedRectangle(float x1, float y1, float u1, float v1, float x2, float y2, float u2, float v2, string texturePath, Color4 color, TextureMinFilter min, TextureMagFilter mag)`
    1. `drawTexturedRectangle(float x1, float y1, float u1, float v1, float x2, float y2, float u2, float v2, Bitmap textureBitmap, Color4 color, TextureMinFilter min, TextureMagFilter mag)`
    1. `drawTexturedRectangle(float x1, float y1, float u1, float v1, float x2, float y2, float u2, float v2, Texture texture, Color4 color)`
1. ***drawTexturedQuad() overloads***
    1. `drawTexturedQuad(float x1, float y1, float z1, float u1, float v1, float x2, float y2, float z2, float u2, float v2, float x3, float y3, float z3, float u3, float v3, float x4, float y4, float z4, float u4, float v4, string texturePath, Color4 color, TextureMinFilter min, TextureMagFilter mag)`
    1. `drawTexturedQuad(float x1, float y1, float z1, float u1, float v1, float x2, float y2, float z2, float u2, float v2, float x3, float y3, float z3, float u3, float v3, float x4, float y4, float z4, float u4, float v4, Bitmap textureBitmap, Color4 color, TextureMinFilter min, TextureMagFilter mag)`
    1. `drawTexturedQuad(float x1, float y1, float z1, float u1, float v1, float x2, float y2, float z2, float u2, float v2, float x3, float y3, float z3, float u3, float v3, float x4, float y4, float z4, float u4, float v4, Texture texture, Color4 color)`

#### Settings Class:
1. `addButton(string t, float x, float y, int w, int h, Color4 c, Func<object> func, Font f)`
1. `addSetting(string key, object value)`
1. `readSettings()` => Reads settings.cfg
1. `writeSettings()` => Writes to settings.cfg
##### Settings.cfg Example
```
width=200
height=300
useTexture=false
r=0
g=127
b=256
a=1  
```

    
