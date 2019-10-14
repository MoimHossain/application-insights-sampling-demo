//using Microsoft.ApplicationInsights;
//using Microsoft.ApplicationInsights.Channel;
//using Microsoft.ApplicationInsights.DataContracts;
//using Microsoft.ApplicationInsights.Extensibility;
//using Microsoft.ApplicationInsights.Metrics;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Text;

//namespace AI.Sampling.Example.TelemetryProcessors
//{
//    public class DependencyFilteringWithMetricsTelemetryProcessor
//                                            : ITelemetryProcessor, IDisposable
//    {
//        private readonly ITelemetryProcessor next;
//        private readonly TelemetryClient telemetryClient;
//        private readonly ConcurrentDictionary<string, Tuple<Metric, Metric>> metrics
//          = new ConcurrentDictionary<string, Tuple<Metric, Metric>>();
//        private readonly MetricManager manager;
//        public DependencyFilteringWithMetricsTelemetryProcessor(
//          ITelemetryProcessor next, TelemetryConfiguration configuration)
//        {
//            this.next = next;
//            this.telemetryClient = new TelemetryClient(configuration);
//            this.manager = configuration.GetMetricManager(); 
//        }
//        public void Process(ITelemetry item)
//        {
//            // Check telemetry type
//            if (item is DependencyTelemetry)
//            {
//                var d = item as DependencyTelemetry;
//                // Increment counters
//                var metrics = this.metrics.GetOrAdd(d.Type, (type) =>
//                {
//                    var dimensions = new Dictionary<string, string> { { "type", type } };

//                    telemetryClient.GetMetric("# of dependencies", new MetricConfiguration(10000, new int[] { 500}));

//                    var numberOfDependencies =
//                      this.manager.CreateNewSeries("# of dependencies", "", dimensions,

//                      new MetricSeriesConfigurationForAccumulator(
//                                                        seriesCountLimit: 10000,
//                                                        valuesPerDimensionLimit: 5000,
//                                                        seriesConfig: new MetricSeriesConfigurationForAccumulator(restrictToUInt32Values: false))
//                      );
//                    var dependenciesDuration =
//                       this.manager.CreateMetric("dependencies duration (ms)", dimensions);
//                    return new Tuple<Metric, Metric>(
//                      numberOfDependencies, dependenciesDuration);
//                });
//                // Increment values of the metrics in memory
//                metrics.Item1.TrackValue(1);
//                metrics.Item2.TrackValue(d.Duration.TotalMilliseconds);
//                if (d.Duration < TimeSpan.FromMilliseconds(100))
//                {
//                    // If dependency duration > 100 ms then stop telemetry
//                    // processing and return from the pipeline
//                    return;
//                }
//            }
//            this.next.Process(item);
//        }
//        public void Dispose()
//        {
//            this.manager.Dispose();
//        }
//    }
//}
