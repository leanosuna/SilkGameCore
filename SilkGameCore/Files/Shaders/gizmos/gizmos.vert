#version 410 core
layout (location = 0) in vec3 vPos;

uniform mat4 uWorld;
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    vec4 world = uWorld * vec4(vPos, 1.0);
    gl_Position = uProjection * uView * uWorld * vec4(vPos, 1.0);
    
}