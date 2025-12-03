using Silk.NET.OpenGL;

namespace Phoenix.Rendering.Gizmos.Geometries
{
    internal class GGCube : GizmoGeometry
    {
        public GGCube(GL gl) : base(gl)
        {
            vertices = [
                0.5f, 0.5f, 0.5f,
                -0.5f, 0.5f, 0.5f,
                0.5f, -0.5f, 0.5f,
                -0.5f, -0.5f, 0.5f,
                0.5f, 0.5f, -0.5f,
                -0.5f, 0.5f, -0.5f,
                0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f
            ];

            indices =
            [
                0, 1,
                0, 2,
                1, 3,
                3, 2,

                4, 5,
                4, 6,
                5, 7,
                7, 6,

                0, 4,
                1, 5,
                2, 6,
                3, 7
            ];

            GenerateVAOVAP();
        }

    }
}
