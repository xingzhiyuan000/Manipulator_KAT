#version 330 core
uniform vec2 ViewportSize;

uniform mat4 transform;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;


layout (location=0) in vec3 aPosition;

out vec3 uvw;
        
void main()
{

    vec4 transformPosition = vec4(aPosition, 1.0);
    transformPosition = transform * transformPosition;
    gl_Position=projectionMatrix*viewMatrix*transformPosition;
    gl_Position = gl_Position.xyww;
 
     uvw = aPosition;
}