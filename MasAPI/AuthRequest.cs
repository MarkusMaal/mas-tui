using MasTUICommon;
using System.Security.Cryptography;
using System.Text;

namespace MasAPI
{
    internal class AuthRequest
    {
        private static readonly string masRoot = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");
        public static bool CheckAuth(string password)
        {
            if (File.Exists(Path.Join(masRoot, "server_auth.txt")))
            {
                var src = File.ReadAllText(Path.Combine(masRoot, "server_auth.txt"));
                var edition = new Edition();

                var checkStr = "";
                foreach (var b in SHA256.HashData(Encoding.BigEndianUnicode.GetBytes(masRoot + password + edition.Pin + edition.Username)))
                {
                    checkStr += b.ToString("X2");
                }
                return checkStr.Equals(src);
            }
            else
            {
                return true;
            }
        }
    }
}
