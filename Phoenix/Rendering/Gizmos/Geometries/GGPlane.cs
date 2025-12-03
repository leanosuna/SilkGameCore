using Silk.NET.OpenGL;

namespace Phoenix.Rendering.Gizmos.Geometries
{
    internal class GGPlane : GizmoGeometry
    {
        public GGPlane(GL gl) : base(gl)
        {
            vertices = [
                -0.5f, 0.0f, -0.5f,
                 0.5f, 0.0f, -0.5f,
                 0.5f, 0.0f,  0.5f,
                -0.5f, 0.0f,  0.5f
            ];
            indices =
            [
                0, 1, 1, 2, 2, 3, 3, 0, 0, 2
            ];
            GenerateVAOVAP();
        }
    }
}
