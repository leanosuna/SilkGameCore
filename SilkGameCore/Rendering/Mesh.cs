using Silk.NET.OpenGL;
using SilkGameCore.Rendering.Textures;
using System.Numerics;

namespace SilkGameCore.Rendering
{
    public class Mesh : IDisposable
    {
        public Mesh(GL gl, MeshAttributes attributes, List<Vertex> vertices, uint[] indices, List<GLTexture> textures)
        {
            GL = gl;
            Textures = textures;
            _attributes = attributes;
            Vertices = vertices;
            SetupMesh(indices, vertices);
        }

        public List<Vertex> Vertices;
        public Matrix4x4 Transform { get; internal set; }

        public IReadOnlyList<GLTexture> Textures { get; private set; }
        private VertexArrayObject<float, uint> VAO { get; set; }
        private uint IndicesLength { get; set; }
        GL GL { get; }

        BufferObject<uint> EBO;
        uint _VAHandle;
        uint _VBhandle;

        public Action? PreDraw { get; set; } = null;

        private MeshAttributes _attributes;
        private (nuint vertexSize, int elementsCount, List<(int count, VertexAttribPointerType type)> attData) _attributesData;
        private unsafe void SetupMesh(uint[] indices, List<Vertex> vertices)
        {
            IndicesLength = (uint)indices.Length;


            var attdata = CalculateAttributeData();
            (var data, var totalBytes) = PushData(vertices, attdata.strideBytes, attdata.layout);

            _VAHandle = GL.GenVertexArray();
            GL.BindVertexArray(_VAHandle);

            _VBhandle = GL.GenBuffer();
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, _VBhandle);

