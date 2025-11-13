using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SilkGameCore.Rendering.Animation;
using SilkGameCore.Rendering.Textures;
using System.Numerics;
using AssimpMesh = Silk.NET.Assimp.Mesh;
using AssimpPPS = Silk.NET.Assimp.PostProcessSteps;
namespace SilkGameCore.Rendering.Geometry
{
    public class Model : IDisposable
    {
        private readonly GL GL;
        private readonly bool _extractTextures;
        private Assimp _assimp;
        public List<GLTexture> _texturesLoaded = new List<GLTexture>();
        public string Directory { get; protected set; } = string.Empty;
        private MeshAttributes _meshAttributes;
        public List<ModelPart> Parts { get; protected set; } = new List<ModelPart>();

        public Dictionary<string, BoneInfo> BoneInfoMap { get; } = new Dictionary<string, BoneInfo>();


        public const AssimpPPS DefaultAssimpPost =
                AssimpPPS.FindDegenerates |
                AssimpPPS.FindInvalidData |
                AssimpPPS.FlipUVs |
                AssimpPPS.JoinIdenticalVertices |
                AssimpPPS.ImproveCacheLocality |
                AssimpPPS.OptimizeMeshes |
                AssimpPPS.Triangulate |
                AssimpPPS.LimitBoneWeights;

        public const MeshAttributes DefaultMeshAttributes =
            MeshAttributes.Position3D |
            MeshAttributes.TexCoord |
            MeshAttributes.Normals;
        public Model(GL gl, string path, AssimpPPS postProcessSteps = DefaultAssimpPost,
            MeshAttributes meshAttributes = DefaultMeshAttributes, bool extractTextures = false)
        {
            var assimp = Assimp.GetApi();
            _assimp = assimp;
            _meshAttributes = meshAttributes;
            GL = gl;
            _extractTextures = extractTextures;
            Log.Info($"loading model {path}");

            LoadModel(path, postProcessSteps);
        }

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
            Directory = Path.GetDirectoryName(path) ?? string.Empty;

            ProcessNode(scene->MRootNode, scene, Matrix4x4.Identity);

            //Bones = _boneDict.Select(b => b.Value).ToArray();

            //for (var i = 0; i < Bones.Length; i++)
            //{
            //    Log.Debug($"b{i} {Bones[i].Name}");
            //}


        }

        private unsafe void ProcessNode(Node* node, Scene* scene, Matrix4x4 parentTransform)
        {
            var meshes = new List<Mesh>();


            //Matrix4x4 currentTransform = parentTransform * node->MTransformation;
            //var relativeTransform = Matrix4x4.Transpose(currentTransform);
            var nTransform = node->MTransformation;
            nTransform.Transpose();
            Matrix4x4 currentTransform = parentTransform * nTransform;
            var relativeTransform = currentTransform;

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

            if (meshes.Count > 0)
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

                //Console.WriteLine($"mesh bones{mesh->MNumBones}");
                for (int b = 0; b < Vertex.MAX_BONE_INFLUENCE; b++)
                {
                    vertex.BoneIds[b] = -1;
                    vertex.Weights[b] = 0.0f;
                }


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
                //_assimp.GetMaterialTexture()
                //LoadMaterialTextures(scene->MMaterials);
                var texCount = scene->MNumTextures;
                ////gloss, difuse, normal, specular
                for (var i = 0; i < texCount; i++)
                {
                    var tex = scene->MTextures[i];
                    var name = tex->MFilename.AsString;
                    var w = tex->MWidth;
                    var h = tex->MHeight;

                    var t = new GLTexture(GL, tex->PcData, w, h);

                    _texturesLoaded.Add(t);

                }

                //var mc = scene->MNumMaterials;
                //for (var i = 0; i < mc; i++)
                //{
                //    var mat = scene->MMaterials[i];
                //    //var pc = mat->MNumProperties;
                //    //for (var j = 0; j < pc; j++)
                //    //{
                //    //    var p = mat->MProperties[j];
                //    //    var key = p->MKey.AsString;

                //    //}
                //    LoadMaterialTextures(mat, TextureType.Diffuse);

                //}


            }


