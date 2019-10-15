
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AI.Sampling.Example.Workers
{
    public class FilterWithMetricsWorker : BackgroundService
    {
        private readonly ILogger<FilteringWorker> _logger;
        private TelemetryConfiguration configuration;
        private static HttpClient http = new HttpClient();
        
        public FilterWithMetricsWorker(ILogger<FilteringWorker> logger)
        {
            _logger = logger;
            configuration = TelemetryConfigurationExtensions.CreateConfiguration().ConfigureFilteringWithMetrics();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = new TelemetryClient(configuration);
            var iteration = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var operation = client.StartOperation<RequestTelemetry>("Process item"))
                {
                    client.TrackEvent("IterationStarted", properties: new Dictionary<string, string>() { { "iteration", iteration.ToString() } });
                    client.TrackTrace($"Iteration {iteration} started", SeverityLevel.Information);

                    try
                    {
                        if (iteration % 10 == 0)
                        {
                            Cmd.Ln.EOL()
                                .Red("Iteration ").Yellow(iteration).Red(" failed with exception.");
                            throw new ApplicationException("Some error took place.");
                        }

                        await http.GetStringAsync(Constants.URL);
                    }
                    catch (Exception exc)
                    {
                        // This call will not throw
                        client.TrackException(exc);
                        operation.Telemetry.Success = false;
                    }
                    client.StopOperation(operation);

                    Cmd.Ln.EOL()
                        .Gray("Iteration: ").Yellow(iteration)
                        .Gray(", Elapsed time: ").Green(operation.Telemetry.Duration);
                    iteration++;
                }
            }
        }
    }
}
