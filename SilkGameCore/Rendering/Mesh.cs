using Silk.NET.OpenGL;
using SilkGameCore.Rendering.Textures;
using System.Numerics;

namespace SilkGameCore.Rendering
{
    public class Mesh : IDisposable
    {
        public Mesh(GL gl, float[] vertices, uint[] indices, List<GLTexture> textures)
        {
            GL = gl;
            Textures = textures;
            SetupMesh(indices, vertices);
        }
        public Matrix4x4 Transform { get; internal set; }

        public IReadOnlyList<GLTexture> Textures { get; private set; }
        VertexArrayObject<float, uint> VAO { get; set; }
        uint IndicesLength { get; set; }
        GL GL { get; }

        public Action? PreDraw { get; set; } = null;

        public unsafe void SetupMesh(uint[] indices, float[] vertices)
        {
            var EBO = new BufferObject<uint>(GL, indices, BufferTargetARB.ElementArrayBuffer);
            var VBO = new BufferObject<float>(GL, vertices, BufferTargetARB.ArrayBuffer);
            IndicesLength = (uint)indices.Length;
            VAO = new VertexArrayObject<float, uint>(GL, VBO, EBO);

            VAO.Bind();
            //Warning: this should be updated if the vertex structure changes in Model.BuildVertices()
            uint vertexSize = 8; // 3 for position, 2 for texture coordinates, and 3 for normals
            var offset = 0;
            var size = 3;
            VAO.VertexAttributePointer(0, size, VertexAttribPointerType.Float, vertexSize, offset);
            offset += size;
            size = 2;
            VAO.VertexAttributePointer(1, size, VertexAttribPointerType.Float, vertexSize, offset);
            offset += size;
            size = 3;
            VAO.VertexAttributePointer(2, size, VertexAttribPointerType.Float, vertexSize, offset);

            GL.BindVertexArray(0);
        }

        public unsafe void Draw()
        {
            VAO.Bind();

            GL.DrawElements(PrimitiveType.Triangles, IndicesLength, DrawElementsType.UnsignedInt, null);
        }

        public void Dispose()
        {
            VAO.Dispose();
        }
    }
}
