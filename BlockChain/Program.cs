using BlockChain.Models;
using BlockChain.Service;

var cryptoService = new CryptoService();
var walletAlice = new Wallet(cryptoService);
var walletBob = new Wallet(cryptoService);

Console.WriteLine("==================================================");
Console.WriteLine("TESTING BLOCKCHAIN PROTECTIVE MECHANISMS");
Console.WriteLine("==================================================");

// 1. STATE RECOVERY & VALIDATION TEST
Console.WriteLine("\n--- Scenario 1: State Rebuild and Validation ---");
var bcs1 = new BlockChainService(1);

// Seed Alice with 100 coins
var seedTx = TransactionService.CreateTransaction("SYSTEM", walletAlice.PublicKey, 100, null);
bcs1.AddTransaction(seedTx);
bcs1.AddBlock();

// Alice transfers 40 coins to Bob
var transferTx = TransactionService.CreateTransaction(walletAlice.PublicKey, walletBob.PublicKey, 40, walletAlice.PrivateKey, fee: 1.0m);
bcs1.AddTransaction(transferTx);
bcs1.AddBlock();

Console.WriteLine($"[Before Rebuild] Alice: {bcs1.GetBalance(walletAlice.PublicKey)}, Bob: {bcs1.GetBalance(walletBob.PublicKey)}");
bool isValid = bcs1.ValidateAndRebuildState();
Console.WriteLine($"[Rebuild Valid Chain] Status: {isValid}");
Console.WriteLine($"[After Rebuild] Alice: {bcs1.GetBalance(walletAlice.PublicKey)}, Bob: {bcs1.GetBalance(walletBob.PublicKey)}");

// Inject invalid block (Alice transfers 1000 coins without funds)
var invalidTx = new Transaction(walletAlice.PublicKey, walletBob.PublicKey, 1000, fee: 1.0m);
TransactionService.SignTransaction(invalidTx, walletAlice.PrivateKey);

var lastBlock = bcs1.Chain.Last();
var invalidBlock = new Block(lastBlock.Index + 1, new List<Transaction> { invalidTx }, lastBlock.Hash, DateTime.UtcNow)
{
    Hash = "invalid_hash_bypass_pow",
    DifficultyAtMining = 1
};
bcs1.Chain.Add(invalidBlock);

Console.WriteLine("\nInjecting invalid block with negative balance transaction...");
bool isStillValid = bcs1.ValidateAndRebuildState();
Console.WriteLine($"[Rebuild Invalid Chain] Status: {isStillValid}");
Console.WriteLine($"BalancesState count after failure (should be 0): {bcs1.BalancesState.Count}");


// 2. MEMPOOL TTL EVICTION TEST
Console.WriteLine("\n--- Scenario 2: Mempool TTL Eviction ---");
var bcs2 = new BlockChainService(1);

// Seed Alice
var seedTx2 = TransactionService.CreateTransaction("SYSTEM", walletAlice.PublicKey, 200, null);
bcs2.AddTransaction(seedTx2);
bcs2.AddBlock();

// Add fresh transaction
var freshTx = TransactionService.CreateTransaction(walletAlice.PublicKey, walletBob.PublicKey, 10, walletAlice.PrivateKey, fee: 1.0m);
bcs2.AddTransaction(freshTx);

// Add stale transaction manually modifying timestamp
var staleTx = TransactionService.CreateTransaction(walletAlice.PublicKey, walletBob.PublicKey, 20, walletAlice.PrivateKey, fee: 1.0m);
staleTx.Timestamp = DateTime.UtcNow.AddSeconds(-75);

bcs2.PendingTransactions.Add(staleTx);

Console.WriteLine($"Pending transactions count before eviction: {bcs2.PendingTransactions.Count}");
int evicted = bcs2.EvictStaleTransactions(60);
Console.WriteLine($"Evicted count: {evicted}");
Console.WriteLine($"Pending transactions count after eviction: {bcs2.PendingTransactions.Count}");


// 3. ANTISPAM FILTER TEST
Console.WriteLine("\n--- Scenario 3: Anti-Spam Filter ---");
var bcs3 = new BlockChainService(1);

// Seed Alice
var seedTx3 = TransactionService.CreateTransaction("SYSTEM", walletAlice.PublicKey, 500, null);
bcs3.AddTransaction(seedTx3);
bcs3.AddBlock();

try
{
    for (int i = 1; i <= 4; i++)
    {
        Console.WriteLine($"Sending transaction #{i} from Alice...");
        var tx = TransactionService.CreateTransaction(walletAlice.PublicKey, walletBob.PublicKey, 5, walletAlice.PrivateKey, fee: 1.0m);
        bcs3.AddTransaction(tx);
    }
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Transaction blocked: {ex.Message}");
}

Console.WriteLine($"Total transactions in mempool: {bcs3.PendingTransactions.Count}");
Console.WriteLine("==================================================");