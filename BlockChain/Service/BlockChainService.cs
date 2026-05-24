using BlockChain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain.Service
{
    public class BlockChainService
    {
        public List<Block> Chain { get; set; }
        private HashingService _hashingService;
        public BlockChainService()
        {
            _hashingService = new HashingService();
            Chain = new List<Block>();
            AddGenesisBlock();
        }

        private void AddGenesisBlock()
        {
            var block = new Block(0, "Genesis Block", "0", DateTime.Parse("2024-06-01T00:00:00Z"));
            block.Hash = _hashingService.ComputeHash(block);
            Chain.Add(block);
        }

        public void AddBlock(string data)
        {
            var lastBlock = Chain.Last();
            var newBLock = new Block(lastBlock.Index + 1, data, lastBlock.Hash, DateTime.UtcNow);
            newBLock.Hash = _hashingService.ComputeHash(newBLock);
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
            }
            return true;
        }
    }
}
