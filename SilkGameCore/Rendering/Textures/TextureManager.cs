using Silk.NET.OpenGL;

namespace SilkGameCore.Rendering.Textures
{
    public class TextureManager
    {
        Dictionary<string, GLTexture> _loadedTextures = new Dictionary<string, GLTexture>();

        GL GL;
        public TextureManager(SilkGameGL game)
        {
            GL = game.GL;
        }

        /// <summary>
        /// Gets a texture ID from the current GL loaded textures 
        /// </summary>
        /// <param name="name">The unique friendly name for this texture</param>
        /// <param name="path">The file path for this texture</param>
        /// <returns>The texture ID</returns>
        public uint? GetTexture(string name)
        {
            if (_loadedTextures.TryGetValue(name, out var GLTex))
            {
                return GLTex.GetHandle();
            }
            return null;
        }

        /// <summary>
        /// Gets a texture ID from the current GL loaded textures 
        /// (loads the texture into GL if not found)
        /// </summary>
        /// <param name="name">The unique friendly name for this texture</param>
        /// <param name="path">The file path for this texture</param>
        /// <returns>The texture ID</returns>
        public uint GetOrLoadTexture(string name, string path)
        {
            if (_loadedTextures.TryGetValue(name, out var GLTex))
            {
                return GLTex.GetHandle();
            }

            GLTex = new GLTexture(GL, path);
            _loadedTextures.Add(name, GLTex);

            return GLTex.GetHandle();
        }
    }
}
