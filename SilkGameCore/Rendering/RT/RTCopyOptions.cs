using Silk.NET.Maths;

namespace SilkGameCore.Rendering.RT
{
    public class RTCopyOptions
    {
        public string Name;
        public string FramebufferName;
        public Vector4D<int> Corners;

        /// <summary>
        /// Describes the options for one of the render targets  involved in a copy operation
        /// </summary>
        /// <param name="name">The name of the render target</param>
        /// <param name="position">The position to copy from / to</param>
        /// <param name="size">The portion of the target to copy from / to</param>
        public RTCopyOptions(string name, Vector2D<int> position, Vector2D<int> size)
        {
            Name = name.ToLower();
            FramebufferName = RTManager.DefaultColorName;
            Corners = new Vector4D<int>(position.X, position.Y, position.X + size.X, position.Y + size.Y);
        }
        /// <summary>
        /// Describes the options for one of the render targets  involved in a copy operation
        /// </summary>
        /// <param name="name">The name of the render target</param>
        /// <param name="corners">the 4 points defining the copy rectangle </param>
        public RTCopyOptions(string name, Vector4D<int> corners)
        {
            Name = name.ToLower();
            FramebufferName = RTManager.DefaultColorName;
            Corners = corners;
        }
        /// <summary>
        /// Describes the options for one of the render targets  involved in a copy operation
        /// </summary>
        /// <param name="name">The name of the render target</param>
        /// <param name="frameBufferName">The name of the render target's framebuffer to copy</param>
        /// <param name="position">The position to copy from / to</param>
        /// <param name="size">The portion of the target to copy from / to</param>
        public RTCopyOptions(string name, string frameBufferName, Vector2D<int> position, Vector2D<int> size)
        {
            Name = name.ToLower();
            FramebufferName = frameBufferName.ToLower();
            Corners = new Vector4D<int>(position.X, position.Y, position.X + size.X, position.Y + size.Y);
        }
        /// <summary>
        /// Describes the options for one of the render targets  involved in a copy operation
        /// </summary>
        /// <param name="name">The name of the render target</param>
        /// <param name="frameBufferName">The name of the render target's framebuffer to copy</param>
        /// <param name="corners">the 4 points defining the copy rectangle </param>
        public RTCopyOptions(string name, string frameBufferName, Vector4D<int> corners)
        {
            Name = name.ToLower();
            FramebufferName = frameBufferName.ToLower();
            Corners = corners;
        }
    }
}
