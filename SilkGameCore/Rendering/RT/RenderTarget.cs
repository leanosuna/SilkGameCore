using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkGameCore.Rendering.RT
{
    public class RenderTarget
    {
        
        public uint FrameBuffer;
        public Dictionary<string, uint> TextureColorBuffers;
        public uint ColorTargetCount;
        public int Width;
        public int Height;
        public RenderTarget()
        {
        
        }
    }
}
