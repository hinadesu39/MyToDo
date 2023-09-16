using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Extensions
{
    public static class StringExtensions
    {
        public static string GetMD5(this string obj)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var hash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(obj));
            return Convert.ToBase64String(hash);
        }
    }
}
