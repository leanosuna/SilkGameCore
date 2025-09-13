using Silk.NET.OpenGL;

namespace SilkGameCore.Rendering.Gizmos.Geometries
{
    internal class GGLineSegment : GizmoGeometry
    {
        public GGLineSegment(GL gl) : base(gl)
        {
            vertices = [
                0.0f, 0.0f, 0.0f,
                1.0f, 1.0f, 1.0f
            ];
            indices =
            [
                0, 1
            ];
            GenerateVAOVAP();
        }
    }
}
