using Silk.NET.Input;
using System.Numerics;

namespace SilkGameCore.Cameras
{
    public class FreeCamera : MoveCamera
    {
        public FreeCamera(SilkGameGL game, Vector3 position, float yaw, float pitch, float fov, float nearPlane, float farPlane, float aspectRatio)
            : base(game, position, yaw, pitch, fov, nearPlane, farPlane, aspectRatio)
        {

        }


        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            var updateView = false;

            var dir = Vector3.Zero;
            if (_game.InputManager.KeyDown(Key.W))
            {
                dir += Front;
            }
            if (_game.InputManager.KeyDown(Key.S))
            {
                dir -= Front;
            }
            if (_game.InputManager.KeyDown(Key.A))
            {
                dir -= Right;
            }
            if (_game.InputManager.KeyDown(Key.D))
            {
                dir += Right;
            }
            if (_game.InputManager.KeyDown(Key.Space))
            {
                dir += Up;
            }
            if (_game.InputManager.KeyDown(Key.ControlLeft))
            {
                dir -= Up;
            }

            if (dir != Vector3.Zero)
            {
                dir = Vector3.Normalize(dir);

                var speed = MoveSpeed;

                if (_game.InputManager.KeyDown(Key.ShiftLeft))
                    speed *= 5f;
                Position += dir * speed * (float)deltaTime;


                updateView = true;
            }
            var updateCameraVectors = false;
            if (_game.InputManager.KeyDown(Key.Up))
            {
                Pitch += (float)deltaTime;
                updateView = true;
                updateCameraVectors = true;
            }
            if (_game.InputManager.KeyDown(Key.Down))
            {
                Pitch -= (float)deltaTime;
                updateView = true;
                updateCameraVectors = true;
            }
            if (_game.InputManager.KeyDown(Key.Right))
            {
                Yaw += (float)deltaTime;
                updateView = true;
                updateCameraVectors = true;
            }
            if (_game.InputManager.KeyDown(Key.Left))
            {
                Yaw -= (float)deltaTime;
                updateView = true;
                updateCameraVectors = true;
            }


            if (updateCameraVectors)
                CalculateVectors();

            if (updateView)
                CalculateView();
        }



    }
}
