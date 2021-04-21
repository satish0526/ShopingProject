namespace CustomersApp.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Exam.OrderDetail")]
    public partial class OrderDetail
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ItemId { get; set; }

        public int? Quantity { get; set; }

        public virtual Item Item { get; set; }

        public virtual Order Order { get; set; }
    }
}
