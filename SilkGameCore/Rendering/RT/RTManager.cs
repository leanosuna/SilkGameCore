using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace SilkGameCore.Rendering.RT
{
    public class RTManager
    {
        private Dictionary<string, RenderTarget> _targets = new Dictionary<string, RenderTarget>();
        private GL GL;
        private Vector2D<int> _windowSize;
        internal const string DefaultColorName = "color";
        private RenderTarget _screen = new RenderTarget();
        internal RTManager(SilkGameGL game)
        {
            GL = game.GL;
            _windowSize = game.WindowSize;
            _screen.FrameBuffer = 0;
            _screen.Width = _windowSize.X;
            _screen.Height = _windowSize.Y;

        }
        /// <summary>
        /// Creates a new render target (must have an unique name)
        /// with only one color framebuffer named <see cref="DefaultColorName"/>
        /// </summary>
        /// <param name="name">The name of the render target</param>
        public void CreateRenderTarget(string name)
        {
            CreateRenderTarget(name, _windowSize, [DefaultColorName]);
        }
        /// <summary>
        /// Creates a new render target (must have an unique name)
        /// with as many framebuffer targets as names in targetNames
        /// </summary>
        /// <param name="name">The name of the render target</param>
        /// <param name="targetNames">Name of the color framebuffers</param>
        public void CreateRenderTarget(string name, string[] targetNames)
        {
            CreateRenderTarget(name, _windowSize, targetNames);
        }
        /// <summary>
        /// Creates a new render target (must have an unique name)
        /// with as many framebuffer targets as names in targetNames,
        /// of a custom size.
        /// </summary>
        /// <param name="name">The name of the render target</param>
        /// <param name="targetNames">Name of the color framebuffers</param>
        /// <param name="size">Size of the framebuffers</param>
        public unsafe void CreateRenderTarget(string name, Vector2D<int> size, string[] targetNames)
        {
            var n = name.ToLower();
            if (_targets.TryGetValue(n, out var existingRt))
            {
                throw new Exception($"Render target {n} already exists");
            }

            RenderTarget rt = new RenderTarget();
            rt.Width = size.X;
            rt.Height = size.Y;
            _targets[n] = rt;
            rt.ColorTargetCount = (uint)targetNames.Length;

            GL.GenFramebuffers(1, out rt.FrameBuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, rt.FrameBuffer);

            var buffers = new Dictionary<string, uint>();

            uint tcb;
            GLEnum[] attachments = new GLEnum[rt.ColorTargetCount];
            for (int i = 0; i < rt.ColorTargetCount; i++)
            {
                // create color texture (RGBA8)
                GL.GenTextures(1, out tcb);
                buffers.Add(targetNames[i], tcb);

                GL.BindTexture(TextureTarget.Texture2D, tcb);
                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    (int)InternalFormat.Rgba8,
                    (uint)size.X,
                    (uint)size.Y,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    null);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

                var att = FramebufferAttachment.ColorAttachment0 + i;
                var cAtt = GLEnum.ColorAttachment0 + i;
                attachments[i] = cAtt;
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                    att, TextureTarget.Texture2D, tcb, 0);

            }
            GL.DrawBuffers(rt.ColorTargetCount, attachments);

            rt.TextureColorBuffers = buffers;
            // create and attach a depth+stencil renderbuffer
            GL.GenRenderbuffers(1, out uint rboDepth);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
            GL.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth24Stencil8, (uint)size.X, (uint)size.Y);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rboDepth);

            // unbind and tidy
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

        }
        /// <summary>
        /// Selects a render target as GL output 
        /// (passing in "screen" or "" selects the game window)
        /// </summary>
        /// <param name="name">The name of the render target to bind</param>
        public void SetAsActive(string name)
        {
            var n = name.ToLower();
            if (n == "screen" || n == "")
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                return;
            }

            if (!_targets.TryGetValue(n, out var rt))
                throw new Exception($"target {n} not found.");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, rt.FrameBuffer);
        }
        /// <summary>
        /// Gets the GL Texture ID of a render target's colorbuffer, useful to set targets as 
        /// texture uniforms
        /// </summary>
        /// <param name="rtName">The render target</param>
        /// <param name="colorBufferName">The colorbuffer we want its texture id from</param>
        /// <returns>GL Texture ID</returns>
        public uint GetTargetTextureID(string rtName, string colorBufferName)
        {
            var rt = GetTarget(rtName);
            var cbn = colorBufferName.ToLower();

            if (!rt.TextureColorBuffers.TryGetValue(cbn, out var id))
                throw new Exception($"color buffer '{cbn}' not found.");


            return id;
        }
        /// <summary>
        /// Gets a render target object from its unique name
        /// </summary>
        /// <param name="name">The render target name</param>
        /// <returns>The render target object</returns>
        public RenderTarget GetTarget(string name)
        {
            if (name == "screen")
                return _screen;

            var n = name.ToLower();
            if (!_targets.TryGetValue(n, out var rt))
                throw new Exception($"target '{n}' not found.");

            return rt;
        }

        //TODO: this uses _windowSize, which could break using smaller than fullscreenrender targets 

        /// <summary>
        /// Copies the default color buffer of a render target to another.
        /// Must only be used with <see cref="DefaultColorName"/> buffers.
        /// </summary>
        /// <param name="from">Source render target name</param>
        /// <param name="to">Destination render target name</param>
        public void CopyToRenderTarget(string from, string to)
        {
            var src = GetTarget(from);
            var dest = GetTarget(to);

            CopyToRenderTarget(
                new RTCopyOptions(from, DefaultColorName, new Vector4D<int>(0, 0, _windowSize.X, _windowSize.Y)),
                new RTCopyOptions(to, DefaultColorName, new Vector4D<int>(0, 0, _windowSize.X, _windowSize.Y)));
        }

        /// <summary>
        /// Copies the default color buffer of a render target to another,
        /// using <see cref="RTCopyOptions"/> options
        /// </summary>
        /// <param name="from">Source render target name</param>
        /// <param name="to">Destination render target name</param>

        public void CopyToRenderTarget(RTCopyOptions from, RTCopyOptions to)
        {
            var src = GetTarget(from.Name);

            var sbn = from.FramebufferName;
            var readBuffer = GLEnum.False;
            for (int i = 0; i < src.TextureColorBuffers.Count; i++)
            {
                if (src.TextureColorBuffers.ElementAt(i).Key == sbn)
                {
                    readBuffer = GLEnum.ColorAttachment0 + i;
                    break;
                }
            }
            var dest = GetTarget(to.Name);
            if (readBuffer == GLEnum.False)
            {
                throw new Exception($"source color buffer '{sbn}' not found in target.");
            }

            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, src.FrameBuffer);
            GL.ReadBuffer(GLEnum.ColorAttachment0);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, dest.FrameBuffer);
            GL.DrawBuffer(GLEnum.Front);

            GL.BlitFramebuffer(from.Corners.X, from.Corners.Y, from.Corners.Z, from.Corners.W,
                to.Corners.X, to.Corners.Y, to.Corners.Z, to.Corners.W,
                ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);


        }
        /// <summary>
        /// Copies the default color buffer of a render target to another,
        /// using <see cref="RTCopyOptions"/> options
        /// </summary>
        /// <param name="copies">The pairs of <see cref="RTCopyOptions"/> to copy</param>
        public void CopyToRenderTarget((RTCopyOptions from, RTCopyOptions to)[] copies)
        {
            foreach (var c in copies)
            {
                var from = c.from;
                var to = c.to;
                var dest = GetTarget(to.Name);

                var src = GetTarget(from.Name);

                var sbn = from.FramebufferName;
                var readBuffer = GLEnum.False;
                for (int i = 0; i < src.TextureColorBuffers.Count; i++)
                {
                    if (src.TextureColorBuffers.ElementAt(i).Key == sbn)
                    {
                        readBuffer = GLEnum.ColorAttachment0 + i;
                        break;
                    }
                }
                if (readBuffer == GLEnum.False)
                {
                    throw new Exception($"source color buffer '{sbn}' not found in target.");
                }

                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, src.FrameBuffer);
                GL.ReadBuffer(readBuffer);
                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, dest.FrameBuffer);
                GL.DrawBuffer(GLEnum.Front);

                GL.BlitFramebuffer(from.Corners.X, from.Corners.Y, from.Corners.Z, from.Corners.W,
                    to.Corners.X, to.Corners.Y, to.Corners.Z, to.Corners.W,
                    ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

            }
        }
    }
}
