using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using System.Numerics;

namespace SilkGameCore.Rendering.Animation
{
    public class AnimatedModel : IDisposable
    {
        public List<ModelPart> Parts { get; private set; }

        private readonly GL GL;
        private Model _model;
        private Assimp _assimp;

        Dictionary<string, Animator> _animators = new Dictionary<string, Animator>();
        Animator _selectedAnimator;

        public Animator GetCurrentAnimator()
        {
            return _selectedAnimator;
        }
        public AnimatedModel(GL gl, string modelPath, string animationsPath, string[] animationNames)
        {
            if (animationNames.Count() == 0)
                throw new Exception("At least one animation is required");

            GL = gl;
            _assimp = Assimp.GetApi();
            _model = new Model(gl: GL, path: modelPath,
            meshAttributes:
            MeshAttributes.Position3D
            | MeshAttributes.TexCoord
            | MeshAttributes.Normals
            | MeshAttributes.boneIds
            | MeshAttributes.boneWeights
            );

            Parts = _model.Parts;
            foreach (var anim in animationNames)
            {
                var _animation = new Animation(animationsPath + anim, _model);

                if (!_animators.TryAdd(anim, new Animator(_animation)))
                    throw new Exception("Animation must be unique");
            }

            _selectedAnimator = _animators.First().Value;
        }
        public Matrix4x4[] GetAnimationBoneMatrices()
        {
            return _selectedAnimator.FinalBoneMatrices;
        }
        public void SelectAnimation(string name)
        {
            if (!_animators.TryGetValue(name, out var anim))
            {
                throw new Exception($"Animation {name} not found");
            }
            _selectedAnimator = anim;

        }
        public void Draw()
        {
            _model.Draw();
        }
        public void Update(float deltaTime)
        {
            _selectedAnimator.UpdateAnimation(deltaTime);
        }
        public void Dispose()
        {
            _model.Dispose();
        }
    }
}
