using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CustomersApp.Models
{
    public class CustomerModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Email { get; set; }

        public long ContactNo { get; set; }
        public virtual List<CustomerAddressModel> CustomerAddresses { get; set; } = new List<CustomerAddressModel>();
    }
}