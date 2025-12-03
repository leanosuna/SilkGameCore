using System.Numerics;

namespace Phoenix.Rendering.Animation
{
    public class BoneInfo
    {
        public int ID { get; set; }
        public Matrix4x4 Offset { get; set; }

        public BoneInfo(int id, Matrix4x4 offset)
        {
            ID = id;
            Offset = offset;
        }
    }
}
