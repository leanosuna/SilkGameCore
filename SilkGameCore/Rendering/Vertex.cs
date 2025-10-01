using System.Numerics;
using System.Runtime.InteropServices;

namespace SilkGameCore.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector2 TexCoords;
        public Vector3 Bitangent;

        public const int MAX_BONE_INFLUENCE = 4;
        public const int MAX_BONE_COUNT = 60;

        public Vector4 BoneIds;
        public Vector4 Weights;
    }
}