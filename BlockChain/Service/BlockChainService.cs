using BlockChain.Models;

namespace BlockChain.Service
{
    public class BlockChainService
    {
        public List<Block> Chain { get; set; }
        private HashingService _hashingService;
        private MiningService _miningService;

        private string Prefix = "c0ffee";

        public BlockChainService(string prefix)
        {
            _hashingService = new HashingService();
            _miningService = new MiningService(_hashingService);
            Chain = new List<Block>();
            AddGenesisBlock();
            this.Prefix = prefix;
        }

        private void AddGenesisBlock()
        {
            var block = new Block(0, "Satoshi Nakamoto :)", "Genesis Block", "0", DateTime.Parse("2024-06-01T00:00:00Z"));
            block.Hash = _hashingService.ComputeHash(block);
            Chain.Add(block);
        }

        public void AddBlock(string data, string author)
        {
            var lastBlock = Chain.Last();
            var newBLock = new Block(lastBlock.Index + 1, author, data, lastBlock.Hash, DateTime.UtcNow);
            newBLock.Hash = _hashingService.ComputeHash(newBLock);

            _miningService.MineBlock(newBLock, Prefix);
            Chain.Add(newBLock);
        }

        public bool IsValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                var currentBlock = Chain[i];
                var prevBlock = Chain[i - 1];
                if (currentBlock.Hash != _hashingService.ComputeHash(currentBlock))
                {
                    return false;
                }
                if (currentBlock.PrevHash != prevBlock.Hash)
                {
                    return false;
                }
                if(!currentBlock.Hash.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
