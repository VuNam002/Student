using System.Security.Cryptography;
using System.Text;

namespace Student_management.Helpers
{
    public static class HashHelper
    {
        /// <summary>
        /// Tính toán MD5 hash của chuỗi đầu vào
        /// </summary>
        /// <param name="input">Chuỗi cần hash</param>
        /// <returns>Chuỗi hash MD5 dạng hex (lowercase)</returns>
        public static string ComputeMd5Hash(string? input)
        {
            input ??= string.Empty;

            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// So sánh mật khẩu đã hash với hash được lưu trữ
        /// </summary>
        /// <param name="plainPassword">Mật khẩu thô</param>
        /// <param name="hashedPassword">Mật khẩu đã hash</param>
        /// <returns>True nếu khớp, False nếu không khớp</returns>
        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword) || string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            var computedHash = ComputeMd5Hash(plainPassword);
            return string.Equals(computedHash, hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}