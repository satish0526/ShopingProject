using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CustomersApp.Models
{
    public class OrderDetailModel
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        [Required]
        public int? Quantity { get; set; }
        public decimal? GrandTotal { get; set; }
       // public decimal? UnitPrice { get; set; }
        public string RecordStatus { get; set; }
    }
}