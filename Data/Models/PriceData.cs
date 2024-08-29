﻿namespace Data.Models
{
    public class PriceData
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
