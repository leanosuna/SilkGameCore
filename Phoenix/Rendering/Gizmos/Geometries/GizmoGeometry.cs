using Silk.NET.OpenGL;

namespace Phoenix.Rendering.Gizmos.Geometries
{
    internal abstract class GizmoGeometry
    {
        private readonly GL GL;
        private VertexArrayObject<float, ushort> _VAO = default!;
        private uint _indicesLength;
        internal ushort[] indices = default!;
        internal float[] vertices = default!;

        public GizmoGeometry(GL gl)
        {
            GL = gl;
        }
        internal void GenerateVAOVAP()
        {
            _indicesLength = (uint)indices.Length;
            var EBO = new BufferObject<ushort>(GL, indices, BufferTargetARB.ElementArrayBuffer);
            var VBO = new BufferObject<float>(GL, vertices, BufferTargetARB.ArrayBuffer);
            _VAO = new VertexArrayObject<float, ushort>(GL, VBO, EBO);

            _VAO.Bind();

            uint vertexSize = 3; // 3 position floats

            _VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, vertexSize, 0);

            GL.BindVertexArray(0);
        }
        public virtual unsafe void Draw()
        {
            _VAO.Bind();
            GL.DrawElements(PrimitiveType.Lines, _indicesLength, DrawElementsType.UnsignedShort, null);
        }
    }
}
