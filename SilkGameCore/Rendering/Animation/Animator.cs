using System.Numerics;

namespace SilkGameCore.Rendering.Animation
{
    public class Animator
    {
        public Matrix4x4[] FinalBoneMatrices { get; set; }

        public Animation CurrentAnimation { get; private set; }

        private AnimatedModel _animModel;
        Dictionary<string, Animation> _animationMap = new Dictionary<string, Animation>();
        public Matrix4x4 InverseGlobalTransform { get; set; } = default!;

        public Animator(AnimatedModel model)
        {
            _animModel = model;
            FinalBoneMatrices = new Matrix4x4[Vertex.MAX_BONE_COUNT];

            for (int i = 0; i < Vertex.MAX_BONE_COUNT; i++)
                FinalBoneMatrices[i] = Matrix4x4.Identity;
        }

        public void Update(float deltaTime)
        {
            CurrentAnimation.Update(deltaTime);
            var nodes = _animModel.AnimatorNodes;
            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];

                var pid = node.ParentID;
                var parentTransform = pid != -1 ? nodes[pid].Transform : Matrix4x4.Identity;

                Matrix4x4 localTransform;
                if (node.IsBone)
                {
                    var animTransform = CurrentAnimation.Transforms[node.ModelBoneID];

                    animTransform.Transpose();

                    localTransform = animTransform;
                }
                else
                    localTransform = node.BindTransform;

                node.Transform = parentTransform * localTransform;

                if (node.IsBone)
                {
                    var final = InverseGlobalTransform * node.Transform * node.Offset;
                    final.Transpose();
                    FinalBoneMatrices[node.ModelBoneID] = final;
                }
            }
        }

        Vector3 SkinVertexCPU(Vertex v)
        {
            var pos = v.Position;
            int[] boneIDs = [(int)v.BoneIds.X, (int)v.BoneIds.Y, (int)v.BoneIds.Z, (int)v.BoneIds.W];
            float[] weights = [v.Weights.X, v.Weights.Y, v.Weights.Z, v.Weights.W];
            //Vector3 skinned = Vector3.Zero;
            Matrix4x4 boneTransform = new Matrix4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            for (int i = 0; i < boneIDs.Length; i++)
            {
                var boneID = boneIDs[i];
                //if (boneID >= 0)
                //{ 
                //    var m = FinalBoneMatrices[boneID];
                //    var transformed = Vector3.Transform(pos, m); // beware your row/column convention here
                //    skinned += transformed * weights[i];
                //}

                if (boneID >= 0)
                    boneTransform += FinalBoneMatrices[boneID] * weights[i];

            }
            return Vector3.Transform(pos, boneTransform);
            //return skinned;
        }

        public void AddAnimation(Animation animation)
        {
            if (!_animationMap.TryAdd(animation.Name, animation))
                throw new Exception("Animations must be unique");
        }
        public void SelectAnimation(string name, bool fromStart = true)
        {
            if (!_animationMap.TryGetValue(name, out var anim))
            {
                throw new Exception($"Animation {name} not found");
            }
            CurrentAnimation = anim;
            if (fromStart)
                CurrentAnimation.Reset();
        }


    }


}
