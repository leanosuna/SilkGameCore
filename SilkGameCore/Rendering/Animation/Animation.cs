using Silk.NET.Assimp;
using System.Numerics;

namespace SilkGameCore.Rendering.Animation
{
    public unsafe class Animation
    {
        public string Name { get; private set; }
        public float Duration { get; private set; }
        public float TicksPerSecond { get; private set; }
        public float CurrentTime { get; private set; }
        public Matrix4x4[] Transforms { get; private set; }

        private Keyframe[][] _keyframes;

        private int _boneCount;

        public unsafe Animation(string name, Scene* scene, Model model)
        {
            Name = name;
            var assAnimation = scene->MAnimations[0];
            Duration = (float)assAnimation->MDuration;
            TicksPerSecond = (float)assAnimation->MTicksPerSecond;
            if (TicksPerSecond <= 0)
                TicksPerSecond = 25.0f;


            _keyframes = ReadKeyFrames(assAnimation, model);

            _boneCount = _keyframes.GetLength(0);

            Transforms = new Matrix4x4[Vertex.MAX_BONE_COUNT];

            for (int b = 0; b < Vertex.MAX_BONE_COUNT; b++)
            {
                Transforms[b] = Matrix4x4.Identity;
            }
        }

        unsafe Keyframe[][] ReadKeyFrames(Silk.NET.Assimp.Animation* anim, Model model)
        {
            var boneCount = model.BoneInfoMap.Count;
            var keyframes = new List<Keyframe>[boneCount];
            for (int i = 0; i < boneCount; i++)
                keyframes[i] = new List<Keyframe>();

            for (int c = 0; c < anim->MNumChannels; c++)
            {
                var channel = anim->MChannels[c];
                var nodeName = channel->MNodeName;

                if (!model.BoneInfoMap.TryGetValue(nodeName, out var info))
                    continue;

                int maxKeys = (int)Math.Max(Math.Max(channel->MNumPositionKeys, channel->MNumRotationKeys), channel->MNumScalingKeys);
                for (int k = 0; k < maxKeys; k++)
                {
                    var t = (float)(
                        (k < channel->MNumPositionKeys) ? channel->MPositionKeys[k].MTime :
                        (k < channel->MNumRotationKeys) ? channel->MRotationKeys[k].MTime :
                        channel->MScalingKeys[k].MTime
                    );

                    var pos = (k < channel->MNumPositionKeys) ? channel->MPositionKeys[k].MValue : channel->MPositionKeys[channel->MNumPositionKeys - 1].MValue;
                    var rot = (k < channel->MNumRotationKeys) ? channel->MRotationKeys[k].MValue : channel->MRotationKeys[channel->MNumRotationKeys - 1].MValue;
                    var scl = (k < channel->MNumScalingKeys) ? channel->MScalingKeys[k].MValue : channel->MScalingKeys[channel->MNumScalingKeys - 1].MValue;

                    keyframes[info.ID].Add(new Keyframe((float)t, pos, rot, scl));
                }
            }

            var result = new Keyframe[boneCount][];
            for (int i = 0; i < boneCount; i++)
                result[i] = keyframes[i].ToArray();
            return result;
        }
        public void Reset()
        {
            CurrentTime = 0.0f;
        }
        public void Update(float deltaTime)
        {
            CurrentTime += TicksPerSecond * deltaTime;
            CurrentTime %= Duration;

            for (int b = 0; b < _boneCount; b++)
            {
                var keys = _keyframes[b];
                if (keys.Length == 0)
                {
                    Transforms[b] = Matrix4x4.Identity;
                    continue;
                }

                var i0 = GetStartingIndex(CurrentTime, keys);
                var i1 = Math.Min(i0 + 1, keys.Length - 1);

                var (s, r, p) = Interpolate(keys[i0], keys[i1], CurrentTime);

                var M = Matrix4x4.CreateScale(s)
                      * Matrix4x4.CreateFromQuaternion(r)
                      * Matrix4x4.CreateTranslation(p);

                Transforms[b] = M;
            }
        }
        private int GetStartingIndex(float time, Keyframe[] keys)
        {
            if (keys.Length == 0) return 0;
            for (int index = 0; index < keys.Length - 1; index++)
            {
                if (time < keys[index + 1].TimeStamp)
                    return index;
            }
            return Math.Max(0, keys.Length - 2);
        }

        private (Vector3 scale, Quaternion rotation, Vector3 position)
            Interpolate(Keyframe current, Keyframe next, float time)
        {
            var lerpFactor = 0.0f;
            var midWayLength = time - current.TimeStamp;
            var framesDiff = next.TimeStamp - current.TimeStamp;
            lerpFactor = midWayLength / framesDiff;

            var scale = Vector3.Lerp(current.Scale, next.Scale, lerpFactor);
            var rotation = Quaternion.Slerp(current.Rotation, next.Rotation, lerpFactor);
            var position = Vector3.Lerp(current.Position, next.Position, lerpFactor);
            return (scale, rotation, position);
        }

    }

    public class Keyframe
    {
        public float TimeStamp { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; private set; }
        public Vector3 Scale { get; }

        public Keyframe(float timeStamp, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            TimeStamp = timeStamp;
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }
}
