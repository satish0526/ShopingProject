using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CustomersApp.Models
{
    public class OrderModel
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public string RefNumber { get; set; }

        [StringLength(2000)]
        public string Remarks { get; set; }
        public List<OrderDetailModel> OrderDetailModels { get; set; } = new List<OrderDetailModel>();
    }
}