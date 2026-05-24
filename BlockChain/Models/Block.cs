namespace BlockChain.Models
{
    public class Block
    {
        // The index of the block in the blockchain
        public int Index { get; set; }
        public string Author { get; set; }

        // Date and time of the block creation
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }

        // Hash of the current block, which is created on base of the data and other properties
        public string Hash { get; set; }
        public string PrevHash { get; set; }

        public Block(int index, string author, string data, string prevHash, DateTime timestamp)
        {
            Index = index;
            Author = author;
            Timestamp = timestamp;
            Data = data;
            PrevHash = prevHash;
            Hash = "";
        }

        public Block()
        {

        }
    }
}