using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.AssetImport
{
    public sealed class AssetManifest
    {
        public List<AssetEntry> Assets { get; set; } = new();
        public string BaseDirectory { get; set; } = Directory.GetCurrentDirectory();
        
    }
}
