#version 330

out vec4 outputColor;

in vec2 texCoord;
uniform vec4 lightColor;

uniform sampler2D texture0;

void main()
{
    outputColor = texture(texture0, texCoord) * lightColor;
    //outputColor = vec4(1f, 1f, 1f, 1f);
}