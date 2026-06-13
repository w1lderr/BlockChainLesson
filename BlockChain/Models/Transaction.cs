namespace BlockChain.Models
{
    public class Transaction
    {
        public string Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public byte[] Signature { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public Transaction(string from, string to, decimal amount, decimal fee = 0)
        {
            Id = Guid.NewGuid().ToString();
            From = from;
            To = to;
            Amount = amount;
            Fee = fee;
        }

        public string ToRawString()
        {
            return $"{From}{To}{Amount}{Fee}{Timestamp.ToString("O")}";
        }

        public override string ToString()
        {
            return $"Transaction: {Id}, From: {From}, To: {To}, Amount: {Amount}, Fee: {Fee}, Timestamp: {Timestamp}";
        }
    }
}