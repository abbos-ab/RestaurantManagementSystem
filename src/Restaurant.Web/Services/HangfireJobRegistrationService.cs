using Hangfire;
using Restaurant.Infrastructure.Jobs;

namespace Restaurant.Web.Services;

public class HangfireJobRegistrationService : IHostedService
{
    private readonly IRecurringJobManager _recurringJobManager;

    public HangfireJobRegistrationService(IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _recurringJobManager.AddOrUpdate<RemoveExpiredRefreshTokens>(
            recurringJobId: "RemoveExpiredRefreshTokensJob",
            methodCall: job => job.ExecuteAsync(),
            cronExpression: Cron.Weekly(DayOfWeek.Tuesday, 21)
        );

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}