﻿using ImGuiNET;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using System.Numerics;

namespace SilkGameCore.Rendering.GUI
{
    public class GUIManager
    {
        private ImGuiController _controller;
        private SilkGameGL _game;
        private GL GL;
        private Dictionary<int, ImFontPtr> _fonts = new Dictionary<int, ImFontPtr>();
        private int _buttonId = 0;

        internal GUIManager(SilkGameGL game)
        {
            _game = game;
            GL = game.GL;

            var inputContext = game.InputManager.GetInputContext();
            _controller = new ImGuiController(GL, game.Window, inputContext);

            LoadDefaultFont();

        }
        public void LoadDefaultFont()
        {
            List<int> sizes = new List<int>();
            for (int i = 10; i <= 100; i += 5)
                sizes.Add(i);

            LoadFontTTF(EmbeddedHelper.ExtractPath("CascadiaMono.ttf", "Files.Fonts"), sizes.ToArray());
        }
        /// <summary>
        /// Loads a ttf font to ImGui, with the defined sizes
        /// </summary>
        /// <param name="path">The path to the font file</param>
        /// <param name="sizes">The font sizes to load</param>
        public unsafe void LoadFontTTF(string path, int[] sizes)
        {
            if (sizes.Length == 0)
                throw new Exception("must contain at least one font size");

            var io = ImGui.GetIO();

            _fonts.Clear();

            foreach (var size in sizes)
                _fonts[size] = io.Fonts.AddFontFromFileTTF(path, size);


            io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out int width, out int height, out int bytesPerPixel);

            uint fontTex;
            GL.GenTextures(1, out fontTex);
            GL.BindTexture(TextureTarget.Texture2D, fontTex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                          (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            io.Fonts.SetTexID((nint)fontTex);

            io.Fonts.ClearTexData();

            var first = _fonts.First();
            ImGui.PushFont(first.Value);

            SetFontSize(first.Key);
        }

        /// <summary>
        /// Selects a font size and pushes it to imgui
        /// </summary>
        /// <param name="size">n</param>
        public void SetFontSize(int size)
        {

            if (!_fonts.TryGetValue(size, out var font))
                throw new Exception($"font size {size} not found");

            ImGui.PushFont(font);
        }
        /// <summary>
        /// Draws text on the screen without any window or background
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="position">The position coordinates of the text to draw</param>
        /// <param name="color">The color of the text to draw</param>
        /// <param name="size">The font size of the text to draw</param>
        public void DrawText(string text, Vector2 position, Vector4 color, int size)
        {
            SetFontSize(size);
            var drawList = ImGui.GetForegroundDrawList();
            drawList.AddText(position, ImGui.ColorConvertFloat4ToU32(color), text);
        }

        /// <summary>
        /// Draws text on the screen without any window or background, centered on a position
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="position">The position coordinates of the text to draw</param>
        /// <param name="color">The color of the text to draw</param>
        /// <param name="size">The font size of the text to draw</param>
        public void DrawCenteredText(string text, Vector2 position, Vector4 color, int size)
        {
            SetFontSize(size);
            var drawList = ImGui.GetForegroundDrawList();
            var textSize = ImGui.CalcTextSize(text);

            drawList.AddText(position - textSize, ImGui.ColorConvertFloat4ToU32(color), text);
        }


        public void DrawImg(string name, string path, Vector2 position, Vector2 size)
        {
            DrawImg(name, path, position, size, Vector2.Zero, Vector2.One);
        }
        public void DrawImg(string name, string path, Vector2 position, Vector2 size, Vector2 uvMin, Vector2 uvMax)
        {
            uint texID = _game.TextureManager.GetOrLoadTexture(name, path);

            var drawList = ImGui.GetForegroundDrawList();
            drawList.AddImage((nint)texID, position, position + size, uvMin, uvMax);
        }

        public void DrawImgUnsafe(string name, Vector2 position, Vector2 size)
        {
            DrawImgUnsafe(name, position, size, Vector2.Zero, Vector2.One);
        }
        public void DrawImgUnsafe(string name, Vector2 position, Vector2 size, Vector2 uvMin, Vector2 uvMax)
        {
            uint? tex = _game.TextureManager.GetTexture(name);
            if (!tex.HasValue)
            {
                throw new Exception($"texture {name} not found");
            }
            var drawList = ImGui.GetForegroundDrawList();
            drawList.AddImage((nint)tex.Value, position, position + size, uvMin, uvMax);
        }

        public void DrawSimpleButton(string name, Vector2 position, Vector2 size, Action action)
        {
            var guiName = $"btn_{_buttonId}";
            //Console.WriteLine(guiName);
            ImGui.SetNextWindowPos(position);

            ImGui.Begin(guiName, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse);
            if (ImGui.Button(name))
                action.Invoke();
            ImGui.End();

            _buttonId++;
        }
        public void Update(double delta)
        {
            _buttonId = 0;
            _controller.Update((float)delta);
            //ImGui.NewFrame();
        }
        public void Render()
        {
            _controller.Render();

        }

    }
}
