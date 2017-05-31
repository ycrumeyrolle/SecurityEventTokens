using System;
using System.Text;

namespace AspNetCore.SecurityEventTokens
{
    /// <summary>
    /// Encodes and Decodes strings as Base64Url encoding.
    /// </summary>
    public static class Base64UrlEncoder
    {
        private static char base64PadCharacter = '=';
        private static string doubleBase64PadCharacter = "==";
        private static char base64Character62 = '+';
        private static char base64Character63 = '/';
        private static char base64UrlCharacter62 = '-';
        private static char base64UrlCharacter63 = '_';

        /// <summary>
        /// Performs base64url encoding which differs from regular base64 encoding as follows
        /// * padding is skipped so the pad character '=' doesn't have to be percent encoded
        /// * the 62nd and 63rd regular base64 encoding characters ('+' and '/') are replace with ('-' and '_')
        /// The changes make the encoding alphabet file and URL safe.
        /// </summary>
        /// <param name="value">string to encode.</param>
        /// <returns>Base64Url encoding of the UTF8 bytes.</returns>
        public static string Encode(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return Encode(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Converts a subset of an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-64-url digits. Parameters specify
        /// the subset as an offset in the input array, and the number of elements in the array to convert.
        /// </summary>
        /// <param name="value">An array of 8-bit unsigned integers.</param>
        /// <param name="length">An offset in inArray.</param>
        /// <param name="offset">The number of elements of inArray to convert.</param>
        /// <returns>The string representation in base 64 url encodingof length elements of inArray, starting at position offset.</returns>
        /// <exception cref="ArgumentNullException">'inArray' is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">offset or length is negative OR offset plus length is greater than the length of inArray.</exception>
        public static string Encode(byte[] value, int offset, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string s = Convert.ToBase64String(value, offset, length);
            s = s.Split(base64PadCharacter)[0]; // Remove any trailing padding
            s = s.Replace(base64Character62, base64UrlCharacter62);  // 62nd char of encoding
            s = s.Replace(base64Character63, base64UrlCharacter63);  // 63rd char of encoding
            return s;
        }

        /// <summary>
        /// Converts a subset of an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-64-url digits. Parameters specify
        /// the subset as an offset in the input array, and the number of elements in the array to convert.
        /// </summary>
        /// <param name="value">An array of 8-bit unsigned integers.</param>
        /// <returns>The string representation in base 64 url encodingof length elements of inArray, starting at position offset.</returns>
        /// <exception cref="ArgumentNullException">'inArray' is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">offset or length is negative OR offset plus length is greater than the length of inArray.</exception>
        public static string Encode(byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string s = Convert.ToBase64String(value, 0, value.Length);
            s = s.Split(base64PadCharacter)[0]; // Remove any trailing padding
            s = s.Replace(base64Character62, base64UrlCharacter62);  // 62nd char of encoding
            s = s.Replace(base64Character63, base64UrlCharacter63);  // 63rd char of encoding

            return s;
        }

        /// <summary>
        ///  Converts the specified string, which encodes binary data as base-64-url digits, to an equivalent 8-bit unsigned integer array.</summary>
        /// <param name="value">base64Url encoded string.</param>
        /// <returns>UTF8 bytes.</returns>
        public static byte[] DecodeBytes(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // 62nd char of encoding
            value = value.Replace(base64UrlCharacter62, base64Character62);

            // 63rd char of encoding
            value = value.Replace(base64UrlCharacter63, base64Character63);

            // check for padding
            switch (value.Length % 4)
            {
                case 0:
                    // No pad chars in this case
                    break;
                case 2:
                    // Two pad chars
                    value += doubleBase64PadCharacter;
                    break;
                case 3:
                    // One pad char
                    value += base64PadCharacter;
                    break;
                default:
                    throw new FormatException(value);
            }

            return Convert.FromBase64String(value);
        }

        /// <summary>
        /// Decodes the string from Base64UrlEncoded to UTF8.
        /// </summary>
        /// <param name="value">string to decode.</param>
        /// <returns>UTF8 string.</returns>
        public static string Decode(string value)
        {
            return Encoding.UTF8.GetString(DecodeBytes(value));
        }
    }
}
