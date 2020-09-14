#version 330 core
out vec4 FragColor;

//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:
// uniform vec3 lightColor; //The color of the light.
// uniform vec3 lightPos; //The position of the light.
uniform sampler2D texture0;

in vec3 Normal; //The normal of the fragment is calculated in the vertex shader.
in vec3 FragPos; //The fragment position.
in vec2 textureCords;

void main()
{
    // float ambientStrength = 0.1;
    // vec3 ambient = ambientStrength * lightColor;
    // vec3 norm = normalize(Normal);
    // vec3 lightDir = normalize(lightPos - FragPos);
    // float diff = max(dot(norm, lightDir), 0.0);
    // vec3 diffuse = diff * lightColor;
    // vec3 result = (ambient + diffuse) * vec3(texture(texture0, textureCords));
    // FragColor = vec4(result, 1);
    FragColor = texture(texture0, textureCords);
}