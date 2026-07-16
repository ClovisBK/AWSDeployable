
using AuthService.Services.Interfaces;

namespace AuthService.BackgroundServices
{
    public class LoanExpiryBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LoanExpiryBackgroundService> _logger;

        public LoanExpiryBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<LoanExpiryBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var loanService = scope.ServiceProvider.GetRequiredService<ILoanService>();

                    await loanService.CancelExpiredLoansAsync();
                    _logger.LogInformation("Expired loan check ran at {Time}", DateTime.UtcNow);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error during expired loan cancellation.");
                }
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
