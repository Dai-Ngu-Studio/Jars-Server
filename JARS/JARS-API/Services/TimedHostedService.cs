using JARS_DAL.Repository;

namespace BackgroundTasksSample.Services
{
    #region snippet1

    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly IContractRepository _contractRepository = new ContractRepository();
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer = null!;

        public TimedHostedService(ILogger<TimedHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            DateTime dt = new DateTime(2003, 5, 1);
            var abc = _contractRepository.GetActiveContractsAsync("abc");

            _logger.LogInformation(
                "Timed Hosted Service is working");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

    #endregion
}