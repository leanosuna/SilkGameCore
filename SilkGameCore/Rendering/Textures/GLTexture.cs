using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SilkGameCore.Rendering.Textures
{
    public class GLTexture : IDisposable
    {
        private uint _handle;
        private GL GL;

        public string Path { get; set; } = default!;

        public int Width, Height;

        public unsafe GLTexture(GL gl, string path)
        {
            GL = gl;
            Path = path;
            _handle = GL.GenTexture();
            Bind();

            using (var img = Image.Load<Rgba32>(path))
            {
                gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
                Width = img.Width;
                Height = img.Height;
                img.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        fixed (void* data = accessor.GetRowSpan(y))
                        {
                            gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                        }
                    }
                });
            }

            SetParameters();
        }
        public unsafe GLTexture(GL gl, void* data, uint width, uint height)
        {
            GL = gl;

            _handle = GL.GenTexture();
            Bind();

            GL.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            SetParameters();

        }
        public unsafe GLTexture(GL gl, Span<byte> data, uint width, uint height)
        {
            GL = gl;

            _handle = GL.GenTexture();
            Bind();

            fixed (void* d = &data[0])
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
                SetParameters();
            }
        }

        private void SetParameters()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.DecrWrap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.DecrWrap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
            GL.GenerateMipmap(TextureTarget.Texture2D);
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