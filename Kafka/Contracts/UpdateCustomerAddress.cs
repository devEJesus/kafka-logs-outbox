namespace Kafka.Contracts
{
    public record UpdateCustomerAddress
    {
        public Guid CommandId { get; init; }
        public DateTime Timestamp { get; init; }
        public string CustomerId { get; init; }
        public string HouseNumber { get; init; }
        public string Street { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public string PostalCode { get; init; }

    }
}
