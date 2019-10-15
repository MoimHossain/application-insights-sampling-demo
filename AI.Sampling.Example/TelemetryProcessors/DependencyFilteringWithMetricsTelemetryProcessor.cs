

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Metrics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace AI.Sampling.Example.TelemetryProcessors
{
    public class DependencyFilteringWithMetricsTelemetryProcessor
                                            : ITelemetryProcessor, IDisposable
    {
        private readonly ITelemetryProcessor next;
        private readonly TelemetryClient telemetryClient;
        
        public DependencyFilteringWithMetricsTelemetryProcessor(
          ITelemetryProcessor next, TelemetryConfiguration configuration)
        {
            this.next = next;
            this.telemetryClient = new TelemetryClient(configuration);
        }


        public void Process(ITelemetry item)
        {
            // Check telemetry type
            if (item is DependencyTelemetry)
            {
                var d = item as DependencyTelemetry;

                telemetryClient.GetMetric("# of dependencies").TrackValue(1);
                telemetryClient.GetMetric($"{d.Type} Dependencies durations (ms)").TrackValue(d.Duration.TotalMilliseconds);

                if (d.Duration < TimeSpan.FromMilliseconds(100))
                {
                    // If dependency duration > 100 ms then stop telemetry
                    // processing and return from the pipeline
                    return;
                }
            }
            this.next.Process(item);
        }


        public void Dispose()
        {
           
        }
    }
}
