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

        private int _boneCount;
        private Matrix4x4[] _blendedTransform;
        public Animator(AnimatedModel model)
        {
            _animModel = model;
            _boneCount = model.GetBaseModel().BoneInfoMap.Count;
            _blendedTransform = new Matrix4x4[_boneCount];
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
        public void BlendBetween(string a1, string a2, float ammount, float deltaTime)
        {
            if (!_animationMap.TryGetValue(a1, out var anim1))
                return;
            if (!_animationMap.TryGetValue(a2, out var anim2))
                return;

            BlendBetween(anim1, anim2, ammount, deltaTime);
        }

        public void BlendBetween(Animation a1, Animation a2, float ammount, float deltaTime)
        {
            a1.UpdateFrameSRT(deltaTime);
            a2.UpdateFrameSRT(deltaTime);

            for (var i = 0; i < _boneCount; i++)
            {
                var blend = a1.CurrentFrame[i].Interpolate(a2.CurrentFrame[i], ammount);
                _blendedTransform[i] = blend.AsMatrix();
            }


            var nodes = _animModel.AnimatorNodes;
            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];

                var pid = node.ParentID;
                var parentTransform = pid != -1 ? nodes[pid].Transform : Matrix4x4.Identity;

                Matrix4x4 localTransform;
                if (node.IsBone)
                {
                    var animTransform = _blendedTransform[node.ModelBoneID];

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
