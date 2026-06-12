namespace BlockChain.Models
{
    public class Block
    {
        // The index of the block in the blockchain
        public int Index { get; set; }
        public string Author { get; set; }

        // Date and time of the block creation
        public DateTime Timestamp { get; set; }
        public List<Transaction> Transactions { get; set; }

        // Hash of the current block, which is created on base of the data and other properties
        public string Hash { get; set; }
        public string PrevHash { get; set; }
        public int Nonce { get; set; }
        public int DifficultyAtMining { get; set; }

        public Block(int index, List<Transaction> transactions, string prevHash, DateTime timestamp)
        {
            Index = index;
            Timestamp = timestamp;
            Transactions = transactions;
            PrevHash = prevHash;
            Hash = "";
            Nonce++;
        }
    }
}