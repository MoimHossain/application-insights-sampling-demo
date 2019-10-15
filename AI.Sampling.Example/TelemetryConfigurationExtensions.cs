


using AI.Sampling.Example.TelemetryProcessors;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation.Experimental;
using Microsoft.ApplicationInsights.WindowsServer.Channel.Implementation;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AI.Sampling.Example
{
    public static class TelemetryConfigurationExtensions
    {
        public static TelemetryConfiguration Configure(this TelemetryConfiguration configuration)
        {
            // Automatically collect dependency calls
            var dependencies = new DependencyTrackingTelemetryModule();
            dependencies.Initialize(configuration);
            // Automatically correlate all telemetry data with request
            configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());

            return configuration;
        }


        public static TelemetryConfiguration ConfigureWithSampling(this TelemetryConfiguration configuration, 
            AdaptiveSamplingWorker worker)
        {
            // Automatically collect dependency calls
            var dependencies = new DependencyTrackingTelemetryModule();
            dependencies.Initialize(configuration);
            // Automatically correlate all telemetry data with request
            configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());


            // Build telemetry processing pipeline
            configuration.TelemetryProcessorChainBuilder
              // This telemetry processor will be executed
              // first for all telemetry items to calculate the size and # of items
              .Use((next) =>
              {
                  return new TelemetryRecordSizeProcessor(next, size => worker.OnCollectedItems(size));
              })
              // This is a standard fixed sampling processor that'll let only 10%
              .Use((next) =>
              {
                  return new SamplingTelemetryProcessor(next)
                  {
                      IncludedTypes = "Dependency",
                      SamplingPercentage = 10,
                  };
              })
              // This is a standard adaptive sampling telemetry processor
              // that will sample in/out any telemetry item it receives
              .Use((next) =>
              {
                  var settings = new SamplingPercentageEstimatorSettings
                  {
                      MaxTelemetryItemsPerSecond = 1, // Default: 5 calls/sec
                      SamplingPercentageIncreaseTimeout = TimeSpan.FromSeconds(1), // Default: 2 min
                      SamplingPercentageDecreaseTimeout = TimeSpan.FromSeconds(1), // Default: 30 sec
                      EvaluationInterval = TimeSpan.FromSeconds(1), // Default: 15 sec
                      InitialSamplingPercentage = 25, // Default: 100%  
                  };

                  var adaptiveSamplingProcessor = new AdaptiveSamplingTelemetryProcessor(settings, new AdaptiveSamplingPercentageEvaluatedCallback(AdaptiveSamplingEvaluated), next)
                  {
                      ExcludedTypes = "Event", // Exclude custom events from being sampled                    
                  };

                  return adaptiveSamplingProcessor;
              })
              // This telemetry processor will be executed ONLY when telemetry is sampled in
              .Use((next) =>
              {
                  return new TelemetryRecordSizeProcessor(next, size => worker.OnSentItems(size));
              })
              .Build();

            return configuration;
        }

        public static TelemetryConfiguration ConfigureFilteringWithMetrics(
         this TelemetryConfiguration configuration)
        {
            // Automatically collect dependency calls
            var dependencies = new DependencyTrackingTelemetryModule();
            dependencies.Initialize(configuration);
            // Automatically correlate all telemetry data with request
            configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
            // Build telemetry processing pipeline
            configuration.TelemetryProcessorChainBuilder
              // This telemetry processor will be executed ONLY when telemetry is sampled in
              .Use((next) =>
              {
                  return new DependencyFilteringWithMetricsTelemetryProcessor(next, configuration);
              })
              // This is a standard fixed sampling processor that'll let only 10%
              .Use((next) =>
              {
                  return new SamplingTelemetryProcessor(next)
                  {
                      IncludedTypes = "Dependency",
                      SamplingPercentage = 10,
                  };
              })
              // This is a standard adaptive sampling telemetry processor
              // that will sample in/out any telemetry item it receives
              .Use((next) =>
              {
                  var settings = new SamplingPercentageEstimatorSettings
                  {
                      MaxTelemetryItemsPerSecond = 1, // Default: 5 calls/sec
                      SamplingPercentageIncreaseTimeout = TimeSpan.FromSeconds(1), // Default: 2 min
                      SamplingPercentageDecreaseTimeout = TimeSpan.FromSeconds(1), // Default: 30 sec
                      EvaluationInterval = TimeSpan.FromSeconds(1), // Default: 15 sec
                      InitialSamplingPercentage = 25, // Default: 100%  
                  };

                  var adaptiveSamplingProcessor = new AdaptiveSamplingTelemetryProcessor(settings, new AdaptiveSamplingPercentageEvaluatedCallback(AdaptiveSamplingEvaluated), next)
                  {
                      ExcludedTypes = "Event", // Exclude custom events from being sampled                    
                  };

                  return adaptiveSamplingProcessor;
              })
              .Build();

            return configuration;
        }

        public static TelemetryConfiguration ConfigureWithFiltering(
            this TelemetryConfiguration configuration)
        {
            // Automatically collect dependency calls
            var dependencies = new DependencyTrackingTelemetryModule();
            dependencies.Initialize(configuration);
            // Automatically correlate all telemetry data with request
            configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
            // Build telemetry processing pipeline
            configuration.TelemetryProcessorChainBuilder
              // This telemetry processor will be executed ONLY when telemetry is sampled in
              .Use((next) =>
              {
                  return new DependencyFilteringTelemetryProcessor(next);
              })
              // This is a standard fixed sampling processor that'll let only 10%
              .Use((next) =>
              {
                  return new SamplingTelemetryProcessor(next)
                  {
                      IncludedTypes = "Dependency",
                      SamplingPercentage = 10,
                  };
              })
              // This is a standard adaptive sampling telemetry processor
              // that will sample in/out any telemetry item it receives
              .Use((next) =>
              {
                  var settings = new SamplingPercentageEstimatorSettings
                  {
                      MaxTelemetryItemsPerSecond = 1, // Default: 5 calls/sec
                      SamplingPercentageIncreaseTimeout = TimeSpan.FromSeconds(1), // Default: 2 min
                      SamplingPercentageDecreaseTimeout = TimeSpan.FromSeconds(1), // Default: 30 sec
                      EvaluationInterval = TimeSpan.FromSeconds(1), // Default: 15 sec
                      InitialSamplingPercentage = 25, // Default: 100%  
                  };

                  var adaptiveSamplingProcessor = new AdaptiveSamplingTelemetryProcessor(settings, new AdaptiveSamplingPercentageEvaluatedCallback(AdaptiveSamplingEvaluated), next)
                  {
                      ExcludedTypes = "Event", // Exclude custom events from being sampled                    
                  };

                  return adaptiveSamplingProcessor;
              })
              .Build();

            return configuration;
        }

        public static TelemetryConfiguration ConfigureWithExemplification(
            this TelemetryConfiguration configuration)
        {
            // Automatically collect dependency calls
            var dependencies = new DependencyTrackingTelemetryModule();
            dependencies.Initialize(configuration);
            // Automatically correlate all telemetry data with request
            configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
            // Build telemetry processing pipeline
            configuration.TelemetryProcessorChainBuilder
              // This telemetry processor will be executed ONLY when telemetry is sampled in
              .Use((next) =>
              {
                  return new DependencyExemplificationTelemetryProcessor(next);
              })
              // This is a standard fixed sampling processor that'll let only 10%
              .Use((next) =>
              {
                  return new SamplingTelemetryProcessor(next)
                  {
                      IncludedTypes = "Dependency",
                      SamplingPercentage = 10,
                  };
              })
              // This is a standard adaptive sampling telemetry processor
              // that will sample in/out any telemetry item it receives
              .Use((next) =>
              {
                  var settings = new SamplingPercentageEstimatorSettings
                  {
                      MaxTelemetryItemsPerSecond = 1, // Default: 5 calls/sec
                      SamplingPercentageIncreaseTimeout = TimeSpan.FromSeconds(1), // Default: 2 min
                      SamplingPercentageDecreaseTimeout = TimeSpan.FromSeconds(1), // Default: 30 sec
                      EvaluationInterval = TimeSpan.FromSeconds(1), // Default: 15 sec
                      InitialSamplingPercentage = 25, // Default: 100%  
                  };

                  var adaptiveSamplingProcessor = new AdaptiveSamplingTelemetryProcessor(settings, new AdaptiveSamplingPercentageEvaluatedCallback(AdaptiveSamplingEvaluated), next)
                  {
                      ExcludedTypes = "Event", // Exclude custom events from being sampled                    
                  };

                  return adaptiveSamplingProcessor;
              })
              .Build();

            return configuration;
        }

        public static void AdaptiveSamplingEvaluated(
            double afterSamplingTelemetryItemRatePerSecond, 
            double currentSamplingPercentage, 
            double newSamplingPercentage, 
            bool isSamplingPercentageChanged, 
            SamplingPercentageEstimatorSettings settings)
        {
            if(isSamplingPercentageChanged)
            {
                Cmd.Ln.EOL()
                    .Gray("New Sampling Rate: ").Green(newSamplingPercentage).Green("% ")
                    .Gray(", Before it was: ").Cyan(currentSamplingPercentage).Cyan("% ");
            }
        }



        public static TelemetryConfiguration CreateConfiguration()
        {
            var instrumentationKey = Environment.GetEnvironmentVariable("ApplicationInsightKey");

            return new TelemetryConfiguration
            {
                InstrumentationKey = instrumentationKey
            };
        }
    }
}
