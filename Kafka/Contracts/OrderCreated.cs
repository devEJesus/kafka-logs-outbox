namespace Kafka.Contracts
{
    public record OrderCreated
    {
        // The compiler will ERROR if these aren't set during initialization
        public required Guid Id { get; init; }
        public required string Item { get; init; }
        public required decimal Amount { get; init; }

        // This one is optional (no 'required' keyword)
        public string? Department { get; init; }
    }
}