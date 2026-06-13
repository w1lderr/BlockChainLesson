using BlockChain.Models;
using System.Net.Sockets;
using System.Text.Json;

namespace BlockChain.Service.P2P
{
    public class P2PClient
    {
        private readonly List<string> _peers = new List<string>();

        public void Connect(string peerAddress)
        {
            if (!_peers.Contains(peerAddress))
            {
                _peers.Add(peerAddress);
            }
        }

        public async Task BroadcatTransactionAsync(Transaction transaction)
        {
            var jsonTransaction = JsonSerializer.Serialize(transaction);

            try
            {
                foreach (var peer in _peers)
                {
                    var parts = peer.Split(':');
                    var ip = parts[0];
                    var port = int.Parse(parts[1]);

                    using var client = new TcpClient();
                    await client.ConnectAsync(ip, port);

                    await using var stream = client.GetStream();
                    await using var writer = new StreamWriter(stream) { AutoFlush = true };
                    await writer.WriteAsync(jsonTransaction);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
