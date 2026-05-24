namespace BlockChain.Service
{
    public class DisplayService
    {
        public void DisplayChain(List<Models.Block> chain)
        {
            foreach (var block in chain)
            {
                Console.WriteLine($"Index: {block.Index}");
                Console.WriteLine($"Author: {block.Author}");
                Console.WriteLine($"Timestamp: {block.Timestamp}");
                Console.WriteLine($"Data: {block.Data}");
                Console.WriteLine($"Hash: {block.Hash}");
                Console.WriteLine($"Previous Hash: {block.PrevHash}");
                Console.WriteLine(new string('-', 50));
            }
        }
    }
}