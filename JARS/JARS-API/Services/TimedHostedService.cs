using FirebaseAdmin.Messaging;
using JARS_API.Utilities;
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
            var contracts = _contractRepository.CreateBillByContract();
            contracts.Wait();
            List<Message> messages = new List<Message>();
            foreach (var contract in contracts.Result)
            {
                foreach (var accountDevice in contract.Account.AccountDevices)
                {
                    Notification noti = new NotificationBuilder().AddTitle("JARS")
                        .AddBody("A bill was created for contract " + contract.Name).Build();
                    Message messageForA = new MessageBuilder().AddToken(accountDevice.FcmToken).AddNotification(noti)
                        .Build();
                    messages.Add(messageForA);
                }
            }

            try
            {
                BatchResponse batchResponse = FirebaseCloudMessagingUtility.SendMessagesAsync(messages)
                    .GetAwaiter().GetResult();
                FcmTokenHandler.HandleBatchResponse(batchResponse, messages).GetAwaiter();
            }
            catch (FirebaseMessagingException)
            {
//burh
            }
            catch (Exception)
            {
                // log
            }

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