

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Sampling.Example
{
    public  class TelemetryRecordSizeProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor next;
        private Action<int> onAddSize;
        public TelemetryRecordSizeProcessor(ITelemetryProcessor next,
          Action<int> onAddSize)
        {
            this.next = next;
            this.onAddSize = onAddSize;
        }
        public void Process(ITelemetry item)
        {
            try
            {
                item.Sanitize();
                byte[] content =
                  JsonSerializer.Serialize(new List<ITelemetry>() { item }, false);
                int size = content.Length;
                string json = Encoding.Default.GetString(content);
                this.onAddSize(size);
            }
            finally
            {
                this.next.Process(item);
            }
        }
    }
}
