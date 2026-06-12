namespace BlockChain.Service
{
    public class DisplayService
    {
        public void PrintChain(List<Models.Block> chain)
        {
            DisplayChain(chain);
        }

        public void DisplayChain(List<Models.Block> chain)
        {
            foreach (var block in chain)
            {
                Console.WriteLine($"Index: {block.Index}");
                Console.WriteLine($"Timestamp: {block.Timestamp}");
                Console.WriteLine($"Hash: {block.Hash}");
                Console.WriteLine($"Previous Hash: {block.PrevHash}");
                Console.WriteLine(new string('-', 50));

                var transactions = block.Transactions;
                foreach (var transaction in transactions)
                {
                    Console.WriteLine($"    Transaction ID: {transaction.Id}");
                    Console.WriteLine($"    From: {transaction.From}");
                    Console.WriteLine($"    To: {transaction.To}");
                    Console.WriteLine($"    Amount: {transaction.Amount}");
                    Console.WriteLine($"    Timestamp: {transaction.Timestamp}");
                    Console.WriteLine(new string(' ', 4) + new string('-', 40));
                }
            }
        }
    }
}