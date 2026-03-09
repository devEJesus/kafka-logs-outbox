using Confluent.Kafka;
using Kafka.Contracts;
using KafkaFlow;
using KafkaFlow.Outbox;
using KafkaFlow.Outbox.Postgres;
using KafkaFlow.Producers;
using KafkaFlow.Serializer;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

// Shared datasource for Postgres
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
var dataSource = dataSourceBuilder.Build();

builder.Services
    .AddSingleton(dataSource)
    .AddPostgresOutboxBackend();

builder.Services
    .AddSingleton(dataSource)
    .AddPostgresOutboxBackend();

builder.Services.AddKafka(kafka => kafka
    .UseMicrosoftLog()
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { "localhost:9092" })

        // Outbox dispatcher
        .AddOutboxDispatcher(dispatcher =>
            dispatcher.WithPartitioner(Partitioner.Murmur2Random)
        )

        .AddProducer("order-producer", producer => producer
            .DefaultTopic("order-topic")
            .WithOutbox()
            .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
        )
    )
);


var app = builder.Build();
var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/new-order", async (IProducerAccessor producerAccessor) =>
{
    var orderCreated = new OrderCreated
    {
        Id = Guid.NewGuid(),
        Item = "Enterprise license",
        Amount = 500.00m
    };

    var producer = producerAccessor.GetProducer("order-producer");

    await producer.ProduceAsync(orderCreated.Id.ToString(), orderCreated);

    return Results.Ok(orderCreated);
})
.WithName("NewOrder");

app.Run();