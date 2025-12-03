#version 410 core

out vec4 FragColor;

uniform vec3 uColor;
uniform bool uHit;

void main()
{
    vec3 col = uColor;
    if(uHit)
    {
        if(col == vec3(1, 0, 0))
            col = vec3(1, 1, 0);
        else
            col = vec3(1, 0, 0);
    }
    FragColor = vec4(col, 1.0);

}