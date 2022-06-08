using System.IO;
using Tommy;

namespace TommyForUnity.Runtime
{
    public static class TommyExtensionMethods
    {
        public static bool WriteToFile(this TomlNode node, string filePath, bool createIfNotExist = true, bool forceAscii = false)
        {
            if (!File.Exists(filePath) && !createIfNotExist) return false;
            using var writer = File.CreateText(filePath);
            TOML.ForceASCII = forceAscii;
            node.WriteTo(writer);
            writer.Flush();
            return true;
        }
    }
}
