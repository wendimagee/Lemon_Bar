using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class DrinkSale
    {
        public int Id { get; set; }
        public string DrinkId { get; set; }
        public decimal? NetCost { get; set; }
        public decimal? SalePrice { get; set; }
        public DateTime? SaleDate { get; set; }
        public string User { get; set; }

        public virtual AspNetUser UserNavigation { get; set; }
    }
}
