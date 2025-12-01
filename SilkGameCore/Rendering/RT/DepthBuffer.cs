using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkGameCore.Rendering.RT
{
    public class DepthBuffer
    {
        public uint Handle { get; internal set; }

        public Vector2 Size { get; internal set; }
        public uint Width => (uint)Size.X;
        public uint Height => (uint)Size.Y;
        public GLEnum Format { get; private set; }

        public bool FollowsWindowSize { get; private set; }
        public DepthBuffer(Vector2 size, GLEnum format = GLEnum.Depth24Stencil8)
        {
            Size = size;
            Format = format;
            FollowsWindowSize = false;
        }

        public DepthBuffer(GLEnum format = GLEnum.Depth24Stencil8)
        {
            FollowsWindowSize = true;
            Size = Vector2.Zero;
            Format = format;
        }
    }
}
