using Silk.NET.Assimp;
using System.Numerics;

namespace SilkGameCore.Rendering.Animation
{
    public class Bone
    {
        public Matrix4x4 Transform { get; private set; } = Matrix4x4.Identity;
        public bool TransformIsIdentity { get; private set; }
        public string Name { get; private set; }

        public int ID { get; private set; }
        uint numPositions, numRotations, numScalings;

        KeyPosition[] Positions;
        KeyRotation[] Rotations;
        KeyScale[] Scalings;

        public Vector3 CurrentPosition;
        public Quaternion CurrentRotation;
        public Vector3 CurrentScale;
        public Matrix4x4 Offset;
        public unsafe Bone(string name, int id, NodeAnim* channel, Matrix4x4 offset)
        {
            //Console.WriteLine("Creating bone: " + name);

            Offset = Matrix4x4.Transpose(offset);
            Name = name;
            ID = id;

            numPositions = channel->MNumPositionKeys;
            Positions = new KeyPosition[numPositions];
            for (int positionIndex = 0; positionIndex < numPositions; positionIndex++)
            {
                var position = channel->MPositionKeys[positionIndex].MValue;
                float timeStamp = (float)channel->MPositionKeys[positionIndex].MTime;
                KeyPosition data = new KeyPosition(position, timeStamp);
                Positions[positionIndex] = data;
            }
            //Vector3 pos;
            //float timestamp;
            //foreach (var p in Positions)
            //{
            //    Console.WriteLine($"{p.Position}");
            //}

            //Console.WriteLine($"distinct pos {Positions.Distinct().Count()}");

            Console.WriteLine($"bone {name}");
            numRotations = channel->MNumRotationKeys;
            Rotations = new KeyRotation[numRotations];
            for (int rotationIndex = 0; rotationIndex < numRotations; rotationIndex++)
            {
                var rotation = channel->MRotationKeys[rotationIndex].MValue;
                float timeStamp = (float)channel->MRotationKeys[rotationIndex].MTime;
                KeyRotation data = new KeyRotation(rotation, timeStamp);

                Console.WriteLine($"({rotation.X},{rotation.Y},{rotation.Z},{rotation.W}) {timeStamp}");
                Rotations[rotationIndex] = data;
            }
            //Console.WriteLine($"distinct rot {Rotations.Distinct().Count()}");

            numScalings = channel->MNumScalingKeys;
            Scalings = new KeyScale[numScalings];
            for (int scalingIndex = 0; scalingIndex < numScalings; scalingIndex++)
            {
                var scale = channel->MScalingKeys[scalingIndex].MValue;
                float timeStamp = (float)channel->MScalingKeys[scalingIndex].MTime;
                KeyScale data = new KeyScale(scale, timeStamp);
                Scalings[scalingIndex] = data;
            }

            //foreach (var q in Rotations)
            //{
            //    Console.WriteLine($"{q.Orientation}");
            //}

            //Console.WriteLine($"distinct sca {Scalings.Distinct().Count()}");

        }
        Matrix4x4 TestPosition(float time)
        {
            //return Matrix4x4.Identity;
            return Matrix4x4.CreateTranslation(Vector3.Lerp(new Vector3(0, 0, 20), new Vector3(0, 20, 20), time));
        }

        Matrix4x4 TestRotation(float time)
        {
            return Matrix4x4.CreateFromAxisAngle(new Vector3(0, 1, 0), time * MathF.PI * 2);
        }

        Matrix4x4 TestScale(float time)
        {
            return Matrix4x4.CreateScale(Vector3.Lerp(new Vector3(0.5f), new Vector3(1f), time));
        }


        public void Update(Matrix4x4 node, float time)
        {
            var translation = InterpolatePosition(time);
            var rotation = InterpolateRotation(time);
            var scaling = InterpolateScaling(time);
            //Matrix4x4.Decompose(node, out _, out _, out Vector3 nodeTranslation);

            //nodeTranslation += Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 20, 0), time);

            //var translation = Matrix4x4.CreateTranslation(nodeTranslation);
            //var translation = TestPosition(time);
            //var rotation = TestRotation(time);
            //var scaling = TestScale(time);



            Transform = scaling * rotation * translation;
            //Transform = translation * rotation;
            //Transform = translation * rotation * scaling;
            TransformIsIdentity = Transform.IsIdentity;

        }
        int GetPositionIndex(float time)
        {
            for (int index = 0; index < numPositions - 1; index++)
            {
                if (time < Positions[index + 1].TimeStamp)
                    return index;
            }
            return -1;
        }
        int GetRotationIndex(float time)
        {
            for (int index = 0; index < numRotations - 1; index++)
            {
                if (time < Rotations[index + 1].TimeStamp)
                    return index;
            }
            return -1;
        }
        int GetScalingIndex(float time)
        {
            for (int index = 0; index < numScalings - 1; index++)
            {
                if (time < Scalings[index + 1].TimeStamp)
                    return index;
            }
            return -1;
        }

