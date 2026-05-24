using BlockChain.Models;
using System.Security.Cryptography;
using System.Text;

namespace BlockChain.Service
{
    public class HashingService
    {
        public string ComputeHash(Block block)
        {
            string rawData = $"{block.Index}{block.Timestamp}{block.Data}{block.PrevHash}{block.Author}";
            return ComputeHash(rawData);
        }

        private string ComputeHash(string rawData)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(rawData);
            byte[] hashBytes = SHA256.HashData(inputBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