            fixed (byte* p = data)
            {
                GL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)totalBytes, (void*)p, BufferUsageARB.StaticDraw);
            }

            EBO = new BufferObject<uint>(GL, indices, BufferTargetARB.ElementArrayBuffer);
            EBO.Bind();

            SetVAOAttributes(attdata);

            // unbind VAO, VBO, EBO
            GL.BindVertexArray(0);
            //GL.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            //GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
        }

        unsafe void SetVAOAttributes((int strideBytes, List<(MeshAttributes attr, int components, int bytes, bool isInt)> layout) attdata)
        {
            int attributeByteOffset = 0;
            for (uint attribIndex = 0; attribIndex < attdata.layout.Count; attribIndex++)
            {
                var entry = attdata.layout[(int)attribIndex];
                if (entry.isInt)
                {
                    GL.VertexAttribIPointer(attribIndex,
                                            entry.components,
                                            VertexAttribIType.Int,
                                            (uint)attdata.strideBytes,
                                            (void*)attributeByteOffset);
                }
                else
                {
                    GL.VertexAttribPointer(attribIndex,
                                           entry.components,
                                           VertexAttribPointerType.Float,
                                           false,
                                           (uint)attdata.strideBytes,
                                           (void*)attributeByteOffset);
                }
                GL.EnableVertexAttribArray(attribIndex);
                attributeByteOffset += entry.bytes;
            }

        }

        private (byte[], int) PushData(List<Vertex> vertices, int strideBytes,
            List<(MeshAttributes attr, int components, int bytes, bool isInt)> layout)
        {
            var vertexCount = vertices.Count;
            var totalBytes = strideBytes * vertexCount;
            var data = new byte[totalBytes];
            int byteOffset = 0;
            for (int vi = 0; vi < vertexCount; vi++)
            {
                var v = vertices[vi];

                foreach (var entry in layout)
                {
                    switch (entry.attr)
                    {
                        case MeshAttributes.Position3D:
                            PushFloatArray([v.Position.X, v.Position.Y, v.Position.Z], data, ref byteOffset);
                            break;
                        case MeshAttributes.Position2D:
                            PushFloatArray([v.Position.X, v.Position.Y], data, ref byteOffset);
                            break;
                        case MeshAttributes.TexCoord:
                            PushFloatArray([v.TexCoords.X, v.TexCoords.Y], data, ref byteOffset);
                            break;
                        case MeshAttributes.Normals:
                            PushFloatArray([v.Normal.X, v.Normal.Y, v.Normal.Z], data, ref byteOffset);
                            break;
                        case MeshAttributes.Tangents:
                            PushFloatArray([v.Tangent.X, v.Tangent.Y, v.Tangent.Z], data, ref byteOffset);
                            break;
                        case MeshAttributes.Bitangents:
                            PushFloatArray([v.Bitangent.X, v.Bitangent.Y, v.Bitangent.Z], data, ref byteOffset);
                            break;
                        case MeshAttributes.boneIds:
                            PushIntArray([(int)v.BoneIds.X, (int)v.BoneIds.Y, (int)v.BoneIds.Z, (int)v.BoneIds.W], data, ref byteOffset); // int[]
                            break;
                        case MeshAttributes.boneWeights:
                            PushFloatArray([v.Weights.X, v.Weights.Y, v.Weights.Z, v.Weights.W], data, ref byteOffset); // float[]
                            break;
                        default:
                            throw new NotSupportedException(entry.attr.ToString());
                    }
                }
            }

            if (byteOffset != totalBytes)
                throw new Exception($"Buffer fill mismatch ({byteOffset} != {totalBytes})");

            return (data, totalBytes);
        }

        private (int strideBytes, List<(MeshAttributes attr, int components, int bytes, bool isInt)> layout)
            CalculateAttributeData()
        {
            int strideBytes = 0;
            var layout = new List<(MeshAttributes attr, int components, int bytes, bool isInt)>();
            foreach (MeshAttributes a in Enum.GetValues(typeof(MeshAttributes)))
            {
                if (_attributes.HasFlag(a))
                {
                    int bytes = SizeOfAttribute(a);
                    int comps = ItemCountOfAttribute(a);
                    bool isInt = (a == MeshAttributes.boneIds);
                    layout.Add((a, comps, bytes, isInt));
                    strideBytes += bytes;
                }
            }
            return (strideBytes, layout);
        }
        private void PushFloatArray(float[] src, byte[] data, ref int byteOffset)
        {
            var byteCount = src.Length * sizeof(float);
            System.Buffer.BlockCopy(src, 0, data, byteOffset, byteCount);
            byteOffset += byteCount;
        }
        private void PushIntArray(int[] src, byte[] data, ref int byteOffset)
        {
            //Console.WriteLine($"Pushing int array of length {src.Length} at offset {byteOffset}");
            var byteCount = src.Length * sizeof(int);
            System.Buffer.BlockCopy(src, 0, data, byteOffset, byteCount);
            byteOffset += byteCount;
        }

        public unsafe void Draw()
        {
            //bind VAO
            GL.BindVertexArray(_VAHandle);

            //GL.BindBuffer(BufferTargetARB.ArrayBuffer, _VBhandle);
            //EBO.Bind();
            GL.DrawElements(PrimitiveType.Triangles, IndicesLength, DrawElementsType.UnsignedInt, null);
        }

        public void Dispose()
        {
            EBO.Dispose();
            GL.DeleteBuffer(_VBhandle);
            GL.DeleteVertexArray(_VAHandle);
        }

        public static int ItemCountOfAttribute(MeshAttributes attr)
        {
            switch (attr)
            {
                case MeshAttributes.Position2D:
                case MeshAttributes.TexCoord:
                    return 2;
                case MeshAttributes.Position3D:
                case MeshAttributes.Normals:
                case MeshAttributes.Tangents:
                case MeshAttributes.Bitangents:
                    return 3;
                case MeshAttributes.boneIds:
                    return Vertex.MAX_BONE_INFLUENCE;
                case MeshAttributes.boneWeights:
                    return Vertex.MAX_BONE_INFLUENCE;
                default:
                    return -1;
            }
        }

        public static int SizeOfAttribute(MeshAttributes attr)
        {
            switch (attr)
            {
                case MeshAttributes.Position2D:
                case MeshAttributes.TexCoord:
                    return 2 * sizeof(float);
                case MeshAttributes.Position3D:
                case MeshAttributes.Normals:
                case MeshAttributes.Tangents:
                case MeshAttributes.Bitangents:
                    return 3 * sizeof(float);
                case MeshAttributes.boneIds:
                    return Vertex.MAX_BONE_INFLUENCE * sizeof(int);
                case MeshAttributes.boneWeights:
                    return Vertex.MAX_BONE_INFLUENCE * sizeof(float);

                default:
                    return -1;
            }
        }


        public static VertexAttribPointerType TypeOfAttribute(MeshAttributes attr)
        {
            switch (attr)
            {
                case MeshAttributes.Position2D:
                case MeshAttributes.Position3D:
                case MeshAttributes.TexCoord:
                case MeshAttributes.Normals:
                case MeshAttributes.Tangents:
                case MeshAttributes.Bitangents:
                case MeshAttributes.boneWeights:
                    return VertexAttribPointerType.Float;

                case MeshAttributes.boneIds:
                    return VertexAttribPointerType.Int;
                default:
                    return VertexAttribPointerType.Float;
            }
        }

        float[] ToFloatArray(Vector3 vector)
        {
            return [vector.X, vector.Y, vector.Z];
        }
        float[] ToFloatArray(Vector2 vector)
        {
            return [vector.X, vector.Y];
        }

        public List<float[]> ToFloatArrayList(List<Vector3> vectors)
        {
            var fs = new List<float[]>();

            foreach (var v in vectors)
            {
                float[] floats = [v.X, v.Y, v.Z];

                fs.Add(floats);
            }


            return fs;
        }

    }

    [Flags]
    public enum MeshAttributes
    {
        Position2D = 1 << 0,    // 0001
        Position3D = 1 << 1,    // 0010
        TexCoord = 1 << 2,      // 0100
        Normals = 1 << 3,       // 1000
        Tangents = 1 << 4,      // 10000
        Bitangents = 1 << 5,    // 100000
        boneIds = 1 << 6,       // 1000000
        boneWeights = 1 << 7    //10000000
    }

    unsafe struct VertexData
    {
        public nuint BufferSize;
        public void* BufferData;
    }


}
