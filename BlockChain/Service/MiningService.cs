using BlockChain.Models;

namespace BlockChain.Service
{
    public class MiningService
    {
        private readonly HashingService _hashingService;

        public MiningService(HashingService hashingService)
        {
            _hashingService = hashingService;
        }

        public long MineBlock(Block block, int difficulty)
        {
            var target = new String('0', difficulty);

            while (true)
            {
                block.Nonce++;
                block.Hash = _hashingService.ComputeHash(block);

                if (block.Nonce % 10000 == 0)
                {
                    Console.Write(".");
                }

                if (block.Hash.ToLowerInvariant().StartsWith(target))
                {
                    Console.WriteLine($"Block mined: {block.Hash} with nonce: {block.Nonce}");
                    return block.Nonce;
                }
            }
        }
    }
}