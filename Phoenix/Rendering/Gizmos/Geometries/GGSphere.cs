using Silk.NET.OpenGL;

namespace Phoenix.Rendering.Gizmos.Geometries
{
    internal class GGSphere : GizmoGeometry
    {
        public GGSphere(GL gl, int subdivisions = 64) : base(gl)
        {
            var circle = Gizmos.GenerateCirclePositions(subdivisions);
            var vertexList = new List<float>();
            var indexList = new List<ushort>();
            // vertical
            foreach (var pos in circle)
            {
                vertexList.Add(pos.X);
                vertexList.Add(pos.Y);
                vertexList.Add(0);
            }
            var count = circle.Length;
            //horizontal
            foreach (var pos in circle)
            {
                vertexList.Add(pos.X);
                vertexList.Add(0);
                vertexList.Add(pos.Y);
            }
            count += circle.Length;

            //generate diag line
            foreach (var pos in circle)
            {
                vertexList.Add(0);
                vertexList.Add(pos.X);
                vertexList.Add(pos.Y);
            }
            count += circle.Length;

            for (ushort i = 0; i < count; i++)
            {

                indexList.Add(i);
                ushort nextIndex;
                if (i != 0 && (i + 1) % subdivisions == 0)
                {
                    nextIndex = (ushort)(i - (subdivisions - 1));
                }
                else
                    nextIndex = (ushort)(i + 1);

                indexList.Add(nextIndex);
            }

            vertices = vertexList.ToArray();
            indices = indexList.ToArray();
            GenerateVAOVAP();
        }
    }
}
