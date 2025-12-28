#version 330 core             

in vec2 vUV;
in vec3 vNormal;
out vec4 pixelColor;


void main()
{
    pixelColor = vec4(vec3(1.0,1.0,1.0), 1.0);
}