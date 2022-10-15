using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace CucuTools.IO
{
    public class DirectoryUnit
    {
        public static DirectoryUnit StreamingAssets { get; }
        public static DirectoryUnit DataPath { get; }
        public static DirectoryUnit PersistentData { get; }

        static DirectoryUnit()
        {
            StreamingAssets = new DirectoryUnit(Application.streamingAssetsPath);
            DataPath = new DirectoryUnit(Application.dataPath);
            PersistentData = new DirectoryUnit(Application.persistentDataPath);
        }

        public FileUtility FileUtility
        {
            get => _fileUtility;
            set => _fileUtility = value;
        }

        public string DirectoryPath
        {
            get => _directoryPath;
            set => _directoryPath = value;
        }

        private FileUtility _fileUtility;
        private string _directoryPath;

        public DirectoryUnit(string directoryPath, FileUtility fileUtility)
        {
            _directoryPath = directoryPath;
            _fileUtility = fileUtility ?? FileUtility.Singleton;
        }

        public DirectoryUnit(string directoryPath) : this(directoryPath, null)
        {
        }

        public DirectoryUnit() : this("", null)
        {
        }

        public bool Exists()
        {
            return DirectoryExists(DirectoryPath);
        }

        public bool ExistsFile(string fileName)
        {
            return Exists() && FileUtility.Exists(GetFilePath(fileName));
        }

        public DirectoryInfo CreateDirectory()
        {
            return Directory.CreateDirectory(DirectoryPath);
        }

        #region CRUD

        public void CreateNewFile(string fileName)
        {
            if (!Exists()) CreateDirectory();
            FileUtility.CreateNew(GetFilePath(fileName));
        }

        public void CreateFile(string fileName, byte[] content)
        {
            if (!Exists()) CreateDirectory();
            FileUtility.Create(GetFilePath(fileName), content);
        }

        public void CreateFile(string fileName, string content)
        {
            if (!Exists()) CreateDirectory();
            FileUtility.Create(GetFilePath(fileName), content);
        }
        
        public async Task CreateFileAsync(string fileName, byte[] content)
        {
            if (!Exists()) CreateDirectory();
            await FileUtility.CreateAsync(GetFilePath(fileName), content);
        }

        public async Task CreateFileAsync(string fileName, string content)
        {
            if (!Exists()) CreateDirectory();
            await FileUtility.CreateAsync(GetFilePath(fileName), content);
        }

        public byte[] ReadFile(string fileName)
        {
            if (!ExistsFile(fileName)) return null;
            return FileUtility.Read(GetFilePath(fileName));
        }

        public string ReadFileString(string fileName)
        {
            if (!ExistsFile(fileName)) return null;
            return FileUtility.ReadString(GetFilePath(fileName));
        }
        
        public async Task<byte[]> ReadFileAsync(string fileName)
        {
            if (!ExistsFile(fileName)) return null;
            return await FileUtility.ReadAsync(GetFilePath(fileName));
        }

        public async Task<string> ReadFileStringAsync(string fileName)
        {
            if (!ExistsFile(fileName)) return null;
            return await FileUtility.ReadStringAsync(GetFilePath(fileName));
        }

        public void WriteFile(string fileName, byte[] content)
        {
            if (!ExistsFile(fileName)) return;
            FileUtility.Write(GetFilePath(fileName), content);
        }

        public void WriteFile(string fileName, string content)
        {
            if (!ExistsFile(fileName)) return;
            FileUtility.Write(GetFilePath(fileName), content);
        }
        
        public async Task WriteFileAsync(string fileName, byte[] content)
        {
            if (!ExistsFile(fileName)) return;
            await FileUtility.WriteAsync(GetFilePath(fileName), content);
        }

        public async Task WriteFileAsync(string fileName, string content)
        {
            if (!ExistsFile(fileName)) return;
            await FileUtility.WriteAsync(GetFilePath(fileName), content);
        }

        public void AppendFile(string fileName, byte[] content)
        {
            if (!Exists()) CreateDirectory();
            FileUtility.Append(GetFilePath(fileName), content);
        }

        public void AppendFile(string fileName, string content)
        {
            if (!Exists()) CreateDirectory();
            FileUtility.Append(GetFilePath(fileName), content);
        }
        
        public async Task AppendFileAsync(string fileName, byte[] content)
        {
            if (!Exists()) CreateDirectory();
            await FileUtility.AppendAsync(GetFilePath(fileName), content);
        }

        public async Task AppendFileAsync(string fileName, string content)
        {
            if (!Exists()) CreateDirectory();
            await FileUtility.AppendAsync(GetFilePath(fileName), content);
        }

        public void DeleteFile(string fileName)
        {
            if (!Exists()) return;
            FileUtility.Delete(GetFilePath(fileName));
        }
        
        public void DeleteFiles(params string[] filesNames)
        {
            foreach (var fileName in filesNames)
                DeleteFile(fileName);
        }

        #endregion

        public string GetFilePath(string fileName)
        {
            return GetFilePath(this, fileName);
        }

        public static string GetFilePath(DirectoryUnit directoryUnit, string fileName)
        {
            return Path.Combine(directoryUnit.DirectoryPath, Path.GetFileName(fileName));
        }

        public static bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
    }
}