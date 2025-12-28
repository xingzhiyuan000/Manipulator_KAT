#version 330 core
uniform vec2 ViewportSize;

uniform mat4 transform;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform mat3 normalMatrix;

//uniform float time;
layout (location=0) in vec3 aPosition;
//layout (location=2) in vec4 aColor;
layout (location=1) in vec2 aUV;
layout (location=2) in vec3 aNormal;


//out vec4 vColor;
out vec2 vUV;
out vec3 vNormal;
out vec3 worldPosition;
        
void main()
{

    vec4 transformPosition = vec4(aPosition, 1.0);
    transformPosition = transform * transformPosition;
    worldPosition = transformPosition.xyz; 
    //gl_Position=orthoMatrix*viewMatrix*transform*position;
    gl_Position=projectionMatrix*viewMatrix*transformPosition;
    //vColor=aColor*(sin(time/1000*3.14)+1.0)/2.0;
    //vColor=aColor;
    vUV=aUV;
    //vNormal=aNormal;
    vNormal =  normalMatrix * aNormal;
}