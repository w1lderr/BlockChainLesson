using BlockChain.Models;

namespace BlockChain.Service
{
    public class MiningService
    {
        private readonly HashingService _hashingService;
        private const int ChunkSize = 1_000_000;
        public MiningService(HashingService hashingService)
        {
            _hashingService = hashingService;
        }

        public long MineBlock(Block block, int difficulty)
        {
            var target = new string('0', difficulty);
            using var cts = new CancellationTokenSource();
            var token = cts.Token;
            long winningNonce = -1;
            string winningHash = string.Empty;
            object winLock = new object();

            int threadCount = Environment.ProcessorCount;
            long nextChunkStart = 1;
            var tasks = new Task[threadCount];

            for (int t = 0; t < threadCount; t++)
            {
                tasks[t] = Task.Run(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        long chunkStart = Interlocked.Add(ref nextChunkStart, ChunkSize) - ChunkSize;

                        for (long localNonce = chunkStart;
                             localNonce < chunkStart + ChunkSize;
                             localNonce++)
                        {
                            if (token.IsCancellationRequested)
                                return;

                            string localHash = _hashingService.ComputeHash(block, localNonce);

                            if (localNonce % 500_000 == 0)
                                Console.Write(".");

                            if (localHash.ToLowerInvariant().StartsWith(target))
                            {
                                lock (winLock)
                                {
                                    if (winningNonce == -1)
                                    {
                                        winningNonce = localNonce;
                                        winningHash = localHash;
                                    }
                                }

                                cts.Cancel();
                                return;
                            }
                        }
                    }
                }, token);
            }

            Task.WaitAll(tasks.Select(t =>
                t.ContinueWith(_ => { }, TaskContinuationOptions.None)).ToArray());

            if (winningNonce == -1)
                throw new InvalidOperationException("Mining failed — no nonce found.");

            block.Nonce = (int)winningNonce;
            block.Hash = winningHash;

            Console.WriteLine($"\nBlock mined: {block.Hash} | nonce: {block.Nonce} | threads: {threadCount}");
            return winningNonce;
        }
    }
}