// Decompiled with JetBrains decompiler
// Type: Program.Shaders
// Assembly: Program, Version=1.7.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF30FF81-E0B0-4B54-A928-DD8AA8C2D21D
// Assembly location: C:\Users\Hugo Teixeira\.nuget\packages\opentk.3d.library\1.7.0\lib\netcoreapp3.1\Program.dll

namespace Program
{
  /// <summary>
  ///     Shaders
  /// </summary>
  public static class Shaders
    {
      /// <summary>
      /// </summary>
      public const string LightingFrag =
            "#version 330\r\n\r\nout vec4 FragColor;\r\n//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:\r\nuniform vec4 objectColor; //The color of the object.\r\nuniform vec3 lightColor; //The color of the light.\r\nuniform vec3 lightPos; //The position of the light.\r\n\r\nin vec3 Normal; //The normal of the fragment is calculated in the vertex shader.\r\nin vec3 FragPos; //The fragment position.\r\n\r\nvoid main()\r\n{\r\n    float ambientStrength = 0.25;\r\n    vec3 ambient = ambientStrength * lightColor;\r\n    vec3 norm = normalize(Normal);\r\n    vec3 lightDir = normalize(lightPos - FragPos);\r\n    float diff = max(dot(norm, lightDir), 0.0) * 0.75;\r\n    vec3 diffuse = diff * lightColor;\r\n    vec4 result = vec4((ambient + diffuse),1.0) * objectColor;\r\n    FragColor = result;\r\n}";

      /// <summary>
      /// </summary>
      public const string ShaderFrag =
            "#version 330\r\n\r\nout vec4 FragColor;\r\nuniform vec3 lightColor;\r\n\r\nvoid main()\r\n{\r\n    FragColor = vec4(lightColor, 1.0); // set all 4 vector values to 1.0\r\n}\r\n";

      /// <summary>
      /// </summary>
      public const string ShaderVert =
            "#version 330\r\n\r\nlayout (location = 0) in vec3 aPos;\r\nlayout (location = 1) in vec3 aNormal;\r\n\r\nuniform mat4 model;\r\nuniform mat4 view;\r\nuniform mat4 projection;\r\n\r\nout vec3 Normal;\r\nout vec3 FragPos;\r\n\r\nvoid main()\r\n{\r\n    gl_Position = vec4(aPos, 1.0) * model * view * projection;\r\n    FragPos = vec3(vec4(aPos, 1.0) * model);\r\n    Normal = -(aNormal * mat3(transpose(inverse(model))));\r\n}";

      /// <summary>
      /// </summary>
      public const string Shader2DFrag =
            "#version 330\r\n\r\nout vec4 FragColor;\r\nuniform vec4 lightColor;\r\n\r\nvoid main()\r\n{\r\n    FragColor = lightColor; \r\n\r\n}";

      /// <summary>
      /// </summary>
      public const string Shader2DVert =
            "#version 330\r\n\r\nlayout (location = 0) in vec3 aPos;\r\n\r\nvoid main(void)\r\n{\r\n    gl_Position = vec4(aPos, 1.0);    \r\n}";

      /// <summary>
      /// </summary>
      public const string TextureFrag =
            "#version 330\r\n\r\nout vec4 FragColor;\r\nuniform sampler2D texture0;\r\n\r\nin vec3 Normal;\r\nin vec3 FragPos;\r\nin vec2 textureCords;\r\n\r\nvoid main()\r\n{\r\n    FragColor = texture(texture0, textureCords);\r\n}";

      /// <summary>
      /// </summary>
      public const string TextureVert =
            "#version 330\r\n\r\nlayout (location = 0) in vec3 aPos;\r\nlayout (location = 1) in vec3 aNormal;\r\nlayout (location = 2) in vec2 aTexture;\r\n\r\nuniform mat4 model;\r\nuniform mat4 view;\r\nuniform mat4 projection;\r\n\r\nout vec3 Normal;\r\nout vec3 FragPos;\r\nout vec2 textureCords;\r\n\r\nvoid main()\r\n{\r\n    gl_Position = vec4(aPos, 1.0) * model * view * projection;\r\n    textureCords = aTexture;\r\n    FragPos = vec3(vec4(aPos, 1.0) * model);\r\n    Normal = -(aNormal * mat3(transpose(inverse(model))));\r\n}";

      /// <summary>
      /// </summary>
      public const string Texture2DFrag =
            "#version 330\r\n\r\nout vec4 outputColor;\r\n\r\nin vec2 texCoord;\r\nuniform vec4 lightColor;\r\n\r\nuniform sampler2D texture0;\r\n\r\nvoid main()\r\n{\r\n    outputColor = texture(texture0, texCoord) * lightColor;\r\n}";

      /// <summary>
      /// </summary>
      public const string Texture2DVert =
            "#version 330\r\n\r\nlayout(location = 0) in vec3 aPosition;\r\nlayout(location = 1) in vec2 aTexCoord;\r\n\r\nout vec2 texCoord;\r\n\r\nvoid main(void)\r\n{\r\n    texCoord = aTexCoord;\r\n\r\n    gl_Position = vec4(aPosition, 1.0);\r\n}";
    }
}