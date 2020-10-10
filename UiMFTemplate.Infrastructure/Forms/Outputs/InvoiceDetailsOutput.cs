namespace UiMFTemplate.Infrastructure.Forms.Outputs
{
    using System;
    using System.Collections.Generic;
    using UiMetadataFramework.Core.Binding;

    [OutputFieldType("invoice-details-output")]
    public class InvoiceDetailsOutput
    {
        public IEnumerable<OrderDetails> Orders { get; set; }
        public IEnumerable<ProductSummary> ProductSummaries { get; set; }
        public int Id { get; set; }
    }

    public class ProductSummary
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderDetails
    {
        [OutputField(OrderIndex = 7, Label = "Created on")]
        public DateTime CreatedDate { get; set; }

        public string Id { get; set; }

        public int FId { get; set; }

        public string CustomerAddress { get; set; }


        public string CustomerName { get; set; }
        public string CustomerState { get; set; }

        public string CustomerPhone { get; set; }

        public string Price { get; set; }

        public string RecieptNumber { get; set; }

        public string Status { get; set; }
        public string Notes { get; set; }
        public IEnumerable<DeliveryItem> DeliveryItems { get; set; }
        public AdminStore Store { get; set; }
    }

    public class DeliveryItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Reference { get; set; }
        public string ImageLink { get; set; }
    }

    public class AdminStore
    {
        public string Address { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string ManagerName { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string Phone { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public string StoreOfferId { get; set; }
    }
}