            if (_meshAttributes.HasFlag(MeshAttributes.boneIds) && _meshAttributes.HasFlag(MeshAttributes.boneWeights))
            {
                ExtractBoneWeights(vertices, mesh, scene);
            }

            var result = new Mesh(GL, _meshAttributes, vertices, indices.ToArray(), textures);
            return result;
        }


        private unsafe void ExtractBoneWeights(List<Vertex> vertices, AssimpMesh* mesh, Scene* scene)
        {
            // Temporary dictionary to collect all influences per vertex
            var vertexInfluences = new Dictionary<int, List<(int BoneId, float Weight)>>();

            for (int boneID = 0; boneID < mesh->MNumBones; boneID++)
            {
                string boneName = mesh->MBones[boneID]->MName;


                //if (!_boneDict.TryGetValue(boneName, out var boneInfo))
                //{
                //    _boneDict.Add(boneName, new Animation.Bone(boneName,
                //        //Matrix4x4.Transpose
                //        (mesh->MBones[boneID]->MOffsetMatrix)));

                //}
                var offset = mesh->MBones[boneID]->MOffsetMatrix;
                //offset.Transpose();
                BoneInfoMap.TryAdd(boneName, new BoneInfo(boneID, offset));

                var weights = mesh->MBones[boneID]->MWeights;
                var numWeights = mesh->MBones[boneID]->MNumWeights;

                for (int wi = 0; wi < numWeights; wi++)
                {
                    int vertexId = (int)weights[wi].MVertexId;
                    float weight = weights[wi].MWeight;

                    if (!vertexInfluences.TryGetValue(vertexId, out var list))
                    {
                        list = new List<(int, float)>();
                        vertexInfluences[vertexId] = list;
                    }

                    list.Add((boneID, weight));
                }
            }

            foreach (var kvp in vertexInfluences)
            {
                int vertexId = kvp.Key;
                var influences = kvp.Value;


                List<(int BoneId, float Weight)> topInfluences;

                if (influences.Count > Vertex.MAX_BONE_INFLUENCE)
                {
                    //Log.Debug($"ami {vertexId}");
                    influences.Sort((a, b) => b.Weight.CompareTo(a.Weight));
                    topInfluences = influences.Take(Vertex.MAX_BONE_INFLUENCE).ToList();

                    float total = topInfluences.Sum(x => x.Weight);
                    if (total > 0)
                    {
                        for (int i = 0; i < topInfluences.Count; i++)
                            topInfluences[i] = (topInfluences[i].BoneId, topInfluences[i].Weight / total);
                    }

                }
                else
                {
                    topInfluences = influences;
                    for (int i = topInfluences.Count; i < Vertex.MAX_BONE_INFLUENCE; i++)
                    {
                        topInfluences.Add((-1, 0.0f));
                    }
                }

                var vertex = vertices[vertexId];
                vertex.BoneIds =
                    new Vector4(topInfluences[0].BoneId, topInfluences[1].BoneId, topInfluences[2].BoneId, topInfluences[3].BoneId);
                vertex.Weights =
                    new Vector4(topInfluences[0].Weight, topInfluences[1].Weight, topInfluences[2].Weight, topInfluences[3].Weight);
                vertices[vertexId] = vertex;
            }
        }

        private unsafe List<GLTexture> LoadMaterialTextures(Material* mat, TextureType type)
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
                    var texture = new GLTexture(GL, path);
                    texture.Path = path;
                    textures.Add(texture);
                    _texturesLoaded.Add(texture);
                }

            }
            return textures;
        }

        public void Dispose()
        {
            foreach (var part in Parts)
            {
                part.Dispose();
            }

            _texturesLoaded.Clear();
        }
    }
}
