namespace F360Task.EventBusRabbitMQ.Tests.BackgroundServices;

public class RabbitMQInitializerHostedServiceTests
{
    public Mock<IRabbitMQConnectionProvider> _mockProvider;
    public Mock<ILogger<RabbitMQInitializerHostedService>> _mockLogger;
    public RabbitMQInitializerHostedService _rabbitMQInitializerHostedService;

    public RabbitMQInitializerHostedServiceTests()
    {
        _mockProvider = new Mock<IRabbitMQConnectionProvider>();
        _mockLogger = new Mock<ILogger<RabbitMQInitializerHostedService>>();
        _rabbitMQInitializerHostedService = new RabbitMQInitializerHostedService(
            _mockProvider.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task StartAsync_Should_Call_InitializeAsync_When_Not_Cancelled()
    {
        //Arrange
        var cancelationToken = CancellationToken.None;


        //Act
        await _rabbitMQInitializerHostedService.StartAsync(cancelationToken);

        //Assert
        _mockProvider.Verify(p => p.InitializeAsync(cancelationToken), Times.Once());
    }

    [Fact]
    public async Task StartAsync_Should_Not_Call_InitializeAsync_When_Cancelled()
    {
        //Arrange
        var cancelationToken = new CancellationToken(canceled: true);

        //Act
        await _rabbitMQInitializerHostedService.StartAsync(cancelationToken);

        //Assert
        _mockProvider.Verify(p => p.InitializeAsync(It.IsAny<CancellationToken>()), Times.Never);

    }


    [Fact]
    public async Task StopAsync_Should_Call_CloseAsync()
    {
        //Arrange
        var cancelactionToken = CancellationToken.None;


        //Act
        await _rabbitMQInitializerHostedService.StopAsync(cancelactionToken);


        //Assert
        _mockProvider.Verify(p => p.CloseAsync(), Times.Once);    }
}
