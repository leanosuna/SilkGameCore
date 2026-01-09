using Phoenix.AssetImport.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.AssetImport
{
    public sealed class AssetLoadPipeline
    {
        public void Load(AssetManifest manifest)
        {
            Parallel.ForEach(manifest.Assets, (item) => 
            {
                var paths = GetOutputPath(manifest.BaseDirectory, item.Path);
                
                switch (item.Type)
                {
                    case AssetType.Model: ReadModel(item, paths); break;
                    case AssetType.Texture: ReadTexture(item, paths); break;
                    case AssetType.Shader: ReadShader(item, paths); break;

                }
            });
        }

        private void ReadShader(AssetEntry asset, (string before, string outputDir, string after) boa)
        {
            Console.WriteLine($"> {boa.before} > {boa.outputDir}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"OK {boa.before}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void ReadTexture(AssetEntry asset, (string before, string outputDir, string after) boa)
        {
            Console.WriteLine($"> {boa.before} > {boa.outputDir}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"OK {boa.before}");
            Console.ForegroundColor = ConsoleColor.White;

        }

        private void ReadModel(AssetEntry asset, (string before, string outputDir, string after) boa)
        {
            Console.WriteLine($"> {boa.before} > {boa.outputDir}");

            //BinaryModelReader.Load(asset);


            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine($"OK {boa.before}");
            //Console.ForegroundColor = ConsoleColor.White;

        }

        public (string,string,string) GetOutputPath(string root, string assetPath)
        {
            //C:\dev\vs\PhoenixDev\assetToolTest\Content\
            //C:\dev\vs\PhoenixDev\assetToolTest\Content\m16\m16.fbx
            var len = root.Length + 1;
            var assetLen = assetPath.Length - len;

            //var baseDir = assetPath.Take(len).ToString();
            //var assetDir = assetPath.TakeLast(assetLen - len).ToString();

            var pathBefore = string.Concat(assetPath.TakeLast(assetLen));

            var bin = "ContentBin\\";
            var pathOutput = assetPath.Insert(len, bin);

            var pathAfter = string.Concat(pathOutput.TakeLast(assetLen + bin.Length));

            //assetPath.TakeLast(len)


            var relative = Path.GetRelativePath(root, assetPath);
            
            var buildPath = Path.Combine(root, "ContentBin", relative);

            var buildPathWithExtension = Path.ChangeExtension(buildPath, ".bin");


            return (pathBefore, buildPathWithExtension, pathAfter);
        }
    }

}
