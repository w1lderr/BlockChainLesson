using BlockChain.Models;
using BlockChain.Service;

var displayService = new DisplayService();
var blockChainService = new BlockChainService(1);
var walletAlice = new Wallet(new CryptoService());
var walletBob = new Wallet(new CryptoService());

Console.WriteLine("--- Seeding Alice with 10 coins ---");
var seedTx = TransactionService.CreateTransaction("SYSTEM", walletAlice.PublicKey, 10, null);
blockChainService.AddTransaction(seedTx);
blockChainService.AddBlock();

Console.WriteLine($"Alice balance: {blockChainService.GetBalance(walletAlice.PublicKey)}");
Console.WriteLine($"Bob balance: {blockChainService.GetBalance(walletBob.PublicKey)}");

Console.WriteLine("\n--- Alice sends 10 coins to Bob (first tx) ---");
var tx1 = TransactionService.CreateTransaction(walletAlice.PublicKey, walletBob.PublicKey, 10, walletAlice.PrivateKey);
blockChainService.AddTransaction(tx1);
Console.WriteLine("First transaction added to Mempool.");

Console.WriteLine($"Alice balance (including pending): {blockChainService.GetBalance(walletAlice.PublicKey)}");

Console.WriteLine("\n--- Alice tries to send another 10 coins to Bob (double spend attempt) ---");
try
{
    var tx2 = TransactionService.CreateTransaction(walletAlice.PublicKey, walletBob.PublicKey, 10, walletAlice.PrivateKey);
    blockChainService.AddTransaction(tx2);
    Console.WriteLine("Second transaction added to Mempool (unexpected!).");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Rejected: {ex.Message}");
}

Console.WriteLine("\n--- Mining block with pending transactions ---");
blockChainService.AddBlock();

Console.WriteLine($"\nAlice final balance: {blockChainService.GetBalance(walletAlice.PublicKey)}");
Console.WriteLine($"Bob final balance: {blockChainService.GetBalance(walletBob.PublicKey)}");