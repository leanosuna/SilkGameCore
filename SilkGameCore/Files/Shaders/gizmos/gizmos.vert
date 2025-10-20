#version 410 core
layout (location = 0) in vec3 vPos;
uniform mat4 uWorld;
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    //gl_Position = vec4(vPos, 1.0) * uWorld * uView * uProjection;
    gl_Position = uProjection * uView * uWorld * vec4(vPos, 1.0);
}