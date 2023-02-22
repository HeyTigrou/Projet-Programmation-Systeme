using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Client
{
    public static class Serializer
    {
        public static byte[] Serialize(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }

        public static string Deserialize(byte[] buffer, int count)
        {
            return Encoding.UTF8.GetString(buffer, 0, count);
        }
    }
}
