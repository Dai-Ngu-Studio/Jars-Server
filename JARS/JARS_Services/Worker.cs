using FirebaseAdmin.Messaging;
using JARS_API.Utilities;
using JARS_DAL.Repository;

namespace JARS_Services;

public class Worker : BackgroundService
{
    private readonly IContractRepository _contractRepository = new ContractRepository();

    private readonly ILogger<Worker> _logger;
    private Timer _timer = null;
    private Timer _timerDemo = null;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromDays(1));
        _timerDemo = new Timer(DoWorkDemo, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(1));
    }

    private void DoWorkDemo(object? state)
    {
        var contracts = _contractRepository.CreateBillByContractDemo();
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
                _logger.LogInformation(accountDevice.FcmToken);

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
        }
        catch (Exception)
        {
            // log
        }

        _logger.LogInformation(
            "Demo background service is running");
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
}