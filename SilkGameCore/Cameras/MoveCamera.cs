using System.Numerics;

namespace SilkGameCore.Cameras
{
    public abstract class MoveCamera : Camera
    {
        public float MoveSpeed = 10f;
        public bool MouseAim = true;
        internal SilkGameGL _game;

        public MoveCamera(SilkGameGL game, Vector3 position, float yaw, float pitch, float fov, float nearPlane, float farPlane, float aspectRatio)
           : base(position, yaw, pitch, fov, nearPlane, farPlane, aspectRatio)
        {
            _game = game;
        }

        public override void Update(double deltaTime)
        {
            CalculateMouseAim();
        }

        internal void CalculateMouseAim()
        {
            if (!MouseAim)
                return;

            var mouseDelta = _game.InputManager.MouseDelta;
            if (mouseDelta != Vector2.Zero)
            {
                Yaw += mouseDelta.X;
                Pitch -= mouseDelta.Y;
                Pitch = Math.Clamp(Pitch, -((MathF.PI * 0.5f) - 0.05f), (MathF.PI * 0.5f) - 0.05f);

                CalculateVectors();
                CalculateView();
            }

        }
    }
}
