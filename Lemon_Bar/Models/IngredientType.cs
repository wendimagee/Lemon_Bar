using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class IngredientType
    {
        public int Id { get; set; }
        public string ApiingId { get; set; }
        public string ApistrIngredient { get; set; }
        public string ApistrType { get; set; }
        public string ApistrAlcohol { get; set; }
        public string ApistrAbv { get; set; }
        public string IngCategory { get; set; }
    }
}
