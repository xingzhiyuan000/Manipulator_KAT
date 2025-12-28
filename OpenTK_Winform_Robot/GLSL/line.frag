#version 330 core             

in vec3 vColor;
out vec4 pixelColor;
//uniform vec4 lineColor;

void main()
{
    //FragColor = lineColor;
    //FragColor = vec4(1.0,0.0,0.0,1.0);
    pixelColor=vec4(vColor, 1.0);
}