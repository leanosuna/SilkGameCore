using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Phoenix.Rendering.Textures
{
    public class GLTexture : IDisposable
    {
        private uint _handle;
        private GL GL;

        public string Path { get; set; } = default!;
        public string Name { get; set; } = default!;
        public Vector2 Size
        {
            get;
            internal set;
        }

        public int Width => (int)Size.X;
        public int Height => (int)Size.Y;

        public unsafe GLTexture(GL gl, string path, 
            InternalFormat format = InternalFormat.Rgba8, GLEnum wrapS = GLEnum.DecrWrap, 
            GLEnum wrapT = GLEnum.DecrWrap, GLEnum minFilter = GLEnum.LinearMipmapLinear, 
            GLEnum magFilter = GLEnum.Linear, bool genMipMap = true, int baseLevel = 0, int maxLevel = 8)
        {
            GL = gl;
            Path = path;
            Name = path.Split("\\").Last();
            _handle = GL.GenTexture();
            Bind();

            using (var img = Image.Load<Rgba32>(path))
            {
                gl.TexImage2D(TextureTarget.Texture2D, 
                    0, 
                    format, 
                    (uint)img.Width, 
                    (uint)img.Height, 
                    0, 
                    PixelFormat.Rgba, 
                    PixelType.UnsignedByte, 
                    null);

                Size = new Vector2(img.Width, img.Height);
                img.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        fixed (void* data = accessor.GetRowSpan(y))
                        {
                            gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, 
                                (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                        }
                    }
                });
            }

            SetParameters(wrapS, wrapT, minFilter, magFilter, genMipMap, baseLevel, maxLevel);
        }
        public unsafe GLTexture(GL gl, string name, void* data, uint width, uint height, InternalFormat format = InternalFormat.Rgba8,
            GLEnum wrapS = GLEnum.DecrWrap, GLEnum wrapT = GLEnum.DecrWrap, GLEnum minFilter = GLEnum.LinearMipmapLinear,
            GLEnum magFilter = GLEnum.Linear, bool genMipMap = true, int baseLevel = 0, int maxLevel = 8)
        {
            GL = gl;
            Size = new Vector2((int)width, (int)height);
            Path = ""; 
            Name = name;
            _handle = GL.GenTexture();
            Bind();
            GL.TexImage2D(TextureTarget.Texture2D, 
                0, 
                (int)format, 
                width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            SetParameters(wrapS, wrapT, minFilter, magFilter, genMipMap, baseLevel, maxLevel);
        }

        public unsafe GLTexture(GL gl, uint width, uint height, out uint handle, InternalFormat format = InternalFormat.Rgba8,
            GLEnum wrapS = GLEnum.DecrWrap, GLEnum wrapT = GLEnum.DecrWrap, GLEnum minFilter = GLEnum.LinearMipmapLinear,
            GLEnum magFilter = GLEnum.Linear, bool genMipMap = true, int baseLevel = 0, int maxLevel = 8)
        {
            GL = gl;
            Size = new Vector2((int)width, (int)height);
            Path = "";
            Name = "";
            _handle = GL.GenTexture();
            Bind();
            GL.TexImage2D(TextureTarget.Texture2D,
                0,
                (int)format,
                width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
            SetParameters(wrapS, wrapT, minFilter, magFilter, genMipMap, baseLevel, maxLevel);

            handle = _handle;
        }
        
        //public unsafe GLTexture(GL gl, Span<byte> data, uint width, uint height)
        //{
        //    GL = gl;

        //    _handle = GL.GenTexture();
        //    Bind();

        //    fixed (void* d = &data[0])
        //    {
        //        GL.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
        //        SetParameters();
        //    }
        //}

        private void SetParameters(GLEnum wrapS, GLEnum wrapT, GLEnum minFilter, GLEnum magFilter, bool genMipMap = true, int baseLevel = 0, int maxLevel = 8)
        {
            GL.TexParameter(TextureTarget.Texture2D, GLEnum.TextureWrapS, (int)wrapS);
            GL.TexParameter(TextureTarget.Texture2D, GLEnum.TextureWrapT, (int)wrapT);
            GL.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMinFilter, (int)(genMipMap? GLEnum.LinearMipmapLinear : minFilter));
            GL.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)magFilter);
            if(genMipMap)
            {
                GL.TexParameter(TextureTarget.Texture2D, GLEnum.TextureBaseLevel, baseLevel);
                GL.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMaxLevel, maxLevel);
                GL.GenerateMipmap(TextureTarget.Texture2D);
            }
        }


        public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            GL.ActiveTexture(textureSlot);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }
        public uint GetHandle()
        {
            return _handle;
        }

        public void Dispose()
        {
            GL.DeleteTexture(_handle);
        }
    }
}