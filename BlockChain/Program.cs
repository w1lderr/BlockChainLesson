using BlockChain.Service;

var displayService = new DisplayService();
var blockChainService = new BlockChainService(1);

blockChainService.AddBlock("Alice pays Bob 10 BTC", "Alice");
blockChainService.AddBlock("Toma toma pays Amigo 1 BTC", "Toma toma");
blockChainService.AddBlock("Charlie King pays Dave 7 BTC", "Charlie King");
blockChainService.AddBlock("Dave pays Eva 1 BTC", "Dave");

displayService.DisplayChain(blockChainService.Chain);

blockChainService.PrintDifficultyHistory();

var isValid = blockChainService.IsValid();
if (isValid == "true")
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Blockchain is valid.");
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Blockchain is invalid.");
    Console.WriteLine("Error details: " + isValid);
}
Console.ResetColor();

Console.WriteLine("\n--- Mining time benchmark by difficulty ---");
foreach (int d in new int[] { 1, 2, 3, 4 })
{
    Console.WriteLine($"\nDifficulty: {d}");
    var sw = System.Diagnostics.Stopwatch.StartNew();
    var chain = new BlockChainService(d);
    chain.AddBlock("Alice pays Bob 10 BTC", "Alice");
    sw.Stop();
    Console.WriteLine($"Time to mine block with difficulty {d}: {sw.ElapsedMilliseconds} ms");
}

Console.WriteLine("\n--- Long chain: watch proportional difficulty adaptation ---");
var adaptiveChain = new BlockChainService(2);
string[] authors = { "Alice", "Bob", "Charlie", "Dave", "Eve", "Frank", "Grace" };
string[] txs = { "10 BTC", "5 ETH", "3 BTC", "2 ETH", "7 BTC", "1 BTC", "4 ETH" };

for (int i = 0; i < authors.Length; i++)
{
    adaptiveChain.AddBlock($"{authors[i]} pays {txs[i]}", authors[i]);
}

adaptiveChain.PrintDifficultyHistory();