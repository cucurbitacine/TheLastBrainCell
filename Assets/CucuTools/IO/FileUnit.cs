using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CucuTools.IO
{
    public class FileUnit
    {
        public DirectoryUnit DirectoryUnit
        {
            get => _directoryUnit;
            set => _directoryUnit = value;
        }

        public string FileName
        {
            get => _fileName ?? (_fileName = "");
            set => _fileName = value;
        }
        
        private DirectoryUnit _directoryUnit;
        private string _fileName;

        public FileUnit(string fileName, DirectoryUnit directoryUnit)
        {
            _fileName = Path.GetFileName(fileName);
            _directoryUnit = directoryUnit;
        }

        public FileUnit(DirectoryUnit directoryUnit) : this("", directoryUnit)
        {
        }

        public FileUnit(string path) : this(Path.GetFileName(path), new DirectoryUnit(Path.GetDirectoryName(path)))
        {
        }
        
        public bool Exists()
        {
            return DirectoryUnit.ExistsFile(FileName);
        }

        public string GetExt()
        {
            if (!FileName.Contains(".")) return null;
            return FileName.Split('.').LastOrDefault();
        }
        
        public void SetExt(string ext)
        {
            FileName = FileName.FileExt(ext);
        }
        
        #region CRUD

        public void CreateNew()
        {
            DirectoryUnit.CreateNewFile(FileName);
        }

        public void Create(byte[] content)
        {
            DirectoryUnit.CreateFile(FileName, content);
        }

        public void Create(string content)
        {
            DirectoryUnit.CreateFile(FileName, content);
        }
        
        public async Task CreateAsync(byte[] content)
        {
            await DirectoryUnit.CreateFileAsync(FileName, content);
        }

        public async Task CreateAsync(string content)
        {
            await DirectoryUnit.CreateFileAsync(FileName, content);
        }

        public byte[] Read()
        {
            return DirectoryUnit.ReadFile(FileName);
        }

        public string ReadString()
        {
            return DirectoryUnit.ReadFileString(FileName);
        }
        
        public async Task<byte[]> ReadAsync()
        {
            return await DirectoryUnit.ReadFileAsync(FileName);
        }

        public async Task<string> ReadStringAsync()
        {
            return await DirectoryUnit.ReadFileStringAsync(FileName);
        }

        public void Write(byte[] content)
        {
            DirectoryUnit.WriteFile(FileName, content);
        }

        public void Write(string content)
        {
            DirectoryUnit.WriteFile(FileName, content);
        }
        
        public async Task WriteAsync(byte[] content)
        {
            await DirectoryUnit.WriteFileAsync(FileName, content);
        }

        public async Task WriteAsync(string content)
        {
            await DirectoryUnit.WriteFileAsync(FileName, content);
        }

        public void Append(byte[] content)
        {
            DirectoryUnit.AppendFile(FileName, content);
        }

        public void Append(string content)
        {
            DirectoryUnit.AppendFile(FileName, content);
        }
        
        public async Task AppendAsync(byte[] content)
        {
            await DirectoryUnit.AppendFileAsync(FileName, content);
        }

        public async Task AppendAsync(string content)
        {
            await DirectoryUnit.AppendFileAsync(FileName, content);
        }

        public void Delete()
        {
            DirectoryUnit.DeleteFile(FileName);
        }

        #endregion
    }
}