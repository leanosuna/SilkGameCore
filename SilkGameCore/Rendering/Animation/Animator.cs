using System.Numerics;

namespace SilkGameCore.Rendering.Animation
{
    public class Animator
    {
        public Matrix4x4[] FinalBoneMatrices { get; private set; }
        public Animation _animation;
        float _currentTime = 0;
        float _deltaTime;
        public Animator(Animation animation)
        {
            _animation = animation;
            FinalBoneMatrices = new Matrix4x4[Vertex.MAX_BONE_COUNT];

            for (int i = 0; i < Vertex.MAX_BONE_COUNT; i++)
                FinalBoneMatrices[i] = Matrix4x4.Identity;
        }
        public void UpdateAnimation(float deltaTime)
        {
            _deltaTime = deltaTime;
            if (_animation != null)
            {
                _currentTime += _animation.TicksPerSecond * deltaTime;
                _currentTime %= _animation.Duration;
                //_currentTime += deltaTime;
                //_currentTime %= 1;
                //Console.WriteLine("-----------------------");


                CalculateBoneTransform(_animation.RootNode, Matrix4x4.Identity);
            }
        }

        string[] allowList = ["mixamorig6:Spine", "mixamorig6:Spine1", "mixamorig6:Spine2", "mixamorig6:Neck"];
        void CalculateBoneTransform(AssimpNodeData node, Matrix4x4 parentTransform)
        {
            var name = node.Name;
            var nodeTransform = node.Transform;


            //if (_animation.BoneMap.TryGetValue(name, out var animBone))
            //{
            //    //if (animBone.Name == "mixamorig6:Hips")
            //    //{
            //    animBone.Update(nodeTransform, _currentTime);
            //    nodeTransform = Matrix4x4.Transpose(animBone.Transform);
            //    //nodeTransform = animBone.Transform;
            //    //}
            //}


            var globalTransform = parentTransform * nodeTransform;

            var map = _animation.BoneInfoMap;
            if (map.TryGetValue(name, out var boneinfo))
            {
                int index = boneinfo.ID;
                var offset = boneinfo.Offset;

                var mx = globalTransform * offset;

                FinalBoneMatrices[index] = Matrix4x4.Transpose(mx);

            }

            //foreach (var child in node.Children)
            //    CalculateBoneTransform(child, globalTransform);


            foreach (var child in node.Children)
                CalculateBoneTransform(child, globalTransform);

        }

        //public void DrawDebug(Gizmos.Gizmos gizmos)
        //{
        //    Console.WriteLine("---");
        //    DrawDebugRec(gizmos, _animation.RootNode, 0);
        //}
        //void DrawDebugRec(Gizmos.Gizmos gizmos, AssimpNodeData node, int level)
        //{
        //    var start = node.BonePosition;
        //    foreach (var child in node.Children)
        //    {
        //        var end = child.BonePosition;
        //        gizmos.AddLine(start, end, levelColors[level % 7]);
        //        gizmos.AddCube(end, new Vector3(0.02f), levelColors[level % 7]);
        //        Console.WriteLine($"l{level} {node.Name} - {start}->{end}");

        //        DrawDebugRec(gizmos, child, level + 1);
        //    }
        //}

        //public void DrawDebug(SilkGameGL game, Matrix4x4 vp)
        //{
        //    var gizmos = game.Gizmos;
        //    var gui = game.GUIManager;

        //    var bones = _animation.Bones;
        //    for (int i = 0; i < bones.Length; i++)
        //    {
        //        var position = bones[i].CurrentPosition;
        //        gizmos.AddCube(position, new Vector3(0.1f), Vector3.One);

        //        Vector4 clipSpacePos = Vector4.Transform(new Vector4(position, 1.0f), vp);
        //        Vector3 ndc = new Vector3(
        //            clipSpacePos.X / clipSpacePos.W,
        //            clipSpacePos.Y / clipSpacePos.W,
        //            clipSpacePos.Z / clipSpacePos.W
        //        );
        //        float screenX = (ndc.X + 1f) * 0.5f * game.WindowSize.X;
        //        float screenY = (1f - ndc.Y) * 0.5f * game.WindowSize.Y;

        //        var posv2 = new Vector2(screenX, screenY);
        //        gui.DrawText(bones[i].Name, posv2, Vector4.One, 10);

        //    }
        //}
        Matrix4x4 transformS()
        {
            var m = Matrix4x4.CreateTranslation(new Vector3(0, 1 * _currentTime, 0));
            return m;
        }

        Matrix4x4 transformS1()
        {
            var m = Matrix4x4.CreateTranslation(new Vector3(0, 1, 0));
            return m;
        }

        Matrix4x4 transformS2()
        {
            var q = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), MathF.Sin(_currentTime * MathF.PI * 2));
            var m = Matrix4x4.CreateFromQuaternion(q);
            return m;
        }


    }

}
