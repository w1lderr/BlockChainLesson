using BlockChain.Models;
using BlockChain.Service;
using System.Diagnostics;

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

Console.WriteLine("\n--- Mining Reward Halving Demonstration ---");
Console.WriteLine($"Block 2 reward: {blockChainService.MiningReward}");
for (int i = 0; i < 4; i++)
{
    blockChainService.AddBlock();
}
Console.WriteLine($"Block 6 reward (after 5 blocks): {blockChainService.MiningReward}");

Console.WriteLine($"\nTotal Supply: {blockChainService.GetTotalSupply()}");

Console.WriteLine("\n--- Snapshot & Rebuild Demonstration ---");
string snapshotPath = "balances_snapshot.json";
blockChainService.SaveSnapshot(snapshotPath);
Console.WriteLine("Snapshot saved.");

Console.WriteLine("Clearing state and rebuilding...");
blockChainService.RebuildState();
Console.WriteLine($"Alice balance after rebuild: {blockChainService.GetBalance(walletAlice.PublicKey)}");

blockChainService.BalancesState.Clear();
Console.WriteLine($"Alice balance after simulated loss: {blockChainService.GetBalance(walletAlice.PublicKey)}");

blockChainService.LoadSnapshot(snapshotPath);
Console.WriteLine($"Alice balance after snapshot load: {blockChainService.GetBalance(walletAlice.PublicKey)}");

if (System.IO.File.Exists(snapshotPath))
{
    System.IO.File.Delete(snapshotPath);
}

Console.WriteLine("\n--- Running Stress Test (10,000 blocks) ---");
var stressChainService = new BlockChainService(1);
var rand = new Random();
var key1 = "AliceStressKey";
var key2 = "BobStressKey";

for (int i = 0; i < 10000; i++)
{
    var txs = new List<Transaction>
    {
        new Transaction("SYSTEM", key1, 50) { Signature = new byte[] { 0 } },
        new Transaction(key1, key2, rand.Next(1, 10)) { Signature = new byte[] { 0 } }
    };
    var block = new Block(i + 1, txs, stressChainService.Chain.Last().Hash, DateTime.UtcNow)
    {
        Hash = Guid.NewGuid().ToString()
    };
    stressChainService.Chain.Add(block);
}

stressChainService.RebuildState();

var sw = new Stopwatch();

sw.Start();
var balanceLegacy = stressChainService.GetBalanceLegacy(key1);
sw.Stop();
var timeLegacy = sw.Elapsed.TotalMilliseconds;
Console.WriteLine($"Legacy Search (Loop) Balance: {balanceLegacy} | Time: {timeLegacy:F4} ms");

sw.Restart();
var balanceNew = stressChainService.GetBalance(key1);
sw.Stop();
var timeNew = sw.Elapsed.TotalMilliseconds;
Console.WriteLine($"New Search (Dictionary) Balance: {balanceNew} | Time: {timeNew:F4} ms");