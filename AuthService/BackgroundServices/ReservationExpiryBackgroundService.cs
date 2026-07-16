using AuthService.Services.Interfaces;

namespace AuthService.BackgroundServices
{
    public class ReservationExpiryBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReservationExpiryBackgroundService> _logger;

        public ReservationExpiryBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<ReservationExpiryBackgroundService> logger)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

                    await reservationService.ProcessExpiredNotifiedReservationAsync();
                    _logger.LogInformation("Reservation expiry check ran at {Time}", DateTime.UtcNow);

                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error during reservation expiry processing");
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
