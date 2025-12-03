using Phoenix.Rendering.Textures;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Rendering.Shaders
{
    public class ShaderTextureUniform
    {
        int _location;
        public GLShader _shader;
        int _slot;
        public ShaderTextureUniform(GLShader shader, string name, int slot)
        {
            _shader = shader;
            _location = shader.GetUniformLocation(name);
            _slot = slot;
        }

        public void Set(GLTexture tex)
        {
            tex.Bind(TextureUnit.Texture0 + _slot);
            _shader.SetTextureUniform(_location, tex, _slot);
        }
        public void Set(uint tex)
        {
            _shader.SetTextureUniform(_location, tex, _slot);
        }
    }
}
