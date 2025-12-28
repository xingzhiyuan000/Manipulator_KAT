#version 330 core
uniform vec2 ViewportSize;

uniform mat4 transform;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform mat3 normalMatrix;
uniform mat4 matrices[100];

layout (location=0) in vec3 aPosition;
layout (location=1) in vec2 aUV;
layout (location=2) in vec3 aNormal;


out vec2 vUV;
out vec3 vNormal;
out vec3 worldPosition;
        
void main()
{
    vec4 transformPosition = vec4(aPosition, 1.0);
    transformPosition = transform * matrices[gl_InstanceID]*transformPosition;
    worldPosition = transformPosition.xyz; 
    gl_Position=projectionMatrix*viewMatrix*transformPosition;
    vUV=aUV;
    vNormal =  normalMatrix * aNormal;
}