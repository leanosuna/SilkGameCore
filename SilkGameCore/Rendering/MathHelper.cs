using System.Numerics;

namespace SilkGameCore.Rendering
{
    public static class MathHelper
    {
        public const float TwoPi = MathF.PI * 2.0f;
        public const float PiOver2 = MathF.PI / 2.0f;
        public const float PiOver4 = MathF.PI / 4.0f;

        public static Matrix4x4 RotationMxFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            return Matrix4x4.CreateFromQuaternion(RotationFromYawPitchRoll(yaw, pitch, roll));
        }

        public static Quaternion RotationFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            return Quaternion.CreateFromAxisAngle(Vector3.UnitY, yaw) *
                Quaternion.CreateFromAxisAngle(Vector3.UnitX, pitch) *
                Quaternion.CreateFromAxisAngle(Vector3.UnitZ, roll);
        }

        public static float ToRad(float degrees)
        {
            return degrees * MathF.PI / 180.0f;
        }
        public static float ToDeg(float radians)
        {
            return radians * 180.0f / MathF.PI;
        }
    }
}
