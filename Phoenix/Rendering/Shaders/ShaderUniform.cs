using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Rendering.Shaders
{
    public class ShaderUniform<Type>
        where Type : unmanaged
    {
        int _location;
        private GLShader _shader;
        public ShaderUniform(GLShader shader, string name)
        {
            _shader = shader;
            _location = shader.GetUniformLocation(name);
        }

        public void Set(Type value)
        {
            _shader.SetUniform(_location, value);
        }
    }
}
