

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

namespace AI.Sampling.Example
{
    public class AdaptiveSamplingWorker : BackgroundService
    {
        private readonly ILogger<AdaptiveSamplingWorker> _logger;
        private TelemetryConfiguration configuration;
        private static HttpClient http = new HttpClient();
        // Initialize state for the telemetry size calculation
        private int collectedItems = 0;
        private int sentItems = 0;

        public AdaptiveSamplingWorker(ILogger<AdaptiveSamplingWorker> logger)
        {
            _logger = logger;

            //configuration = TelemetryConfigurationExtensions.CreateConfiguration().Configure();
            configuration = TelemetryConfigurationExtensions.CreateConfiguration().ConfigureWithSampling(this);
        }

        public void OnCollectedItems(int size)
        {
            Interlocked.Add(ref collectedItems, size);
        }

        public void OnSentItems(int size)
        {
            Interlocked.Add(ref sentItems, size);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = new TelemetryClient(configuration);
            var iteration = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var operation = client.StartOperation<RequestTelemetry>("Process item"))
                {
                    client.TrackEvent("Iteration Started",
                    properties: new Dictionary<string, string>() { { "iteration", iteration.ToString() } });
                    client.TrackTrace($"Iteration {iteration} started", SeverityLevel.Information);

                    Console.WriteLine($"Iteration {iteration}. " +
                          $"Elapsed time: {operation.Telemetry.Duration}. " +
                          $"Collected Telemetry: {collectedItems} (bytes). " +
                          $"Sent Telemetry: {sentItems} (bytes). " +
                          $"Ratio: {1.0 * collectedItems / sentItems}");

                    try
                    {
                        if (iteration % 10 == 0)
                            throw new ApplicationException("Some error took place.");

                        await http.GetStringAsync(Constants.URL);
                    }
                    catch (Exception exc)
                    {
                        // This call will not throw
                        client.TrackException(exc);
                        operation.Telemetry.Success = false;
                    }
                    client.StopOperation(operation);
                    Console.WriteLine($"Iteration {iteration}. Elapsed time: { operation.Telemetry.Duration}");
                    iteration++;
                }
            }
        }
    }
}
