using BlockChain.Service;

var displayService = new DisplayService();
var blockChainService = new BlockChainService();

blockChainService.AddBlock("Alice pays Bob 10 BTC", "Alice");
blockChainService.AddBlock("Toma toma pays Amigo 1 BTC", "Toma toma");
blockChainService.AddBlock("Charlie King pays Dave 7 BTC", "Charlie King");
blockChainService.AddBlock("Dave pays Eva 1 BTC", "Dave");

displayService.DisplayChain(blockChainService.Chain);

if (blockChainService.IsValid())
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Blockchain is valid.");
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Blockchain is invalid.");
}