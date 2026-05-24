using BlockChain.Models;
using BlockChain.Service;

var displayService = new DisplayService();
var blockChainService = new BlockChainService();

blockChainService.AddBlock("Alice pays Bob 10 BTC");
blockChainService.AddBlock("Toma toma pays Amigo 1 BTC");
blockChainService.AddBlock("Charlie King pays Dave 7 BTC");
blockChainService.AddBlock("Dave pays Eva 1 BTC");

displayService.DisplayChain(blockChainService.Chain);

if(blockChainService.IsValid())
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Blockchain is valid.");
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Blockchain is invalid.");
}