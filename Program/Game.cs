using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Program.Shaders;
using Boolean = System.Boolean;

namespace Program
{
    public class Game : GameWindow
    {

        private List<TexturedObject> mainTexturedObjects = new List<TexturedObject> { };
        private List<Object> mainObjects = new List<Object> { };
        private Lamp mainLamp;
        private Shader _lampShader, _lightingShader, _textureShader, _2dShader;
        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;
        private readonly Vector3 _lightPos = new Vector3(-10.0f, 10.0f, 20.0f);
        private int x, y;
        public Boolean RenderLight;
        public float cameraSpeed = 20f;
        public float sensitivity = 0.2f;
        public Game(int width, int height, string title)
            : base(width, height, GraphicsMode.Default, title)
        {
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            _textureShader = new Shader("Shaders/texture.vert", "Shaders/texture.frag");
            _2dShader = new Shader("Shaders/shader2d.vert", "Shaders/shader2d.frag");
            _lightingShader.Use();
            _lampShader.Use();
            _textureShader.Use();
            _2dShader.Use();
                                                        
            _camera = new Camera(Vector3.UnitZ * 3, Width / (float)Height);
            x = 0; y = 0;
            CursorVisible = false;
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (RenderLight) { mainLamp.show(_camera, _lampShader); };
            foreach (Object obj in mainObjects)
            {
                obj.show(_camera);
            }
            foreach (TexturedObject obj in mainTexturedObjects)
            {
                obj.show(_camera);
            }

            SwapBuffers();

            base.OnRenderFrame(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {

            if (!Focused) // check to see if the window is focused
            {
                return;
            }

            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (input.IsKeyDown(Key.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Key.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Key.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Key.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Key.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Key.LShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = Mouse.GetState();

            if (_firstMove) // this bool variable is initially set to true
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top
            }

            Mouse.SetPosition(1920 / 2, 1080 / 2);
            base.OnUpdateFrame(e);
        }
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }
        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            

            GL.DeleteProgram(_lampShader.Handle);
            GL.DeleteProgram(_lightingShader.Handle);
            GL.DeleteProgram(_2dShader.Handle);
            GL.DeleteProgram(_textureShader.Handle);

            foreach(Object obj in mainObjects)
            {
                obj.Dispose();
            }
            foreach(TexturedObject obj in mainTexturedObjects)
            {
                obj.Dispose();
            }
            if(mainLamp != null) { mainLamp.Dispose(); }



            base.OnUnload(e);
        }
        public static float[] loadObj(string Path)
        {
            string[] lines = System.IO.File.ReadAllLines(Path);
            List<float[]> vertices = new List<float[]> { };
            List<float> final = new List<float> { };
            foreach (string line in lines)
            {
                string[] line_splitted = line.Split(" ");
                if (line_splitted[0] == "v")
                {
                    float[] toAdd = new float[3];
                    toAdd[0] = (float.Parse(line_splitted[1]));
                    toAdd[1] = (float.Parse(line_splitted[2]));
                    toAdd[2] = (float.Parse(line_splitted[3]));
                    vertices.Add(toAdd);
                }
                if (line_splitted[0] == "f")
                {
                    string[] t1 = line_splitted[1].Split("//");
                    string[] t2 = line_splitted[2].Split("//");
                    string[] t3 = line_splitted[3].Split("//");



                    float[] v1 = vertices[int.Parse(t1[0]) - 1];
                    float[] v2 = vertices[int.Parse(t2[0]) - 1];
                    float[] v3 = vertices[int.Parse(t3[0]) - 1];

                    Vector3 _v1 = new Vector3(v1[0], v1[1], v1[2]);
                    Vector3 _v2 = new Vector3(v2[0], v2[1], v2[2]);
                    Vector3 _v3 = new Vector3(v3[0], v3[1], v3[2]);

                    Vector3 l1 = _v2 - _v1;
                    Vector3 l2 = _v3 - _v1;

                    Vector3 n = Vector3.Cross(l2, l1);

                    final.Add(v1[0]); final.Add(v1[1]); final.Add(v1[2]); final.Add(n.X); final.Add(n.Y); final.Add(n.Z);
                    final.Add(v2[0]); final.Add(v2[1]); final.Add(v2[2]); final.Add(n.X); final.Add(n.Y); final.Add(n.Z);
                    final.Add(v3[0]); final.Add(v3[1]); final.Add(v3[2]); final.Add(n.X); final.Add(n.Y); final.Add(n.Z);
                }

            }


            return final.ToArray();
        }
        public static float[] loadObjTextured(string Path)
        {
            string[] lines = System.IO.File.ReadAllLines(Path);
            List<float[]> vertices = new List<float[]> { };
            List<float[]> textureCords = new List<float[]> { };
            List<float> final = new List<float> { };
            foreach (string line in lines)
            {
                string[] line_splitted = line.Split(" ");
                if (line_splitted[0] == "v")
                {
                    float[] toAdd = new float[3];
                    toAdd[0] = (float.Parse(line_splitted[1]));
                    toAdd[1] = (float.Parse(line_splitted[2]));
                    toAdd[2] = (float.Parse(line_splitted[3]));
                    vertices.Add(toAdd);
                }
                if (line_splitted[0] == "vt")
                {
                    float[] toAdd = new float[2];
                    toAdd[0] = (float.Parse(line_splitted[1]));
                    toAdd[1] = (-(float.Parse(line_splitted[2]) - 1));
                    textureCords.Add(toAdd);
                }
                if (line_splitted[0] == "f")
                {
                    string[] t1 = line_splitted[1].Split("/");
                    string[] t2 = line_splitted[2].Split("/");
                    string[] t3 = line_splitted[3].Split("/");



                    float[] v1 = vertices[int.Parse(t1[0]) - 1];
                    float[] v2 = vertices[int.Parse(t2[0]) - 1];
                    float[] v3 = vertices[int.Parse(t3[0]) - 1];
                    float[] tex1 = textureCords[int.Parse(t1[1]) - 1];
                    float[] tex2 = textureCords[int.Parse(t2[1]) - 1];
                    float[] tex3 = textureCords[int.Parse(t3[1]) - 1];

                    Vector3 _v1 = new Vector3(v1[0], v1[1], v1[2]);
                    Vector3 _v2 = new Vector3(v2[0], v2[1], v2[2]);
                    Vector3 _v3 = new Vector3(v3[0], v3[1], v3[2]);

                    Vector3 l1 = _v2 - _v1;
                    Vector3 l2 = _v3 - _v1;

                    Vector3 n = Vector3.Cross(l2, l1);

                    final.Add(v1[0]); final.Add(v1[1]); final.Add(v1[2]); final.Add(n.X); final.Add(n.Y); final.Add(n.Z); final.Add(tex1[0]); final.Add(tex1[1]);
                    final.Add(v2[0]); final.Add(v2[1]); final.Add(v2[2]); final.Add(n.X); final.Add(n.Y); final.Add(n.Z); final.Add(tex2[0]); final.Add(tex2[1]);
                    final.Add(v3[0]); final.Add(v3[1]); final.Add(v3[2]); final.Add(n.X); final.Add(n.Y); final.Add(n.Z); final.Add(tex3[0]); final.Add(tex3[1]);
                }

            }


            return final.ToArray();
        }
        private class Object
        {
            private int _vertexBufferObject;
            private int _mainObject;
            private float[] vertices;
            private float rot_x, rot_y, rot_z;
            private Vector3 pos, color;
            private Shader shader;
            private Lamp lamp;
            private float Scale = 1.0f;
            public Object(string path, Shader lightingShader, Lamp _lamp, Vector3 col)
            {
                vertices = loadObj(path);

                _vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                _mainObject = GL.GenVertexArray();
                GL.BindVertexArray(_mainObject);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

                var positionLocation = lightingShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

                var normalLocation = lightingShader.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                rot_x = 0.0f; rot_y = 0.0f; rot_z = 0.0f;
                pos = new Vector3(0.0f, 0.0f, 0.0f);
                shader = lightingShader;
                lamp = _lamp;
                color = col;
            }
            public Object(float[] _vertices, Shader lightingShader, Lamp _lamp, Vector3 col)
            {
                vertices = _vertices;

                _vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                _mainObject = GL.GenVertexArray();
                GL.BindVertexArray(_mainObject);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

                var positionLocation = lightingShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

                var normalLocation = lightingShader.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                rot_x = 0.0f; rot_y = 0.0f; rot_z = 0.0f;
                pos = new Vector3(0.0f, 0.0f, 0.0f);
                shader = lightingShader;
                lamp = _lamp;
                color = col;
            }
            public void show(Camera camera)
            {
                GL.BindVertexArray(_mainObject);

                shader.Use();


                shader.SetMatrix4("model",  (Matrix4.CreateScale(Scale) *  Matrix4.CreateRotationX(rot_x) * Matrix4.CreateRotationX(rot_y) * Matrix4.CreateRotationZ(rot_z)) * Matrix4.CreateTranslation(pos));
                shader.SetMatrix4("view", camera.GetViewMatrix());
                shader.SetMatrix4("projection", camera.GetProjectionMatrix());

                shader.SetVector3("objectColor", color);
                shader.SetVector3("lightColor", lamp.lightColor);
                shader.SetVector3("lightPos", lamp.pos);

                GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 6);
            }
            public void setRotationX(float angle)
            {
                rot_x = angle;
            }
            public void setRotationY(float angle)
            {
                rot_y = angle;
            }
            public void setRotationZ(float angle)
            {
                rot_z = angle;
            }
            public void setPositionInSpace(float x, float y, float z)
            {
                pos = new Vector3(x, y, z);
            }
            public void setScale(float scale)
            {
                Scale = scale; 
            }
            public void Dispose()
            {
                GL.DeleteBuffer(_vertexBufferObject);
                GL.DeleteVertexArray(_mainObject);
            }
        }
        private class Lamp
        {
            private int _vertexBufferObject;
            private int _mainObject;
            public Vector3 pos, lightColor;
            float[] vertices;
            public Lamp(Vector3 _pos, Vector3 _lightColor, Shader lampShader)
            {
                pos = _pos;
                lightColor = _lightColor;
                vertices = loadObj("Objs/sphere.obj");

                _vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                _mainObject = GL.GenVertexArray();
                GL.BindVertexArray(_mainObject);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

                var positionLocation = lampShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

                lampShader.SetVector3("lightColor", _lightColor);
            }
            public void show(Camera camera, Shader lampShader)
            {
                GL.BindVertexArray(_mainObject);

                lampShader.Use();

                Matrix4 lampMatrix = Matrix4.Identity;
                lampMatrix *= Matrix4.CreateScale(0.2f);
                lampMatrix *= Matrix4.CreateTranslation(pos);

                lampShader.SetMatrix4("model", lampMatrix);
                lampShader.SetMatrix4("view", camera.GetViewMatrix());
                lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());

                GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 6);
            }
            public void Dispose()
            {
                GL.DeleteBuffer(_vertexBufferObject);
                GL.DeleteVertexArray(_mainObject);
            }
        }
        private class TexturedObject
        {
            private int _vertexBufferObject;
            private int _mainObject;
            private float[] vertices;
            private float rot_x, rot_y, rot_z;
            private Texture texture;
            private Vector3 pos;
            private Shader shader;
            public TexturedObject(string path, Shader textureShader, string texturePath)
            {
                vertices = loadObjTextured(path);

                _vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                _mainObject = GL.GenVertexArray();
                GL.BindVertexArray(_mainObject);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

                var positionLocation = textureShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                var normalLocation = textureShader.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

                var textureLocation = textureShader.GetAttribLocation("aTexture");
                GL.EnableVertexAttribArray(textureLocation);
                GL.VertexAttribPointer(textureLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

                texture = new Texture(texturePath);
                texture.Use();

                rot_x = 0.0f; rot_y = 0.0f; rot_z = 0.0f;
                pos = new Vector3(0.0f, 0.0f, 0.0f);
                shader = textureShader;
            }

            public void show(Camera camera)
            {
                GL.BindVertexArray(_mainObject);

                texture.Use();
                shader.Use();


                shader.SetMatrix4("model", (Matrix4.CreateRotationX(rot_x) * Matrix4.CreateRotationX(rot_y) * Matrix4.CreateRotationZ(rot_z)) * Matrix4.CreateTranslation(pos));
                shader.SetMatrix4("view", camera.GetViewMatrix());
                shader.SetMatrix4("projection", camera.GetProjectionMatrix());

                //textureShader.SetVector3("lightColor", lamp.lightColor);
                //textureShader.SetVector3("lightPos", lamp.pos);
                GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 8);
            }
            public void setRotationX(float angle)
            {
                rot_x = angle;
            }
            public void setRotationY(float angle)
            {
                rot_y = angle;
            }
            public void setRotationZ(float angle)
            {
                rot_z = angle;
            }
            public void setPositionInSpace(float x, float y, float z)
            {
                pos = new Vector3(x, y, z);
            }
            public void Dispose()
            {
                GL.DeleteBuffer(_vertexBufferObject);
                GL.DeleteVertexArray(_mainObject);
                GL.DeleteTexture(texture.Handle);
            }

        }
        public int createCube(Vector3 color)
        {
            mainObjects.Add(new Object("Objs/cube.obj", _lightingShader, mainLamp, color));
            return mainObjects.Count - 1;
        }
        public int createSphere(Vector3 color)
        {
            mainObjects.Add(new Object("Objs/sphere.obj", _lightingShader, mainLamp, color));
            return mainObjects.Count - 1;
        }
        public int createTorus(Vector3 color)
        {
            mainObjects.Add(new Object("Objs/torus.obj", _lightingShader, mainLamp, color));
            return mainObjects.Count - 1;
        }
        public int createCilinder(Vector3 color)
        {
            mainObjects.Add(new Object("Objs/cilinder.obj", _lightingShader, mainLamp, color));
            return mainObjects.Count - 1;
        }
        public int createPlane(float x1, float y1, float z1,
                               float x2, float y2, float z2,
                               float x3, float y3, float z3,
                               float x4, float y4, float z4, Vector3 color)
        {
            Vector3 l1 = new Vector3(x2 - x1, y2 - y1, z2 - z1);
            Vector3 l2 = new Vector3(x3 - x1, y3 - y1, z3 - z1);
            Vector3 normal = Vector3.Cross(l1, l2);

            float[] vertices =
            {
                x1, y1, z1, normal.X,  normal.Y, normal.Z,
                x2, y2, z2, normal.X,  normal.Y, normal.Z,
                x3, y3, z3, normal.X,  normal.Y, normal.Z,

                x4, y4, z4, normal.X,  normal.Y, normal.Z,
                x2, y2, z2, normal.X,  normal.Y, normal.Z,
                x3, y3, z3, normal.X,  normal.Y, normal.Z,
            };
            mainObjects.Add(new Object(vertices, _lightingShader, mainLamp, color));
            return mainObjects.Count - 1;
        }
        public void openTexturedObj(string Obj, string Texture)
        {
            mainTexturedObjects.Add(new TexturedObject(Obj, _textureShader, Texture));
        }
        public void createMainLight(Vector3 pos, Vector3 color)
        {
            mainLamp = new Lamp(pos, color, _lampShader);
        }
        public void rotateObject(float x, float y, float z, int handle)
        {
            mainObjects[handle].setRotationX(x);
            mainObjects[handle].setRotationY(y);
            mainObjects[handle].setRotationZ(z);
        }
        public void scaleObject(float scale, int handle)
        {
            mainObjects[handle].setScale(scale);
        }
        public void translateObject(float x, float y, float z, int handle)
        {
            mainObjects[handle].setPositionInSpace(x, y, z);
        }
    }

   

    

}
