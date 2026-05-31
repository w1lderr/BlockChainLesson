using BlockChain.Models;

namespace BlockChain.Service
{
    public class BlockChainService
    {
        public List<Block> Chain { get; set; }
        private HashingService _hashingService;
        private MiningService _miningService;

        private int Difficulty = 1;

        public BlockChainService(int difficulty)
        {
            _hashingService = new HashingService();
            _miningService = new MiningService(_hashingService);
            Chain = new List<Block>();
            AddGenesisBlock();
            this.Difficulty = difficulty;
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

            _miningService.MineBlock(newBLock, Difficulty);
            Chain.Add(newBLock);
        }

        public string IsValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                var currentBlock = Chain[i];
                var prevBlock = Chain[i - 1];

                if (i == 2)
                {
                    currentBlock.Data = "some trash data^^&$%^^&&^*%*&^";
                }

                if (i == 3)
                {
                    currentBlock.Hash = "354792959250dfgsdufkgsdjfkhbhljkgo94ilkal%^&*(*&^%$#$%^&*(";
                }

                if (i == 4)
                {
                    currentBlock.PrevHash = "834925467925)(*&^%$#@#$^&*(*$%^&*(*%^&*";
                }

                if (currentBlock.Hash != _hashingService.ComputeHash(currentBlock))
                {
                    return $"Error in block №[{i}]: The hash does not match the block data (Data/Timestamp/Nonce has been altered).";
                }
                if (currentBlock.PrevHash != prevBlock.Hash)
                {
                    return $"Error in block №[{i}]: The chain is broken (PreviousHash does not match the hash of the previous block).";
                }
                if (!currentBlock.Hash.StartsWith(new String('0', Difficulty)))
                {
                    return $"Error in block №[{i}]: The hash does not meet the current difficulty ({Difficulty}).";
                }
            }
            return "true";
        }
    }
}