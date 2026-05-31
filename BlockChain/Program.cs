using BlockChain.Service;

var displayService = new DisplayService();
var blockChainService = new BlockChainService(1);

blockChainService.AddBlock("Alice pays Bob 10 BTC", "Alice");
blockChainService.AddBlock("Toma toma pays Amigo 1 BTC", "Toma toma");
blockChainService.AddBlock("Charlie King pays Dave 7 BTC", "Charlie King");
blockChainService.AddBlock("Dave pays Eva 1 BTC", "Dave");

displayService.DisplayChain(blockChainService.Chain);

var isValid = blockChainService.IsValid();

if (isValid == "true")
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(isValid);
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Blockchain is invalid.");
    Console.WriteLine("Error details: " + isValid);
}

foreach (int d in new int[] { 1, 2, 3, 4, 5 })
{
    Console.WriteLine(d);
    var sw = System.Diagnostics.Stopwatch.StartNew();
    var blockchain = new BlockChainService(d);
    blockchain.AddBlock("Alice pays Bob 10 BTC", "Alice");
    sw.Stop();
    Console.WriteLine($"Time taken to mine a block with difficulty {d}: {sw.ElapsedMilliseconds} ms");
}