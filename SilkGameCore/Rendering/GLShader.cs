using Silk.NET.OpenGL;
using SilkGameCore.Rendering.Textures;
using System.Numerics;

namespace SilkGameCore.Rendering
{
    public class GLShader : IDisposable
    {
        private uint handle;
        private GL GL;
        private bool ignoreUniformsNotFound;

        private Dictionary<string, int> uniformLocations = new Dictionary<string, int>();

        //public static APIVersion APIVersion { get; set; }
        public GLShader(GL glContext, string path, bool ignoreUniformsNotFound = false) :
            this(glContext, $"{path}.vert", $"{path}.frag", ignoreUniformsNotFound)
        {

        }
        public GLShader(GL glContext, string vertexPath, string fragmentPath, bool ignoreUniformsNotFound = false)
        {
            GL = glContext;

            uint vertex = LoadShader(ShaderType.VertexShader, vertexPath);
            uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);

            handle = GL.CreateProgram();
            GL.AttachShader(handle, vertex);
            GL.AttachShader(handle, fragment);
            GL.LinkProgram(handle);
            GL.GetProgram(handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {GL.GetProgramInfoLog(handle)}");
            }
            GL.DetachShader(handle, vertex);
            GL.DetachShader(handle, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
            this.ignoreUniformsNotFound = ignoreUniformsNotFound;
        }

        public void SetAsCurrentGLProgram()
        {
            GL.UseProgram(handle);
        }


        int GetUniformLocation(string name)
        {
            if (uniformLocations.TryGetValue(name, out int location))
                return location;

            location = GL.GetUniformLocation(handle, name);

            if (location != -1)
                uniformLocations.Add(name, location);
            return location;

        }
        public unsafe void SetUniform<T>(string name, T value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
            {
                if (ignoreUniformsNotFound)
                    return;
                throw new Exception($"[{name}] not found on shader.");
            }
            switch (value)
            {
                case bool b: GL.Uniform1(location, b ? 1 : 0); break;
                case int i: GL.Uniform1(location, i); break;
                case float f: GL.Uniform1(location, f); break;
                case double d: GL.Uniform1(location, d); break;
                case Vector2 v2: GL.Uniform2(location, v2); break;
                case Vector3 v3: GL.Uniform3(location, v3); break;
                case Vector4 v4: GL.Uniform4(location, v4); break;

                case float[] fa:
                    fixed (float* ptr = fa)
                    {
                        GL.Uniform1(location, (uint)fa.Length, ptr);
                    }
                    break;
                case Vector2[] v2a:
                    fixed (Vector2* ptr = v2a)
                    {
                        GL.Uniform2(location, (uint)v2a.Length, (float*)ptr);
                    }
                    break;
                case Vector3[] v3a:
                    fixed (Vector3* ptr = v3a)
                    {
                        GL.Uniform3(location, (uint)v3a.Length, (float*)ptr);
                    }
                    break;
                case Vector4[] v4a:
                    fixed (Vector4* ptr = v4a)
                    {
                        GL.Uniform4(location, (uint)v4a.Length, (float*)ptr);
                    }
                    break;

                case Matrix4x4 m:
                    GL.UniformMatrix4(location, 1, false, (float*)&m); break;
                case Matrix4x4[] mm:
                    fixed (Matrix4x4* ptr = mm)
                    {
                        GL.UniformMatrix4(location, (uint)mm.Length, false, (float*)ptr);
                    }
                    break;
                default: throw new Exception($"{typeof(T).Name} missing GL.UniformT entry");
            }

        }
        public void SetTextureUniform(string name, uint tex, int slot)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + slot);
            GL.BindTexture(TextureTarget.Texture2D, tex);
            SetUniform(name, slot);
        }
        public void SetTextureUniform(string name, GLTexture tex, int slot)
        {
            tex.Bind(TextureUnit.Texture0 + slot);
            SetUniform(name, slot);
        }

        public void Dispose()
        {
            GL.DeleteProgram(handle);
        }
        //Experimental automatic version insert
        //private string ApiVersionInsert()
        //{
        //    var minorIsOneDigit = APIVersion.MinorVersion / 10 == 0;
        //    var version = $"{APIVersion.MajorVersion}{APIVersion.MinorVersion}";

        //    if (minorIsOneDigit) version += "0";

        //    return $"#version {version} core";
        //}

        private uint LoadShader(ShaderType type, string path)
        {
            //string ver = ApiVersionInsert();
            string src = File.ReadAllText(path);
            uint handle = GL.CreateShader(type);
            GL.ShaderSource(handle, src);
            GL.CompileShader(handle);
            string infoLog = GL.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
            }

            return handle;
        }
    }
}