        int GetStartingIndex(float time, IKeyframe[] keys)
        {
            for (int index = 0; index < keys.Length - 1; index++)
            {
                if (time < keys[index + 1].TimeStamp)
                    return index;
            }
            return -1;
        }
        float GetScaleFactor(float lastTimeStamp, float nextTimeStamp, float time)
        {
            var scaleFactor = 0.0f;
            var midWayLength = time - lastTimeStamp;
            var framesDiff = nextTimeStamp - lastTimeStamp;
            scaleFactor = midWayLength / framesDiff;
            return scaleFactor;
        }
        Matrix4x4 InterpolatePosition(float time)
        {
            if (numPositions == 1)
                return Matrix4x4.CreateTranslation(Positions[0].Position);

            var p0I = GetStartingIndex(time, Positions);
            var p1I = p0I + 1;
            var p0 = Positions[p0I];
            var p1 = Positions[p1I];
            var scaleFactor = GetScaleFactor(p0.TimeStamp, p1.TimeStamp, time);

            CurrentPosition = Vector3.Lerp(p0.Position, p1.Position, scaleFactor);
            return Matrix4x4.CreateTranslation(CurrentPosition);
        }
        Matrix4x4 InterpolateRotation(float time)
        {
            if (numRotations == 1)
                return Matrix4x4.CreateFromQuaternion(Rotations[0].Orientation);

            var p0I = GetStartingIndex(time, Rotations);
            var p1I = p0I + 1;
            var p0 = Rotations[p0I];
            var p1 = Rotations[p1I];
            var scaleFactor = GetScaleFactor(p0.TimeStamp, p1.TimeStamp, time);

            var rotQuat = Quaternion.Slerp(p0.Orientation, p1.Orientation, scaleFactor);

            if (Quaternion.Normalize(rotQuat) != rotQuat)
            {
                rotQuat = Quaternion.Normalize(rotQuat);
            }
            return Matrix4x4.CreateFromQuaternion(rotQuat);

            //return Matrix4x4.CreateFromQuaternion(Quaternion.Slerp(p0.Orientation, p1.Orientation, scaleFactor));
            //return Matrix4x4.CreateFromQuaternion(p0.Orientation);
        }

        Matrix4x4 InterpolateScaling(float time)
        {
            if (numScalings == 1)
                return Matrix4x4.CreateScale(Scalings[0].Scale);

            var p0I = GetStartingIndex(time, Scalings);
            var p1I = p0I + 1;
            var p0 = Scalings[p0I];
            var p1 = Scalings[p1I];
            var scaleFactor = GetScaleFactor(p0.TimeStamp, p1.TimeStamp, time);

            return Matrix4x4.CreateScale(Vector3.Lerp(p0.Scale, p1.Scale, scaleFactor));
            //return Matrix4x4.CreateScale(p0.Scale);
        }


    }

    internal interface IKeyframe
    {
        public float TimeStamp { get; }
    }

    internal class KeyPosition : IKeyframe
    {
        public Vector3 Position { get; private set; }
        public float TimeStamp { get; }
        public KeyPosition(Vector3 position, float timeStamp)
        {
            Position = position;
            TimeStamp = timeStamp;
        }
    }


    internal class KeyRotation : IKeyframe
    {
        public Quaternion Orientation { get; private set; }
        public float TimeStamp { get; set; }

        public KeyRotation(Quaternion orientation, float timeStamp)
        {
            Orientation = orientation;
            TimeStamp = timeStamp;
        }
    }

    internal class KeyScale : IKeyframe
    {
        public Vector3 Scale { get; private set; }
        public float TimeStamp { get; set; }

        public KeyScale(Vector3 scale, float timeStamp)
        {
            Scale = scale;
            TimeStamp = timeStamp;
        }
    }



}
