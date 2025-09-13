namespace SilkGameCore.Rendering
{
    public class ModelPart : IDisposable
    {
        public string Name { get; private set; }
        public List<Mesh> Meshes { get; private set; }

        public Action? PreDraw { get; set; } = null;
        public ModelPart(string name, List<Mesh> meshes)
        {
            Name = name;
            Meshes = meshes;
        }

        public void Draw()
        {
            if (PreDraw != null)
                PreDraw.Invoke();

            foreach (var mesh in Meshes)
            {
                if (mesh.PreDraw != null)
                    mesh.PreDraw.Invoke();
                mesh.Draw();
            }
        }

        public void Dispose()
        {
            foreach (var mesh in Meshes)
            {
                mesh.Dispose();
            }
        }
    }
}
