using System.IO;
using UnityEngine;

namespace Physalia
{
    public static class FileUtility
    {
        public static string ProjectRoot
        {
            get
            {
                DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
                directory = directory.Parent;

                string rootPath = directory.FullName;
                rootPath = rootPath.Replace('\\', '/');
                return rootPath;
            }
        }

        public static string LoadTextFile(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate);
            using var reader = new StreamReader(fs);
            return reader.ReadToEnd();
        }

        public static void WriteTextFile(string filePath, string content)
        {
            using var fs = new FileStream(filePath, FileMode.Create);
            using var writer = new StreamWriter(fs);
            writer.Write(content);
        }
    }
}
