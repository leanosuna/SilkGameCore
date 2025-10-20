namespace SilkGameCore.Rendering.RT
{
    public class RenderTarget
    {

        public uint FrameBuffer;
        public Dictionary<string, uint> TextureColorBuffers = default!;
        public uint ColorTargetCount;
        public int Width;
        public int Height;
        public RenderTarget()
        {

        }
    }
}
