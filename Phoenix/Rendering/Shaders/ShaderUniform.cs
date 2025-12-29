using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Phoenix.Rendering.Shaders
{
    public class ShaderUniform<Type>
    {
        int _location;
        private GLShader _shader;
        public ShaderUniform(GLShader shader, string name, bool throwIfNotFound = true)
        {
            _shader = shader;
            _location = shader.GetUniformLocation(name);
            if (_location == -1 && throwIfNotFound)
                throw new Exception($"Uniform [{name}] not found");
        }

        public void Set(Type value)
        {
            _shader.SetUniform(_location, value);
        }
    }
}
