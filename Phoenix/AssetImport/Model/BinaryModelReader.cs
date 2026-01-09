using Phoenix.Rendering;
using Phoenix.Rendering.Geometry;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Phoenix.AssetImport.Model
{
    internal static class BinaryModelReader
    {
        public static BinaryModel Load(string path)
        {
            //Console.WriteLine("reading...");
            using var fs = File.OpenRead(path);
            using var br = new BinaryReader(fs);

            var assetType = br.ReadString();
            var ver = br.ReadUInt32();
            var partsCount = br.ReadInt32();
            //Console.WriteLine($"ASSET TYPE {assetType}");
            //Console.WriteLine($"VER {ver}");
            //Console.WriteLine($"parts: {partsCount}");
            List<BinaryModelPart> parts = new List<BinaryModelPart>();
            for (int p = 0; p < partsCount; p++)
            {
                var partName = br.ReadString();
                var meshCount = br.ReadInt32();

                //Console.WriteLine($"name: {partName}");
                //Console.WriteLine($"meshes: {meshCount}");
                List<BinaryMesh> meshes = new List<BinaryMesh>();
                for (int m = 0; m < meshCount; m++)
                {
                    var meshName = br.ReadString();
                    var index = br.ReadInt32();
                    var transform = br.ReadStruct<Matrix4x4>();
                    var indicesLength = br.ReadInt32();
                    var indices = br.ReadArray<uint>(indicesLength);
                    var verticesLength = br.ReadInt32();
                    var vertices = br.ReadArray<Vertex>(verticesLength);


                    meshes.Add(new BinaryMesh(meshName, vertices, indices, transform));

                    //Console.WriteLine($"mName: {meshName}");
                    //Console.WriteLine($"mIndex {index}");
                    //Console.WriteLine($"indices {indicesLength}");
                    //Console.WriteLine($"vertices {verticesLength}");

                }

                parts.Add(new BinaryModelPart(partName, meshes));
            }
            return new BinaryModel(parts);
        }
    }
}
