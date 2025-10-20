using System.Numerics;

namespace SilkGameCore.Rendering.Animation
{
    public class AnimationNode
    {
        public string Name { get; private set; }
        public int ParentID { get; private set; }
        public Matrix4x4 BaseTransform { get; set; }

        public Matrix4x4 Transform { get; set; }
        public bool IsBone { get; private set; }
        public Matrix4x4 Offset { get; private set; }
        public int ModelBoneID { get; private set; }

        public bool TransformSet { get; set; }
        public AnimationNode(string name, int parentID, int modelBoneID, Matrix4x4 offset)
        {
            Name = name;
            ParentID = parentID;
            BaseTransform = Matrix4x4.Identity;
            Transform = Matrix4x4.Identity;
            IsBone = true;
            Offset = offset;
            ModelBoneID = modelBoneID;
        }

        public AnimationNode(string name, int parentID, Matrix4x4 baseTransform)
        {
            Name = name;
            ParentID = parentID;
            BaseTransform = baseTransform;
            Transform = Matrix4x4.Identity;
            IsBone = false;
            Offset = Matrix4x4.Identity;
            ModelBoneID = -1;
        }
    }
}
