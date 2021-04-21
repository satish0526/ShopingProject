using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CustomersApp.Models
{
    public class CustomerAddressModel
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        [StringLength(100)]
        public string AddressLine1 { get; set; }

        [StringLength(100)]
        public string AddressLine2 { get; set; }

        [StringLength(100)]
        public string AddressLine3 { get; set; }

        [Required]
        [StringLength(100)]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(200)]
        public string Country { get; set; }
        public string RecordStatus { get; set; }
    }
}