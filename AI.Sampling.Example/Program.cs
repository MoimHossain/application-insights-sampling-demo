using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AI.Sampling.Example.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AI.Sampling.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }


        
// 1. Sampling ratio is determined by the number of events per second occurring in the app
// 2. The AI SDK randomly selects requests to be sampled 
//    when the request begins (so, it is not known whether it will fail or succeed)
// 3. AI SDK assigns itemCount=<sampling ratio>

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<AdaptiveSamplingWorker>();
                });
    }
}
