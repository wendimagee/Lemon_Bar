using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class Item
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public decimal TotalCost { get; set; }
        public double Quantity { get; set; }
        public decimal UnitCost { get { return Math.Round(TotalCost / Convert.ToDecimal(Quantity), 2); } }
        public string Units { get; set; }
        public bool Garnish { get; set; }
        public string User { get; set; }

        public virtual AspNetUser UserNavigation { get; set; }
    }
}
