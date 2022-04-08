using FileTransfer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics;
using System.Text;

namespace FileTransfer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDistributedCache distributedCache;

        public HomeController(ILogger<HomeController> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            this.distributedCache = distributedCache;
        }

        public async Task<IActionResult> Index()
        {
            DateTime dateTime = DateTime.Now;
            var key = "time";
            var chachedDate = distributedCache
                .SetStringAsync(key, dateTime.ToString(),
                    new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                        SlidingExpiration = TimeSpan.FromMinutes(3)
                    });


            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Index(FileInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


           

            var key = "pdf";
            var chachedDate = distributedCache
                .SetAsync(key, ReadFully(model.File!.OpenReadStream()),
                    new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                        SlidingExpiration = TimeSpan.FromMinutes(3)
                    });

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static byte[] ReadFully(Stream input)
        {
            if (input is MemoryStream ms)
            {
                return ms.ToArray();
            }
            using (ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}