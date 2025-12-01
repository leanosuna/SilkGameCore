using Silk.NET.OpenGL;
using SilkGameCore.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;


namespace SilkGameCore.Rendering.RT
{
    public class RenderTexture
    {
        public uint Handle 
        {
            get;
            internal set; 
        } = uint.MaxValue;
        internal bool IsBound => Handle != uint.MaxValue;
        public bool FollowsWindowSize { get; private set; }

        public Vector2 SizeMultiplier { get; private set; }
        public Vector2 Size
        {
            get;
            internal set;
        }
        
        public uint Width => (uint)Size.X;
        public uint Height => (uint)Size.Y;

        public InternalFormat Format { get; private set; }

        public GLEnum WrapS {  get; private set; }
        public GLEnum WrapT { get; private set; }
        public GLEnum MinFilter { get; private set;  }
        public GLEnum MagFilter { get; private set; }
        

        internal GLTexture texture = default!;

        
        //dynamic targets
        public RenderTexture(InternalFormat format = InternalFormat.Rgba,
            GLEnum wrapS = GLEnum.ClampToEdge, GLEnum wrapT = GLEnum.ClampToEdge,
            GLEnum minFilter = GLEnum.Linear, GLEnum magFilter = GLEnum.Linear)
        {
            FollowsWindowSize = true;
            SizeMultiplier = Vector2.One;
            Size = Vector2.Zero;
            Format = format;
            WrapS = wrapS;
            WrapT = wrapT;
            MinFilter = minFilter;
            MagFilter = magFilter;
        }
        public RenderTexture(float sizeMultiplierX, float sizeMultiplierY, InternalFormat format = InternalFormat.Rgba,
            GLEnum wrapS = GLEnum.ClampToEdge, GLEnum wrapT = GLEnum.ClampToEdge, 
            GLEnum minFilter = GLEnum.Linear, GLEnum magFilter = GLEnum.Linear)
        {
            FollowsWindowSize = true;
            SizeMultiplier = new Vector2(sizeMultiplierX, sizeMultiplierY);
            Size = Vector2.Zero;
            Format = format;
            WrapS = wrapS;
            WrapT = wrapT;
            MinFilter = minFilter;
            MagFilter = magFilter;

        }
        //static target
        public RenderTexture(Vector2 size, InternalFormat format = InternalFormat.Rgba,
            GLEnum wrapS = GLEnum.ClampToEdge, GLEnum wrapT = GLEnum.ClampToEdge,
            GLEnum minFilter = GLEnum.Linear, GLEnum magFilter = GLEnum.Linear)
        {
            FollowsWindowSize = false;
            Size = size;
            Format = format;
            WrapS = wrapS;
            WrapT = wrapT;
            MinFilter = minFilter;
            MagFilter = magFilter;

        }

        public static implicit operator uint(RenderTexture target)
        {
            return target.Handle;
        }
    }
}
