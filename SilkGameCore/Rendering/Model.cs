using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SilkGameCore.Rendering.Textures;
using System.Numerics;
using AssimpMesh = Silk.NET.Assimp.Mesh;
using AssimpPPS = Silk.NET.Assimp.PostProcessSteps;
namespace SilkGameCore.Rendering
{
    public class Model : IDisposable
    {
        const AssimpPPS defaultOptions =
                AssimpPPS.FindDegenerates |
                AssimpPPS.FindInvalidData |
                AssimpPPS.FlipUVs |
                AssimpPPS.JoinIdenticalVertices |
                AssimpPPS.ImproveCacheLocality |
                AssimpPPS.OptimizeMeshes |
                AssimpPPS.Triangulate;
        public Model(GL gl, string path, AssimpPPS postProcessSteps = defaultOptions, bool extractTextures = false)
        {
            var assimp = Assimp.GetApi();
            _assimp = assimp;
            GL = gl;
            _extractTextures = extractTextures;
            Console.WriteLine($"Loading 3D model [{path}]");
            LoadModel(path, postProcessSteps);
        }
        private readonly GL GL;
        private readonly bool _extractTextures;
        private Assimp _assimp;
        private List<GLTexture> _texturesLoaded = new List<GLTexture>();
        public string Directory { get; protected set; } = string.Empty;

        public List<ModelPart> Parts { get; protected set; } = new List<ModelPart>();

        public void Draw()
        {
            foreach (var part in Parts)
            {
                part.Draw();
            }
        }
        private unsafe void LoadModel(string path, AssimpPPS options)
        {
            var scene = _assimp.ImportFile(path, (uint)options);

            if (scene == null || scene->MFlags == Assimp.SceneFlagsIncomplete || scene->MRootNode == null)
            {
                var error = _assimp.GetErrorStringS();
                throw new Exception(error);
            }

            //Directory = path;
            Directory = Path.GetDirectoryName(path) ?? string.Empty;

            ProcessNode(scene->MRootNode, scene, Matrix4x4.Identity);
        }

        private unsafe void ProcessNode(Node* node, Scene* scene, Matrix4x4 parentTransform)
        {
            var meshes = new List<Mesh>();


            Matrix4x4 currentTransform = parentTransform * node->MTransformation;

            var relativeTransform = Matrix4x4.Transpose(currentTransform);
            for (var i = 0; i < node->MNumMeshes; i++)
            {
                var assimpMesh = scene->MMeshes[node->MMeshes[i]];

                var mesh = ProcessMesh(assimpMesh, scene);
                mesh.Transform = relativeTransform;
                meshes.Add(mesh);

            }

            var name = "no-name";
            if (!string.IsNullOrEmpty(node->MName))
                name = node->MName;

            Parts.Add(new ModelPart(name, meshes));

            for (var i = 0; i < node->MNumChildren; i++)
            {
                ProcessNode(node->MChildren[i], scene, currentTransform);
            }
        }

        private unsafe Mesh ProcessMesh(AssimpMesh* mesh, Scene* scene)
        {
            // data to fill
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            List<GLTexture> textures = new List<GLTexture>();

            // walk through each of the mesh's vertices
            for (uint i = 0; i < mesh->MNumVertices; i++)
            {
                Vertex vertex = new Vertex();
                vertex.BoneIds = new int[Vertex.MAX_BONE_INFLUENCE];
                vertex.Weights = new float[Vertex.MAX_BONE_INFLUENCE];

                //vertex.Position = Vector3.Transform(mesh->MVertices[i], transform);
                vertex.Position = mesh->MVertices[i];

                // normals
                if (mesh->MNormals != null)
                    vertex.Normal = mesh->MNormals[i];
                // tangent
                if (mesh->MTangents != null)
                    vertex.Tangent = mesh->MTangents[i];
                // bitangent
                if (mesh->MBitangents != null)
                    vertex.Bitangent = mesh->MBitangents[i];

                // texture coordinates
                if (mesh->MTextureCoords[0] != null) // does the mesh contain texture coordinates?
                {
                    // a vertex can contain up to 8 different texture coordinates. We thus make the assumption that we won't 
                    // use models where a vertex can have multiple texture coordinates so we always take the first set (0).
                    Vector3 texcoord3 = mesh->MTextureCoords[0][i];
                    vertex.TexCoords = new Vector2(texcoord3.X, texcoord3.Y);
                }

                vertices.Add(vertex);
            }

            // now walk through each of the mesh's faces (a face is a mesh its triangle) and retrieve the corresponding vertex indices.
            for (uint i = 0; i < mesh->MNumFaces; i++)
            {
                Face face = mesh->MFaces[i];
                // retrieve all indices of the face and store them in the indices vector
                for (uint j = 0; j < face.MNumIndices; j++)
                    indices.Add(face.MIndices[j]);
            }

            // TODO: Process materials.
            // Its optional to prevent having the same issues as monogame's fbx loader
            // where model loading fails if it can't find the textures for that model.
            if (_extractTextures)
            {

            }

            var result = new Mesh(GL, BuildVertices(vertices), BuildIndices(indices), textures);
            return result;
        }

        private unsafe List<GLTexture> LoadMaterialTextures(Material* mat, TextureType type, string typeName)
        {
            var textureCount = _assimp.GetMaterialTextureCount(mat, type);
            List<GLTexture> textures = new List<GLTexture>();
            for (uint i = 0; i < textureCount; i++)
            {
                AssimpString path = "";
                Return r = _assimp.GetMaterialTexture(mat, type, i, &path, null, null, null, null, null, null);
                if (r == Return.Failure)
                {
                    return textures;
                }
                //This should use TextureManager calls.
                bool skip = false;
                for (int j = 0; j < _texturesLoaded.Count; j++)
                {
                    if (_texturesLoaded[j].Path == path)
                    {
                        textures.Add(_texturesLoaded[j]);
                        skip = true;
                        break;
                    }
                }
                if (!skip)
                {
                    var texture = new GLTexture(GL, Directory, type);
                    texture.Path = path;
                    textures.Add(texture);
                    _texturesLoaded.Add(texture);
                }

            }
            return textures;
        }
        //Warning: modifing this also requires updating Mesh.SetupMesh()
        private float[] BuildVertices(List<Vertex> vertexCollection)
        {
            var vertices = new List<float>();

            foreach (var vertex in vertexCollection)
            {
                vertices.Add(vertex.Position.X);
                vertices.Add(vertex.Position.Y);
                vertices.Add(vertex.Position.Z);
                vertices.Add(vertex.TexCoords.X);
                vertices.Add(vertex.TexCoords.Y);
                vertices.Add(vertex.Normal.X);
                vertices.Add(vertex.Normal.Y);
                vertices.Add(vertex.Normal.Z);
            }

            return vertices.ToArray();
        }

        private uint[] BuildIndices(List<uint> indices)
        {
            return indices.ToArray();
        }


        public void Dispose()
        {
            foreach (var part in Parts)
            {
                part.Dispose();
            }

            _texturesLoaded = null;
        }
    }
}
