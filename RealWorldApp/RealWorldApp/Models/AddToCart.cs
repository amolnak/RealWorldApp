﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FoodApp.Models
{
    public class AddToCart
    {
        public string Price { get; set; }
        public string Qty { get; set; }
        public string TotalAmount { get; set; }
        public int ProductId { get; set; }
        public Guid CustomerId { get; set; }
    }
}
