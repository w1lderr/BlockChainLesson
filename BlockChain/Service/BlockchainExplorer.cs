using BlockChain.Models;

// Створюємо аліас типу Blockchain для класу BlockChainService, 
// щоб клас BlockchainExplorer відповідав умові завдання та приймав Blockchain.
using Blockchain = BlockChain.Service.BlockChainService;

namespace BlockChain.Service
{
    public class BlockchainExplorer
    {
        private readonly Blockchain _blockchain;

        public BlockchainExplorer(Blockchain blockchain)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
        }

        /// <summary>
        /// Повертає суму всіх переказів (Amount) за всю історію блокчейну.
        /// </summary>
        public decimal GetTotalVolume()
        {
            return _blockchain.Chain
                .SelectMany(b => b.Transactions)
                .Sum(t => t.Amount);
        }

        /// <summary>
        /// Знаходить транзакцію з найбільшою сумою.
        /// </summary>
        public Transaction? GetLargestTransaction()
        {
            return _blockchain.Chain
                .SelectMany(b => b.Transactions)
                .OrderByDescending(t => t.Amount)
                .FirstOrDefault();
        }

        /// <summary>
        /// Повертає всі транзакції, де вказана адреса є відправником або отримувачем.
        /// </summary>
        public List<Transaction> GetAddressHistory(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return new List<Transaction>();
            }

            return _blockchain.Chain
                .SelectMany(b => b.Transactions)
                .Where(t => string.Equals(t.From, address, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(t.To, address, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Шукає транзакцію за її Id по всьому блокчейну.
        /// Повертає кортеж із знайденим блоком та самою транзакцією.
        /// Якщо транзакцію не знайдено — повертає (null, null).
        /// </summary>
        public (Block? block, Transaction? tx) FindTransactionLocation(string txId)
        {
            if (string.IsNullOrEmpty(txId))
            {
                return (null, null);
            }

            foreach (var block in _blockchain.Chain)
            {
                var tx = block.Transactions
                    .FirstOrDefault(t => string.Equals(t.Id, txId, StringComparison.OrdinalIgnoreCase));

                if (tx != null)
                {
                    return (block, tx);
                }
            }

            return (null, null);
        }
    }
}
