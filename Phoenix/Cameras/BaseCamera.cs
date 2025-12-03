using System.Numerics;

namespace Phoenix.Cameras
{
    public abstract class BaseCamera : Camera
    {
        public Vector3 Position;
        public Vector3 Front;
        public Vector3 Up;
        public Vector3 Right;
        public float Yaw;
        public float Pitch;
        public float FOV;
        public float AspectRatio;
        public float NearPlane;
        public float FarPlane;
        Vector3 tempFront;

        public BaseCamera(Vector3 position, float yaw, float pitch, float fov, float nearPlane, float farPlane, float aspectRatio)
        {
            Position = position;
            Yaw = yaw;
            Pitch = pitch;
            FOV = fov;
            NearPlane = nearPlane;
            FarPlane = farPlane;
            AspectRatio = aspectRatio;
            Up = Vector3.UnitY;
            CalculateVectors();
            CalculateView();
            CalculateProjection();
        }
        protected void CalculateVectors()
        {
            tempFront.X = MathF.Cos(Yaw) * MathF.Cos(Pitch);
            tempFront.Y = MathF.Sin(Pitch);
            tempFront.Z = MathF.Sin(Yaw) * MathF.Cos(Pitch);

            Front = Vector3.Normalize(tempFront);

            var flatFront = new Vector3(tempFront.X, 0, tempFront.Z);

            flatFront = Vector3.Normalize(flatFront);

            Right = Vector3.Normalize(Vector3.Cross(flatFront, Up));
        }
        protected void CalculateView()
        {
            View = Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }
        protected void CalculateProjection()
        {
            Projection = Matrix4x4.CreatePerspectiveFieldOfView(FOV, AspectRatio, NearPlane, FarPlane);
        }
        public abstract void Update(double deltaTime);



    }
}
