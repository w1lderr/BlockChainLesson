using BlockChain.Models;
using BlockChain.Service;
using BlockChain.Service.P2P;
using Microsoft.Extensions.DependencyInjection;

var service = new ServiceCollection();
service.AddSingleton<BlockChainService>(new BlockChainService(difficulty: 1));
service.AddSingleton<CryptoService, CryptoService>();
service.AddSingleton<P2PClient, P2PClient>();
service.AddSingleton<P2PServer, P2PServer>();

var provider = service.BuildServiceProvider();

var blockChainService = provider.GetRequiredService<BlockChainService>();
var p2pServer = provider.GetRequiredService<P2PServer>();
var p2pClient = provider.GetRequiredService<P2PClient>();
var cryptoService = provider.GetRequiredService<CryptoService>();

var myWallet = new Wallet(cryptoService);
Console.WriteLine($"My wallet address: {myWallet.PublicKey}");
Console.WriteLine("Enter port: ");
int port = int.Parse(Console.ReadLine() ?? "5001");

p2pServer.Start(port);

blockChainService.BalancesState[myWallet.PublicKey] = 10000m;

while (true)
{
    Console.WriteLine("\nMenu:");
    Console.WriteLine("1. Connect to another node");
    Console.WriteLine("2. Create a new transaction");
    Console.WriteLine("3. Show mempool");
    Console.WriteLine("4. Quit");

    Console.Write("Your choice: ");

    switch (Console.ReadLine())
    {
        case "1":
            Console.Write("Enter peer address (e.g., 127.0.0.1:6002): ");
            var peerAddress = Console.ReadLine();
            if (!string.IsNullOrEmpty(peerAddress))
            {
                p2pClient.Connect(peerAddress);
                Console.WriteLine($"Added peer: {peerAddress}");
            }
            break;

        case "2":
            Console.Write("Enter recipient public key (wallet address): ");
            var recipient = Console.ReadLine();
            if (string.IsNullOrEmpty(recipient)) break;

            Console.Write("Enter amount: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                Console.Write("Enter fee (default 1): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal fee))
                {
                    fee = 1.0m;
                }

                try
                {
                    var tx = TransactionService.CreateTransaction(myWallet.PublicKey, recipient, amount, myWallet.PrivateKey, fee);
                    blockChainService.AddTransaction(tx);
                    Console.WriteLine($"Transaction created and added to mempool: {tx.Id}");

                    Console.WriteLine("[Gossip] Пересилаю транзакцію іншим вузлам...");
                    _ = p2pClient.BroadcatTransactionAsync(tx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            break;

        case "3":
            Console.WriteLine("\nMempool transactions:");
            if (blockChainService.PendingTransactions.Count == 0)
            {
                Console.WriteLine("(empty)");
            }
            else
            {
                foreach (var tx in blockChainService.PendingTransactions)
                {
                    Console.WriteLine(tx);
                }
            }
            break;

        case "4":
            return;
    }
}