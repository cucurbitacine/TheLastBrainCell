using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace CucuTools.Serialization
{
    /// <summary>
    /// Serialization Core
    /// </summary>
    public abstract class Serializator
    {
        public static Serializator Current { get; } = new SerilizatorByJsonUtility();
        
        public abstract byte[] Serialize<T>(T t);
        public abstract T Deserialize<T>(byte[] bytes);
    }

    public class SerilizatorByMemoryStream : Serializator
    {
        public override byte[] Serialize<T>(T t)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, t);
                return ms.ToArray();
            }
        }

        public override T Deserialize<T>(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return (T) new BinaryFormatter().Deserialize(ms);
            }
        }
    }

    /// <summary>
    /// Serialization: Object -> Json (by JsonUtility) -> byte[] (by Encoding)
    /// </summary>
    public class SerilizatorByJsonUtility : Serializator
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8; 
        
        public override byte[] Serialize<T>(T t)
        {
            return Encoding.GetBytes(JsonUtility.ToJson(t));
        }

        public override T Deserialize<T>(byte[] bytes)
        {
            return JsonUtility.FromJson<T>(Encoding.GetString(bytes));
        }
    }
}