using System;
using System.Text;

namespace Eir.Common.Test.Extensions
{
    public static class EncodingExtensions
    {
        public static byte[] RemoveBom(this Encoding encoding, byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            byte[] bomBytes = encoding.GetPreamble();
            if ((bomBytes.Length == 0) || (bytes.Length < bomBytes.Length))
            {
                return bytes;
            }

            for (int i = 0; i < bomBytes.Length; i++)
            {
                if (bomBytes[i] != bytes[i])
                {
                    return bytes;
                }
            }

            var trimmedBytes = new byte[bytes.Length - bomBytes.Length];
            Array.Copy(bytes, bomBytes.Length, trimmedBytes, 0, trimmedBytes.Length);
            return trimmedBytes;
        }
    }
}