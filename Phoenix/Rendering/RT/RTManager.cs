using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using Phoenix.Rendering.Textures;
using System.Globalization;
using System.Linq;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace Phoenix.Rendering.RT
{
    public class RTManager
    {
        private GL GL;
        private List<RenderTarget> _dynamicTargets = new List<RenderTarget>();
        private Dictionary<string, RenderTarget> _renderTargets = new Dictionary<string, RenderTarget>();
        private PhoenixGame _game;
        private int _rtGenNameCount = 0;
        internal RTManager(PhoenixGame game)
        {
            _game = game;
            GL = game.GL;
        }

        string GenName() 
        {
            var name = $"rt-{_rtGenNameCount}";
            _rtGenNameCount ++;
            return name;
        }
        public RenderTarget CreateRenderTarget(Vector2 size)
        {
            RenderTexture tex = new RenderTexture(size);
            return CreateRenderTarget(GenName(),[tex], null);
        }

        public RenderTarget CreateRenderTarget()
        {
            RenderTexture tex = new RenderTexture();
            return CreateRenderTarget(GenName(),[tex], null);
        }

        public RenderTarget CreateRenderTarget(string name, List<RenderTexture> targetTextures)
        {
            return CreateRenderTarget(name,targetTextures, null);
        }
        public RenderTarget CreateRenderTarget(string name, Vector2 size)
        {
            RenderTexture tex = new RenderTexture(size);
            return CreateRenderTarget(name, [tex], null);
        }

        public RenderTarget CreateRenderTarget(string name)
        {
            RenderTexture tex = new RenderTexture();
            return CreateRenderTarget(name, [tex], null);
        }

        public RenderTarget CreateRenderTarget(List<RenderTexture> targetTextures)
        {
            return CreateRenderTarget(GenName(), targetTextures, null);
        }

        public unsafe RenderTarget CreateRenderTarget(string name, List<RenderTexture> targetTextures, DepthBuffer? depthBuffer)
        {
            if (_renderTargets.ContainsKey(name))
            {
                throw new Exception($"targets cant have the same name {name}");
            }

            GL.GenFramebuffers(1, out uint rtFrameBuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, rtFrameBuffer);

            var targetCount = (uint)targetTextures.Count;

            CreateRenderTextures(targetTextures);

            if(depthBuffer != null)
            {
                if (depthBuffer.FollowsWindowSize)
                    depthBuffer.Size = _game.WindowSize;

                GL.GenRenderbuffers(1, out uint rboDepth);
                depthBuffer.Handle = rboDepth;
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
                GL.RenderbufferStorage(GLEnum.Renderbuffer, depthBuffer.Format, depthBuffer.Width, depthBuffer.Height);

                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rboDepth);

            }

            // unbind and tidy
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            var rt = new RenderTarget(GL, name, rtFrameBuffer, targetTextures.ToArray());
            
            rt.DepthBuffer = depthBuffer;
            _renderTargets.Add(name.ToLower(CultureInfo.InvariantCulture), rt);

            if (targetTextures.Any(t => t.FollowsWindowSize))
                _dynamicTargets.Add(rt);


            return rt;
        }

        public RenderTarget FindByName(string name)
        {
            if (!_renderTargets.TryGetValue(name.ToLower(CultureInfo.InvariantCulture), out var rt))
                throw new Exception($"RT {name} not found");    
            return rt;
        }
        
        private unsafe void CreateRenderTextures(List<RenderTexture> targetTextures)
        {
            uint targetCount = (uint)targetTextures.Count();
            
            GLEnum[] attachments = new GLEnum[targetCount];
            for (int i = 0; i < targetCount; i++)
            {
                var tex = targetTextures[i];

                if (tex.FollowsWindowSize)
                    tex.Size = _game.WindowSize;

                tex.texture = new GLTexture(GL, 
                    tex.Width, 
                    tex.Height, 
                    out var handle, 
                    tex.Format, 
                    tex.WrapS, 
                    tex.WrapT, 
                    tex.MinFilter, 
                    tex.MagFilter, 
                    false, 0, 0);

                tex.Handle = handle;
                
                var att = FramebufferAttachment.ColorAttachment0 + i;
                var cAtt = GLEnum.ColorAttachment0 + i;
                attachments[i] = cAtt;
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                    att, TextureTarget.Texture2D, handle, 0);

            }
            GL.DrawBuffers(targetCount, attachments);
        }

        /// <summary>
        /// Selects a render target as GL output 
        /// </summary>
        /// <param name="name">The name of the render target to bind</param>
        public void RenderTo(RenderTarget target)
        {
            if(!target.IsBound)
                throw new Exception($"target not bound (its handle is uint.MaxValue)");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, target.FrameBuffer);
        }
        /// <summary>
        /// Selects the screen as GL output 
        /// </summary>
        /// <param name="name">The name of the render target to bind</param>
        public void RenderToScreen()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }

        public unsafe void HandleWindowResize()
        {
            foreach(var rt in _dynamicTargets)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, rt.FrameBuffer);

                foreach (var tex in rt.RenderTextures)
                {
                    if(tex.FollowsWindowSize)
                    {
                        tex.Size = _game.WindowSize * tex.SizeMultiplier;

                        GL.BindTexture(TextureTarget.Texture2D, tex.Handle);

                        tex.texture.Size = tex.Size;

                        GL.TexImage2D(
                            TextureTarget.Texture2D,
                            0,
                            (int)tex.Format,
                            tex.Width,
                            tex.Height,
                            0,
                            PixelFormat.Rgba,
                            PixelType.UnsignedByte,
                            null);

                    }

                }
                
                var db = rt.DepthBuffer;
                if (db is not null)
                {
                    if(db.FollowsWindowSize)
                    {
                        db.Size = _game.WindowSize;

                        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, db.Handle);
                        GL.RenderbufferStorage(
                            RenderbufferTarget.Renderbuffer, 
                            db.Format,
                            db.Width, 
                            db.Height);

                        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
                    }
                }
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}
