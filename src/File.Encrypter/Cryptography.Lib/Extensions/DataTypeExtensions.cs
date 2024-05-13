using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptography.Lib.Extensions
{
    public static class DataTypeExtensions
    {
        public static string BytesArrayToString(this byte[] bytes)
        {
            var result = string.Empty;

            result = Convert.ToBase64String(bytes);

            return result;
        }
    }
}
