using Silk.NET.OpenGL;

namespace SilkGameCore.Rendering.Gizmos.Geometries
{
    internal class GGCylinder : GizmoGeometry
    {
        public GGCylinder(GL gl, int subdivisions = 64) : base(gl)
        {
            var circle = Gizmos.GenerateCirclePositions(subdivisions);
            var vertexList = new List<float>();
            var indexList = new List<ushort>();

            var horizontalCount = 0;
            //center
            foreach (var pos in circle)
            {
                vertexList.Add(pos.X);
                vertexList.Add(0);
                vertexList.Add(pos.Y);
            }
            horizontalCount += circle.Length;

            //floor
            foreach (var pos in circle)
            {
                vertexList.Add(pos.X);
                vertexList.Add(-1);
                vertexList.Add(pos.Y);
            }
            horizontalCount += circle.Length;

            //ceiling
            foreach (var pos in circle)
            {
                vertexList.Add(pos.X);
                vertexList.Add(1);
                vertexList.Add(pos.Y);
            }
            horizontalCount += circle.Length;

            for (ushort i = 0; i < horizontalCount; i++)
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
            //vertical lines
            var circle2 = Gizmos.GenerateCirclePositions(4);

            var startIndex = (ushort)horizontalCount;
            var index = startIndex;
            foreach (var pos in circle2)
            {
                vertexList.Add(pos.X);
                vertexList.Add(1);
                vertexList.Add(pos.Y);
                index++;
                vertexList.Add(pos.X);
                vertexList.Add(-1);
                vertexList.Add(pos.Y);
                index++;
                indexList.Add((ushort)(index - 2));
                indexList.Add((ushort)(index - 1));
            }


            vertices = vertexList.ToArray();
            indices = indexList.ToArray();
            GenerateVAOVAP();
        }
    }
}
