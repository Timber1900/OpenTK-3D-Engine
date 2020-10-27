// Decompiled with JetBrains decompiler
// Type: Program.MainRenderWindow
// Assembly: Program, Version=1.7.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF30FF81-E0B0-4B54-A928-DD8AA8C2D21D
// Assembly location: C:\Users\Hugo Teixeira\.nuget\packages\opentk.3d.library\1.7.0\lib\netcoreapp3.1\Program.dll

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Program
{
  public class MainRenderWindow : GameWindow
  {
    private readonly List<MainRenderWindow.TexturedObject> _mainTexturedObjects = new List<MainRenderWindow.TexturedObject>();
    private readonly List<MainRenderWindow.Object> _mainObjects = new List<MainRenderWindow.Object>();
    private MainRenderWindow.Lamp _mainLamp;
    private Shader _lampShader;
    private Shader _lightingShader;
    private Shader _textureShader;
    private Shader _2dShader;
    private Shader _2dTextured;
    private Camera _camera;
    private bool _firstMove = true;
    private Vector2 _lastPos;
    protected bool RenderLight;
    private float cameraSpeed = 20f;
    private float sensitivity = 0.2f;
    protected bool UseDepthTest = true;
    protected bool UseAlpha = true;
    protected bool KeyboardAndMouseInput = true;
    protected bool loadedFont;
    protected bool showSet;
    protected bool lastTime = true;
    protected bool useSettings;
    public MainRenderWindow.Settings set = new MainRenderWindow.Settings();

    protected MainRenderWindow(int width, int height, string title)
      : base(width, height, GraphicsMode.Default, title)
    {
    }

    protected override void OnLoad(EventArgs e)
    {
      Console.WriteLine("Test1");

      if (this.UseDepthTest)
        GL.Enable(EnableCap.DepthTest);
      if (this.UseAlpha)
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
      GL.Enable(EnableCap.Blend);
      Console.WriteLine("Test1");
      //this._lightingShader = new Shader(Shaders.ShaderVert, Shaders.LightingFrag);
      Console.WriteLine("Test2");
      //this._lampShader = new Shader(Shaders.ShaderVert, Shaders.ShaderFrag);
      Console.WriteLine("Test3");
      //this._2dShader = new Shader(Shaders.Shader2DVert, Shaders.Shader2DFrag);
      Console.WriteLine("Test4");
      //this._textureShader = new Shader(Shaders.TextureVert, Shaders.TextureFrag);
      Console.WriteLine("Test5");
      //this._2dTextured = new Shader(Shaders.Texture2DVert, Shaders.Texture2DFrag);
      Console.WriteLine("Test1");
      //this._lightingShader.Use();
      //this._lampShader.Use();
      //this._textureShader.Use();
      //this._2dShader.Use();
      //this._2dTextured.Use();
      this._camera = new Camera(Vector3.UnitZ * 3f, (float) this.Width / (float) this.Height);
      this.CursorVisible = !this.KeyboardAndMouseInput;
    }

    protected void setClearColor(Color4 color) => GL.ClearColor(color);

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      if (this.RenderLight)
        this._mainLamp.show(this._camera, this._lampShader);
      foreach (MainRenderWindow.Object mainObject in this._mainObjects)
        mainObject.show(this._camera);
      foreach (MainRenderWindow.TexturedObject mainTexturedObject in this._mainTexturedObjects)
        mainTexturedObject.show(this._camera);
      if (this.showSet)
      {
        if (Mouse.GetState().IsButtonDown(MouseButton.Left))
          this.checkClicks(this.set);
        this.showSettings(this.set);
      }
      this.SwapBuffers();
      base.OnRenderFrame(e);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      if (!this.Focused)
        return;
      KeyboardState state1 = Keyboard.GetState();
      MouseState state2 = Mouse.GetState();
      if (state1.IsKeyDown(Key.Escape) && this.lastTime && this.useSettings)
      {
        this.showSet = !this.showSet;
        this.lastTime = false;
      }
      if (state1.IsKeyUp(Key.Escape))
        this.lastTime = true;
      if (this.KeyboardAndMouseInput)
      {
        if (state1.IsKeyDown(Key.W))
          this._camera.Position += this._camera.Front * this.cameraSpeed * (float) e.Time;
        if (state1.IsKeyDown(Key.S))
          this._camera.Position -= this._camera.Front * this.cameraSpeed * (float) e.Time;
        if (state1.IsKeyDown(Key.A))
          this._camera.Position -= this._camera.Right * this.cameraSpeed * (float) e.Time;
        if (state1.IsKeyDown(Key.D))
          this._camera.Position += this._camera.Right * this.cameraSpeed * (float) e.Time;
        if (state1.IsKeyDown(Key.Space))
          this._camera.Position += this._camera.Up * this.cameraSpeed * (float) e.Time;
        if (state1.IsKeyDown(Key.ShiftLeft))
          this._camera.Position -= this._camera.Up * this.cameraSpeed * (float) e.Time;
        if (this._firstMove)
        {
          this._lastPos = new Vector2((float) state2.X, (float) state2.Y);
          this._firstMove = false;
        }
        else
        {
          float num1 = (float) state2.X - this._lastPos.X;
          float num2 = (float) state2.Y - this._lastPos.Y;
          this._lastPos = new Vector2((float) state2.X, (float) state2.Y);
          this._camera.Yaw += num1 * this.sensitivity;
          this._camera.Pitch -= num2 * this.sensitivity;
        }
        Mouse.SetPosition(960.0, 540.0);
      }
      base.OnUpdateFrame(e);
    }

    protected override void OnResize(EventArgs e)
    {
      GL.Viewport(0, 0, this.Width, this.Height);
      base.OnResize(e);
    }

    protected override void OnUnload(EventArgs e)
    {
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);
      GL.DeleteProgram(this._lampShader.Handle);
      GL.DeleteProgram(this._lightingShader.Handle);
      GL.DeleteProgram(this._2dShader.Handle);
      GL.DeleteProgram(this._textureShader.Handle);
      foreach (MainRenderWindow.Object mainObject in this._mainObjects)
        mainObject.Dispose();
      foreach (MainRenderWindow.TexturedObject mainTexturedObject in this._mainTexturedObjects)
        mainTexturedObject.Dispose();
      this._mainLamp?.Dispose();
      base.OnUnload(e);
    }

    private static float[] loadObj(string path)
    {
      string[] strArray1 = File.ReadAllLines(path);
      List<float[]> numArrayList = new List<float[]>();
      List<float> floatList = new List<float>();
      foreach (string str in strArray1)
      {
        string[] strArray2 = str.Split(" ");
        if (strArray2[0] == "v")
        {
          float[] numArray = new float[3]
          {
            float.Parse(strArray2[1]),
            float.Parse(strArray2[2]),
            float.Parse(strArray2[3])
          };
          numArrayList.Add(numArray);
        }
        if (strArray2[0] == "f")
        {
          string[] strArray3 = strArray2[1].Split("//");
          string[] strArray4 = strArray2[2].Split("//");
          string[] strArray5 = strArray2[3].Split("//");
          float[] numArray1 = numArrayList[int.Parse(strArray3[0]) - 1];
          float[] numArray2 = numArrayList[int.Parse(strArray4[0]) - 1];
          float[] numArray3 = numArrayList[int.Parse(strArray5[0]) - 1];
          Vector3 vector3_1 = new Vector3(numArray1[0], numArray1[1], numArray1[2]);
          Vector3 vector3_2 = new Vector3(numArray2[0], numArray2[1], numArray2[2]);
          Vector3 vector3_3 = new Vector3(numArray3[0], numArray3[1], numArray3[2]);
          Vector3 right = vector3_2 - vector3_1;
          Vector3 vector3_4 = vector3_1;
          Vector3 vector3_5 = Vector3.Cross(vector3_3 - vector3_4, right);
          floatList.Add(numArray1[0]);
          floatList.Add(numArray1[1]);
          floatList.Add(numArray1[2]);
          floatList.Add(vector3_5.X);
          floatList.Add(vector3_5.Y);
          floatList.Add(vector3_5.Z);
          floatList.Add(numArray2[0]);
          floatList.Add(numArray2[1]);
          floatList.Add(numArray2[2]);
          floatList.Add(vector3_5.X);
          floatList.Add(vector3_5.Y);
          floatList.Add(vector3_5.Z);
          floatList.Add(numArray3[0]);
          floatList.Add(numArray3[1]);
          floatList.Add(numArray3[2]);
          floatList.Add(vector3_5.X);
          floatList.Add(vector3_5.Y);
          floatList.Add(vector3_5.Z);
        }
      }
      return floatList.ToArray();
    }

    private static float[] loadObjTextured(string path)
    {
      string[] strArray1 = File.ReadAllLines(path);
      List<float[]> numArrayList1 = new List<float[]>();
      List<float[]> numArrayList2 = new List<float[]>();
      List<float> floatList = new List<float>();
      foreach (string str in strArray1)
      {
        string[] strArray2 = str.Split(" ");
        if (strArray2[0] == "v")
        {
          float[] numArray = new float[3]
          {
            float.Parse(strArray2[1]),
            float.Parse(strArray2[2]),
            float.Parse(strArray2[3])
          };
          numArrayList1.Add(numArray);
        }
        if (strArray2[0] == "vt")
        {
          float[] numArray = new float[2]
          {
            float.Parse(strArray2[1]),
            (float) -((double) float.Parse(strArray2[2]) - 1.0)
          };
          numArrayList2.Add(numArray);
        }
        if (strArray2[0] == "f")
        {
          string[] strArray3 = strArray2[1].Split("/");
          string[] strArray4 = strArray2[2].Split("/");
          string[] strArray5 = strArray2[3].Split("/");
          float[] numArray1 = numArrayList1[int.Parse(strArray3[0]) - 1];
          if (int.Parse(strArray4[0]) - 1 >= 0 && numArrayList1.Count > int.Parse(strArray4[0]) - 1)
          {
            float[] numArray2 = numArrayList1[int.Parse(strArray4[0]) - 1];
            float[] numArray3 = numArrayList1[int.Parse(strArray5[0]) - 1];
            float[] numArray4 = numArrayList2[int.Parse(strArray3[1]) - 1];
            float[] numArray5 = numArrayList2[int.Parse(strArray4[1]) - 1];
            float[] numArray6 = numArrayList2[int.Parse(strArray5[1]) - 1];
            Vector3 vector3_1 = new Vector3(numArray1[0], numArray1[1], numArray1[2]);
            Vector3 vector3_2 = new Vector3(numArray2[0], numArray2[1], numArray2[2]);
            Vector3 vector3_3 = new Vector3(numArray3[0], numArray3[1], numArray3[2]);
            Vector3 right = vector3_2 - vector3_1;
            Vector3 vector3_4 = vector3_1;
            Vector3 vector3_5 = Vector3.Cross(vector3_3 - vector3_4, right);
            floatList.Add(numArray1[0]);
            floatList.Add(numArray1[1]);
            floatList.Add(numArray1[2]);
            floatList.Add(vector3_5.X);
            floatList.Add(vector3_5.Y);
            floatList.Add(vector3_5.Z);
            floatList.Add(numArray4[0]);
            floatList.Add(numArray4[1]);
            floatList.Add(numArray2[0]);
            floatList.Add(numArray2[1]);
            floatList.Add(numArray2[2]);
            floatList.Add(vector3_5.X);
            floatList.Add(vector3_5.Y);
            floatList.Add(vector3_5.Z);
            floatList.Add(numArray5[0]);
            floatList.Add(numArray5[1]);
            floatList.Add(numArray3[0]);
            floatList.Add(numArray3[1]);
            floatList.Add(numArray3[2]);
            floatList.Add(vector3_5.X);
            floatList.Add(vector3_5.Y);
            floatList.Add(vector3_5.Z);
            floatList.Add(numArray6[0]);
            floatList.Add(numArray6[1]);
          }
        }
      }
      return floatList.ToArray();
    }

    public int createCube(Vector3 color)
    {
      this._mainObjects.Add(new MainRenderWindow.Object("Objs/cube.obj", this._lightingShader, this._mainLamp, color));
      return this._mainObjects.Count - 1;
    }

    public int createSphere(Vector3 color)
    {
      this._mainObjects.Add(new MainRenderWindow.Object("Objs/sphere.obj", this._lightingShader, this._mainLamp, color));
      return this._mainObjects.Count - 1;
    }

    public int createTorus(Vector3 color)
    {
      this._mainObjects.Add(new MainRenderWindow.Object("Objs/torus.obj", this._lightingShader, this._mainLamp, color));
      return this._mainObjects.Count - 1;
    }

    public int createCylinder(Vector3 color)
    {
      this._mainObjects.Add(new MainRenderWindow.Object("Objs/cilinder.obj", this._lightingShader, this._mainLamp, color));
      return this._mainObjects.Count - 1;
    }

    public int createPlane(
      float x1,
      float y1,
      float z1,
      float x2,
      float y2,
      float z2,
      float x3,
      float y3,
      float z3,
      float x4,
      float y4,
      float z4,
      Vector3 color)
    {
      Vector3 vector3 = Vector3.Cross(new Vector3(x2 - x1, y2 - y1, z2 - z1), new Vector3(x3 - x1, y3 - y1, z3 - z1));
      this._mainObjects.Add(new MainRenderWindow.Object(new float[36]
      {
        x1,
        y1,
        z1,
        vector3.X,
        vector3.Y,
        vector3.Z,
        x2,
        y2,
        z2,
        vector3.X,
        vector3.Y,
        vector3.Z,
        x3,
        y3,
        z3,
        vector3.X,
        vector3.Y,
        vector3.Z,
        x4,
        y4,
        z4,
        vector3.X,
        vector3.Y,
        vector3.Z,
        x2,
        y2,
        z2,
        vector3.X,
        vector3.Y,
        vector3.Z,
        x3,
        y3,
        z3,
        vector3.X,
        vector3.Y,
        vector3.Z
      }, this._lightingShader, this._mainLamp, color));
      return this._mainObjects.Count - 1;
    }

    public void openTexturedObj(string obj, string texture) => this._mainTexturedObjects.Add(new MainRenderWindow.TexturedObject(obj, this._textureShader, texture));

    public void openObj(string obj, Vector3 color) => this._mainObjects.Add(new MainRenderWindow.Object(obj, this._lightingShader, this._mainLamp, color));

    public void createMainLight(Vector3 pos, Vector3 color) => this._mainLamp = new MainRenderWindow.Lamp(pos, color, this._lampShader);

    public void rotateObject(float x, float y, float z, int handle)
    {
      this._mainObjects[handle].setRotationX(x);
      this._mainObjects[handle].setRotationY(y);
      this._mainObjects[handle].setRotationZ(z);
    }

    public void rotateTexturedObject(float x, float y, float z, int handle)
    {
      this._mainTexturedObjects[handle].setRotationX(x);
      this._mainTexturedObjects[handle].setRotationY(y);
      this._mainTexturedObjects[handle].setRotationZ(z);
    }

    public void scaleObject(float scale, int handle) => this._mainObjects[handle].setScale(scale);

    public void translateObject(float x, float y, float z, int handle) => this._mainObjects[handle].setPositionInSpace(x, y, z);

    public void translateTexturedObject(float x, float y, float z, int handle) => this._mainTexturedObjects[handle].setPositionInSpace(x, y, z);

    protected void drawTexturedRectangle(
      float x1,
      float y1,
      float u1,
      float v1,
      float x2,
      float y2,
      float u2,
      float v2,
      string texturePath,
      Color4 color,
      TextureMinFilter min,
      TextureMagFilter mag)
    {
      Texture texture = new Texture(texturePath, min, mag);
      double num1 = (double) x1 - (double) (this.Width / 2);
      float num2 = y1 - (float) (this.Height / 2);
      double num3 = (double) (this.Width / 2);
      float num4 = (float) (num1 / num3);
      float num5 = num2 / (float) (this.Height / 2);
      double num6 = (double) x2 - (double) (this.Width / 2);
      float num7 = y2 - (float) (this.Height / 2);
      double num8 = (double) (this.Width / 2);
      float num9 = (float) (num6 / num8);
      float num10 = num7 / (float) (this.Height / 2);
      float[] data = new float[30]
      {
        num4,
        num5,
        0.0f,
        u1,
        v2,
        num4,
        num10,
        0.0f,
        u1,
        v1,
        num9,
        num10,
        0.0f,
        u2,
        v1,
        num4,
        num5,
        0.0f,
        u1,
        v2,
        num9,
        num5,
        0.0f,
        u2,
        v2,
        num9,
        num10,
        0.0f,
        u2,
        v1
      };
      int num11 = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      GL.BufferData<float>(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.DynamicDraw);
      texture.Use();
      this._2dTextured.Use();
      int num12 = GL.GenVertexArray();
      GL.BindVertexArray(num12);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      int attribLocation1 = this._2dTextured.GetAttribLocation("aPosition");
      GL.EnableVertexAttribArray(attribLocation1);
      GL.VertexAttribPointer(attribLocation1, 3, VertexAttribPointerType.Float, false, 20, 0);
      int attribLocation2 = this._2dTextured.GetAttribLocation("aTexCoord");
      GL.EnableVertexAttribArray(attribLocation2);
      GL.VertexAttribPointer(attribLocation2, 2, VertexAttribPointerType.Float, false, 20, 12);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      GL.BindVertexArray(num12);
      texture.Use();
      this._2dTextured.Use();
      this._2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
      GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
      GL.DeleteBuffer(num11);
      GL.DeleteTexture(texture.Handle);
      GL.DeleteVertexArray(num12);
    }

    protected void drawTexturedRectangle(
      float x1,
      float y1,
      float u1,
      float v1,
      float x2,
      float y2,
      float u2,
      float v2,
      Bitmap textureBitmap,
      Color4 color,
      TextureMinFilter min,
      TextureMagFilter mag)
    {
      Texture texture = new Texture(textureBitmap, min, mag);
      double num1 = (double) x1 - (double) (this.Width / 2);
      float num2 = y1 - (float) (this.Height / 2);
      double num3 = (double) (this.Width / 2);
      float num4 = (float) (num1 / num3);
      float num5 = num2 / (float) (this.Height / 2);
      double num6 = (double) x2 - (double) (this.Width / 2);
      float num7 = y2 - (float) (this.Height / 2);
      double num8 = (double) (this.Width / 2);
      float num9 = (float) (num6 / num8);
      float num10 = num7 / (float) (this.Height / 2);
      float[] data = new float[30]
      {
        num4,
        num5,
        0.0f,
        u1,
        v2,
        num4,
        num10,
        0.0f,
        u1,
        v1,
        num9,
        num10,
        0.0f,
        u2,
        v1,
        num4,
        num5,
        0.0f,
        u1,
        v2,
        num9,
        num5,
        0.0f,
        u2,
        v2,
        num9,
        num10,
        0.0f,
        u2,
        v1
      };
      int num11 = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      GL.BufferData<float>(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.DynamicDraw);
      texture.Use();
      this._2dTextured.Use();
      int num12 = GL.GenVertexArray();
      GL.BindVertexArray(num12);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      int attribLocation1 = this._2dTextured.GetAttribLocation("aPosition");
      GL.EnableVertexAttribArray(attribLocation1);
      GL.VertexAttribPointer(attribLocation1, 3, VertexAttribPointerType.Float, false, 20, 0);
      int attribLocation2 = this._2dTextured.GetAttribLocation("aTexCoord");
      GL.EnableVertexAttribArray(attribLocation2);
      GL.VertexAttribPointer(attribLocation2, 2, VertexAttribPointerType.Float, false, 20, 12);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      GL.BindVertexArray(num12);
      texture.Use();
      this._2dTextured.Use();
      this._2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
      GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
      GL.DeleteBuffer(num11);
      GL.DeleteTexture(texture.Handle);
      GL.DeleteVertexArray(num12);
    }

    protected void drawTexturedRectangle(
      float x1,
      float y1,
      float u1,
      float v1,
      float x2,
      float y2,
      float u2,
      float v2,
      Texture texture,
      Color4 color)
    {
      double num1 = (double) x1 - (double) (this.Width / 2);
      float num2 = y1 - (float) (this.Height / 2);
      double num3 = (double) (this.Width / 2);
      float num4 = (float) (num1 / num3);
      float num5 = num2 / (float) (this.Height / 2);
      double num6 = (double) x2 - (double) (this.Width / 2);
      float num7 = y2 - (float) (this.Height / 2);
      double num8 = (double) (this.Width / 2);
      float num9 = (float) (num6 / num8);
      float num10 = num7 / (float) (this.Height / 2);
      float[] data = new float[30]
      {
        num4,
        num5,
        0.0f,
        u1,
        v2,
        num4,
        num10,
        0.0f,
        u1,
        v1,
        num9,
        num10,
        0.0f,
        u2,
        v1,
        num4,
        num5,
        0.0f,
        u1,
        v2,
        num9,
        num5,
        0.0f,
        u2,
        v2,
        num9,
        num10,
        0.0f,
        u2,
        v1
      };
      int num11 = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      GL.BufferData<float>(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.DynamicDraw);
      texture.Use();
      this._2dTextured.Use();
      int num12 = GL.GenVertexArray();
      GL.BindVertexArray(num12);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      int attribLocation1 = this._2dTextured.GetAttribLocation("aPosition");
      GL.EnableVertexAttribArray(attribLocation1);
      GL.VertexAttribPointer(attribLocation1, 3, VertexAttribPointerType.Float, false, 20, 0);
      int attribLocation2 = this._2dTextured.GetAttribLocation("aTexCoord");
      GL.EnableVertexAttribArray(attribLocation2);
      GL.VertexAttribPointer(attribLocation2, 2, VertexAttribPointerType.Float, false, 20, 12);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      GL.BindVertexArray(num12);
      texture.Use();
      this._2dTextured.Use();
      this._2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
      GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
      GL.DeleteBuffer(num11);
      GL.DeleteVertexArray(num12);
    }

    protected void drawLine(float x1, float y1, float x2, float y2, Color4 color)
    {
      double num1 = (double) x1 - (double) (this.Width / 2);
      float num2 = y1 - (float) (this.Height / 2);
      double num3 = (double) (this.Width / 2);
      float num4 = (float) (num1 / num3);
      float num5 = num2 / (float) (this.Height / 2);
      double num6 = (double) x2 - (double) (this.Width / 2);
      float num7 = y2 - (float) (this.Height / 2);
      double num8 = (double) (this.Width / 2);
      float num9 = (float) (num6 / num8);
      float num10 = num7 / (float) (this.Height / 2);
      float[] data = new float[6]
      {
        num4,
        num5,
        0.0f,
        num9,
        num10,
        0.0f
      };
      int num11 = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      GL.BufferData<float>(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.DynamicDraw);
      this._2dShader.Use();
      int num12 = GL.GenVertexArray();
      GL.BindVertexArray(num12);
      int attribLocation = this._2dShader.GetAttribLocation("aPos");
      GL.EnableVertexAttribArray(attribLocation);
      GL.VertexAttribPointer(attribLocation, 3, VertexAttribPointerType.Float, false, 12, 0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      GL.BindVertexArray(num12);
      this._2dShader.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
      this._2dShader.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      GL.DeleteBuffer(num11);
      GL.DeleteVertexArray(num12);
    }

    protected void drawRectangle(float x1, float y1, float x2, float y2, Color4 color)
    {
      double num1 = (double) x1 - (double) (this.Width / 2);
      float num2 = y1 - (float) (this.Height / 2);
      double num3 = (double) (this.Width / 2);
      float num4 = (float) (num1 / num3);
      float num5 = num2 / (float) (this.Height / 2);
      double num6 = (double) x2 - (double) (this.Width / 2);
      float num7 = y2 - (float) (this.Height / 2);
      double num8 = (double) (this.Width / 2);
      float num9 = (float) (num6 / num8);
      float num10 = num7 / (float) (this.Height / 2);
      float[] data = new float[18]
      {
        num4,
        num5,
        0.0f,
        num9,
        num5,
        0.0f,
        num4,
        num10,
        0.0f,
        num9,
        num5,
        0.0f,
        num9,
        num10,
        0.0f,
        num4,
        num10,
        0.0f
      };
      int num11 = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      GL.BufferData<float>(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.DynamicDraw);
      this._2dShader.Use();
      int num12 = GL.GenVertexArray();
      GL.BindVertexArray(num12);
      int attribLocation = this._2dShader.GetAttribLocation("aPos");
      GL.EnableVertexAttribArray(attribLocation);
      GL.VertexAttribPointer(attribLocation, 3, VertexAttribPointerType.Float, false, 12, 0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      GL.BindVertexArray(num12);
      this._2dShader.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
      this._2dShader.Use();
      GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
      GL.DeleteBuffer(num11);
      GL.DeleteVertexArray(num12);
    }

    protected void drawTexturedLine(
      float x1,
      float y1,
      float u1,
      float v1,
      float x2,
      float y2,
      float u2,
      float v2,
      Texture texture,
      Color4 color)
    {
      double num1 = (double) x1 - (double) (this.Width / 2);
      float num2 = y1 - (float) (this.Height / 2);
      double num3 = (double) (this.Width / 2);
      float num4 = (float) (num1 / num3);
      float num5 = num2 / (float) (this.Height / 2);
      double num6 = (double) x2 - (double) (this.Width / 2);
      float num7 = y2 - (float) (this.Height / 2);
      double num8 = (double) (this.Width / 2);
      float num9 = (float) (num6 / num8);
      float num10 = num7 / (float) (this.Height / 2);
      float[] data = new float[10]
      {
        num4,
        num5,
        0.0f,
        u1,
        v1,
        num9,
        num10,
        0.0f,
        u2,
        v2
      };
      int num11 = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      GL.BufferData<float>(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.DynamicDraw);
      texture.Use();
      this._2dTextured.Use();
      int num12 = GL.GenVertexArray();
      GL.BindVertexArray(num12);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      int attribLocation1 = this._2dTextured.GetAttribLocation("aPosition");
      GL.EnableVertexAttribArray(attribLocation1);
      GL.VertexAttribPointer(attribLocation1, 3, VertexAttribPointerType.Float, false, 20, 0);
      int attribLocation2 = this._2dTextured.GetAttribLocation("aTexCoord");
      GL.EnableVertexAttribArray(attribLocation2);
      GL.VertexAttribPointer(attribLocation2, 2, VertexAttribPointerType.Float, false, 20, 12);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num11);
      GL.BindVertexArray(num12);
      texture.Use();
      this._2dTextured.Use();
      this._2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      GL.DeleteBuffer(num11);
      GL.DeleteVertexArray(num12);
    }

    protected void drawTexturedQuad(
      float x1,
      float y1,
      float z1,
      float u1,
      float v1,
      float x2,
      float y2,
      float z2,
      float u2,
      float v2,
      float x3,
      float y3,
      float z3,
      float u3,
      float v3,
      float x4,
      float y4,
      float z4,
      float u4,
      float v4,
      string texturePath,
      Color4 color,
      TextureMinFilter min,
      TextureMagFilter mag)
    {
      Texture texture = new Texture(texturePath, min, mag);
      double num1 = (double) x1 - (double) (this.Width / 2);
      float num2 = y1 - (float) (this.Height / 2);
      double num3 = (double) (this.Width / 2);
      float num4 = (float) (num1 / num3);
      float num5 = num2 / (float) (this.Height / 2);
      double num6 = (double) x2 - (double) (this.Width / 2);
      float num7 = y2 - (float) (this.Height / 2);
      double num8 = (double) (this.Width / 2);
      float num9 = (float) (num6 / num8);
      float num10 = num7 / (float) (this.Height / 2);
      double num11 = (double) x3 - (double) (this.Width / 2);
      float num12 = y3 - (float) (this.Height / 2);
      double num13 = (double) (this.Width / 2);
      float num14 = (float) (num11 / num13);
      float num15 = num12 / (float) (this.Height / 2);
      double num16 = (double) x4 - (double) (this.Width / 2);
      float num17 = y4 - (float) (this.Height / 2);
      double num18 = (double) (this.Width / 2);
      float num19 = (float) (num16 / num18);
      float num20 = num17 / (float) (this.Height / 2);
      float[] data = new float[30]
      {
        num4,
        num5,
        z1,
        u1,
        (float) -((double) v1 - 1.0),
        num9,
        num10,
        z2,
        u2,
        (float) -((double) v2 - 1.0),
        num14,
        num15,
        z3,
        u3,
        (float) -((double) v3 - 1.0),
        num4,
        num5,
        z2,
        u1,
        (float) -((double) v1 - 1.0),
        num14,
        num15,
        z3,
        u3,
        (float) -((double) v3 - 1.0),
        num19,
        num20,
        z4,
        u4,
        (float) -((double) v4 - 1.0)
      };
      int num21 = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, num21);
      GL.BufferData<float>(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.DynamicDraw);
      texture.Use();
      this._2dTextured.Use();
      int num22 = GL.GenVertexArray();
      GL.BindVertexArray(num22);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num21);
      int attribLocation1 = this._2dTextured.GetAttribLocation("aPosition");
      GL.EnableVertexAttribArray(attribLocation1);
      GL.VertexAttribPointer(attribLocation1, 3, VertexAttribPointerType.Float, false, 20, 0);
      int attribLocation2 = this._2dTextured.GetAttribLocation("aTexCoord");
      GL.EnableVertexAttribArray(attribLocation2);
      GL.VertexAttribPointer(attribLocation2, 2, VertexAttribPointerType.Float, false, 20, 12);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num21);
      GL.BindVertexArray(num22);
      texture.Use();
      this._2dTextured.Use();
      this._2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
      GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
      GL.DeleteBuffer(num21);
      GL.DeleteTexture(texture.Handle);
      GL.DeleteVertexArray(num22);
    }

    protected void drawTexturedQuad(
      float x1,
      float y1,
      float z1,
      float u1,
      float v1,
      float x2,
      float y2,
      float z2,
      float u2,
      float v2,
      float x3,
      float y3,
      float z3,
      float u3,
      float v3,
      float x4,
      float y4,
      float z4,
      float u4,
      float v4,
      Bitmap textureBitmap,
      Color4 color,
      TextureMinFilter min,
      TextureMagFilter mag)
    {
      Texture texture = new Texture(textureBitmap, min, mag);
      double num1 = (double) x1 - (double) (this.Width / 2);
      float num2 = y1 - (float) (this.Height / 2);
      double num3 = (double) (this.Width / 2);
      float num4 = (float) (num1 / num3);
      float num5 = num2 / (float) (this.Height / 2);
      double num6 = (double) x2 - (double) (this.Width / 2);
      float num7 = y2 - (float) (this.Height / 2);
      double num8 = (double) (this.Width / 2);
      float num9 = (float) (num6 / num8);
      float num10 = num7 / (float) (this.Height / 2);
      double num11 = (double) x3 - (double) (this.Width / 2);
      float num12 = y3 - (float) (this.Height / 2);
      double num13 = (double) (this.Width / 2);
      float num14 = (float) (num11 / num13);
      float num15 = num12 / (float) (this.Height / 2);
      double num16 = (double) x4 - (double) (this.Width / 2);
      float num17 = y4 - (float) (this.Height / 2);
      double num18 = (double) (this.Width / 2);
      float num19 = (float) (num16 / num18);
      float num20 = num17 / (float) (this.Height / 2);
      float[] data = new float[30]
      {
        num4,
        num5,
        z1,
        u1,
        (float) -((double) v1 - 1.0),
        num9,
        num10,
        z2,
        u2,
        (float) -((double) v2 - 1.0),
        num14,
        num15,
        z3,
        u3,
        (float) -((double) v3 - 1.0),
        num4,
        num5,
        z2,
        u1,
        (float) -((double) v1 - 1.0),
        num14,
        num15,
        z3,
        u3,
        (float) -((double) v3 - 1.0),
        num19,
        num20,
        z4,
        u4,
        (float) -((double) v4 - 1.0)
      };
      int num21 = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, num21);
      GL.BufferData<float>(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.DynamicDraw);
      texture.Use();
      this._2dTextured.Use();
      int num22 = GL.GenVertexArray();
      GL.BindVertexArray(num22);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num21);
      int attribLocation1 = this._2dTextured.GetAttribLocation("aPosition");
      GL.EnableVertexAttribArray(attribLocation1);
      GL.VertexAttribPointer(attribLocation1, 3, VertexAttribPointerType.Float, false, 20, 0);
      int attribLocation2 = this._2dTextured.GetAttribLocation("aTexCoord");
      GL.EnableVertexAttribArray(attribLocation2);
      GL.VertexAttribPointer(attribLocation2, 2, VertexAttribPointerType.Float, false, 20, 12);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num21);
      GL.BindVertexArray(num22);
      texture.Use();
      this._2dTextured.Use();
      this._2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
      GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
      GL.DeleteBuffer(num21);
      GL.DeleteTexture(texture.Handle);
      GL.DeleteVertexArray(num22);
    }

    protected void drawTexturedQuad(
      float x1,
      float y1,
      float z1,
      float u1,
      float v1,
      float x2,
      float y2,
      float z2,
      float u2,
      float v2,
      float x3,
      float y3,
      float z3,
      float u3,
      float v3,
      float x4,
      float y4,
      float z4,
      float u4,
      float v4,
      Texture texture,
      Color4 color,
      TextureMinFilter min,
      TextureMagFilter mag)
    {
      double num1 = (double) x1 - (double) (this.Width / 2);
      float num2 = y1 - (float) (this.Height / 2);
      double num3 = (double) (this.Width / 2);
      float num4 = (float) (num1 / num3);
      float num5 = num2 / (float) (this.Height / 2);
      double num6 = (double) x2 - (double) (this.Width / 2);
      float num7 = y2 - (float) (this.Height / 2);
      double num8 = (double) (this.Width / 2);
      float num9 = (float) (num6 / num8);
      float num10 = num7 / (float) (this.Height / 2);
      double num11 = (double) x3 - (double) (this.Width / 2);
      float num12 = y3 - (float) (this.Height / 2);
      double num13 = (double) (this.Width / 2);
      float num14 = (float) (num11 / num13);
      float num15 = num12 / (float) (this.Height / 2);
      double num16 = (double) x4 - (double) (this.Width / 2);
      float num17 = y4 - (float) (this.Height / 2);
      double num18 = (double) (this.Width / 2);
      float num19 = (float) (num16 / num18);
      float num20 = num17 / (float) (this.Height / 2);
      float[] data = new float[30]
      {
        num4,
        num5,
        z1,
        u1,
        (float) -((double) v1 - 1.0),
        num9,
        num10,
        z2,
        u2,
        (float) -((double) v2 - 1.0),
        num14,
        num15,
        z3,
        u3,
        (float) -((double) v3 - 1.0),
        num4,
        num5,
        z2,
        u1,
        (float) -((double) v1 - 1.0),
        num14,
        num15,
        z3,
        u3,
        (float) -((double) v3 - 1.0),
        num19,
        num20,
        z4,
        u4,
        (float) -((double) v4 - 1.0)
      };
      int num21 = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, num21);
      GL.BufferData<float>(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.DynamicDraw);
      texture.Use();
      this._2dTextured.Use();
      int num22 = GL.GenVertexArray();
      GL.BindVertexArray(num22);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num21);
      int attribLocation1 = this._2dTextured.GetAttribLocation("aPosition");
      GL.EnableVertexAttribArray(attribLocation1);
      GL.VertexAttribPointer(attribLocation1, 3, VertexAttribPointerType.Float, false, 20, 0);
      int attribLocation2 = this._2dTextured.GetAttribLocation("aTexCoord");
      GL.EnableVertexAttribArray(attribLocation2);
      GL.VertexAttribPointer(attribLocation2, 2, VertexAttribPointerType.Float, false, 20, 12);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num21);
      GL.BindVertexArray(num22);
      texture.Use();
      this._2dTextured.Use();
      this._2dTextured.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
      GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
      GL.DeleteBuffer(num21);
      GL.DeleteVertexArray(num22);
    }

    protected void drawQuad(
      float x1,
      float y1,
      float z1,
      float x2,
      float y2,
      float z2,
      float x3,
      float y3,
      float z3,
      float x4,
      float y4,
      float z4,
      Color4 color)
    {
      double num1 = (double) x1 - (double) (this.Width / 2);
      float num2 = y1 - (float) (this.Height / 2);
      double num3 = (double) (this.Width / 2);
      float num4 = (float) (num1 / num3);
      float num5 = num2 / (float) (this.Height / 2);
      double num6 = (double) x2 - (double) (this.Width / 2);
      float num7 = y2 - (float) (this.Height / 2);
      double num8 = (double) (this.Width / 2);
      float num9 = (float) (num6 / num8);
      float num10 = num7 / (float) (this.Height / 2);
      double num11 = (double) x3 - (double) (this.Width / 2);
      float num12 = y3 - (float) (this.Height / 2);
      double num13 = (double) (this.Width / 2);
      float num14 = (float) (num11 / num13);
      float num15 = num12 / (float) (this.Height / 2);
      double num16 = (double) x4 - (double) (this.Width / 2);
      float num17 = y4 - (float) (this.Height / 2);
      double num18 = (double) (this.Width / 2);
      float num19 = (float) (num16 / num18);
      float num20 = num17 / (float) (this.Height / 2);
      float[] data = new float[18]
      {
        num4,
        num5,
        z1,
        num9,
        num10,
        z2,
        num14,
        num15,
        z3,
        num9,
        num10,
        z2,
        num14,
        num15,
        z3,
        num19,
        num20,
        z4
      };
      int num21 = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, num21);
      GL.BufferData<float>(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.DynamicDraw);
      this._2dShader.Use();
      int num22 = GL.GenVertexArray();
      GL.BindVertexArray(num22);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num21);
      int attribLocation = this._2dShader.GetAttribLocation("aPos");
      GL.EnableVertexAttribArray(attribLocation);
      GL.VertexAttribPointer(attribLocation, 3, VertexAttribPointerType.Float, false, 12, 0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num21);
      GL.BindVertexArray(num22);
      this._2dShader.Use();
      this._2dShader.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
      GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
      GL.DeleteBuffer(num21);
      GL.DeleteVertexArray(num22);
    }

    protected void drawEllipse(float x, float y, float radiusX, float radiusY, Color4 color)
    {
      int length = (int) Math.Floor(Math.Sqrt((double) radiusX * (double) radiusX + (double) radiusY * (double) radiusY));
      Vector3[] vector3Array = new Vector3[length];
      double num1 = (double) x - (double) (this.Width / 2);
      float num2 = y - (float) (this.Height / 2);
      double num3 = (double) (this.Width / 2);
      float num4 = (float) (num1 / num3);
      float num5 = num2 / (float) (this.Height / 2);
      float num6 = radiusX / (float) (this.Width / 2);
      float num7 = radiusY / (float) (this.Height / 2);
      float num8 = 6.283185f / (float) (length - 1);
      for (int index = 0; index < length; ++index)
      {
        float num9 = (float) index * num8;
        vector3Array[index] = new Vector3((float) Math.Cos((double) num9) * num6 + num4, (float) Math.Sin((double) num9) * num7 - num5, 0.0f);
      }
      List<float> floatList = new List<float>()
      {
        num4,
        -num5,
        0.0f
      };
      for (int index = 0; index < length; ++index)
        floatList.AddRange((IEnumerable<float>) new float[3]
        {
          vector3Array[index].X,
          vector3Array[index].Y,
          vector3Array[index].Z
        });
      float[] array = floatList.ToArray();
      int num10 = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, num10);
      GL.BufferData<float>(BufferTarget.ArrayBuffer, array.Length * 4, array, BufferUsageHint.DynamicDraw);
      this._2dShader.Use();
      int num11 = GL.GenVertexArray();
      GL.BindVertexArray(num11);
      int attribLocation = this._2dShader.GetAttribLocation("aPos");
      GL.EnableVertexAttribArray(attribLocation);
      GL.VertexAttribPointer(attribLocation, 3, VertexAttribPointerType.Float, false, 12, 0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num10);
      GL.BindVertexArray(num11);
      this._2dShader.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
      this._2dShader.Use();
      GL.DrawArrays(PrimitiveType.TriangleFan, 0, length + 1);
      GL.DeleteBuffer(num10);
      GL.DeleteVertexArray(num11);
    }

    public void drawTriangle(
      float x1,
      float y1,
      float x2,
      float y2,
      float x3,
      float y3,
      Color4 color)
    {
      double num1 = (double) x1 - (double) (this.Width / 2);
      float num2 = y1 - (float) (this.Height / 2);
      double num3 = (double) (this.Width / 2);
      float num4 = (float) (num1 / num3);
      float num5 = num2 / (float) (this.Height / 2);
      double num6 = (double) x2 - (double) (this.Width / 2);
      float num7 = y2 - (float) (this.Height / 2);
      double num8 = (double) (this.Width / 2);
      float num9 = (float) (num6 / num8);
      float num10 = num7 / (float) (this.Height / 2);
      double num11 = (double) x3 - (double) (this.Width / 2);
      float num12 = y3 - (float) (this.Height / 2);
      double num13 = (double) (this.Width / 2);
      float num14 = (float) (num11 / num13);
      float num15 = num12 / (float) (this.Height / 2);
      float[] data = new float[9]
      {
        num4,
        -num5,
        0.0f,
        num9,
        -num10,
        0.0f,
        num14,
        -num15,
        0.0f
      };
      int num16 = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, num16);
      GL.BufferData<float>(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.DynamicDraw);
      this._2dShader.Use();
      int num17 = GL.GenVertexArray();
      GL.BindVertexArray(num17);
      int attribLocation = this._2dShader.GetAttribLocation("aPos");
      GL.EnableVertexAttribArray(attribLocation);
      GL.VertexAttribPointer(attribLocation, 3, VertexAttribPointerType.Float, false, 12, 0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, num16);
      GL.BindVertexArray(num17);
      this._2dShader.SetVector4("lightColor", new Vector4(color.R, color.G, color.B, color.A));
      this._2dShader.Use();
      GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
      GL.DeleteBuffer(num16);
      GL.DeleteVertexArray(num17);
    }

    protected void Clear() => GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

    public void drawText(
      string text,
      int px,
      float x,
      float y,
      MainRenderWindow.Font f,
      Color4 col)
    {
      byte[] bytes = Encoding.ASCII.GetBytes(text);
      int num1 = 0;
      foreach (byte num2 in bytes)
      {
        int index = Array.IndexOf<int>(f.data["ids"], (int) num2);
        float num3 = (float) f.data["xs"][index] / (float) f.fontWidth;
        float num4 = (float) f.data["ys"][index] / (float) f.fontWidth;
        int num5 = (int) ((double) px / (double) f.data["heights"][index] * (double) f.data["widths"][index]);
        int num6 = px;
        float num7 = num3 + (float) f.data["widths"][index] / (float) f.fontWidth;
        float num8 = num4 - (float) f.data["heights"][index] / (float) f.fontWidth;
        this.drawTexturedQuad(x + (float) num1, y, 1f, num3, num8, x + (float) num1, y + (float) num6, 1f, num3, num4, x + (float) num5 + (float) num1, y + (float) num6, 1f, num7, num4, x + (float) num5 + (float) num1, y, 1f, num7, num8, f.font, col, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
        num1 += num5;
      }
    }

    public int getPhraseLength(string text, int px, MainRenderWindow.Font f)
    {
      byte[] bytes = Encoding.ASCII.GetBytes(text);
      int num1 = 0;
      foreach (byte num2 in bytes)
      {
        int index = Array.IndexOf<int>(f.data["ids"], (int) num2);
        int num3 = (int) ((double) px / (double) f.data["heights"][index] * (double) f.data["widths"][index]);
        num1 += num3;
      }
      return num1;
    }

    public void SetSettings(MainRenderWindow.Settings s) => this.set = s;

    public void showSettings(MainRenderWindow.Settings s)
    {
      int int32_1 = Convert.ToInt32(s.settings["width"]);
      int int32_2 = Convert.ToInt32(s.settings["height"]);
      Vector2 vector2 = new Vector2((float) ((this.Width - int32_1) / 2), (float) ((this.Height - int32_2) / 2));
      if (Convert.ToBoolean(s.settings["useTexture"]))
      {
        string texturePath = Convert.ToString(s.settings["texturePath"]);
        this.drawTexturedRectangle(vector2.X, vector2.Y, 0.0f, 0.0f, vector2.X + (float) int32_1, vector2.Y + (float) int32_2, 1f, 1f, texturePath, Color4.White, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
      }
      else
      {
        Color4 color = new Color4((float) Convert.ToInt32(s.settings["r"]), (float) Convert.ToInt32(s.settings["g"]), (float) Convert.ToInt32(s.settings["b"]), (float) Convert.ToInt32(s.settings["a"]));
        this.drawRectangle(vector2.X, vector2.Y, vector2.X + (float) int32_1, vector2.Y + (float) int32_2, color);
      }
      foreach (MainRenderWindow.Settings.Button button in s.buttons)
      {
        float x1 = button.pos.X + vector2.X;
        float y1 = button.pos.Y + vector2.Y;
        this.drawRectangle(x1, y1, x1 + (float) button.width, y1 + (float) button.height, button.col);
        if (button.l == -1)
          button.l = this.getPhraseLength(button.Text, Math.Min(button.height, 32), button.font);
        this.drawText(button.Text, Math.Min(button.height, 32), x1 + (float) ((button.width - button.l) / 2), y1 + (float) ((button.height - Math.Min(button.height, 32)) / 2), button.font, Color4.White);
        button.setCol(Color4.Blue);
      }
    }

    private void checkClicks(MainRenderWindow.Settings s)
    {
      int int32_1 = Convert.ToInt32(s.settings["width"]);
      int int32_2 = Convert.ToInt32(s.settings["height"]);
      foreach (MainRenderWindow.Settings.Button button in s.buttons)
      {
        Vector2 vector2 = new Vector2((float) ((this.Width - int32_1) / 2), (float) ((this.Height - int32_2) / 2));
        MouseState cursorState = Mouse.GetCursorState();
        float num1 = (float) (cursorState.X - this.X - 8) - vector2.X;
        float num2 = (float) -(cursorState.Y - this.Y - 30 - this.Height) - vector2.Y;
        if ((double) num1 >= (double) button.pos.X && (double) num1 <= (double) button.pos.X + (double) button.width && ((double) num2 >= (double) button.pos.Y && (double) num2 <= (double) button.pos.Y + (double) button.height))
        {
          button.setCol(Color4.Red);
          object obj = button.onClick();
        }
      }
    }

    private class Object
    {
      private readonly int _vertexBufferObject;
      private readonly int _mainObject;
      private readonly float[] _vertices;
      private float _rotX;
      private float _rotY;
      private float _rotZ;
      private Vector3 _pos;
      private readonly Vector3 _color;
      private readonly Shader _shader;
      private readonly MainRenderWindow.Lamp _lamp;
      private float _scale = 1f;

      public Object(string path, Shader lightingShader, MainRenderWindow.Lamp lamp, Vector3 col)
      {
        this._vertices = MainRenderWindow.loadObj(path);
        this._vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, this._vertexBufferObject);
        GL.BufferData<float>(BufferTarget.ArrayBuffer, this._vertices.Length * 4, this._vertices, BufferUsageHint.StaticDraw);
        this._mainObject = GL.GenVertexArray();
        GL.BindVertexArray(this._mainObject);
        GL.BindBuffer(BufferTarget.ArrayBuffer, this._vertexBufferObject);
        int attribLocation1 = lightingShader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(attribLocation1);
        GL.VertexAttribPointer(attribLocation1, 3, VertexAttribPointerType.Float, false, 24, 0);
        int attribLocation2 = lightingShader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(attribLocation2);
        GL.VertexAttribPointer(attribLocation2, 3, VertexAttribPointerType.Float, false, 24, 12);
        this._rotX = 0.0f;
        this._rotY = 0.0f;
        this._rotZ = 0.0f;
        this._pos = new Vector3(0.0f, 0.0f, 0.0f);
        this._shader = lightingShader;
        this._lamp = lamp;
        this._color = col;
      }

      public Object(
        float[] vertices,
        Shader lightingShader,
        MainRenderWindow.Lamp lamp,
        Vector3 col)
      {
        this._vertices = vertices;
        this._vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, this._vertexBufferObject);
        GL.BufferData<float>(BufferTarget.ArrayBuffer, this._vertices.Length * 4, this._vertices, BufferUsageHint.StaticDraw);
        this._mainObject = GL.GenVertexArray();
        GL.BindVertexArray(this._mainObject);
        GL.BindBuffer(BufferTarget.ArrayBuffer, this._vertexBufferObject);
        int attribLocation1 = lightingShader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(attribLocation1);
        GL.VertexAttribPointer(attribLocation1, 3, VertexAttribPointerType.Float, false, 24, 0);
        int attribLocation2 = lightingShader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(attribLocation2);
        GL.VertexAttribPointer(attribLocation2, 3, VertexAttribPointerType.Float, false, 24, 12);
        this._rotX = 0.0f;
        this._rotY = 0.0f;
        this._rotZ = 0.0f;
        this._pos = new Vector3(0.0f, 0.0f, 0.0f);
        this._shader = lightingShader;
        this._lamp = lamp;
        this._color = col;
      }

      public void show(Camera camera)
      {
        GL.BindVertexArray(this._mainObject);
        this._shader.Use();
        this._shader.SetMatrix4("model", Matrix4.CreateScale(this._scale) * Matrix4.CreateRotationX(this._rotX) * Matrix4.CreateRotationX(this._rotY) * Matrix4.CreateRotationZ(this._rotZ) * Matrix4.CreateTranslation(this._pos));
        this._shader.SetMatrix4("view", camera.GetViewMatrix());
        this._shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        this._shader.SetVector3("objectColor", this._color);
        this._shader.SetVector3("lightColor", this._lamp.LightColor);
        this._shader.SetVector3("lightPos", this._lamp.Pos);
        GL.DrawArrays(PrimitiveType.Triangles, 0, this._vertices.Length / 6);
      }

      public void setRotationX(float angle) => this._rotX = angle;

      public void setRotationY(float angle) => this._rotY = angle;

      public void setRotationZ(float angle) => this._rotZ = angle;

      public void setPositionInSpace(float x, float y, float z) => this._pos = new Vector3(x, y, z);

      public void setScale(float scale) => this._scale = scale;

      public void Dispose()
      {
        GL.DeleteBuffer(this._vertexBufferObject);
        GL.DeleteVertexArray(this._mainObject);
      }
    }

    private class Lamp
    {
      private readonly int _vertexBufferObject;
      private readonly int _mainObject;
      public readonly Vector3 Pos;
      public readonly Vector3 LightColor;
      private readonly float[] _vertices;

      public Lamp(Vector3 pos, Vector3 lightColor, Shader lampShader)
      {
        this.Pos = pos;
        this.LightColor = lightColor;
        this._vertices = MainRenderWindow.loadObj("Objs/sphere.obj");
        this._vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, this._vertexBufferObject);
        GL.BufferData<float>(BufferTarget.ArrayBuffer, this._vertices.Length * 4, this._vertices, BufferUsageHint.StaticDraw);
        this._mainObject = GL.GenVertexArray();
        GL.BindVertexArray(this._mainObject);
        GL.BindBuffer(BufferTarget.ArrayBuffer, this._vertexBufferObject);
        int attribLocation = lampShader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(attribLocation);
        GL.VertexAttribPointer(attribLocation, 3, VertexAttribPointerType.Float, false, 24, 0);
        lampShader.SetVector3(nameof (lightColor), lightColor);
      }

      public void show(Camera camera, Shader lampShader)
      {
        GL.BindVertexArray(this._mainObject);
        lampShader.Use();
        Matrix4 data = Matrix4.Identity * Matrix4.CreateScale(0.2f) * Matrix4.CreateTranslation(this.Pos);
        lampShader.SetMatrix4("model", data);
        lampShader.SetMatrix4("view", camera.GetViewMatrix());
        lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());
        GL.DrawArrays(PrimitiveType.Triangles, 0, this._vertices.Length / 6);
      }

      public void Dispose()
      {
        GL.DeleteBuffer(this._vertexBufferObject);
        GL.DeleteVertexArray(this._mainObject);
      }
    }

    private class TexturedObject
    {
      private readonly int _vertexBufferObject;
      private readonly int _mainObject;
      private readonly float[] _vertices;
      private float _rotX;
      private float _rotY;
      private float _rotZ;
      private readonly Texture _texture;
      private Vector3 _pos;
      private readonly Shader _shader;

      public TexturedObject(string path, Shader textureShader, string texturePath)
      {
        this._vertices = MainRenderWindow.loadObjTextured(path);
        this._vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, this._vertexBufferObject);
        GL.BufferData<float>(BufferTarget.ArrayBuffer, this._vertices.Length * 4, this._vertices, BufferUsageHint.StaticDraw);
        this._mainObject = GL.GenVertexArray();
        GL.BindVertexArray(this._mainObject);
        int attribLocation1 = textureShader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(attribLocation1);
        GL.VertexAttribPointer(attribLocation1, 3, VertexAttribPointerType.Float, false, 32, 0);
        int attribLocation2 = textureShader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(attribLocation2);
        GL.VertexAttribPointer(attribLocation2, 3, VertexAttribPointerType.Float, false, 32, 12);
        int attribLocation3 = textureShader.GetAttribLocation("aTexture");
        GL.EnableVertexAttribArray(attribLocation3);
        GL.VertexAttribPointer(attribLocation3, 2, VertexAttribPointerType.Float, false, 32, 24);
        this._texture = new Texture(texturePath, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
        this._texture.Use();
        this._rotX = 0.0f;
        this._rotY = 0.0f;
        this._rotZ = 0.0f;
        this._pos = new Vector3(0.0f, 0.0f, 0.0f);
        this._shader = textureShader;
      }

      public void show(Camera camera)
      {
        GL.BindVertexArray(this._mainObject);
        this._texture.Use();
        this._shader.Use();
        this._shader.SetMatrix4("model", Matrix4.CreateRotationX(this._rotX) * Matrix4.CreateRotationX(this._rotY) * Matrix4.CreateRotationZ(this._rotZ) * Matrix4.CreateTranslation(this._pos));
        this._shader.SetMatrix4("view", camera.GetViewMatrix());
        this._shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        GL.DrawArrays(PrimitiveType.Triangles, 0, this._vertices.Length / 8);
      }

      public void setRotationX(float angle) => this._rotX = angle;

      public void setRotationY(float angle) => this._rotY = angle;

      public void setRotationZ(float angle) => this._rotZ = angle;

      public void setPositionInSpace(float x, float y, float z) => this._pos = new Vector3(x, y, z);

      public void Dispose()
      {
        GL.DeleteBuffer(this._vertexBufferObject);
        GL.DeleteVertexArray(this._mainObject);
        GL.DeleteTexture(this._texture.Handle);
      }
    }

    public class Font
    {
      public Dictionary<string, int[]> data = new Dictionary<string, int[]>();
      public Texture font;
      public int fontWidth;

      public Font(string path, string path2)
      {
        this.data = new Dictionary<string, int[]>();
        this.font = new Texture(path2, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
        using (StreamReader streamReader = new StreamReader(path))
        {
          this.fontWidth = new Bitmap(path2).Width;
          List<int> intList1 = new List<int>();
          List<int> intList2 = new List<int>();
          List<int> intList3 = new List<int>();
          List<int> intList4 = new List<int>();
          List<int> intList5 = new List<int>();
          string str1;
          while ((str1 = streamReader.ReadLine()) != null)
          {
            if (str1.Substring(0, 5) == "char ")
            {
              string[] strArray1 = str1.Substring(5).Split(" ");
              List<int> intList6 = new List<int>();
              foreach (string str2 in strArray1)
              {
                if (str2.Contains("="))
                {
                  string[] strArray2 = str2.Split("=");
                  Regex.Replace(strArray2[1], "\\s+", "");
                  intList6.Add(int.Parse(strArray2[1]));
                }
              }
              intList1.Add(intList6[0]);
              intList2.Add(intList6[1]);
              intList3.Add(-(intList6[2] - this.fontWidth));
              intList4.Add(intList6[3]);
              intList5.Add(intList6[4]);
            }
          }
          this.data.Add("ids", intList1.ToArray());
          this.data.Add("xs", intList2.ToArray());
          this.data.Add("ys", intList3.ToArray());
          this.data.Add("widths", intList4.ToArray());
          this.data.Add("heights", intList5.ToArray());
          streamReader.Close();
        }
      }

      public void Dispose() => GL.DeleteTexture(this.font.Handle);
    }

    public class Settings
    {
      public List<MainRenderWindow.Settings.Button> buttons = new List<MainRenderWindow.Settings.Button>();
      public Dictionary<string, object> settings = new Dictionary<string, object>();

      public void addButton(
        string t,
        float x,
        float y,
        int w,
        int h,
        Color4 c,
        Func<object> func,
        MainRenderWindow.Font f)
      {
        this.buttons.Add(new MainRenderWindow.Settings.Button()
        {
          pos = new Vector2(x, y),
          width = w,
          height = h,
          onClick = func,
          col = c,
          Text = t,
          l = -1,
          font = f
        });
      }

      public void addSetting(string key, object value) => this.settings.Add(key, value);

      public void readSettings()
      {
        try
        {
          using (StreamReader streamReader = new StreamReader("settings.cfg"))
          {
            this.settings = new Dictionary<string, object>();
            string str;
            while ((str = streamReader.ReadLine()) != null)
            {
              string[] strArray = str.Split("=");
              if (new Regex("^[\\d.]+$").IsMatch(strArray[1]))
                this.settings.Add(strArray[0], (object) float.Parse(strArray[1]));
              else
                this.settings.Add(strArray[0], (object) strArray[1]);
            }
          }
        }
        catch (InvalidCastException ex)
        {
          Console.WriteLine((object) ex.GetBaseException());
        }
      }

      public void writeSettings()
      {
        try
        {
          using (FileStream fileStream = File.Create("settings.cfg"))
          {
            string s = "";
            foreach (KeyValuePair<string, object> setting in this.settings)
              s = s + setting.Key + "=" + setting.Value?.ToString() + "\n";
            byte[] bytes = new UTF8Encoding(true).GetBytes(s);
            fileStream.Write(bytes, 0, bytes.Length);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
      }

      public class Button
      {
        public Vector2 pos;
        public string Text;
        public int width;
        public int height;
        public int l;
        public Func<object> onClick;
        public Color4 col;
        public MainRenderWindow.Font font;

        public void setCol(Color4 c) => this.col = c;
      }
    }
  }
}
