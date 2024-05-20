using Grpc.Net.Client;
using ProductGrpcServer.Protos;

namespace ProductWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;
    private readonly ProductFactory _factory;

    public Worker(ILogger<Worker> logger, IConfiguration config, ProductFactory factory)
    {
        _logger = logger;
        _config = config;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            try
            {
                using var channel = GrpcChannel.ForAddress(_config.GetValue<string>("WorkerService:ServerUrl"));
                var client = new ProductProtoService.ProductProtoServiceClient(channel);

                _logger.LogInformation("AddProductAsync started..");

                var addProductResponse = await client.AddProductAsync(await _factory.Generate());
                
                _logger.LogInformation("AddProduct Response: {product}", addProductResponse.ToString());
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);

                throw exception;
            }

            await Task.Delay(_config.GetValue<int>("WorkerService:TaskInterval"), stoppingToken);
        }
    }
}