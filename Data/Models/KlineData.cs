namespace Data.Models
{
    public class KlineData
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Symbol { get; set; }
        public string Interval { get; set; }
        public long KlineStartTime { get; set; }
        public long KlineCloseTime { get; set; }
        public decimal ClosePrice { get; set; }
        public int NumberOfTrades { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
