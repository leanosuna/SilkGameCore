using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SilkGameCore.Input;
using SilkGameCore.Rendering;
using SilkGameCore.Rendering.Gizmos;
using SilkGameCore.Rendering.GUI;
using SilkGameCore.Rendering.RT;
using SilkGameCore.Rendering.Textures;
using SilkGameCore.Sound;

namespace SilkGameCore
{
    public abstract class SilkGameGL
    {
        public GL GL { get; private set; }
        public IWindow Window { get; private set; }
        public Vector2D<int> WindowSize { get; private set; }
        public InputManager InputManager { get; private set; }
        public FullScreenQuad FullScreenQuad { get; private set; }
        public RTManager RTManager { get; private set; }
        public TextureManager TextureManager { get; private set; }
        public Gizmos Gizmos { get; private set; }
        public GUIManager GUIManager { get; private set; }
        public SoundManager SoundManager { get; private set; }
        public SilkGameGL()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1600, 900);
            options.Title = "Silk.NET OPENGL Game";
            options.VSync = true;
            var glApi = new APIVersion(4, 1);
            options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, glApi);
            //GLShader.APIVersion = glApi;

            Window = Silk.NET.Windowing.Window.Create(options);
            WindowSize = Window.Size;

            Window.Load += InternalLoad;
            Window.Update += InternalUpdate;
            Window.Render += InternalRender;
            Window.FramebufferResize += InternalFramebufferResize;
            Window.Closing += InternalOnClose;

        }
        public SilkGameGL(WindowOptions options)
        {
            Window = Silk.NET.Windowing.Window.Create(options);
            //GLShader.APIVersion = options.API.Version;

            WindowSize = Window.Size;

            Window.Load += InternalLoad;
            Window.Update += InternalUpdate;
            Window.Render += InternalRender;
            Window.FramebufferResize += InternalFramebufferResize;
            Window.Closing += InternalOnClose;

        }
        /// <summary>
        /// Run the game (thread gets locked until game window is closed)
        /// </summary>
        public void Run()
        {
            Window.Run();
            //thread blocked here until the window is closed.
            Window.Dispose();
        }
        /// <summary>
        /// Stop the game window
        /// </summary>
        public void Stop()
        {
            Window.Close();
        }
        /// <summary>
        /// This method gets called after GL Window and internal initialization
        /// </summary>
        protected abstract void Initialize();
        /// <summary>
        /// This method gets called every frame. Game logic should go here.
        /// </summary>
        /// <param name="deltaTime">Time in seconds since the last Update() call</param>
        protected abstract void Update(double deltaTime);
        /// <summary>
        /// This method gets called every frame. Game rendering should go here.
        /// </summary>
        /// <param name="deltaTime">Time in seconds since the last Render() call</param>
        protected abstract void Render(double deltaTime);

        /// <summary>
        /// This method gets called every time the window gets resized.
        /// </summary>
        /// <param name="windowSize">The new window size</param>
        protected abstract void OnWindowResize(Vector2D<int> windowSize);

        /// <summary>
        /// This method gets called when the game window is closed
        /// </summary>
        protected abstract void OnClose();

        private void InternalLoad()
        {
            Window.Center();
            GL = GL.GetApi(Window);
            InputManager = new InputManager(this);
            FullScreenQuad = new FullScreenQuad(this);
            TextureManager = new TextureManager(this);
            RTManager = new RTManager(this);
            GUIManager = new GUIManager(this);

            Gizmos = new Gizmos(GL);
            Initialize();
            SoundManager = new SoundManager();
        }
        private void InternalUpdate(double deltaTime)
        {
            InputManager.Update();
            Update(deltaTime);
            GUIManager.Update(deltaTime);
        }
        private void InternalRender(double deltaTime)
        {
            Render(deltaTime);
            GUIManager.Render();
        }
        private void InternalFramebufferResize(Vector2D<int> size)
        {
            WindowSize = size;
            GL.Viewport(size);
            OnWindowResize(size);
        }
        private void InternalOnClose()
        {
            OnClose();
        }


    }
}
