﻿

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AI.Sampling.Example
{
    public class BasicWorker : BackgroundService
    {
        private readonly ILogger<BasicWorker> _logger;
        private TelemetryConfiguration configuration;
        private static HttpClient http = new HttpClient();

        public BasicWorker(ILogger<BasicWorker> logger)
        {
            _logger = logger;

            configuration = TelemetryConfigurationExtensions.CreateConfiguration().Configure();
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