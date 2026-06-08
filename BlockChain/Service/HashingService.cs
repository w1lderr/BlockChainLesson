using BlockChain.Models;
using System.Security.Cryptography;
using System.Text;

namespace BlockChain.Service
{
    public class HashingService
    {
        public string ComputeHash(Block block)
        {
            return ComputeHash(block, block.Nonce);
        }

        public string ComputeHash(Block block, long nonce)
        {
            string rawData = $"{block.Index}{block.Timestamp}{block.Data}{block.PrevHash}{block.Author}{nonce}";
            return ComputeHashString(rawData);
        }

        private string ComputeHashString(string rawData)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(rawData);
            byte[] hashBytes = SHA256.HashData(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}