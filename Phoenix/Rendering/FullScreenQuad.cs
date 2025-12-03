using Silk.NET.OpenGL;

namespace Phoenix.Rendering
{
    /// <summary>
    /// Simple FullScreenQuad geometry and draw helper
    /// </summary>
    public class FullScreenQuad
    {
        GL GL;

        VertexArrayObject<float, byte> VAO { get; set; }

        byte[] indices;
        float[] vertices;
        internal FullScreenQuad(PhoenixGame game)
        {
            GL = game.GL;

            vertices = [
              -1, -1f,  0f, 0f,   // bottom-left
               1f, -1f,  1f, 0f,   // bottom-right
               1f,  1f,  1f, 1f,   // top-right
              -1f,  1f,  0f, 1f    // top-left
            ];
            indices = [
                0, 1, 2,
                2, 3, 0
            ];

            var EBO = new BufferObject<byte>(GL, indices, BufferTargetARB.ElementArrayBuffer);
            var VBO = new BufferObject<float>(GL, vertices, BufferTargetARB.ArrayBuffer);
            VAO = new VertexArrayObject<float, byte>(GL, VBO, EBO);

            VAO.Bind();
            byte vertexSize = 4; // 2 for position, 2 for texture coordinates
            var offset = 0;
            var size = 2;

            VAO.VertexAttributePointer(0, size, VertexAttribPointerType.Float, vertexSize, offset);
            offset += size;
            VAO.VertexAttributePointer(1, size, VertexAttribPointerType.Float, vertexSize, offset);

            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Draws a FullScreen Quad with the current shader
        /// location 0: 2D position
        /// location 1: 2D UV
        /// </summary>
        public unsafe void Draw()
        {
            VAO.Bind();
            GL.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedByte, null);
        }
    }
}
