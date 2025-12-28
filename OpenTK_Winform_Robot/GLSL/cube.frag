#version 330 core             

out vec4 pixelColor;
in vec3 uvw;

uniform samplerCube cubeSampler;

void main()
{
    pixelColor = texture(cubeSampler, uvw);
}