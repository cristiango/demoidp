using System.Security.Cryptography;
using System.Text;

namespace demoidp
{
    public static class StringExtensions
    {
        public static Guid ToDeterministicGuid(this string value)
        {
            var provider = MD5.Create();

            var inputBytes = Encoding.Default.GetBytes(value);

            var hashBytes = provider.ComputeHash(inputBytes);

            //generate a guid from the hash:

            var hashGuid = new Guid(hashBytes);

            return hashGuid;
        }
    }
}
