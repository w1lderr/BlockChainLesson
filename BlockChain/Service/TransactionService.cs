using BlockChain.Models;

namespace BlockChain.Service
{
    public static class TransactionService
    {
        private static readonly CryptoService cryptoService;

        static TransactionService()
        {
            cryptoService = new CryptoService();
        }

        public static Transaction CreateTransaction(string from, string to, decimal amount, string? privateKey)
        {
            var tx = new Transaction(from, to, amount);
            SignTransaction(tx, privateKey);
            var validation = ValidateTransaction(tx);

            if (!validation.isValid)
            {
                throw new InvalidOperationException(validation.error);
            }

            return tx;
        }

        public static (bool isValid, string? error) ValidateTransaction(Transaction transaction)
        {
            if (transaction == null) return (false, "Transaction is null");
            if (string.IsNullOrEmpty(transaction.From)) return (false, "Sender address is required");
            if (string.IsNullOrEmpty(transaction.To)) return (false, "Recepient address is required");
            if (transaction.Amount <= 0) return (false, "Amount must be greater than zero");
            if (transaction.Signature == null || transaction.Signature.Length == 0) return (false, "Transaction must be signed.");

            if (transaction.From == "SYSTEM")
            {
                return (true, null);
            }

            try
            {
                if (!cryptoService.VerifySignature(transaction.ToRawString(), transaction.Signature, transaction.From))
                {
                    return (false, "Invalid signature");
                }
            }
            catch
            {
                return (false, "Signature verification failed");
            }

            return (true, null);
        }

        public static void SignTransaction(Transaction transaction, string? privateKey)
        {
            if (transaction.From == "SYSTEM")
            {
                transaction.Signature = new byte[] { 0 };
                return;
            }
            var signature = cryptoService.SignData(transaction.ToRawString(), privateKey!);
            transaction.Signature = signature;
        }
    }
}