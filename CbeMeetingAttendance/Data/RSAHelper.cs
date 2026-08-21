using System.Security.Cryptography;

namespace CbeMeetingAttendance.Data
{
    public static class RSAHelper
    {
        private static readonly RSA _rsa;

        static RSAHelper()
        {
            // Generate a 2048-bit RSA key pair on startup
            _rsa = RSA.Create(2048);
        }

        /// <summary>
        /// Returns the Public Key in the standard PEM format that frontend 
        /// libraries (like JS Web Crypto API or node-forge) expect.
        /// </summary>
        public static string GetPublicKey()
        {
            // ExportSubjectPublicKeyInfoPem is available in .NET 5+ 
            return _rsa.ExportSubjectPublicKeyInfoPem();
        }

        /// <summary>
        /// You will use this method later in your Login controller 
        /// to unlock the AES key the frontend sends you.
        /// </summary>
        public static byte[] DecryptWithPrivateKey(byte[] encryptedAesKey)
        {
            // OaepSHA256 is the modern, secure padding standard
            return _rsa.Decrypt(encryptedAesKey, RSAEncryptionPadding.OaepSHA256);
        }
    }
}