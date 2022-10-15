using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CucuTools.IO
{
    /// <summary>
    /// Simple read/write files utility
    /// </summary>
    public class FileUtility
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static FileUtility Singleton { get; }

        static FileUtility()
        {
            Singleton = new FileUtility(nameof(Singleton), Encoding.Default);
        }
        
        /// <summary>
        /// Encoding
        /// </summary>
        public Encoding Encoding => _encoding;
        
        /// <summary>
        /// Name
        /// </summary>
        public string Name => _name;

        private Encoding _encoding;
        private string _name;

        public FileUtility(string name, Encoding encoding = null)
        {
            _name = name ?? "";
            _encoding = encoding ?? Encoding.Default;
        }

        public FileUtility(Encoding encoding) : this(encoding?.HeaderName, encoding)
        {
        }

        /// <summary>
        /// Exist file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Existence</returns>
        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }
        
        #region CRUD
        
        /// <summary>
        /// Create new file
        /// </summary>
        /// <param name="filePath">File path</param>
        public void CreateNew(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.CreateNew))
            {
            }
        }

        /// <summary>
        /// Create new file with content
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">Content</param>
        public void Create(string filePath, byte[] content)
        {
            using (var fs = new FileStream(filePath, FileMode.CreateNew))
            {
                fs.Write(content, 0, content.Length);
            }
        }
        
        /// <summary>
        /// Create new file with content
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">Content</param>
        public void Create(string filePath, string content)
        {
            Create(filePath, Encoding.GetBytes(content));
        }
        
        /// <summary>
        /// Create new file async with content
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">Content</param>
        public async Task CreateAsync(string filePath, byte[] content)
        {
            using (var fs = new FileStream(filePath, FileMode.CreateNew))
            {
                await fs.WriteAsync(content, 0, content.Length);
            }
        }
        
        /// <summary>
        /// Create new file async with content
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">Content</param>
        public async Task CreateAsync(string filePath, string content)
        {
            await CreateAsync(filePath, Encoding.GetBytes(content));
        }

        /// <summary>
        /// Read file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Content</returns>
        public byte[] Read(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                var bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        /// <summary>
        /// Read file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>String content</returns>
        public string ReadString(string filePath)
        {
            return Encoding.GetString(Read(filePath));
        }
        
        /// <summary>
        /// Read file async
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Content</returns>
        public async Task<byte[]> ReadAsync(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                var bytes = new byte[fs.Length];
                await fs.ReadAsync(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        /// <summary>
        /// Read file async
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>String content</returns>
        public async Task<string> ReadStringAsync(string filePath)
        {
            return Encoding.GetString(await ReadAsync(filePath));
        }

        /// <summary>
        /// Write content in existing file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">Content</param>
        public void Write(string filePath, byte[] content)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                fs.Write(content, 0, content.Length);
            }
        }

        /// <summary>
        /// Write content in existing file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">Content</param>
        public void Write(string filePath, string content)
        {
            Write(filePath, Encoding.GetBytes(content));
        }
        
        /// <summary>
        /// Write content async in existing file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">Content</param>
        public async Task WriteAsync(string filePath, byte[] content)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                await fs.WriteAsync(content, 0, content.Length);
            }
        }

        /// <summary>
        /// Write content async in existing file. 
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">Content</param>
        public async Task WriteAsync(string filePath, string content)
        {
            await WriteAsync(filePath, Encoding.GetBytes(content));
        }

        /// <summary>
        /// Append file. File will be created if it does not exist.
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">Content</param>
        public void Append(string filePath, byte[] content)
        {
            using (var fs = new FileStream(filePath, FileMode.Append))
            {
                fs.Write(content, 0, content.Length);
            }
        }

        /// <summary>
        /// Append file. File will be created if it does not exist.
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">Content</param>
        public void Append(string filePath, string content)
        {
            Append(filePath, Encoding.GetBytes(content));
        }
        
        /// <summary>
        /// Append file async. File will be created if it does not exist.
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">Content</param>
        public async Task AppendAsync(string filePath, byte[] content)
        {
            using (var fs = new FileStream(filePath, FileMode.Append))
            {
                await fs.WriteAsync(content, 0, content.Length);
            }
        }

        /// <summary>
        /// Append file async. File will be created if it does not exist.
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">Content</param>
        public async Task AppendAsync(string filePath, string content)
        {
            await AppendAsync(filePath, Encoding.GetBytes(content));
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="filePath">File path</param>
        public void Delete(string filePath)
        {
            File.Delete(filePath);
        }
        
        /// <summary>
        /// Delete files
        /// </summary>
        /// <param name="filesPaths">Files paths</param>
        public void Delete(params string[] filesPaths)
        {
            foreach (var filePath in filesPaths)
                Delete(filePath);
        }
        
        #endregion
    }
    
    public static class FileUtilityExt
    {
        public static string FileExt(this string fileName, string ext)
        {
            if (!ext.StartsWith(".")) ext = $".{ext}";

            if (!fileName.EndsWith(ext)) fileName = $"{fileName}{ext}";

            return fileName;
        }
    }
}