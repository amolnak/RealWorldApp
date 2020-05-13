using System;

namespace FoodApp.Models
{
    public class NotifBatches
    {
        public int NotifBatchId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string NotifBatchStatus { get; set; }
    }
}