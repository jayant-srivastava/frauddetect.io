using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace frauddetect.common.core.convertor
{
    public sealed class Convertor
    {
        public string SerializeToJSON(object obj, Type type)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
            string json = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, obj);
                json = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return json;
        }

        public T DeserializeFromJSON<T>(string json)
        {
            T obj = default(T);

            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                obj = (T)serializer.ReadObject(memoryStream);
            }

            return obj;
        }
    }
}
