using System.IO;

namespace cstsd
{
    public static class FileHelpers
    {
        /// <summary>
        /// Returns true if the path is a dir, false if it's a file and null if it's neither or doesn't exist. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool? IsDirFile(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path)) return null;
            var fileAttr = File.GetAttributes(path);
            return !fileAttr.HasFlag(FileAttributes.Directory);
        }
    }
}