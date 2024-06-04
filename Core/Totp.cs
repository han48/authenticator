using ByteDev.Encoding;
using ByteDev.Encoding.Base32;
using QRCoder;

namespace Core
{
    public class Totp
    {

        /// <summary>
        /// Tạo URL cho QR Code để người dùng có thể quét bằng Application Authenticator
        /// </summary>
        /// <param name="base32Secret"></param>
        /// <param name="appName"></param>
        /// <param name="account"></param>
        /// <param name="issuer"></param>
        /// <returns>url</returns>
        public string CreateUrl(string base32Secret, string appName, string account, string? issuer = null)
        {
            if (issuer == null)
            {
                issuer = appName;
            }
            var url = $"otpauth://totp/{appName}:{account}?secret={base32Secret}&issuer={issuer}";
            return url;
        }

        /// <summary>
        /// Tạo QR Code để người dùng có thể quét bằng Application Authenticator
        /// </summary>
        /// <param name="base32Secret"></param>
        /// <param name="appName"></param>
        /// <param name="account"></param>
        /// <param name="issuer"></param>
        /// <returns>base64</returns>
        public string CreateBase64QrCode(string base32Secret, string appName, string account, string? issuer = null)
        {
            string url = CreateUrl(base32Secret, appName, account, issuer);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeImageAsBase64 = qrCode.GetGraphic(20);
            return qrCodeImageAsBase64;
        }

        /// <summary>
        /// Create base32 secret
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string CreateBase32Secret(string account)
        {
            string key = account;
            IEncoder encoder = new Base32Encoder();
            string base32 = encoder.Encode(key);
            return base32;
        }

        /// <summary>
        /// Create Time-based One-Time Password
        /// </summary>
        /// <param name="base32"></param>
        /// <returns></returns>
        public OtpNet.Totp GetTotp(string base32)
        {
            IEncoder encoder = new Base32Encoder();
            byte[] secretKey = encoder.DecodeToBytes(base32);
            OtpNet.Totp totp = new OtpNet.Totp(secretKey);
            return totp;
        }
    }
}