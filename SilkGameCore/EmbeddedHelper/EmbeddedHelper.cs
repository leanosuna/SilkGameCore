using System.Reflection;

namespace SilkGameCore
{
    public static class EmbeddedHelper
    {
        public static string ExtractPath(string fileName, string assemblyPath)
        {
            var resourceName = $"SilkGameCore.{assemblyPath}.{fileName}";

            string tempFile = Path.Combine(Path.GetTempPath(), fileName);
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
                using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }
            return tempFile;
        }
    }
}
