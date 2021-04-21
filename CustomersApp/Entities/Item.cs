namespace CustomersApp.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Exam.Item")]
    public partial class Item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Item()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        public string Specifications { get; set; }
        public decimal UnitPrice { get; set; }

        [StringLength(100)]
        public string Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
