using Phoenix.Rendering.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.AssetImport.Model
{
    public class BinaryModelPart
    {
        public string Name { get; private set; }
        public List<BinaryMesh> Meshes { get; private set; }

        public BinaryModelPart(string name, List<BinaryMesh> meshes)
        {
            Name = name;
            Meshes = meshes;
        }
    }
}
