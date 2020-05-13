using System;

namespace FoodApp.Models
{
    public class ItemAddOns
    {
        public Guid ItemAddOnID { get; set; }
        public string AddOnName { get; set; }
        public double AddOnPrice { get; set; }
    }
}