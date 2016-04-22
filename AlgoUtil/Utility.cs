using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Util
{
    public static class Utility
    {
        public static string GenComponentUri(Type type)
        {
            return "/" + type.Assembly.FullName + ";component";
        }
               
        public static byte[] GenMD5(byte[] input, uint round)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] output = input;

            for (uint i = 0; i < round; i++)
                output = md5.ComputeHash(output);

            return output;
        }

        public static string GenMD5String(string rawString, uint round, Encoding encoder)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] md5bytes = encoder.GetBytes(rawString);

            md5bytes = GenMD5(md5bytes, round);

            StringBuilder result = new StringBuilder(md5bytes.Length * 2);

            for (int i = 0; i < md5bytes.Length; i++)
                result.Append(md5bytes[i].ToString("x2"));

            return result.ToString();
        }

        public static string GenMD5String(string rawString, uint round)
        {
            return GenMD5String(rawString, round, Encoding.UTF8);
        }

    }
}
