

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Sampling.Example
{

    public class DependencyExemplificationTelemetryProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor next;
        public DependencyExemplificationTelemetryProcessor(ITelemetryProcessor next)
        {
            this.next = next;            
        }
        public void Process(ITelemetry item)
        {
            // Check telemetry type
            if (item is DependencyTelemetry)
            {
                var r = item as DependencyTelemetry;
                if (r.Duration > TimeSpan.FromMilliseconds(100))
                {
                    // If dependency duration > 100 ms then "sample in"
                    // this telemetry by setting sampling percentage to 100
                    ((ISupportSampling)item).SamplingPercentage = 100;

                    Cmd.Ln.Green($"Forced 'Sample-In' because of Slow request {r.Duration} (ms).").EOL();
                }
            }
            // Continue with the next telemetry processor
            this.next.Process(item);
        }
    }
}
