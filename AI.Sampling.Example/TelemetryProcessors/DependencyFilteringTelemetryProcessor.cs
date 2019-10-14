

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Sampling.Example.TelemetryProcessors
{

    public class DependencyFilteringTelemetryProcessor : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor next;
        public DependencyFilteringTelemetryProcessor(ITelemetryProcessor next)
        {
            this.next = next;
        }
        public void Process(ITelemetry item)
        {
            // Check telemetry type
            if (item is DependencyTelemetry)
            {
                var d = item as DependencyTelemetry;
                if (d.Duration < TimeSpan.FromMilliseconds(100))
                {
                    Console.WriteLine($"Dropping Telemetry for fast calls. {d.Duration}(ms).");
                    // If dependency duration > 100 ms then stop telemetry
                    // processing and return from the pipeline
                    return;
                }
            }
            this.next.Process(item);
        }
    }
}
