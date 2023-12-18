using System.Text;
using System.Security.Cryptography;

namespace MVScript
{
    public class ScriptOption
    {
        private byte[] _key;
        private byte[] _nonce;

        public ScriptOption(string password, string secret, int iterations = 10000)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
            using (MD5 md5 = MD5.Create())
            using (var pbkdf2 = new Rfc2898DeriveBytes(md5.ComputeHash(passwordBytes), md5.ComputeHash(secretBytes), iterations))
            {
                _key = pbkdf2.GetBytes(32);
                _nonce = pbkdf2.GetBytes(12);
            }
        }

        internal byte[] Encrypt(string plain)
        {
            // Get bytes of plaintext string
            ReadOnlySpan<byte> plainBytes = Encoding.UTF8.GetBytes(plain).AsSpan();

            // Get parameter sizes
            int tagSize = AesGcm.TagByteSizes.MaxSize;
            int cipherSize = plainBytes.Length;

            // We write everything into one big array for easier encoding
            int encryptedDataLength = tagSize + cipherSize;
            Span<byte> encryptedData = encryptedDataLength < 1024
                                     ? stackalloc byte[encryptedDataLength]
                                     : new byte[encryptedDataLength].AsSpan();

            // Copy parameters
            var tag = encryptedData.Slice(0, tagSize);
            var cipherBytes = encryptedData.Slice(tagSize, cipherSize);

            // Encrypt
            using var aes = new AesGcm(_key);
            aes.Encrypt(_nonce, plainBytes, cipherBytes, tag);

            return encryptedData.ToArray();
        }

        internal string Decrypt(byte[] cipher)
        {
            // Decode
            Span<byte> encryptedData = cipher.AsSpan();

            // Get parameter sizes
            int tagSize = AesGcm.TagByteSizes.MaxSize;
            int cipherSize = encryptedData.Length - tagSize;

            // Extract parameters
            var tag = encryptedData.Slice(0, tagSize);
            var cipherBytes = encryptedData.Slice(tagSize, cipherSize);

            // Decrypt
            Span<byte> plainBytes = cipherSize < 1024
                                  ? stackalloc byte[cipherSize]
                                  : new byte[cipherSize];

            using var aes = new AesGcm(_key);
            aes.Decrypt(_nonce, cipherBytes, tag, plainBytes);

            // Convert plain bytes back into string
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
