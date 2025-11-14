#version 410 core
layout (location = 0) in vec3 vPos;
uniform mat4 uWorld;

layout(std140) uniform CommonData
{
    mat4 sView;
    mat4 sProjection;
    float sTime;
    float sDeltaTime;
};

void main()
{
    gl_Position = sProjection * sView * uWorld * vec4(vPos, 1.0);
}