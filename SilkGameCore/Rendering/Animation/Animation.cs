using Silk.NET.Assimp;
using System.Numerics;

namespace SilkGameCore.Rendering.Animation
{
    public unsafe class Animation
    {
        public float Duration { get; private set; }
        public float TicksPerSecond { get; private set; }

        public AssimpNodeData RootNode { get; private set; }

        public Dictionary<string, BoneInfo> BoneInfoMap { get; private set; }
        public Dictionary<string, Bone> BoneMap { get; private set; } = new Dictionary<string, Bone>();

        public unsafe Animation(string path, Model model)
        {
            var assimp = Assimp.GetApi();

            var scene = assimp.ImportFile(path, (uint)0);

            var sceneNull = scene == null;
            var sceneNullRootNode = scene == null;

            if (scene == null || scene->MRootNode == null)
            {
                var error = assimp.GetErrorStringS();
                throw new Exception(error);
            }
            var assAnimation = scene->MAnimations[0];
            Duration = (float)assAnimation->MDuration;
            TicksPerSecond = (float)assAnimation->MTicksPerSecond;

            var _assimpRoot = scene->MRootNode;

            RootNode = ReadHierarchy(_assimpRoot, Matrix4x4.Identity, 0);
            ReadMissingBones(assAnimation, model);

        }

        unsafe AssimpNodeData ReadHierarchy(Node* node, Matrix4x4 parent, int level)
        {
            var name = node->MName;
            var local = node->MTransformation;
            var transform = parent * local;

            var cc = node->MNumChildren;


            var children = new AssimpNodeData[cc];

            for (int i = 0; i < cc; i++)
            {
                children[i] = ReadHierarchy(node->MChildren[i], transform, level + 1);
            }
            var and = new AssimpNodeData(local, name, (int)cc, children);
            return and;
        }

        unsafe void ReadMissingBones(Silk.NET.Assimp.Animation* animation, Model model)
        {
            int size = (int)animation->MNumChannels;
            var boneInfoMap = model.BoneInfoMap;
            var count = model.BoneCount;

            var bones = new List<Bone>();
            for (int i = 0; i < size; i++)
            {
                var channel = animation->MChannels[i];
                var name = channel->MNodeName;


                if (!boneInfoMap.TryGetValue(name, out var boneInfo))
                {
                    boneInfo = new BoneInfo(count, Matrix4x4.Identity);
                    boneInfoMap.Add(name, boneInfo);
                    count++;

                    //Console.WriteLine($"Adding missing bone: {name} id={boneInfo.ID}");
                }

                BoneMap.Add(name, new Bone(name, boneInfo.ID, channel, boneInfo.Offset));
            }

            BoneInfoMap = boneInfoMap;
        }
    }


    public class AssimpNodeData
    {
        public Matrix4x4 Transform { get; set; }
        public string Name { get; set; }
        public int ChildrenCount { get; set; }
        public AssimpNodeData[] Children { get; set; }

        public bool TransformEnabled;
        public string[] TransformOrder = ["parent, child"];
        public bool TransposeEnabled;
        public AssimpNodeData(Matrix4x4 transform, string name, int childrenCount, AssimpNodeData[] children)
        {
            Transform = transform;
            Name = name;
            ChildrenCount = childrenCount;
            Children = children;
        }
    }
}
