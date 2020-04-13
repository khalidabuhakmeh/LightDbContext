using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightDbContext.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LightDbContext.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public List<QueryResult> Light { get; set; }
        public List<QueryResult> Heavy { get; set; }
        [BindProperty(SupportsGet = true)]
        public int NumberOfExecutions { get; set; } = 10;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            var services = HttpContext.RequestServices;
            Light = await ExecuteAsync(
                () => services.GetService(typeof(LightShopDbContext)) as ShopDbContext,
                NumberOfExecutions
            );
            
            Heavy = await ExecuteAsync(
                () => services.GetService(typeof(ShopDbContext)) as ShopDbContext,
                NumberOfExecutions
            );
        }

        public static async Task<List<QueryResult>> ExecuteAsync(
            Func<ShopDbContext> databaseFactory, 
            int numOfExecutions = 10)
        {
            var queryResults = new List<QueryResult>();
            var random = new Random();

            // the first execution is an outlier
            for (var i = 0; i < numOfExecutions + 1; i++)
            {
                // create a database each time
                // closer to what happens in a real-world app
                await using var db = databaseFactory();
                var stopwatch = Stopwatch.StartNew();
                var skip = random.Next(1, 100);

                var items =
                    await db.Items
                        .Where(x => (double)x.Price > 0)
                        .Skip(skip)
                        .Take(100)
                        .ToListAsync();

                stopwatch.Stop();
                queryResults.Add(new QueryResult
                {
                    Elapsed = stopwatch.Elapsed
                });
            }

            return queryResults.Skip(1).ToList();
        }
    }

    public class QueryResult
    {
        public TimeSpan Elapsed { get; set; }
    }
}