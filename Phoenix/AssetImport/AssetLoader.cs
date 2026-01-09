using Phoenix.AssetImport.Model;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.AssetImport
{
    public static class AssetLoader
    {
        public const string ManifestDefaultPath = "Content/asset-manifest.json";

        private static readonly Dictionary<string, BinaryModel> _loadedModels = new();
        private static AssetManifest _assetManifest = default!;
        internal static GL GL;
        public static void Init(GL gl, string path = ManifestDefaultPath)
        {
            GL=gl;
            var manifestPath = Path.Combine(AppContext.BaseDirectory, path);
            _assetManifest = AssetManifestIO.Load(manifestPath);
        }

        public static BinaryModel LoadModel(string name)
        {
            // Normalize input
            var relativeBin = Path.HasExtension(name)
                ? Path.ChangeExtension(name, ".bin")
                : name + ".bin";

            // Normalize separators
            relativeBin = relativeBin.Replace('\\', '/');
            //relativeBin = relativeBin.Replace('/', '\\');

            // Find asset by RELATIVE output path
            var asset = _assetManifest.Assets
                .Find(a => a.OutputFilePath
                    .EndsWith(relativeBin, StringComparison.OrdinalIgnoreCase));

            if (asset == null)
                throw new Exception($"Asset '{name}' not found in manifest");

            // Resolve runtime path
            var fullPath = Path.Combine(
                AppContext.BaseDirectory,
                "Content",
                asset.OutputFilePath
            ).Replace('\\', '/');

            if (!_loadedModels.TryGetValue(fullPath, out var model))
            {
                model = BinaryModelReader.Load(fullPath);
                _loadedModels[fullPath] = model;
            }

            return model;
        }
    }

}
