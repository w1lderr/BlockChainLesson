using BlockChain.Models;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace BlockChain.Service.P2P
{
    public class P2PServer
    {
        private readonly BlockChainService blockChainService;
        private readonly P2PClient p2pClient;

        public P2PServer(BlockChainService blockChainService, P2PClient p2pClient)
        {
            this.blockChainService = blockChainService;
            this.p2pClient = p2pClient;
        }


        public void Start(int port)
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"P2P Server started on port {port}");

            Task.Run(async () =>
            {
                while (true)
                {
                    var client = await listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
            });
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                await using var stream = client.GetStream();
                using var reader = new StreamReader(stream);

                var jsonLine = await reader.ReadLineAsync();

                if (!string.IsNullOrEmpty(jsonLine))
                {
                    var tx = JsonSerializer.Deserialize<Transaction>(jsonLine);

                    if (tx != null && !blockChainService.PendingTransactions.Any(t => t.Id == tx.Id))
                    {
                        blockChainService.AddTransactionToMempool(tx);
                        Console.WriteLine($"[Сервер] Отримано нову транзакцію: {tx.Id}");
                        Console.WriteLine("[Gossip] Пересилаю транзакцію іншим вузлам...");
                        _ = p2pClient.BroadcatTransactionAsync(tx);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
