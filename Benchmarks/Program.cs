using LightDbContext.Models;
using LightDbContext.Pages;

namespace Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    namespace LightDbContext.Benchmarks
    {
        [MemoryDiagnoser]
        [SimpleJob(RuntimeMoniker.NetCoreApp31)]
        public class NoTrackingBenchmarks
        {
            [Params(1, 10, 100)]
            public int NumberOfExecutions { get; set; }
        
            [GlobalSetup]
            public void Setup()
            {
                using var db = new ShopDbContext();
                db.Database.EnsureCreated();
            }

            [Benchmark]
        
            public void Light()
            {
                IndexModel.ExecuteAsync(() => new LightShopDbContext(), NumberOfExecutions);
            }
        
            [Benchmark]
            public void Heavy()
            {
                IndexModel.ExecuteAsync(() => new ShopDbContext(), NumberOfExecutions);
            }
        }
    
        public class Program
        {
            public static void Main(string[] args)
            {
                BenchmarkRunner.Run(typeof(Program).Assembly);
            }
        }
    }
}