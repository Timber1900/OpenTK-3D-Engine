using System;

namespace Program
{
    public static class Shaders
    {
        public static readonly string LightingFrag = 
@"#version 410 core
out vec4 FragColor;
//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:
uniform vec3 objectColor; //The color of the object.
uniform vec3 lightColor; //The color of the light.
uniform vec3 lightPos; //The position of the light.

in vec3 Normal; //The normal of the fragment is calculated in the vertex shader.
in vec3 FragPos; //The fragment position.

void main()
{
    float ambientStrength = 0.25;
    vec3 ambient = ambientStrength * lightColor;
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0) * 0.75;
    vec3 diffuse = diff * lightColor;
    vec3 result = (ambient + diffuse) * objectColor;
    FragColor = vec4(result, 1.0);
}";

        public static readonly string ShaderFrag = 
@"#version 410 core
out vec4 FragColor;
uniform vec3 lightColor;

void main()
{
    FragColor = vec4(lightColor, 1.0); // set all 4 vector values to 1.0
}
";

        public static readonly string ShaderVert = 
@"#version 410 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 Normal;
out vec3 FragPos;

void main()
{
    gl_Position = vec4(aPos, 1.0) * model * view * projection;
    FragPos = vec3(vec4(aPos, 1.0) * model);
    Normal = -(aNormal * mat3(transpose(inverse(model))));
}";

        public static readonly string Shader2DFrag = @"#version 410 core
out vec4 FragColor;
uniform vec4 lightColor;

void main()
{
    FragColor = lightColor; 

}";

        public static readonly string Shader2DVert = @"#version 410 core
layout (location = 0) in vec3 aPos;

void main(void)
{
    gl_Position = vec4(aPos, 1.0f);    
}";
        public static readonly string TextureFrag = 
@"#version 410 core
out vec4 FragColor;
uniform sampler2D texture0;

in vec3 Normal;
in vec3 FragPos;
in vec2 textureCords;

void main()
{
    FragColor = texture(texture0, textureCords);
}";
        public static readonly string TextureVert = @"#version 410 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexture;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 Normal;
out vec3 FragPos;
out vec2 textureCords;

void main()
{
    gl_Position = vec4(aPos, 1.0) * model * view * projection;
    textureCords = aTexture;
    FragPos = vec3(vec4(aPos, 1.0) * model);
    Normal = -(aNormal * mat3(transpose(inverse(model))));
}";
        public static readonly string Texture2DFrag =
@"#version 410
out vec4 outputColor;

in vec2 texCoord;
uniform vec4 lightColor;

uniform sampler2D texture0;

void main()
{
    outputColor = texture(texture0, texCoord) * lightColor;
}";

        public static readonly string Texture2DVert =
@"#version 410 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;

out vec2 texCoord;

void main(void)
{
    texCoord = aTexCoord;

    gl_Position = vec4(aPosition, 1.0);
}";
    }
}
