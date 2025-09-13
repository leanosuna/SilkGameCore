using System.Numerics;

namespace SilkGameCore.Rendering.Gizmos.Geometries
{
    internal struct GizmoGeometryInstance
    {
        public GizmoGeometry Geometry { get; set; }
        public Matrix4x4 World { get; set; }
        public Vector3 Color { get; set; }
        public bool Hit { get; set; }
        public GizmoGeometryInstance(GizmoGeometry type, Matrix4x4 world, Vector3 color, bool hit = false)
        {
            Geometry = type;
            World = world;
            Color = color;
            Hit = hit;
        }

    }

}

