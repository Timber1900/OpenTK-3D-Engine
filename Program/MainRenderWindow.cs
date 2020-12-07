using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static Program.Shaders;
using Boolean = System.Boolean;

namespace Program
{
    public static class Screen
    {
        [DllImport("user32.dll")]
        static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        [StructLayout(LayoutKind.Sequential)]
        struct DEVMODE
        {
          [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
          public string dmDeviceName;
          public short dmSpecVersion;
          public short dmDriverVersion;
          public short dmSize;
          public short dmDriverExtra;
          public int dmFields;
          public int dmPositionX;
          public int dmPositionY;
          public int dmDisplayOrientation;
          public int dmDisplayFixedOutput;
          public short dmColor;
          public short dmDuplex;
          public short dmYResolution;
          public short dmTTOption;
          public short dmCollate;
          [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
          public string dmFormName;
          public short dmLogPixels;
          public int dmBitsPerPel;
          public int dmPelsWidth;
          public int dmPelsHeight;
          public int dmDisplayFlags;
          public int dmDisplayFrequency;
          public int dmICMMethod;
          public int dmICMIntent;
          public int dmMediaType;
          public int dmDitherType;
          public int dmReserved1;
          public int dmReserved2;
          public int dmPanningWidth;
          public int dmPanningHeight;
        }

        public static Vector2 getScreenSize()
        {
            const int ENUM_CURRENT_SETTINGS = -1;

            DEVMODE devMode = default;
            devMode.dmSize = (short)Marshal.SizeOf(devMode);
            EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref devMode);
            return new Vector2( devMode.dmPelsWidth, devMode.dmPelsHeight);
        }
    }
    public class MainRenderWindow : GameWindow
    {
        private readonly List<TexturedObject> _mainTexturedObjects = new List<TexturedObject>();
        private readonly List<Object> _mainObjects = new List<Object>();
        private Lamp _mainLamp;
        private Shader _lampShader, _lightingShader, _textureShader, _2dShader, _2dTextured;
        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;
        protected Boolean RenderLight = false;
        private float cameraSpeed = 20f;
        private float sensitivity = 0.2f;
        protected Boolean UseDepthTest = false, UseAlpha = true, KeyboardAndMouseInput = true, loadedFont = false, showSet = false, lastTime = true, useSettings = false;
        public int Width, Height;
        private static GameWindowSettings createGameWindowSettings(double FPS = 60.0)
        {
            var gws = new GameWindowSettings()
            {
                UpdateFrequency = FPS,
                RenderFrequency = FPS
            };
            return gws;
        }
        
        private static NativeWindowSettings createNativeWindowSettings(int width = 1000, int height = 1000, string title = "OpenTK Window")
        {
            var MonitorSize = Screen.getScreenSize();
            var nws = new NativeWindowSettings()
            {
                Size = new Vector2i(width, height),
                Title = title,
                Location = new Vector2i((int) ((MonitorSize.X - width) / 2), (int) ((MonitorSize.Y - height - 10) / 2))
            };
            return nws;
        }

        public MainRenderWindow(int width, int height, string title, double FPS) :
            base(createGameWindowSettings(FPS), createNativeWindowSettings(width, height, title))
        {
        }
        
        protected override void OnLoad()
        {
            if (UseDepthTest) { GL.Enable(EnableCap.DepthTest); }
            if(UseAlpha) {GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);}
            GL.Enable(EnableCap.Blend);
            _lightingShader = new Shader(ShaderVert, LightingFrag);
            _lampShader = new Shader(ShaderVert, ShaderFrag);
            _2dShader = new Shader(Shader2DVert, Shader2DFrag);
            _textureShader = new Shader(TextureVert, TextureFrag);
            _2dTextured = new Shader(Texture2DVert, Texture2DFrag);
            _lightingShader.Use();
            _lampShader.Use();
            _textureShader.Use();
            _2dShader.Use();
            _2dTextured.Use();

            _lastPos = MouseState.PreviousPosition;

            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            Width = Size.X;
            Height = Size.Y;
            base.OnLoad();
            CursorGrabbed = KeyboardAndMouseInput;
            CursorVisible = !KeyboardAndMouseInput;
        }

        protected void setClearColor(Color4 color)
        {
            GL.ClearColor(color);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (RenderLight) { _mainLamp.show(_camera, _lampShader); }

            foreach (Object obj in _mainObjects)
            {
                obj.show(_camera);
            }
            foreach (TexturedObject obj in _mainTexturedObjects)
            {
                obj.show(_camera);
            }

            if (showSet)
            {
                if (MouseState.IsButtonDown(MouseButton.Left))
                {
                    checkClicks(set);
                }
                showSettings(set);
            }
            
            SwapBuffers();

            base.OnRenderFrame(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!IsFocused) // check to see if the window is focused
            {
                return;
            }
            var input = KeyboardState;
            var mouse = MouseState;

            if (input.IsKeyDown(Keys.Escape) && lastTime)
            {
                if (!useSettings)
                {
                    Close();
                }
                else
                {
                    showSet = !showSet;
                    lastTime = false;

                    if (KeyboardAndMouseInput)
                    {
                        CursorVisible = !CursorVisible;
                        CursorGrabbed = !CursorVisible;
                    }
                }
                
            }
            if (!input.IsKeyDown(Keys.Escape))
            {
                lastTime = true;
            }
            if (KeyboardAndMouseInput && !showSet)
            {
                
                if (input.IsKeyDown(Keys.W))
                {
                    _camera.Position += _camera.Front * cameraSpeed * (float) e.Time; // Forward
                }

                if (input.IsKeyDown(Keys.S))
                {
                    _camera.Position -= _camera.Front * cameraSpeed * (float) e.Time; // Backwards
                }

                if (input.IsKeyDown(Keys.A))
                {
                    _camera.Position -= _camera.Right * cameraSpeed * (float) e.Time; // Left
                }

                if (input.IsKeyDown(Keys.D))
                {
                    _camera.Position += _camera.Right * cameraSpeed * (float) e.Time; // Right
                }

                if (input.IsKeyDown(Keys.Space))
                {
                    _camera.Position += _camera.Up * cameraSpeed * (float) e.Time; // Up
                }

                if (input.IsKeyDown(Keys.LeftShift))
                {
                    _camera.Position -= _camera.Up * cameraSpeed * (float) e.Time; // Down
                }
                // Calculate the offset of the mouse position
                
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top

            }
            
                
            base.OnUpdateFrame(e);
        }

        protected override void OnResize(ResizeEventArgs resizeEventArgs)
        {
            Width = resizeEventArgs.Width;
            Height = resizeEventArgs.Height;
            GL.Viewport(0, 0, resizeEventArgs.Width, resizeEventArgs.Height);
            base.OnResize(resizeEventArgs);
        }
        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);


            GL.DeleteProgram(_lampShader.Handle);
            GL.DeleteProgram(_lightingShader.Handle);
            GL.DeleteProgram(_2dShader.Handle);
            GL.DeleteProgram(_textureShader.Handle);

            foreach(Object obj in _mainObjects)
            {
                obj.Dispose();
            }
            foreach(TexturedObject obj in _mainTexturedObjects)
            {
                obj.Dispose();
            }

            _mainLamp?.Dispose();

