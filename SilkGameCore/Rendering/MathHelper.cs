using System;
using System.Numerics;

namespace SilkGameCore.Rendering
{
    public static class MathHelper
    {
        public const float Pi = MathF.PI;
        public const float TwoPi = MathF.PI * 2.0f;
        public const float PiOver2 = MathF.PI / 2.0f;
        public const float PiOver4 = MathF.PI / 4.0f;

        public static Matrix4x4 RotationMxFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            return Matrix4x4.CreateFromQuaternion(RotationFromYawPitchRoll(yaw, pitch, roll));
        }

        public static void Invert(this ref Matrix4x4 m)
        {
            Matrix4x4.Invert(m, out m);
        }
        public static void Transpose(this ref Matrix4x4 m)
        {
            m = Matrix4x4.Transpose(m);
        }

        public static Quaternion RotationFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            return Quaternion.CreateFromAxisAngle(Vector3.UnitY, yaw) *
                Quaternion.CreateFromAxisAngle(Vector3.UnitX, pitch) *
                Quaternion.CreateFromAxisAngle(Vector3.UnitZ, roll);
        }
        public static void ExtractYawPitchRoll(this Quaternion r, out float yaw, out float pitch, out float roll)
        {
            yaw = MathF.Atan2(2.0f * (r.Y * r.W + r.X * r.Z), 1.0f - 2.0f * (r.X * r.X + r.Y * r.Y));
            pitch = MathF.Asin(2.0f * (r.X * r.W - r.Y * r.Z));
            roll = MathF.Atan2(2.0f * (r.X * r.Y + r.Z * r.W), 1.0f - 2.0f * (r.X * r.X + r.Z * r.Z));
        }
        public static float ToRad(this float degrees)
        {
            return degrees * MathF.PI / 180.0f;
        }
        public static float ToDeg(this float radians)
        {
            return radians * 180.0f / MathF.PI;
        }

        public static float Lerp(float start, float end, float amount)
        {
            return (1 - amount) * start + amount * end;

        }
        public static float WrapAngle(float angle)
        {
            if ((angle > -Pi) && (angle <= Pi))
            {
                return angle;
            }
            angle %= TwoPi;
            if (angle <= -Pi)
            {
                return angle + TwoPi;
            }
            if (angle > Pi)
            {
                return angle - TwoPi;
            }
            return angle;
        }

        public static float[] ToFloatArray(this Vector2 vector)
        {
            return [vector.X, vector.Y];
        }
        public static float[] ToFloatArray(this Vector3 vector)
        {
            return [vector.X, vector.Y, vector.Z];
        }
        public static float[] ToFloatArray(this Matrix4x4 mx)
        {
            return [mx.M11, mx.M12, mx.M13, mx.M14, mx.M21, mx.M22, mx.M23, mx.M24, mx.M31, mx.M32, mx.M33, mx.M34, mx.M41, mx.M42, mx.M43, mx.M44];
        }

        public static List<float[]> ToFloatArrayList(this List<Vector3> vectors)
        {
            var fs = new List<float[]>();

            foreach (var v in vectors)
            {
                float[] floats = [v.X, v.Y, v.Z];

                fs.Add(floats);
            }


            return fs;
        }
    }
}
