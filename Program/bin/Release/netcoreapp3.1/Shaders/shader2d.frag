#version 330 core
out vec4 FragColor;
uniform vec4 lightColor;

void main()
{
    //FragColor = lightColor; // set all 4 vector values to 1.0
    FragColor = vec4(1,1,1,1);
}