            base.OnUnload();
        }
        private static float[] loadObj(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            List<float[]> vertices = new List<float[]>();
            List<float> final = new List<float>();
            foreach (string line in lines)
            {
                string[] lineSlitted = line.Split(" ");
                if (lineSlitted[0] == "v")
                {
                    float[] toAdd = new float[3];
                    toAdd[0] = (float.Parse(lineSlitted[1]));
                    toAdd[1] = (float.Parse(lineSlitted[2]));
                    toAdd[2] = (float.Parse(lineSlitted[3]));
                    vertices.Add(toAdd);
                }
                if (lineSlitted[0] == "f")
                {
                    string[] t1 = lineSlitted[1].Split("//");
                    string[] t2 = lineSlitted[2].Split("//");
                    string[] t3 = lineSlitted[3].Split("//");



                    float[] v1 = vertices[int.Parse(t1[0]) - 1];
                    float[] v2 = vertices[int.Parse(t2[0]) - 1];
                    float[] v3 = vertices[int.Parse(t3[0]) - 1];

                    Vector3 v01 = new Vector3(v1[0], v1[1], v1[2]);
                    Vector3 v02 = new Vector3(v2[0], v2[1], v2[2]);
                    Vector3 v03 = new Vector3(v3[0], v3[1], v3[2]);

                    Vector3 l1 = v02 - v01;
                    Vector3 l2 = v03 - v01;

                    Vector3 n = Vector3.Cross(l2, l1);

                    final.Add(v1[0]); final.Add(v1[1]); final.Add(v1[2]); final.Add(n.X); final.Add(n.Y); final.Add(n.Z);
                    final.Add(v2[0]); final.Add(v2[1]); final.Add(v2[2]); final.Add(n.X); final.Add(n.Y); final.Add(n.Z);
                    final.Add(v3[0]); final.Add(v3[1]); final.Add(v3[2]); final.Add(n.X); final.Add(n.Y); final.Add(n.Z);
                }

            }


            return final.ToArray();
        }
        private static float[] loadObjTextured(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            List<float[]> vertices = new List<float[]>();
            List<float[]> textureCords = new List<float[]>();
            List<float> final = new List<float>();
            foreach (string line in lines)
            {
                string[] lineSlitted = line.Split(" ");
                if (lineSlitted[0] == "v")
                {
                    float[] toAdd = new float[3];
                    toAdd[0] = (float.Parse(lineSlitted[1]));
                    toAdd[1] = (float.Parse(lineSlitted[2]));
                    toAdd[2] = (float.Parse(lineSlitted[3]));
                    vertices.Add(toAdd);
                }
                if (lineSlitted[0] == "vt")
                {
                    float[] toAdd = new float[2];
                    toAdd[0] = (float.Parse(lineSlitted[1]));
                    toAdd[1] = (-(float.Parse(lineSlitted[2]) - 1));
                    textureCords.Add(toAdd);
                }
                if (lineSlitted[0] == "f")
                {
                    string[] t1 = lineSlitted[1].Split("/");
                    string[] t2 = lineSlitted[2].Split("/");
                    string[] t3 = lineSlitted[3].Split("/");



                    float[] v1 = vertices[int.Parse(t1[0]) - 1];
                    if (int.Parse(t2[0]) - 1 >= 0 && vertices.Count > int.Parse(t2[0]) - 1)
                    {
                        float[] v2 = vertices[int.Parse(t2[0]) - 1];
                        float[] v3 = vertices[int.Parse(t3[0]) - 1];
                        float[] tex1 = textureCords[int.Parse(t1[1]) - 1];
                        float[] tex2 = textureCords[int.Parse(t2[1]) - 1];
                        float[] tex3 = textureCords[int.Parse(t3[1]) - 1];

                        Vector3 v01 = new Vector3(v1[0], v1[1], v1[2]);
                        Vector3 v02 = new Vector3(v2[0], v2[1], v2[2]);
                        Vector3 v03 = new Vector3(v3[0], v3[1], v3[2]);

                        Vector3 l1 = v02 - v01;
                        Vector3 l2 = v03 - v01;

                        Vector3 n = Vector3.Cross(l2, l1);

                        final.Add(v1[0]); final.Add(v1[1]); final.Add(v1[2]); final.Add(n.X); final.Add(n.Y); final.Add(n.Z); final.Add(tex1[0]); final.Add(tex1[1]);
                        final.Add(v2[0]); final.Add(v2[1]); final.Add(v2[2]); final.Add(n.X); final.Add(n.Y); final.Add(n.Z); final.Add(tex2[0]); final.Add(tex2[1]);
                        final.Add(v3[0]); final.Add(v3[1]); final.Add(v3[2]); final.Add(n.X); final.Add(n.Y); final.Add(n.Z); final.Add(tex3[0]); final.Add(tex3[1]);
                    }
                }

            }


            return final.ToArray();
        }
        private class Object
        {
            private readonly int _vertexBufferObject;
            private readonly int _mainObject;
            private readonly float[] _vertices;
            private float _rotX, _rotY, _rotZ;
            private Vector3 _pos;
            private readonly Color4 _color;
            private readonly Shader _shader;
            private readonly Lamp _lamp;
            private float _scale = 1.0f;
            public Object(string path, Shader lightingShader, Lamp lamp, Color4 col)
            {
                _vertices = loadObj(path);

                _vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

                _mainObject = GL.GenVertexArray();
                GL.BindVertexArray(_mainObject);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

                var positionLocation = lightingShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

                var normalLocation = lightingShader.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                _rotX = 0.0f; _rotY = 0.0f; _rotZ = 0.0f;
                _pos = new Vector3(0.0f, 0.0f, 0.0f);
                _shader = lightingShader;
                _lamp = lamp;
                _color = col;
            }
            public Object(float[] vertices, Shader lightingShader, Lamp lamp, Color4 col)
            {
                _vertices = vertices;

                _vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, this._vertices.Length * sizeof(float), this._vertices, BufferUsageHint.StaticDraw);

                _mainObject = GL.GenVertexArray();
                GL.BindVertexArray(_mainObject);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

                var positionLocation = lightingShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

                var normalLocation = lightingShader.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                _rotX = 0.0f; _rotY = 0.0f; _rotZ = 0.0f;
                _pos = new Vector3(0.0f, 0.0f, 0.0f);
                _shader = lightingShader;
                _lamp = lamp;
                _color = col;
            }
            public void show(Camera camera)
            {
                GL.BindVertexArray(_mainObject);

                _shader.Use();


                _shader.SetMatrix4("model",  (Matrix4.CreateScale(_scale) *  Matrix4.CreateRotationX(_rotX) * Matrix4.CreateRotationX(_rotY) * Matrix4.CreateRotationZ(_rotZ)) * Matrix4.CreateTranslation(_pos));
                _shader.SetMatrix4("view", camera.GetViewMatrix());
                _shader.SetMatrix4("projection", camera.GetProjectionMatrix());

                _shader.SetVector4("objectColor", new Vector4(_color.R, _color.G, _color.B, _color.A));
                _shader.SetVector3("lightColor", _lamp.LightColor);
                _shader.SetVector3("lightPos", _lamp.Pos);

                GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length / 6);
            }
            public void setRotationX(float angle)
            {
                _rotX = angle;
            }
            public void setRotationY(float angle)
            {
                _rotY = angle;
            }
            public void setRotationZ(float angle)
            {
                _rotZ = angle;
            }
            public void setPositionInSpace(float x, float y, float z)
            {
                _pos = new Vector3(x, y, z);
            }
            public void setScale(float scale)
            {
                _scale = scale; 
            }
            public void Dispose()
            {
                GL.DeleteBuffer(_vertexBufferObject);
                GL.DeleteVertexArray(_mainObject);
            }
        }
        private class Lamp
        {
            private readonly int _vertexBufferObject;
            private readonly int _mainObject;
            public readonly Vector3 Pos;
            public readonly Vector3 LightColor;
            private readonly float[] _vertices;
            public Lamp(Vector3 pos, Vector3 lightColor, Shader lampShader, float Radius)
            {
                Pos = pos;
                LightColor = lightColor;

                //_vertices = loadObj("Objs/sphere.obj");   
                _vertices = CreateSphereVertices(Radius);

                _vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

                _mainObject = GL.GenVertexArray();
                GL.BindVertexArray(_mainObject);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

                var positionLocation = lampShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

                lampShader.SetVector3("lightColor", lightColor);
            }
            public void show(Camera camera, Shader lampShader)
            {
                GL.BindVertexArray(_mainObject);

                lampShader.Use();

                Matrix4 lampMatrix = Matrix4.Identity;
                lampMatrix *= Matrix4.CreateScale(0.2f);
                lampMatrix *= Matrix4.CreateTranslation(Pos);

                lampShader.SetMatrix4("model", lampMatrix);
                lampShader.SetMatrix4("view", camera.GetViewMatrix());
                lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());

                GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length / 6);
            }
            public void Dispose()
            {
                GL.DeleteBuffer(_vertexBufferObject);
                GL.DeleteVertexArray(_mainObject);
            }
        }
        private class TexturedObject
        {
            private readonly int _vertexBufferObject;
            private readonly int _mainObject;
            private readonly float[] _vertices;
            private float _rotX, _rotY, _rotZ;
            private readonly Texture _texture;
            private Vector3 _pos;
            private readonly Shader _shader;
            public TexturedObject(string path, Shader textureShader, string texturePath)
            {
                _vertices = loadObjTextured(path);

                _vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

                _mainObject = GL.GenVertexArray();
                GL.BindVertexArray(_mainObject);
                
                var positionLocation = textureShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                var normalLocation = textureShader.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

                var textureLocation = textureShader.GetAttribLocation("aTexture");
                GL.EnableVertexAttribArray(textureLocation);
                GL.VertexAttribPointer(textureLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

                _texture = new Texture(texturePath, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
                _texture.Use();

                _rotX = 0.0f; _rotY = 0.0f; _rotZ = 0.0f;
                _pos = new Vector3(0.0f, 0.0f, 0.0f);
                _shader = textureShader;
            }

            public void show(Camera camera)
            {
                GL.BindVertexArray(_mainObject);

                _texture.Use();
                _shader.Use();


                _shader.SetMatrix4("model", (Matrix4.CreateRotationX(_rotX) * Matrix4.CreateRotationX(_rotY) * Matrix4.CreateRotationZ(_rotZ)) * Matrix4.CreateTranslation(_pos));
                _shader.SetMatrix4("view", camera.GetViewMatrix());
                _shader.SetMatrix4("projection", camera.GetProjectionMatrix());

                //textureShader.SetVector3("lightColor", lamp.lightColor);
                //textureShader.SetVector3("lightPos", lamp.pos);
                GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length / 8);
            }
            public void setRotationX(float angle)
            {
                _rotX = angle;
            }
            public void setRotationY(float angle)
            {
                _rotY = angle;
            }
            public void setRotationZ(float angle)
            {
                _rotZ = angle;
            }
            public void setPositionInSpace(float x, float y, float z)
            {
                _pos = new Vector3(x, y, z);
            }
            public void Dispose()
            {
                GL.DeleteBuffer(_vertexBufferObject);
                GL.DeleteVertexArray(_mainObject);
                GL.DeleteTexture(_texture.Handle);
            }

        }
        /// <summary>
        /// Creates a cube that is rendered to the screen (Currently needs a .obj file)
        /// </summary>
        /// <param name="color">Color of the cube</param>
        /// <param name="width">Width of the cube</param>
        /// <param name="height">Height of the cube</param>
        /// <param name="depth">Depth of the cube</param>
        /// <returns>Returns a integer handle to make modifications to it</returns>
        public int createCube(Color4 color, float width, float height, float depth)
        {
            var cubeVertex = CreateRectangularPrismVertices(width, height, depth);
            _mainObjects.Add(new Object(cubeVertex, _lightingShader, _mainLamp, color));
            return _mainObjects.Count - 1;
        }
        /// <summary>
        /// Creates a sphere that is rendered to the screen
        /// </summary>
        /// <param name="color">Color of the sphere</param>
        /// <param name="r">Radius of the sphere</param>
        /// <returns>Returns a integer handle to make modifications to it</returns>
        public int createSphere(Color4 color, float r)
        {
            float[] v = CreateSphereVertices(r);
            _mainObjects.Add(new Object(v, _lightingShader, _mainLamp, color));
            return _mainObjects.Count - 1;
        }
        /// <summary>
        /// Creates a Torus (Donut) that is rendered to the screen (Currently needs a .obj file)
        /// </summary>
        /// <param name="color">Color of the torus</param>
        /// <returns>Returns a integer handle to make modifications to it</returns>
        public int createTorus(Color4 color)
        {
            _mainObjects.Add(new Object("Objs/torus.obj", _lightingShader, _mainLamp, color));
            return _mainObjects.Count - 1;
        }
        /// <summary>
        /// Creates a Cylinder that is rendered to the screen (Currently needs a .obj file)
        /// </summary>
        /// <param name="color">Color of the cilinder</param>
        /// <returns>Returns a integer handle to make modifications to it</returns>
        public int createCylinder(Color4 color)
        {
            _mainObjects.Add(new Object("Objs/cilinder.obj", _lightingShader, _mainLamp, color));
            return _mainObjects.Count - 1;
        }
        /// <summary>
        /// Creates a plane that is rendered to the screen(Vertexes must be place in clockwise order)
        /// </summary>
        /// <param name="x1">X pos of a vertex of the plane</param>
        /// <param name="y1">Y pos of a vertex of the plane</param>
        /// <param name="z1">Z pos of a vertex of the plane</param>
        /// <param name="x2">X pos of a vertex of the plane</param>
        /// <param name="y2">Y pos of a vertex of the plane</param>
        /// <param name="z2">Z pos of a vertex of the plane</param>
        /// <param name="x3">X pos of a vertex of the plane</param>
        /// <param name="y3">Y pos of a vertex of the plane</param>
        /// <param name="z3">Z pos of a vertex of the plane</param>
        /// <param name="x4">X pos of a vertex of the plane</param>
        /// <param name="y4">Y pos of a vertex of the plane</param>
        /// <param name="z4">Z pos of a vertex of the plane</param>
        /// <param name="color">Color of the plane</param>
        /// <returns>Returns a integer handle to make modifications to it</returns>
        public int createPlane(float x1, float y1, float z1,
                               float x2, float y2, float z2,
                               float x3, float y3, float z3,
                               float x4, float y4, float z4, Color4 color)
        {
            Vector3 l1 = new Vector3(x2 - x1, y2 - y1, z2 - z1);
            Vector3 l2 = new Vector3(x3 - x1, y3 - y1, z3 - z1);
            Vector3 normal = Vector3.Cross(l2, l1);


            float[] vertices =
            {
                x1, y1, z1, normal.X,  normal.Y, normal.Z,
                x3, y3, z3, normal.X,  normal.Y, normal.Z,
                x2, y2, z2, normal.X,  normal.Y, normal.Z,

                x1, y1, z1, normal.X,  normal.Y, normal.Z,
                x3, y3, z3, normal.X,  normal.Y, normal.Z,
                x4, y4, z4, normal.X,  normal.Y, normal.Z,
            };
            _mainObjects.Add(new Object(vertices, _lightingShader, _mainLamp, color));
            return _mainObjects.Count - 1;
        }
        /// <summary>
        /// Opens and creates a texture object from a .obj file
        /// </summary>
        /// <param name="obj">Path to the .obj file</param>
        /// <param name="texture">Path to the texture .png</param>
        public void openTexturedObj(string obj, string texture)
        {
            _mainTexturedObjects.Add(new TexturedObject(obj, _textureShader, texture));
        }
        /// <summary>
        /// Opens and creates an object from a .obj file
        /// </summary>
        /// <param name="obj">Path to the .obj file</param>
        /// <param name="color">Color of the object</param>
        public void openObj(string obj, Color4 color)
        {
            _mainObjects.Add(new Object(obj, _lightingShader, _mainLamp, color));
        }
        /// <summary>
        /// Creates the main light for the 3D scene, must be called before any other 3D function
        /// </summary>
        /// <param name="pos">Position of the light</param>
        /// <param name="color">Color of the light</param>
        public void createMainLight(Vector3 pos, Vector3 color)
        {
            _mainLamp = new Lamp(pos, color, _lampShader, 1);
        }
        /// <summary>
        /// Rotates an object by a certain amount
        /// </summary>
        /// <param name="x">Value of the x rotation</param>
        /// <param name="y">Value of the y rotation</param>
        /// <param name="z">Value of the z rotation</param>
        /// <param name="handle">Handle of the object to be rotated</param>
        public void rotateObject(float x, float y, float z, int handle)
        {
            _mainObjects[handle].setRotationX(x);
            _mainObjects[handle].setRotationY(y);
            _mainObjects[handle].setRotationZ(z);
        }
        /// <summary>
        /// Rotates a textured object by a certain amount
        /// </summary>
        /// <param name="x">Value of the x rotation</param>
        /// <param name="y">Value of the y rotation</param>
        /// <param name="z">Value of the z rotation</param>
        /// <param name="handle">Handle of the textured object to be rotated</param>
        public void rotateTexturedObject(float x, float y, float z, int handle)
        {
            _mainTexturedObjects[handle].setRotationX(x);
            _mainTexturedObjects[handle].setRotationY(y);
            _mainTexturedObjects[handle].setRotationZ(z);
        }
        /// <summary>
        /// Scales an object by a certain amount
        /// </summary>
        /// <param name="scale">Amount to scale by</param>
        /// <param name="handle">Handle of the object to be rotated</param>
        public void scaleObject(float scale, int handle)
        {
            _mainObjects[handle].setScale(scale);
        }
        /// <summary>
        /// Moves an object to a certain point in space
        /// </summary>
        /// <param name="x">X pos of the point in space</param>
        /// <param name="y">Y pos of the point in space</param>
        /// <param name="z">Z pos of the point in space</param>
        /// <param name="handle">Handle of the object to be rotated</param>
        public void translateObject(float x, float y, float z, int handle)
        {
            _mainObjects[handle].setPositionInSpace(x, y, z);
        }
        /// <summary>
        /// Moves a textured object to a certain point in space
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="handle">Handle of the textured object to be rotated</param>
        public void translateTexturedObject(float x, float y, float z, int handle)
        {
            _mainTexturedObjects[handle].setPositionInSpace(x, y, z);
        }

        private static float[] CreateSphereVertices(float radius)
        {
            var Res = Math.Min(Convert.ToInt32(Math.Ceiling(radius * radius)), 50);
            List<List<Vector3>> unParsedVertices = new List<List<Vector3>>();
            List<float> vertices = new List<float>();
            var i = 0;
            var j = 0;


            for (double psi = 0; psi-Math.PI <= 0.1; psi += Math.PI / Res)
            {
                j = 0;
                List<Vector3> v = new List<Vector3>();

                for (double theta = 0; theta - (2 * Math.PI) < 0.1; theta += Math.PI / Res)
                {
                    var vertex = new Vector3(
                        (float)(radius * Math.Cos(theta) * Math.Sin(psi)),
                        (float)(radius * Math.Sin(theta) * Math.Sin(psi)),
                        (float)(radius * Math.Cos(psi)));
                    var ind = Math.Cos(psi);
                    v.Add(vertex);
                    j++;
                }
                unParsedVertices.Add(v);
                i++;
            }

            for (var index = 0; index < i - 1; index++)
            {
                for (var jIndex = 0; jIndex < j - 1; jIndex++)
                {
                    Vector3 v01 = unParsedVertices[index][ jIndex];
                    Vector3 v02 = unParsedVertices[index + 1][ jIndex];
                    Vector3 v03 = unParsedVertices[index + 1][ jIndex + 1];

                    Vector3 l1 = v02 - v01;
                    Vector3 l2 = v03 - v01;
                    //Normals are the same for each triangle
                    Vector3 n = Vector3.Cross(l2, l1);
                    //First Vertex
                    vertices.Add(unParsedVertices[index][ jIndex].X);
                    vertices.Add(unParsedVertices[index][ jIndex].Y);
                    vertices.Add(unParsedVertices[index][ jIndex].Z);
                    //First Normal
                    vertices.Add(n.X);
                    vertices.Add(n.Y);
                    vertices.Add(n.Z);
                    //Second Vertex
                    vertices.Add(unParsedVertices[index + 1][ jIndex].X);
                    vertices.Add(unParsedVertices[index + 1][ jIndex].Y);
                    vertices.Add(unParsedVertices[index + 1][ jIndex].Z);
                    //Second Normal
                    vertices.Add(n.X);
                    vertices.Add(n.Y);
                    vertices.Add(n.Z);
                    //Third Vertex
                    vertices.Add(unParsedVertices[index + 1][ jIndex + 1].X);
                    vertices.Add(unParsedVertices[index + 1][ jIndex + 1].Y);
                    vertices.Add(unParsedVertices[index + 1][ jIndex + 1].Z);
                    //Third Normal
                    vertices.Add(n.X);
                    vertices.Add(n.Y);
                    vertices.Add(n.Z);
                    
                    
                    //New Triangle
                    v01 = unParsedVertices[index][ jIndex];
                    v03 = unParsedVertices[index + 1][ jIndex + 1];
                    v02 = unParsedVertices[index][ jIndex + 1];

                    l1 = v02 - v01;
                    l2 = v03 - v01;
                    //Normals are the same for each triangle
                    n = Vector3.Cross(l1, l2);
                    
                    //First Vertex
                    vertices.Add(unParsedVertices[index][ jIndex].X);
                    vertices.Add(unParsedVertices[index][ jIndex].Y);
                    vertices.Add(unParsedVertices[index][ jIndex].Z);
                    //First Normal
                    vertices.Add(n.X);
                    vertices.Add(n.Y);
                    vertices.Add(n.Z);
                    //Second Vertex
                    vertices.Add(unParsedVertices[index + 1][ jIndex + 1].X);
                    vertices.Add(unParsedVertices[index + 1][ jIndex + 1].Y);
                    vertices.Add(unParsedVertices[index + 1][ jIndex + 1].Z);
                    //Second Normal
                    vertices.Add(n.X);
                    vertices.Add(n.Y);
                    vertices.Add(n.Z);
                    //Third Vertex
                    vertices.Add(unParsedVertices[index][ jIndex + 1].X);
                    vertices.Add(unParsedVertices[index][ jIndex + 1].Y);
                    vertices.Add(unParsedVertices[index][ jIndex + 1].Z);
                    //Third Normal
                    vertices.Add(n.X);
                    vertices.Add(n.Y);
                    vertices.Add(n.Z);
                }
            }

            return vertices.ToArray();
        }

        private static float[] CreateRectangularPrismVertices(float width, float height, float depth)
        {
            var w = width / 2;
            var h = height / 2;
            var d = depth / 2;
            List<float> vertices = new List<float>();

            float[] v =
            {
                -w, -h, -d,
                 w, -h, -d,
                 w, -h,  d,
                -w, -h,  d,
                -w,  h, -d,
                 w,  h, -d,
                 w,  h,  d,
                -w,  h,  d
            };

            int[] f =
            {
                //Front
                0, 5, 4,
                0, 1, 5,
                //Right
                1, 6, 5,
                1, 2, 6,
                //Back
                2, 7, 6,
                2, 3, 7,
                //Left
                3, 4, 7,
                3, 0, 4,
                //Bottom
                3, 1, 0,
                3, 2, 1,
                //Top
                4, 6, 7,
                4, 5, 6
            };

            for (int i = 0; i < f.Length; i+=3)
            {
                var v01 = new Vector3(v[f[i    ] * 3], v[f[i    ] * 3 + 1], v[f[i    ] * 3 + 2]);
                var v02 = new Vector3(v[f[i + 1] * 3], v[f[i + 1] * 3 + 1], v[f[i + 1] * 3 + 2]);
                var x = v[f[i + 2] * 3];
                var y = v[f[i + 2] * 3 + 1];
                var z = v[f[i + 2] * 3 + 2];
                var v03 = new Vector3(x, y, z);
        
                Vector3 l1 = v02 - v01;
                Vector3 l2 = v03 - v01;
                //Normals are the same for each triangle
                Vector3 n = Vector3.Cross(l1, l2);
                n.Normalize();
                vertices.Add(v01.X);
                vertices.Add(v01.Y);
                vertices.Add(v01.Z);
                vertices.Add(n.X);
                vertices.Add(n.Y);
                vertices.Add(n.Z);
                vertices.Add(v02.X);
                vertices.Add(v02.Y);
                vertices.Add(v02.Z);
                vertices.Add(n.X);
                vertices.Add(n.Y);
                vertices.Add(n.Z);
                vertices.Add(v03.X);
                vertices.Add(v03.Y);
                vertices.Add(v03.Z);
                vertices.Add(n.X);
                vertices.Add(n.Y);
                vertices.Add(n.Z);
            }

            return vertices.ToArray();
        }
        
        //-------------------------------------------
        //2D Functions
        //-------------------------------------------
        
        /// <summary>
        /// Draws a 2D textured rectangle to the screen
        /// </summary>
        /// <param name="x1">X component of the bottom left corner of the rectangle</param>
        /// <param name="y1">Y component of the bottom left corner of the rectangle</param>
        /// <param name="u1">U component of the bottom left corner of the rectangle texture</param>
        /// <param name="v1">V component of the bottom left corner of the rectangle texture</param>
        /// <param name="x2">X component of the top right corner of the rectangle</param>
        /// <param name="y2">Y component of the top right corner of the rectangle</param>
        /// <param name="u2">U component of the top right corner of the rectangle texture</param>
        /// <param name="v2">V component of the top right corner of the rectangle texture</param>
        /// <param name="texturePath">Path to the texture .png</param>
        /// <param name="color">Color to light the texture with</param>
        /// <param name="min">OpenGL Texture filtering tipe (Nearest for blocky, linear for fuzzy)</param>
        /// <param name="mag">OpenGL Texture filtering tipe (Nearest for blocky, linear for fuzzy)</param>
        protected void drawTexturedRectangle(float x1, float y1, float u1, float v1, float x2, float y2, float u2, float v2, string texturePath, Color4 color, TextureMinFilter min, TextureMagFilter mag)
        {
            Texture texture = new Texture(texturePath, min, mag);
            float x1Trans = x1 - (Size.X / 2);
            float y1Trans = y1 - (Size.Y / 2);
            float x1Norm = x1Trans / (Size.X / 2);
            float y1Norm = y1Trans / (Size.Y / 2);
            float x2Trans = x2 - (Size.X / 2);
            float y2Trans = y2 - (Size.Y / 2);
            float x2Norm = x2Trans / (Size.X / 2);
            float y2Norm = y2Trans / (Size.Y / 2);
            
            float[] vertices =
            {
                x1Norm, y1Norm, 0f, u1, v2,
                x1Norm, y2Norm, 0f, u1, v1,
                x2Norm, y2Norm, 0f, u2, v1,
                
                x1Norm, y1Norm, 0f, u1, v2,
                x2Norm, y1Norm, 0f, u2, v2,
                x2Norm, y2Norm, 0f, u2, v1
            };
            
            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
            
            texture.Use();
            _2dTextured.Use();
            
            var mainObject = GL.GenVertexArray();
            GL.BindVertexArray(mainObject);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            var positionLocation = _2dTextured.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _2dTextured.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            
            GL.BindVertexArray(mainObject);
            
            texture.Use();
            _2dTextured.Use();
            
            _2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            
            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteTexture(texture.Handle);
            GL.DeleteVertexArray(mainObject);

        }
        /// <summary>
        /// Draws a 2D textured rectangle to the screen
        /// </summary>
        /// <param name="x1">X component of the bottom left corner of the rectangle</param>
        /// <param name="y1">Y component of the bottom left corner of the rectangle</param>
        /// <param name="u1">U component of the bottom left corner of the rectangle texture</param>
        /// <param name="v1">V component of the bottom left corner of the rectangle texture</param>
        /// <param name="x2">X component of the top right corner of the rectangle</param>
        /// <param name="y2">Y component of the top right corner of the rectangle</param>
        /// <param name="u2">U component of the top right corner of the rectangle texture</param>
        /// <param name="v2">V component of the top right corner of the rectangle texture</param>
        /// <param name="textureBitmap">Bitmap of the texture</param>
        /// <param name="color">Color to light the texture with</param>
        /// <param name="min">OpenGL Texture filtering tipe (Nearest for blocky, linear for fuzzy)</param>
        /// <param name="mag">OpenGL Texture filtering tipe (Nearest for blocky, linear for fuzzy)</param>
        protected void drawTexturedRectangle(float x1, float y1, float u1, float v1, float x2, float y2, float u2, float v2, Bitmap textureBitmap, Color4 color, TextureMinFilter min, TextureMagFilter mag)
        {
            Texture texture = new Texture(textureBitmap, min, mag);
            float x1Trans = x1 - (Size.X / 2);
            float y1Trans = y1 - (Size.Y / 2);
            float x1Norm = x1Trans / (Size.X / 2);
            float y1Norm = y1Trans / (Size.Y / 2);
            float x2Trans = x2 - (Size.X / 2);
            float y2Trans = y2 - (Size.Y / 2);
            float x2Norm = x2Trans / (Size.X / 2);
            float y2Norm = y2Trans / (Size.Y / 2);

            float[] vertices =
            {
                x1Norm, y1Norm, 0f, u1, v2,
                x1Norm, y2Norm, 0f, u1, v1,
                x2Norm, y2Norm, 0f, u2, v1,

                x1Norm, y1Norm, 0f, u1, v2,
                x2Norm, y1Norm, 0f, u2, v2,
                x2Norm, y2Norm, 0f, u2, v1
            };

            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            texture.Use();
            _2dTextured.Use();

            var mainObject = GL.GenVertexArray();
            GL.BindVertexArray(mainObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            var positionLocation = _2dTextured.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _2dTextured.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            GL.BindVertexArray(mainObject);

            texture.Use();
            _2dTextured.Use();

            _2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteTexture(texture.Handle);
            GL.DeleteVertexArray(mainObject);

        }
        /// <summary>
        /// Draws a 2D textured rectangle to the screen
        /// </summary>
        /// <param name="x1">X component of the bottom left corner of the rectangle</param>
        /// <param name="y1">Y component of the bottom left corner of the rectangle</param>
        /// <param name="u1">U component of the bottom left corner of the rectangle texture</param>
        /// <param name="v1">V component of the bottom left corner of the rectangle texture</param>
        /// <param name="x2">X component of the top right corner of the rectangle</param>
        /// <param name="y2">Y component of the top right corner of the rectangle</param>
        /// <param name="u2">U component of the top right corner of the rectangle texture</param>
        /// <param name="v2">V component of the top right corner of the rectangle texture</param>
        /// <param name="texture">The texture to use</param>
        /// <param name="color">Color to light the texture with</param>
        protected void drawTexturedRectangle(float x1, float y1, float u1, float v1, float x2, float y2, float u2, float v2, Texture texture, Color4 color)
        {
            float x1Trans = x1 - (Size.X / 2);
            float y1Trans = y1 - (Size.Y / 2);
            float x1Norm = x1Trans / (Size.X / 2);
            float y1Norm = y1Trans / (Size.Y / 2);
            float x2Trans = x2 - (Size.X / 2);
            float y2Trans = y2 - (Size.Y / 2);
            float x2Norm = x2Trans / (Size.X / 2);
            float y2Norm = y2Trans / (Size.Y / 2);

            float[] vertices =
            {
                x1Norm, y1Norm, 0f, u1, v2,
                x1Norm, y2Norm, 0f, u1, v1,
                x2Norm, y2Norm, 0f, u2, v1,

                x1Norm, y1Norm, 0f, u1, v2,
                x2Norm, y1Norm, 0f, u2, v2,
                x2Norm, y2Norm, 0f, u2, v1
            };

            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            texture.Use();
            _2dTextured.Use();

            var mainObject = GL.GenVertexArray();
            GL.BindVertexArray(mainObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            var positionLocation = _2dTextured.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _2dTextured.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            GL.BindVertexArray(mainObject);

            texture.Use();
            _2dTextured.Use();

            _2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteVertexArray(mainObject);

        }
        /// <summary>
        /// Draws a line to the 2D screen
        /// </summary>
        /// <param name="x1">X pos of one end of the line</param>
        /// <param name="y1">Y pos of one end of the line</param>
        /// <param name="x2">X pos of the other end of the line</param>
        /// <param name="y2">Y pos of the other end of the line</param>
        /// <param name="color">Color of the line</param>
        protected void drawLine(float x1, float y1, float x2, float y2, Color4 color)
        {
            float x1Trans = x1 - (Size.X / 2);
            float y1Trans = y1 - (Size.Y / 2);
            float x1Norm = x1Trans / (Size.X / 2);
            float y1Norm = y1Trans / (Size.Y / 2);
            float x2Trans = x2 - (Size.X / 2);
            float y2Trans = y2 - (Size.Y / 2);
            float x2Norm = x2Trans / (Size.X / 2);
            float y2Norm = y2Trans / (Size.Y / 2);
            float[] vertices =
            {
                x1Norm, y1Norm, 0f,
                
                x2Norm, y2Norm, 0f
            };

            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            _2dShader.Use();
            
            var mainObject = GL.GenVertexArray();
            GL.BindVertexArray(mainObject);

            var positionLocation = _2dShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            GL.BindVertexArray(mainObject);
            
            _2dShader.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
            
            _2dShader.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            
            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteVertexArray(mainObject);
        }
        /// <summary>
        /// Draws a 2D rectangle to the 2D screen
        /// </summary>
        /// <param name="x1">X component of the bottom left vertex of the rectangle</param>
        /// <param name="y1">Y component of the bottom left vertex of the rectangle</param>
        /// <param name="x2">X component of the to right vertex of the rectangle</param>
        /// <param name="y2">Y component of the to right vertex of the rectangle</param>
        /// <param name="color">Color of the rectangle</param>
        protected void drawRectangle(float x1, float y1, float x2, float y2, Color4 color)
        {
            float x1Trans = x1 - (Size.X / 2);
            float y1Trans = y1 - (Size.Y / 2);
            float x1Norm = x1Trans / (Size.X / 2);
            float y1Norm = y1Trans / (Size.Y / 2);
            float x2Trans = x2 - (Size.X / 2);
            float y2Trans = y2 - (Size.Y / 2);
            float x2Norm = x2Trans / (Size.X / 2);
            float y2Norm = y2Trans / (Size.Y / 2);
            float[] vertices =
            {
                x1Norm, y1Norm, 0f,
                x2Norm, y1Norm, 0f,
                x1Norm, y2Norm, 0f,
                
                x2Norm, y1Norm, 0f,
                x2Norm, y2Norm, 0f,
                x1Norm, y2Norm, 0f
            };

            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            _2dShader.Use();
            
            var mainObject = GL.GenVertexArray();
            GL.BindVertexArray(mainObject);

            var positionLocation = _2dShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            GL.BindVertexArray(mainObject);
            
            _2dShader.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
            
            _2dShader.Use();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            
            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteVertexArray(mainObject);

        }
        /// <summary>
        /// Draws a line to the 2D screen with a texture
        /// </summary>
        /// <param name="x1">X pos of one end of the line</param>
        /// <param name="y1">X pos of one end of the line</param>
        /// <param name="u1">U pos of one end of the texture</param>
        /// <param name="v1">V pos of one end of the texture</param>
        /// <param name="x2">X pos of the other end of the line</param>
        /// <param name="y2">X pos of the other end of the line</param>
        /// <param name="u2">U pos of the other end of the texture</param>
        /// <param name="v2">V pos of the other end of the texture</param>
        /// <param name="texture">Path to the texture .png</param>
        /// <param name="color">Color to be overlaid in the texture</param>
        protected void drawTexturedLine(float x1, float y1, float u1, float v1, float x2, float y2, float u2, float v2, Texture texture, Color4 color)
        {
            float x1Trans = x1 - (Size.X / 2);
            float y1Trans = y1 - (Size.Y / 2);
            float x1Norm = x1Trans / (Size.X / 2);
            float y1Norm = y1Trans / (Size.Y / 2);
            float x2Trans = x2 - (Size.X / 2);
            float y2Trans = y2 - (Size.Y / 2);
            float x2Norm = x2Trans / (Size.X / 2);
            float y2Norm = y2Trans / (Size.Y / 2);
            float[] vertices =
            {
                x1Norm, y1Norm, 0f, u1, v1,
                
                x2Norm, y2Norm, 0f, u2, v2
            };
            
            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
            
            texture.Use();
            _2dTextured.Use();
            
            var mainObject = GL.GenVertexArray();
            GL.BindVertexArray(mainObject);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            var positionLocation = _2dTextured.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _2dTextured.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            
            GL.BindVertexArray(mainObject);
            
            texture.Use();
            _2dTextured.Use();
            
            _2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
            
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            
            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteVertexArray(mainObject);
        }
        /// <summary>
        /// Draws a 2D textured quad to the 2D screen given clockwise vertex's
        /// </summary>
        /// <param name="x1">X pos of the first vertex</param>
        /// <param name="y1">Y pos of the first vertex</param>
        /// <param name="z1">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="u1">U pos of the first texture vertex</param>
        /// <param name="v1">V pos of the first texture vertex</param>
        /// <param name="x2">X pos of the second vertex</param>
        /// <param name="y2">Y pos of the second vertex</param>
        /// <param name="z2">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="u2">U pos of the second texture vertex</param>
        /// <param name="v2">V pos of the second texture vertex</param>
        /// <param name="x3">X pos of the third vertex</param>
        /// <param name="y3">Y pos of the third vertex</param>
        /// <param name="z3">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="u3">U pos of the third texture vertex</param>
        /// <param name="v3">V pos of the third texture vertex</param>        
        /// <param name="x4">X pos of the last vertex</param>
        /// <param name="y4">Y pos of the last vertex</param>
        /// <param name="z4">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="u4">U pos of the last texture vertex</param>
        /// <param name="v4">V pos of the last texture vertex</param>        
        /// <param name="texturePath">Path to the texture .png</param>
        /// <param name="color">Color to be overlaid on the texture</param>
        /// <param name="min">OpenGL Texture filtering tipe (Nearest for blocky, linear for fuzzy)</param>
        /// <param name="mag">OpenGL Texture filtering tipe (Nearest for blocky, linear for fuzzy)</param>
        protected void drawTexturedQuad(float x1, float y1, float z1, float u1, float v1, 
                                      float x2, float y2, float z2, float u2, float v2, 
                                      float x3, float y3, float z3, float u3, float v3,
                                      float x4, float y4, float z4, float u4, float v4, string texturePath, Color4 color, TextureMinFilter min, TextureMagFilter mag)
        {
            Texture texture = new Texture(texturePath, min, mag);
            float x1Trans = x1 - (Size.X / 2);
            float y1Trans = y1 - (Size.Y / 2);
            float x1Norm = x1Trans / (Size.X / 2);
            float y1Norm = y1Trans / (Size.Y / 2);
            float x2Trans = x2 - (Size.X / 2);
            float y2Trans = y2 - (Size.Y / 2);
            float x2Norm = x2Trans / (Size.X / 2);
            float y2Norm = y2Trans / (Size.Y / 2);
            float x3Trans = x3 - (Size.X / 2);
            float y3Trans = y3 - (Size.Y / 2);
            float x3Norm = x3Trans / (Size.X / 2);
            float y3Norm = y3Trans / (Size.Y / 2);
            float x4Trans = x4 - (Size.X / 2);
            float y4Trans = y4 - (Size.Y / 2);
            float x4Norm = x4Trans / (Size.X / 2);
            float y4Norm = y4Trans / (Size.Y / 2);

            float[] vertices =
            {
                x1Norm, y1Norm, z1, u1, -(v1 - 1),
                x2Norm, y2Norm, z2, u2, -(v2 - 1),
                x3Norm, y3Norm, z3, u3, -(v3 - 1),

                x1Norm, y1Norm, z2, u1, -(v1 - 1),
                x3Norm, y3Norm, z3, u3, -(v3 - 1),
                x4Norm, y4Norm, z4, u4, -(v4 - 1)
            };

            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
            
            texture.Use();
            _2dTextured.Use();
            
            var mainObject = GL.GenVertexArray();
            GL.BindVertexArray(mainObject);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            var positionLocation = _2dTextured.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _2dTextured.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            
            GL.BindVertexArray(mainObject);
            
            texture.Use();
            _2dTextured.Use();
            
            _2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            
            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteTexture(texture.Handle);
            GL.DeleteVertexArray(mainObject);

        }
        /// <summary>
        /// Draws a 2D textured quad to the 2D screen given clockwise vertex's
        /// </summary>
        /// <param name="x1">X pos of the first vertex</param>
        /// <param name="y1">Y pos of the first vertex</param>
        /// <param name="z1">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="u1">U pos of the first texture vertex</param>
        /// <param name="v1">V pos of the first texture vertex</param>
        /// <param name="x2">X pos of the second vertex</param>
        /// <param name="y2">Y pos of the second vertex</param>
        /// <param name="z2">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="u2">U pos of the second texture vertex</param>
        /// <param name="v2">V pos of the second texture vertex</param>
        /// <param name="x3">X pos of the third vertex</param>
        /// <param name="y3">Y pos of the third vertex</param>
        /// <param name="z3">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="u3">U pos of the third texture vertex</param>
        /// <param name="v3">V pos of the third texture vertex</param>        
        /// <param name="x4">X pos of the last vertex</param>
        /// <param name="y4">Y pos of the last vertex</param>
        /// <param name="z4">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="u4">U pos of the last texture vertex</param>
        /// <param name="v4">V pos of the last texture vertex</param>        
        /// <param name="textureBitmap">Bitmap of the texture</param>
        /// <param name="color">Color to be overlaid on the texture</param>
        /// <param name="min">OpenGL Texture filtering tipe (Nearest for blocky, linear for fuzzy)</param>
        /// <param name="mag">OpenGL Texture filtering tipe (Nearest for blocky, linear for fuzzy)</param>
        protected void drawTexturedQuad(float x1, float y1, float z1, float u1, float v1,
                                      float x2, float y2, float z2, float u2, float v2,
                                      float x3, float y3, float z3, float u3, float v3,
                                      float x4, float y4, float z4, float u4, float v4, Bitmap textureBitmap, Color4 color, TextureMinFilter min, TextureMagFilter mag)
        {
            Texture texture = new Texture(textureBitmap, min, mag);
            float x1Trans = x1 - (Size.X / 2);
            float y1Trans = y1 - (Size.Y / 2);
            float x1Norm = x1Trans / (Size.X / 2);
            float y1Norm = y1Trans / (Size.Y / 2);
            float x2Trans = x2 - (Size.X / 2);
            float y2Trans = y2 - (Size.Y / 2);
            float x2Norm = x2Trans / (Size.X / 2);
            float y2Norm = y2Trans / (Size.Y / 2);
            float x3Trans = x3 - (Size.X / 2);
            float y3Trans = y3 - (Size.Y / 2);
            float x3Norm = x3Trans / (Size.X / 2);
            float y3Norm = y3Trans / (Size.Y / 2);
            float x4Trans = x4 - (Size.X / 2);
            float y4Trans = y4 - (Size.Y / 2);
            float x4Norm = x4Trans / (Size.X / 2);
            float y4Norm = y4Trans / (Size.Y / 2);

            float[] vertices =
            {
                x1Norm, y1Norm, z1, u1, -(v1 - 1),
                x2Norm, y2Norm, z2, u2, -(v2 - 1),
                x3Norm, y3Norm, z3, u3, -(v3 - 1),

                x1Norm, y1Norm, z2, u1, -(v1 - 1),
                x3Norm, y3Norm, z3, u3, -(v3 - 1),
                x4Norm, y4Norm, z4, u4, -(v4 - 1)
            };

            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            texture.Use();
            _2dTextured.Use();

            var mainObject = GL.GenVertexArray();
            GL.BindVertexArray(mainObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            var positionLocation = _2dTextured.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _2dTextured.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            GL.BindVertexArray(mainObject);

            texture.Use();
            _2dTextured.Use();

            _2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteTexture(texture.Handle);
            GL.DeleteVertexArray(mainObject);

        }
        /// <summary>
        /// Draws a 2D textured quad to the 2D screen given clockwise vertex's
        /// </summary>
        /// <param name="x1">X pos of the first vertex</param>
        /// <param name="y1">Y pos of the first vertex</param>
        /// <param name="z1">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="u1">U pos of the first texture vertex</param>
        /// <param name="v1">V pos of the first texture vertex</param>
        /// <param name="x2">X pos of the second vertex</param>
        /// <param name="y2">Y pos of the second vertex</param>
        /// <param name="z2">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="u2">U pos of the second texture vertex</param>
        /// <param name="v2">V pos of the second texture vertex</param>
        /// <param name="x3">X pos of the third vertex</param>
        /// <param name="y3">Y pos of the third vertex</param>
        /// <param name="z3">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="u3">U pos of the third texture vertex</param>
        /// <param name="v3">V pos of the third texture vertex</param>        
        /// <param name="x4">X pos of the last vertex</param>
        /// <param name="y4">Y pos of the last vertex</param>
        /// <param name="z4">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="u4">U pos of the last texture vertex</param>
        /// <param name="v4">V pos of the last texture vertex</param> 
        /// <param name="texture">Texture of the quad</param>
        /// <param name="color">Color to be overlaid on the texture</param>
        protected void drawTexturedQuad(float x1, float y1, float z1, float u1, float v1,
                                      float x2, float y2, float z2, float u2, float v2,
                                      float x3, float y3, float z3, float u3, float v3,
                                      float x4, float y4, float z4, float u4, float v4, Texture texture, Color4 color)
        {
            float x1Trans = x1 - (Size.X / 2);
            float y1Trans = y1 - (Size.Y / 2);
            float x1Norm = x1Trans / (Size.X / 2);
            float y1Norm = y1Trans / (Size.Y / 2);
            float x2Trans = x2 - (Size.X / 2);
            float y2Trans = y2 - (Size.Y / 2);
            float x2Norm = x2Trans / (Size.X / 2);
            float y2Norm = y2Trans / (Size.Y / 2);
            float x3Trans = x3 - (Size.X / 2);
            float y3Trans = y3 - (Size.Y / 2);
            float x3Norm = x3Trans / (Size.X / 2);
            float y3Norm = y3Trans / (Size.Y / 2);
            float x4Trans = x4 - (Size.X / 2);
            float y4Trans = y4 - (Size.Y / 2);
            float x4Norm = x4Trans / (Size.X / 2);
            float y4Norm = y4Trans / (Size.Y / 2);

            float[] vertices =
            {
                x1Norm, y1Norm, z1, u1, -(v1 - 1),
                x2Norm, y2Norm, z2, u2, -(v2 - 1),
                x3Norm, y3Norm, z3, u3, -(v3 - 1),

                x1Norm, y1Norm, z2, u1, -(v1 - 1),
                x3Norm, y3Norm, z3, u3, -(v3 - 1),
                x4Norm, y4Norm, z4, u4, -(v4 - 1)
            };

            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            texture.Use();
            _2dTextured.Use();

            var mainObject = GL.GenVertexArray();
            GL.BindVertexArray(mainObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            var positionLocation = _2dTextured.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _2dTextured.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            GL.BindVertexArray(mainObject);

            texture.Use();
            _2dTextured.Use();

            _2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteVertexArray(mainObject);

        }    
        /// <summary>
        /// Draws a 2D quad to the 2D screen given clockwise vertex's
        /// </summary>
        /// <param name="x1">X pos of the first vertex</param>
        /// <param name="y1">Y pos of the first vertex</param>
        /// <param name="z1">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="x2">X pos of the second vertex</param>
        /// <param name="y2">Y pos of the second vertex</param>
        /// <param name="z2">Z pos used for depth test (NOT 3D!!)</param>        
        /// <param name="x3">X pos of the third vertex</param>
        /// <param name="y3">Y pos of the third vertex</param>
        /// <param name="z3">Z pos used for depth test (NOT 3D!!)</param>        
        /// <param name="x4">X pos of the last vertex</param>
        /// <param name="y4">Y pos of the last vertex</param>
        /// <param name="z4">Z pos used for depth test (NOT 3D!!)</param>
        /// <param name="color">Color of the quad</param>
        protected void drawQuad(float x1, float y1, float z1, 
                                float x2, float y2, float z2, 
                                float x3, float y3, float z3,
                                float x4, float y4, float z4, Color4 color)
        {
            float x1Trans = x1 - (Size.X / 2);
            float y1Trans = y1 - (Size.Y / 2);
            float x1Norm = x1Trans / (Size.X / 2);
            float y1Norm = y1Trans / (Size.Y / 2);
            float x2Trans = x2 - (Size.X / 2);
            float y2Trans = y2 - (Size.Y / 2);
            float x2Norm = x2Trans / (Size.X / 2);
            float y2Norm = y2Trans / (Size.Y / 2);
            float x3Trans = x3 - (Size.X / 2);
            float y3Trans = y3 - (Size.Y / 2);
            float x3Norm = x3Trans / (Size.X / 2);
            float y3Norm = y3Trans / (Size.Y / 2);
            float x4Trans = x4 - (Size.X / 2);
            float y4Trans = y4 - (Size.Y / 2);
            float x4Norm = x4Trans / (Size.X / 2);
            float y4Norm = y4Trans / (Size.Y / 2);
                    
            float[] vertices =
            {
                x1Norm, y1Norm, z1,
                x2Norm, y2Norm, z2,
                x3Norm, y3Norm, z3,
                        
                x2Norm, y2Norm, z2,
                x3Norm, y3Norm, z3,
                x4Norm, y4Norm, z4
            };
                    
            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
                    
            _2dShader.Use();
                    
            var mainObject = GL.GenVertexArray();
            GL.BindVertexArray(mainObject);
                    
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        
            var positionLocation = _2dShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                    
                    
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
                    
            GL.BindVertexArray(mainObject);
                    
            _2dShader.Use();
                    
            _2dShader.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
        
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                    
            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteVertexArray(mainObject);
        
        }
        /// <summary>
        /// Draws a 2D ellipse to the 2D screen
        /// </summary>
        /// <param name="x">X pos of the center of the ellipse</param>
        /// <param name="y">y pos of the center of the ellipse</param>
        /// <param name="radiusX">Radius of the ellipse in the x direction</param>
        /// <param name="radiusY">Radius of the ellipse in the y direction</param>
        /// <param name="color">Color of the ellipse</param>
        protected void drawEllipse(float x, float y, float radiusX, float radiusY, Color4 color)
        {
            int numEllipseVertices = (int)Math.Floor(Math.Sqrt(radiusX * radiusX + radiusY * radiusY));
            Vector3[] tempVertices = new Vector3[numEllipseVertices];
            
            float xTrans = x - (Size.X / 2);
            float yTrans = y - (Size.Y / 2);
            float xNorm = xTrans / (Size.X / 2);
            float yNorm = yTrans / (Size.Y / 2);
            float radiusXNorm = radiusX / (Size.X / 2);
            float radiusYNorm = radiusY / (Size.Y / 2);


            var step = (float)(Math.PI * 2) / (numEllipseVertices - 1);
            
            for(var i=0; i < numEllipseVertices; i++)
            {
                var rad = i * step;
                tempVertices[i] = new Vector3(((float) Math.Cos(rad) * radiusXNorm) + xNorm, ((float) Math.Sin(rad) * radiusYNorm) - yNorm, 0.0f);
            }
            

            var tempVerticesList = new List<float> {xNorm, -yNorm, 0f,};
            for (var i = 0; i < numEllipseVertices; i++)
            {
                tempVerticesList.AddRange(new []
                {
                    tempVertices[i].X, tempVertices[i].Y, tempVertices[i].Z,
                });
            }

            var vertices = tempVerticesList.ToArray();
            

            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            _2dShader.Use();
            
            var mainObject = GL.GenVertexArray();
            GL.BindVertexArray(mainObject);

            var positionLocation = _2dShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            GL.BindVertexArray(mainObject);
            
            _2dShader.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
            
            _2dShader.Use();
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, numEllipseVertices + 1);
            
            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteVertexArray(mainObject);
        }
        /// <summary>
        /// Draws a 2D triangle to the 2D screen, given clockwise points
        /// </summary>
        /// <param name="x1">X pos of the first vertex</param>
        /// <param name="x1">X pos of the first vertex</param>
        /// <param name="y1">Y pos of the first vertex</param>
        /// <param name="x2">X pos of the second vertex</param>
        /// <param name="y2">Y pos of the second vertex</param>
        /// <param name="x3">X pos of the third vertex</param>
        /// <param name="y3">Y pos of the third vertex</param>
        /// <param name="color">Color of the triangle</param>
        public void drawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Color4 color)
        {
            float x1Trans = x1 - (Size.X / 2);
            float y1Trans = y1 - (Size.Y / 2);
            float x1Norm = x1Trans / (Size.X / 2);
            float y1Norm = y1Trans / (Size.Y / 2);
            float x2Trans = x2 - (Size.X / 2);
            float y2Trans = y2 - (Size.Y / 2);
            float x2Norm = x2Trans / (Size.X / 2);
            float y2Norm = y2Trans / (Size.Y / 2);
            float x3Trans = x3 - (Size.X / 2);
            float y3Trans = y3 - (Size.Y / 2);
            float x3Norm = x3Trans / (Size.X / 2);
            float y3Norm = y3Trans / (Size.Y / 2);
            float[] vertices =
            {
                x1Norm, -y1Norm, 0f,
                x2Norm, -y2Norm, 0f,
                x3Norm, -y3Norm, 0f,
            };

            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            _2dShader.Use();
            
            var mainObject = GL.GenVertexArray();
            GL.BindVertexArray(mainObject);

            var positionLocation = _2dShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            GL.BindVertexArray(mainObject);
            
            _2dShader.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
            
            _2dShader.Use();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            
            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteVertexArray(mainObject);

        }
        /// <summary>
        /// Clears the screen
        /// </summary>
        protected void Clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        /// <summary>
        /// Draws text to the screen
        /// </summary>
        /// <param name="text">Text to be drawn</param>
        /// <param name="px">Vertical size of the text</param>
        /// <param name="x">X pos of the bottom left corner of the text</param>
        /// <param name="y">Y pos of the bottom left corner of the text</param>
        /// <param name="f">The font to be used to draw the text</param>
        /// <param name="col">Color of the text</param>
        /// <param name="textAlign">Alignment of the text, default bottom left</param>
        public void drawText(string text, float x, float y, Font f, Color4 col, int textAlign = 0b00001000)
        {
            Bitmap bmt = new Bitmap(Width, Height);
            Graphics grx = Graphics.FromImage(bmt);
            //grx.MeasureString(text, f);
            Vector2i regPos = new Vector2i(Convert.ToInt16(x), Convert.ToInt16(y));
            Vector2i pos = new Vector2i(0, 0);
            int length = Convert.ToInt16(grx.MeasureString(text, f).Width);
            switch (textAlign)
            {
                case 0b0000000:
                    pos = new Vector2i(regPos.X - length, regPos.Y - f.Height);
                    break;
                case 0b0000001:
                    pos = new Vector2i(regPos.X - (length / 2), regPos.Y - f.Height);
                    break;
                case 0b0000010:
                    pos = new Vector2i(regPos.X, regPos.Y - f.Height);
                    break;
                case 0b0000011:
                    pos = new Vector2i(regPos.X - length, regPos.Y - (f.Height / 2));
                    break;
                case 0b0000100:
                    pos = new Vector2i(regPos.X - (length / 2), regPos.Y - (f.Height / 2));
                    break;
                case 0b0000101:
                    pos = new Vector2i(regPos.X, regPos.Y - (f.Height / 2));
                    break;
                case 0b0000110:
                    pos = new Vector2i(regPos.X - length, regPos.Y);
                    break;
                case 0b0000111:
                    pos = new Vector2i(regPos.X - (length / 2), regPos.Y);
                    break;
                case 0b0001000:
                    pos = regPos;
                    break;
                default:
                    throw new Exception("Wrong textAlign value, use the interface \"TextAlign\"");
            }
            grx.DrawString(text, f, Brushes.White, pos.X, Math.Abs(pos.Y - Height) - f.Height);
            drawTexturedRectangle(0, 0, 0, 0, Width, Height, 1, 1, bmt, col, TextureMinFilter.Linear, TextureMagFilter.Linear);
        }

        /// <summary>
        /// Interface containing the text alignments for the drawText() function
        /// </summary>
        public interface ITextAlign
        {
            /// <summary>
            /// Text is drawn from the upper right corner
            /// </summary>
            public static int UpRight      = 0b00000000;
            /// <summary>
            /// Text is drawn from the upper side and centered horizontally
            /// </summary>
            public static int UpCenter     = 0b00000001;
            /// <summary>
            /// Text is drawn from upper left corner
            /// </summary>
            public static int UpLeft       = 0b00000010;
            /// <summary>
            /// Text is drawn from the right side and centered vertically
            /// </summary>
            public static int MiddleRight  = 0b00000011;
            /// <summary>
            /// Text is drawn from the center of the text
            /// </summary>
            public static int MiddleCenter = 0b00000100;
            /// <summary>
            /// Text is drawn from the left side and centered vertically
            /// </summary>
            public static int MiddleLeft   = 0b00000101;
            /// <summary>
            /// Text is drawn from bottom right corner
            /// </summary>
            public static int BottomRight  = 0b00000110;
            /// <summaryb
            /// Text is drawn from the bottom side and centered horizontally
            /// </summary>
            public static int BottomCenter = 0b00000111;
            /// <summary>
            /// Text is drawn from bottom left corner
            /// </summary>
            public static int BottomLeft   = 0b00001000;
        }

        public Settings set = new Settings();

        private void showSettings(Settings s)
        {
            var w = Convert.ToInt32(s.settings["width"]);
            var h = Convert.ToInt32(s.settings["height"]);
            Vector2 pos = new Vector2((Size.X - w) / 2, (Size.Y - h) / 2);
            if (Convert.ToBoolean(s.settings["useTexture"]))
            {
                var path = Convert.ToString(s.settings["texturePath"]);
                drawTexturedRectangle(pos.X, pos.Y, 0, 0, pos.X + w, pos.Y + h, 1, 1, path, Color4.White, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            }
            else
            {
                Color4 col = new Color4(Convert.ToInt32(s.settings["r"]), Convert.ToInt32(s.settings["g"]), Convert.ToInt32(s.settings["b"]), Convert.ToInt32(s.settings["a"]));
                drawRectangle(pos.X, pos.Y, pos.X + w, pos.Y + h, col);
            }
            foreach (Settings.Button b in s.buttons)
            {
                var x = b.pos.X + pos.X;
                var y = b.pos.Y + pos.Y;
                drawRectangle(x, y, x + b.width, y + b.height, b.col);
                drawText(b.Text, x + (b.width), y + (b.height), b.font, Color4.White, ITextAlign.BottomCenter);
                b.setCol(Color4.Blue);
            }
        }
        /// <summary>
        /// Classed used to create settings for your project
        /// TODO add textured buttons
        /// TODO add sliders
        /// TODO add labels
        /// TODO add radio buttons
        /// </summary>
        public class Settings
        {
            public List<Button> buttons = new List<Button>();
            public Dictionary<string, object> settings = new Dictionary<string, object>();

            public class Button
            {
                public Vector2 pos;
                public string Text;
                public int width, height, l;
                public Func<object> onClick;
                public Color4 col;
                public Font font;

                public void setCol(Color4 c)
                {
                    col = c;
                }
            }
            /// <summary>
            /// Adds a simple button to the settings
            /// </summary>
            /// <param name="t">Text on the button</param>
            /// <param name="x">X pos of the button RELATIVE to the settings, at BOTTOM LEFT corner</param>
            /// <param name="y">Y pos of the button RELATIVE to the settings, at BOTTOM LEFT corner</param>
            /// <param name="w">Width of the button</param>
            /// <param name="h">Height of the button</param>
            /// <param name="c">Color of the button</param>
            /// <param name="func">Lambda function that is executed when button is clicked</param>
            /// <param name="f">Font of the button text</param>
            public void addButton(string t, float x, float y, int w, int h, Color4 c, Func<object> func, Font f)
            {
                buttons.Add(new Button { pos = new Vector2(x, y), width = w, height = h, onClick = func, col = c, Text = t, l = -1 , font = f});
            }
            /// <summary>
            /// Adds a setting to the settings dictionary
            /// </summary>
            /// <param name="key">Key of the setting</param>
            /// <param name="value">Value of the setting</param>
            public void addSetting(string key, object value)
            {
                settings.Add(key, value);
            }
            /// <summary>
            /// Reads the settings.cfg file
            /// <para>&nbsp;</para>
            /// Make sure the following items exist on the file <br />
            /// width=             (float)</br>
            /// height=            (float)</br>
            /// useTexture=        (bool)</br>
            /// If useTexture=false</br>
            ///     r=                 (float from 0-1 or int from 0-255)</br>
            ///     g=                 (float from 0-1 or int from 0-255)</br>
            ///     b=                 (float from 0-1 or int from 0-255)</br>
            ///     a=                 (float from 0-1 or int from 0-255)</br>
            /// If useTexture=false</br>
            ///     texturePath=       (String path to background texture)</br>
            /// </summary>
            public void readSettings()
            {
                if (File.Exists("settings.cfg"))
                {
                    using (StreamReader file = new StreamReader("settings.cfg"))
                    {
                        settings = new Dictionary<string, object>();
                        string ln;
                        while ((ln = file.ReadLine()) != null)
                        {
                            if (ln[0] != '#')
                            {
                                var values = ln.Split("=");
                                Regex rx = new Regex(@"^[\d.]+$");
                                if (rx.IsMatch(values[1]))
                                {

                                    settings.Add(values[0], float.Parse(values[1]));
                                }
                                else
                                {
                                    settings.Add(values[0], values[1]);
                                }
                            }
                        }
                    } 
                }
                else
                {
                    settings = new Dictionary<string, object>();

                    settings.Add("width", 200);
                    settings.Add("height", 300);
                    settings.Add("useTexture", false);
                    settings.Add("r", 0);
                    settings.Add("g", 127);
                    settings.Add("b", 256);
                    settings.Add("a", 1);
                    
                    writeSettings();
                }
            }
            /// <summary>
            /// Writes settings dictionary to the settings.cfg file
            /// </summary>
            public void writeSettings()
            {
                try
                {
                    using (FileStream fs = File.Create("settings.cfg"))
                    {
                        string final = "";

                        foreach (KeyValuePair<string, object> entry in settings)
                        {
                            final += entry.Key + "=" + entry.Value + "\n";
                        }

                        byte[] info = new UTF8Encoding(true).GetBytes(final);
                        fs.Write(info, 0, info.Length);
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }
        private void checkClicks(Settings s)
        {
            var w = Convert.ToInt32(s.settings["width"]);
            var h = Convert.ToInt32(s.settings["height"]);
            foreach (Settings.Button b in s.buttons)
            {
                Vector2 pos = new Vector2((Size.X - w) / 2, (Size.Y - h) / 2);
                var x = MousePosition.X - pos.X;
                var y = -(MousePosition.Y - Size.Y) - pos.Y;
                if (x >= b.pos.X && x <= b.pos.X + b.width && y >= b.pos.Y && y <= b.pos.Y + b.height)
                {
                    b.setCol(Color4.Red);
                    b.onClick.Invoke();
                }
            }
        }    

    }
}
