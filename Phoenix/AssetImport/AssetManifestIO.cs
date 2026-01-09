using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Phoenix.AssetImport
{
    public static class AssetManifestIO
    {
        public static AssetManifest Load(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Asset manifest not found at {path}");
            }
            
            return JsonSerializer.Deserialize<AssetManifest>(
                File.ReadAllText(path)
            )!;
        }

        public static void Save(string path, AssetManifest manifest)
        {
            File.WriteAllText(
                path,
                JsonSerializer.Serialize(manifest, new JsonSerializerOptions
                {
                    WriteIndented = true
                })
            );
        }
    }

}
